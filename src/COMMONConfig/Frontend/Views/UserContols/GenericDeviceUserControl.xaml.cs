using gov.sandia.sld.common.db.models;
using System.Collections.Specialized;

namespace COMMONConfig.Frontend.Views.UserContols
{
    /// <summary>
    ///     Interaction logic for GenericDeviceUserControl.xaml
    /// </summary>
    public partial class GenericDeviceUserControl : AbstractUserControl
    {
        public GenericDeviceUserControl()
        {
            InitializeComponent();
        }

        public override AbstractUserControl Initialize(AbstractUserControlContext ctx)
        {
            base.Initialize(ctx);
            MachineNameTextBox.Text = ctx.Info.name;
            IpAddress.Text = ctx.Info.ipAddress;

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
            // Don't need to do anything
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

        private void OnLeaveGroup(object sender, System.Windows.RoutedEventArgs e)
        {
            DeviceInfo di = (DeviceInfo)DataContext;

            if (di == null)
                return;

            di.groupID = -1;
            groupComboBox.SelectedIndex = -1;
            LeaveGroup.IsEnabled = false;
        }
    }
}