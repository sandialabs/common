using System.Collections.Generic;
using System.Collections.ObjectModel;
using gov.sandia.sld.common.db.models;
using GalaSoft.MvvmLight;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using COMMONConfig.Boundaries.Messages;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace COMMONConfig.Frontend.ViewModel
{
    public class GroupsViewModel : ViewModelBase
    {
        private ObservableCollection<Group> _groups;
        private ICommand _addGroup;

        public ICommand AddGroup => _addGroup ?? (_addGroup = new RelayCommand(AddGroupMessage));

        public GroupsViewModel()
        {
            Groups = new ObservableCollection<Group>();
        }

        public ObservableCollection<Group> Groups
        {
            get { return _groups; }
            set
            {
                _groups = value;
                RaisePropertyChanged(nameof(Groups));

                SetupViews();
            }
        }

        private void SetupViews()
        {
        }

        public void UpdateGroups(List<Group> groups)
        {
            Groups.Clear();
            groups.Where(g => g.id >= 0).ToList().ForEach(g => Groups.Add(g));
        }

        protected virtual void AddGroupMessage()
        {
            Messenger.Default.Send(new AddGroupMessage());
        }
    }
}
