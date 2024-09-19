using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.data.wmi;
using gov.sandia.sld.common.db.models;
using gov.sandia.sld.common.utilities;
using Newtonsoft.Json;

namespace COMMONConfig.Frontend.Views.UserContols
{
    /// <summary>
    ///     Interaction logic for MachineUserControl.xaml
    /// </summary>
    public partial class MachineUserControl : AbstractUserControl
    {
        public MachineUserControl()
        {
            InitializeComponent();
        }

        public override AbstractUserControl Initialize(AbstractUserControlContext ctx)
        {
            base.Initialize(ctx);

            machineNameTextBox.Text = ctx.Info.name;
            ipAddress.Text = ctx.Info.ipAddress;
            usernameTextBox.Text = ctx.Info.username;
            Password.Password = ctx.Info.password;

            CollectorsControl.Initialize(ctx.Info);

            if(ctx.Groups != null && ctx.Groups.Count == 0)
                ctx.Groups.CollectionChanged += onGroupsCollectionChanged;

            UpdateGroups(groupComboBox, LeaveGroup);

            DataContext = ctx.Info;

            return this;
        }

        private void onGroupsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateGroups(groupComboBox, LeaveGroup);

            if (_ctx.Groups != null && _ctx.Groups.Count > 0)
                _ctx.Groups.CollectionChanged -= onGroupsCollectionChanged;
        }

        public override void DoDataBinding()
        {
            DeviceInfo dev_info = DataContext as DeviceInfo;
            if (dev_info != null)
            {
                dev_info.DID = new DeviceID(dev_info.id, machineNameTextBox.Text);
                dev_info.ipAddress = ipAddress.Text;
                dev_info.username = usernameTextBox.Text;
                dev_info.password = Password.Password;
            }
        }

        private void onGroupChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DeviceInfo di = (DeviceInfo)DataContext;
            Group g = (Group)groupComboBox.SelectedItem;

            if (di == null || g == null)
                return;

            di.groupID = g.id;
            LeaveGroup.IsEnabled = true;
        }

        private void OnLeaveGroup(object sender, RoutedEventArgs e)
        {
            DeviceInfo di = (DeviceInfo)DataContext;

            if (di == null)
                return;

            di.groupID = -1;
            groupComboBox.SelectedIndex = -1;
            LeaveGroup.IsEnabled = false;
        }

        private void OnTestConnection(object sender, RoutedEventArgs e)
        {
            Remote r = new Remote(ipAddress.Text, usernameTextBox.Text, Password.Password);
            CollectorID c_id = new CollectorID(-1, "Configuration.Disk");
            DataCollector c = new DiskNameCollector(c_id, r);
            GlobalIsRunning.Start();

            c.AttachDataAcquiredHandler(OnDataAcquired);
            c.Acquire();
        }

        private void OnDataAcquired(CollectedData data)
        {
            string message = string.Empty;
            bool success = false;

            if(data == null)
            {
                message = "Null collected data";
            }
            else if(data.DataIsCollected)
            {
                success = true;
                foreach(Data d in data.D)
                {
                    GenericDictionaryData<DriveInfo> dict = (GenericDictionaryData<DriveInfo>)d;
                    if(dict != null)
                    {
                        Dictionary<string, DriveInfo> d2 = dict.Data;
                        CollectorsControl.UpdateDrives(new List<DriveInfo>(d2.Values));
                    }

                    if (!string.IsNullOrEmpty(message))
                        message += "\n";

                    message += JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented,
                        new Newtonsoft.Json.JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                }
            }
            else
            {
                message = "Data collection failure";
                if (string.IsNullOrEmpty(data.Message) == false)
                    message += "\n" + data.Message;
            }

            MessageBox.Show(message, "Connection test", MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);
        }
    }
}