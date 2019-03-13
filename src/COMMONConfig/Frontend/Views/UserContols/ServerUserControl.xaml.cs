using gov.sandia.sld.common.db;

namespace COMMONConfig.Frontend.Views.UserContols
{
    /// <summary>
    ///     Interaction logic for ServerUserControl.xaml
    /// </summary>
    public partial class ServerUserControl : AbstractUserControl
    {
        public ServerUserControl()
        {
            InitializeComponent();
        }

        public override AbstractUserControl Initialize(AbstractUserControlContext ctx)
        {
            base.Initialize(ctx);
            serverNameTextBox.Text = ctx.Info.name;
            collectorsControl.Initialize(ctx.Info);

            DataContext = ctx.Info;

            return this;
        }

        public override void DoDataBinding()
        {
            // Don't have to do anything
        }
    }
}