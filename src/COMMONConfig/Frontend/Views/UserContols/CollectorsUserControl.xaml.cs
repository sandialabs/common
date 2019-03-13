using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using COMMONConfig.Utils;
using COMMONConfig.Utils.CustomControls;
using gov.sandia.sld.common.db.models;
using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data.wmi;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.utilities;

namespace COMMONConfig.Frontend.Views.UserContols
{
    /// <summary>
    ///     Interaction logic for CollectorsUserControl.xaml
    /// </summary>
    public partial class CollectorsUserControl : UserControl
    {
        private readonly ObservableCollection<CheckBox> _collectorCheckBoxes;
        private readonly ObservableCollection<TextBox> _collectorTextBoxes;
        //private ICommand _toggleAll;
        //private bool _toggleCollectors = true;

        public CollectorsUserControl()
        {
            _collectorCheckBoxes = new ObservableCollection<CheckBox>();
            _collectorTextBoxes = new ObservableCollection<TextBox>();

            InitializeComponent();
        }

        //public ICommand ToggleAllCommand
        //{
        //    get { return _toggleAll ?? (_toggleAll = new RelayCommand(p => { ToggleAllCollectors(); })); }
        //}

        //public void ToggleAllCollectors()
        //{
        //    _toggleCollectors = !_toggleCollectors;
        //    foreach (CheckBox cb in _collectorCheckBoxes)
        //    {
        //        cb.IsChecked = _toggleCollectors;

        //        CollectorInfo ci = cb.Tag as CollectorInfo;
        //        if(ci != null)
        //            ci.isEnabled = _toggleCollectors;
        //    }

        //    foreach (TextBox tb in _collectorTextBoxes)
        //        tb.IsEnabled = _toggleCollectors;
        //}

        public CollectorsUserControl Initialize(DeviceInfo di)
        {
            // The system device shouldn't have the ability to choose any drives
            if (di.type == EDeviceType.System)
                MonitoredDrives.Visibility = Visibility.Collapsed;

            /*
            foreach (var i in collector_infos)
            {
                CollectorInfos.Add(i);
            }
            */
            var row_id = 1;
            foreach (var info in di.collectors)
            {
                if (info.SkipConfiguration)
                    continue;

                try
                {
                    var row = new RowDefinition {Height = new GridLength(0, GridUnitType.Auto)};

                    var enabled = new CheckBox
                    {
                        IsChecked = info.isEnabled,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tag = info
                    };
                    enabled.Click += OnEnabledClick;
                    Grid.SetRow(enabled, row_id);
                    Grid.SetColumn(enabled, 0);

                    var namestr = info.collectorType.CollectorName();
                    var name = new Label
                    {
                        Content = namestr,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Grid.SetRow(name, row_id);
                    Grid.SetColumn(name, 1);

                    var frequency = new IntegerTextBox
                    {
                        Text = info.frequencyInMinutes.ToString(),
                        MinWidth = 100,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tag = info
                    };
                    frequency.TextChanged += OnFrequencyChanged;
                    Grid.SetRow(frequency, row_id);
                    Grid.SetColumn(frequency, 2);

                    // Do all these at the bottom so if an exception was thrown
                    // in the middle someplace things won't be so confused.
                    _collectorCheckBoxes.Add(enabled);
                    _collectorTextBoxes.Add(frequency);
                    collectorsGrid.RowDefinitions.Add(row);
                    collectorsGrid.Children.Add(enabled);
                    collectorsGrid.Children.Add(name);
                    collectorsGrid.Children.Add(frequency);

                    ++row_id;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "CollectorsUserControl.Initialize", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            DataContext = di;
            UpdateDriveList();

            return this;
        }

        private void UpdateDriveList()
        {
            DeviceInfo di = DataContext as DeviceInfo;

            DriveStack.Children.Clear();
            foreach (MonitoredDrive drive in di.monitoredDrives.driveMap.Values)
            {
                string content = drive.letter;
                if (string.IsNullOrEmpty(drive.name) == false)
                    content += " -- (" + drive.name + ")";

                CheckBox cb = new CheckBox()
                {
                    Content = content,
                    IsChecked = drive.isMonitored,
                    Tag = drive,
                };
                cb.Click += OnDriveChecked;
                DriveStack.Children.Add(cb);
            }
        }

        internal void UpdateDrives(List<DriveInfo> drives)
        {
            DeviceInfo di = DataContext as DeviceInfo;

            di.monitoredDrives.Clear();
            di.monitoredDrives.AddRange(drives);

            UpdateDriveList();
        }

        private void OnDriveChecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            MonitoredDrive d = cb.Tag as MonitoredDrive;

            if(cb != null && d != null)
                d.isMonitored = cb.IsChecked.Value;
        }

        private void OnChooseDrives(object sender, RoutedEventArgs e)
        {
            DeviceInfo di = DataContext as DeviceInfo;

            Remote r = new Remote(di.ipAddress, di.username, di.password);
            CollectorID c_id = new CollectorID(-1, "CollectorsUserControl.Disk");
            DataCollector c = new DiskNameCollector(c_id, r);
            GlobalIsRunning.IsRunning = true;

            c.AttachDataAcquiredHandler(OnDataAcquired);
            c.Acquire();
        }

        private void OnDataAcquired(CollectedData data)
        {
            DeviceInfo di = DataContext as DeviceInfo;

            if (data.DataIsCollected)
            {
                if(data.D != null && data.D.Count > 0)
                {
                    GenericDictionaryData<DriveInfo> dict = (GenericDictionaryData<DriveInfo>)data.D[0];
                    if (dict != null)
                    {
                        Dictionary<string, DriveInfo> d2 = dict.Data;
                        di.monitoredDrives.Clear();
                        di.monitoredDrives.AddRange(new List<DriveInfo>(dict.Data.Values));

                        UpdateDriveList();
                    }
                }
            }
            else
            {
                string message = "Data collection failure";
                if (string.IsNullOrEmpty(data.Message) == false)
                    message += "\n" + data.Message;

                MessageBox.Show(message, "Drive list retrieval", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnFrequencyChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (IntegerTextBox) sender;
            var info = (CollectorInfo) tb.Tag;
            int i;
            if (int.TryParse(tb.Text, out i) && i >= 0)
                info.frequencyInMinutes = i;
        }

        private void OnEnabledClick(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox) sender;

            var info = (CollectorInfo) cb.Tag;
            info.isEnabled = !info.isEnabled;

            // Find the TextBox that matches the CollectorInfo being
            // updated and enable/disable it as well

            TextBox tb = null;
            foreach (TextBox tb2 in _collectorTextBoxes)
            {
                if (tb2.Tag == info)
                {
                    tb = tb2;
                    break;
                }
            }

            if (tb != null)
                tb.IsEnabled = info.isEnabled;
        }

        //private void FreqAll_OnTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    var tb = (IntegerTextBox) sender;

        //    int i;
        //    if (!int.TryParse(tb.Text, out i))
        //        return;

        //    foreach (var t in _collectorTextBoxes)
        //    {
        //        t.Text = i.ToString();

        //        CollectorInfo ci = t.Tag as CollectorInfo;
        //        if (ci != null)
        //            ci.frequencyInMinutes = i;
        //    }
        //}
    }
}