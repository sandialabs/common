using gov.sandia.sld.common.db;

namespace COMMONConfig.Frontend.Views.UserContols
{
    /// <summary>
    ///     Interaction logic for SystemUserControl.xaml
    /// </summary>
    public partial class SystemUserControl : AbstractUserControl
    {
        public SystemUserControl()
        {
            InitializeComponent();
        }

        public override AbstractUserControl Initialize(AbstractUserControlContext ctx)
        {
            base.Initialize(ctx);
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