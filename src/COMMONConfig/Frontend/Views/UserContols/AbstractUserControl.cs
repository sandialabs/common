using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using gov.sandia.sld.common.db.models;

namespace COMMONConfig.Frontend.Views.UserContols
{
    public class AbstractUserControlContext
    {
        public DeviceInfo Info { get; set; }
        public ObservableCollection<Group> Groups { get; set; }
    }

    public abstract class AbstractUserControl : UserControl, IHandleDataBinding
    {
        protected AbstractUserControlContext _ctx;

        public abstract void DoDataBinding();

        public virtual AbstractUserControl Initialize(AbstractUserControlContext ctx)
        {
            _ctx = ctx;

            DeviceInfo info = ctx.Info;
            Name = info.type + info.id.ToString().Replace('-', '_');
            return this;
        }

        protected void UpdateGroups(ComboBox groupComboBox, Button leaveGroup)
        {
            if (_ctx.Groups == null ||_ctx.Groups.Count == 0)
            {
                ObservableCollection<Group> temp = new ObservableCollection<Group>();
                Group g = new Group() { name = "No groups have been defined" };
                temp.Add(g);
                groupComboBox.ItemsSource = temp;
                groupComboBox.SelectedIndex = 0;
            }
            else
                groupComboBox.ItemsSource = _ctx.Groups;

            groupComboBox.IsEnabled = _ctx.Groups != null && _ctx.Groups.Count > 0;
            groupComboBox.DisplayMemberPath = "name";

            if (_ctx.Groups != null &&_ctx.Info.groupID >= 0)
            {
                for (int i = 0; i < _ctx.Groups.Count; ++i)
                {
                    Group group = _ctx.Groups[i];
                    if (group.id == _ctx.Info.groupID)
                    {
                        groupComboBox.SelectedItem = group;
                        break;
                    }
                }
            }

            leaveGroup.IsEnabled = _ctx.Info.groupID >= 0;
        }
    }
}