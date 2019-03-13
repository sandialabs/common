using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DBTester
{
    class TestData
    {
        public string Text { get; set; }

        public TestData()
        {
            Text = c_text;
        }

        private static string GenerateRandomString()
        {
            const string s = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
            char[] c = new char[10000];
            for (int i = 0; i < c.Length; ++i)
                c[i] = s[c_random.Next(s.Length)];
            return new string(c);
        }

        private static Random c_random = new Random();
        private static string c_text = GenerateRandomString();
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string ConnString
        {
            get
            {
                return "Data Source=" + DataSource.Text + ";Initial Catalog=" + InitialCatalog.Text + ";Integrated Security=True;";
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            DataSource.Text = "(local)";
            InitialCatalog.Text = "DBTester";
            ConnectionString.Content = ConnString;

            c_main_window = this;
        }

        private void OnAddData(object sender, RoutedEventArgs e)
        {
            c_is_running = !c_is_running;

            if (c_is_running)
            {
                c_connection_string = ConnString;
                m_insert_thread = new Thread(new ThreadStart(AddData));
                m_insert_thread.Start();
            }
            else
            {
                m_insert_thread.Join();
            }

            AddDataButton.Content = c_is_running ? "Stop" : "Add Data";
            TruncateButton.IsEnabled = !c_is_running;
        }

        private void OnTruncate(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("TRUNCATE TABLE [" + c_dbname_a + "]", conn))
                    {
                        command.ExecuteNonQuery();
                        AddStatusText("Truncated Table " + c_dbname_a);
                    }
                    using (SqlCommand command = new SqlCommand("TRUNCATE TABLE [" + c_dbname_b + "]", conn))
                    {
                        command.ExecuteNonQuery();
                        AddStatusText("Truncated Table " + c_dbname_b);
                    }
                    using (SqlCommand command =
                        new SqlCommand("DBCC SHRINKDATABASE(" + InitialCatalog.Text + ")", conn))
                    {
                        command.ExecuteNonQuery();
                        c_main_window.AddStatusText("Shrank Database");
                    }
                }
            }
            catch (Exception ex)
            {
                AddStatusText(ex.Message);
            }
        }

        private static void AddData()
        {
            while (c_is_running)
            {
                try
                {
                    Stopwatch watch;
                    long ms;
                    long count_of_rows = 0;

                    if (c_created_a == false)
                    {
                        watch = Stopwatch.StartNew();
                        using (SqlConnection conn = new SqlConnection(c_connection_string))
                        {
                            conn.Open();

                            CreateDB(conn, c_dbname_a);

                            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM [" + c_dbname_a + "]", conn))
                            {
                                SqlDataReader reader = command.ExecuteReader();
                                if(reader.HasRows && reader.Read())
                                    count_of_rows = reader.GetInt64(0);
                            }

                            if (count_of_rows == 0)
                            {
                                using (SqlBulkCopy bulk_copy = new SqlBulkCopy(conn))
                                {
                                    bulk_copy.BulkCopyTimeout = 0;
                                    bulk_copy.DestinationTableName = c_dbname_a;
                                    bulk_copy.ColumnMappings.Add("Text", "Text");

                                    using (var reader = new ObjectDataReader<TestData>(c_data))
                                    {
                                        bulk_copy.WriteToServer(reader);
                                    }
                                }

                                count_of_rows = c_data.Length;

                                ms = watch.ElapsedMilliseconds;
                                c_main_window.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                                    new Action(() =>
                                    {
                                        c_main_window.AddStatusText(string.Format("Inserted {0} records in {1} ms", c_data.Length, ms));
                                    }));
                            }
                        }

                        c_created_a = true;
                    }

                    watch = Stopwatch.StartNew();
                    using (SqlConnection conn = new SqlConnection(c_connection_string))
                    {
                        conn.Open();

                        CreateDB(conn, c_dbname_b);

                        using (SqlCommand command =
                            new SqlCommand("INSERT INTO [" + c_dbname_b + "] (Text) SELECT Text FROM [" + c_dbname_a + "]", conn))
                        {
                            command.CommandTimeout = 0;
                            command.ExecuteNonQuery();
                        }
                    }

                    ms = watch.ElapsedMilliseconds;
                    c_main_window.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                        new Action(() =>
                        {
                            c_main_window.AddStatusText(string.Format("Inserted {0} records in {1} ms", count_of_rows, ms));
                        }));
                }
                catch (Exception ex)
                {
                    c_main_window.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                        new Action(() =>
                        {
                            c_main_window.AddStatusText(ex.Message);
                        }));
                }
            }
        }

        private static void CreateDB(SqlConnection conn, string db_name)
        {
            using (SqlCommand command =
                new SqlCommand("IF OBJECT_ID('[" + db_name + "]', 'U') IS NULL CREATE TABLE [" + db_name + "] (TestID int NOT NULL IDENTITY (1, 1), Text nvarchar(MAX) NOT NULL)", conn))
            {
                command.ExecuteNonQuery();
            }
        }

        private void AddStatusText(string text)
        {
            StatusText.Inlines.Add(text);
            StatusText.Inlines.Add("\n");
        }

        static MainWindow()
        {
            c_data = new TestData[10000];
            for (int i = 0; i < c_data.Length; ++i)
                c_data[i] = new TestData();
        }

        private Thread m_insert_thread;

        // Create an array of 10000 elements. We'll just keep inserting these same 10000 items
        private static TestData[] c_data;
        private static string c_connection_string = string.Empty;
        private static bool c_is_running = false;
        private static MainWindow c_main_window;
        private static bool c_created_a = false;
        private static string c_dbname_a = "DBTesterTable_9E62EB7CA80D4C309B7B78D5B83EB767";
        private static string c_dbname_b = "DBTesterTable_E6FB311FDB2945C480428D27AEB2DA69";
    }
}
