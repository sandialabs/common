using gov.sandia.sld.common.db;
using gov.sandia.sld.common.db.changers;
using gov.sandia.sld.common.utilities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Xunit;

namespace UnitTest.database
{
    public class DatabaseShould
    {
        private const string CreateTable = "CREATE TABLE Temp(A INTEGER NOT NULL PRIMARY KEY, B TEXT)";

        [Fact]
        public void CreateTableOK()
        {
            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Database db = new Database(new Context(fd.Fi));
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    conn.ExecuteNonQuery(CreateTable);
                    Assert.True(conn.DoesTableExist("Temp"));
                    Assert.False(conn.DoesTableExist("NonExistentTable"));
                }
            }
        }

        [Fact]
        public void InsertOK()
        {
            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Database db = new Database(new Context(fd.Fi));
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    conn.ExecuteNonQuery(CreateTable);
                    Assert.True(conn.DoesTableExist("Temp"));

                    int count = 100;
                    Dictionary<int, string> dict = new Dictionary<int, string>();

                    Insert(conn, count, dict);
                    Verify(conn, dict);
                }
            }
        }

        private static void Insert(SQLiteConnection conn, int count, Dictionary<int, string> dict)
        {
            Random r = new Random();
            for (int i = 0; i < count; ++i)
            {
                int a = r.Next();
                string b = Guid.NewGuid().ToString();
                Inserter inserter = new Inserter("Temp", conn);
                inserter.Set("A", a);
                inserter.Set("B", b, false);
                inserter.Execute();

                dict[a] = b;
            }
        }

        private static void Verify(SQLiteConnection conn, Dictionary<int, string> dict)
        {
            foreach(int i in dict.Keys)
            {
                string sql = $"SELECT B FROM Temp WHERE A = {i}";
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    bool read = reader.Read();
                    Assert.True(read);
                    if (read)
                    {
                        Assert.False(reader.IsDBNull(0));
                        string b = reader.GetString(0);
                        Assert.Equal(dict[i], b);
                    }
                }
            }
        }

        [Fact]
        public void UpdateOK()
        {
            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Dictionary<int, string> dict = new Dictionary<int, string>();
                Database db = new Database(new Context(fd.Fi));

                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    conn.ExecuteNonQuery(CreateTable);
                    Assert.True(conn.DoesTableExist("Temp"));

                    Insert(conn, 100, dict);
                    dict = Update(conn, dict);
                    Verify(conn, dict);
                }
            }
        }

        private static Dictionary<int, string> Update(SQLiteConnection conn, Dictionary<int, string> dict)
        {
            Dictionary<int, string> new_dict = new Dictionary<int, string>();
            foreach(int i in dict.Keys)
            {
                Updater u = new Updater("Temp", $"A = {i}", conn);

                // Change the value of what's stored from a GUID to a number.
                // This makes sure were storing numbers inside of a text column OK.

                string b = i.ToString();
                u.Set("B", b, false);
                u.Execute();
                new_dict[i] = b;
            }

            return new_dict;
        }

        [Fact]
        public void InsertNullOK()
        {
            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Database db = new Database(new Context(fd.Fi));
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    conn.ExecuteNonQuery(CreateTable);
                    Assert.True(conn.DoesTableExist("Temp"));

                    int count = 100;
                    Random r = new Random();
                    Dictionary<int, string> dict = new Dictionary<int, string>();
                    for (int i = 0; i < count; ++i)
                    {
                        Inserter inserter = new Inserter("Temp", conn);
                        int a = r.Next();
                        inserter.Set("A", a);

                        // Make every other one null
                        if (i % 2 == 0)
                        {
                            string b = Guid.NewGuid().ToString();
                            inserter.Set("B", b, false);
                            dict[a] = b;
                        }
                        else
                        {
                            inserter.SetNull("B");
                            dict[a] = null;
                        }

                        inserter.Execute();
                    }

                    foreach(int a in dict.Keys)
                    {
                        string sql = $"SELECT B FROM Temp WHERE A = {a}";
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            bool read = reader.Read();
                            Assert.True(read);
                            if (read)
                            {
                                if (dict[a] != null)
                                {
                                    Assert.False(reader.IsDBNull(0));
                                    string b = reader.GetString(0);
                                    Assert.Equal(dict[a], b);
                                }
                                else
                                {
                                    Assert.True(reader.IsDBNull(0));
                                    Assert.Throws<InvalidCastException>(() => reader.GetString(0));
                                }
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public void DeleteOK()
        {
            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Dictionary<int, string> dict = new Dictionary<int, string>();
                Database db = new Database(new Context(fd.Fi));

                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    conn.ExecuteNonQuery(CreateTable);
                    Assert.True(conn.DoesTableExist("Temp"));

                    Insert(conn, 100, dict);
                    Verify(conn, dict);

                    foreach (int i in dict.Keys)
                    {
                        Deleter d = new Deleter("Temp", $"A = {i}", conn);
                        d.Execute();
                    }

                    foreach(int i in dict.Keys)
                    {
                        string sql = $"SELECT B FROM Temp WHERE A = {i}";
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            bool read = reader.Read();
                            Assert.False(read);
                        }
                    }
                }
            }
        }

        [Fact]
        public void NotBeNullable()
        {
            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Database db = new Database(new Context(fd.Fi));
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    conn.ExecuteNonQuery("CREATE TABLE Temp(A INTEGER NOT NULL PRIMARY KEY, B TEXT NOT NULL)");
                    Assert.True(conn.DoesTableExist("Temp"));

                    Inserter i = new Inserter("Temp", conn);
                    i.Set("A", 1);
                    i.SetNull("B");

                    Assert.Throws<SQLiteException>(() => i.Execute());
                }
            }
        }

        [Fact]
        public void ProperlyHandleDates()
        {
            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Database db = new Database(new Context(fd.Fi));
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    conn.ExecuteNonQuery("CREATE TABLE Temp(A INTEGER NOT NULL PRIMARY KEY, B TEXT NOT NULL)");
                    Assert.True(conn.DoesTableExist("Temp"));

                    Dictionary<int, DateTimeOffset> dict = new Dictionary<int, DateTimeOffset>();
                    Random r = new Random();
                    int count = 100;
                    for(int i = 0; i < count; ++i)
                    {
                        int a = r.Next();
                        DateTimeOffset b = new DateTimeOffset(2018, r.Next(1, 12), r.Next(1, 28), r.Next(0, 23), r.Next(0, 59), r.Next(0, 59), r.Next(0, 999), TimeSpan.FromHours(r.Next(-12, 12)));

                        Inserter ins = new Inserter("Temp", conn);
                        ins.Set("A", a);
                        ins.Set("B", b);
                        ins.Execute();

                        dict[a] = b;
                    }

                    foreach(int a in dict.Keys)
                    {
                        string sql = $"SELECT B FROM Temp WHERE A = {a}";
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            bool read = reader.Read();
                            Assert.True(read);
                            if(read)
                            {
                                DateTimeOffset b = DateTimeOffset.Parse(reader.GetString(0));
                                Assert.Equal(dict[a], b);
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public void ProperlyHandleLongs()
        {
            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Database db = new Database(new Context(fd.Fi));
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    conn.ExecuteNonQuery("CREATE TABLE Temp(A INTEGER NOT NULL PRIMARY KEY, B INTEGER NOT NULL)");
                    Assert.True(conn.DoesTableExist("Temp"));

                    Dictionary<int, long> dict = new Dictionary<int, long>();
                    Random r = new Random();
                    int count = 100;
                    for (int i = 0; i < count; ++i)
                    {
                        int a = r.Next();
                        long b = r.NextLong();

                        Inserter ins = new Inserter("Temp", conn);
                        ins.Set("A", a);
                        ins.Set("B", b);
                        ins.Execute();

                        dict[a] = b;
                    }

                    foreach (int a in dict.Keys)
                    {
                        string sql = $"SELECT B FROM Temp WHERE A = {a}";
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            bool read = reader.Read();
                            Assert.True(read);
                            if (read)
                            {
                                long b = reader.GetInt64(0);
                                Assert.Equal(dict[a], b);
                            }
                        }
                    }
                }
            }
        }
    }
}
