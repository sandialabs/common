using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using COMMONConfig.Utils.CustomControls;

namespace COMMONConfig.Frontend.Views.UserContols
{
    /// <summary>
    ///     Interaction logic for IpTextBox.xaml
    /// </summary>
    public partial class IpTextBox : UserControl
    {
        private static readonly Key[] JumpRightKeys = {Key.Right, Key.OemPeriod, Key.Decimal, Key.Space};
        private static readonly Key[] JumpLeftKeys = {Key.Left};

        public static readonly DependencyProperty IpAddressProperty =
            DependencyProperty.Register("IpAddress", typeof(string), typeof(IpTextBox),
                new PropertyMetadata(@"0.0.0.0", TextChangedProperty));

        private readonly IntegerTextBox[] quartets;
        private int currentIndex;

        public IpTextBox()
        {
            InitializeComponent();
            DataObject.AddPastingHandler(this,
                (sender, args) => { IpAddress = args.DataObject.GetData(typeof(string)) as string; });

            currentIndex = 0;

            quartets = new[] {FirstQuartet, SecondQuartet, ThirdQuartet, FourthQuartet};
            FirstQuartet.GotFocus += (sender, args) => currentIndex = 0;
            SecondQuartet.GotFocus += (sender, args) => currentIndex = 1;
            ThirdQuartet.GotFocus += (sender, args) => currentIndex = 2;
            FourthQuartet.GotFocus += (sender, args) => currentIndex = 3;
        }

        public string IpAddress
        {
            get { return GetValue(IpAddressProperty) as string; }
            set
            {
                IPAddress ignored;
                if (IPAddress.TryParse(value, out ignored))
                {
                    SetValue(IpAddressProperty, value);
                }
            }
        }

        private static void TextChangedProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as IpTextBox;
            tb?.SetIpAddress(e.NewValue as string);
        }

        private string GetIpAddress()
        {
            return GetText(FirstQuartet) + @"."
                   + GetText(SecondQuartet) + @"."
                   + GetText(ThirdQuartet) + @"."
                   + GetText(FourthQuartet);
        }

        private void SetIpAddress(string value)
        {
            var qs = value.Split('.');
            if (qs.Length < 3) return;
            FirstQuartet.Text = qs[0];
            SecondQuartet.Text = qs[1];
            ThirdQuartet.Text = qs[2];
            FourthQuartet.Text = qs[3];
        }

        public void PreviewKeyPressed(object sender, KeyEventArgs e)
        {
            HandleKeyJump(e);
        }

        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            var itb = sender as IntegerTextBox;

            if (itb == null) return;

            if (itb.Text.Length >= 3)
            {
                IncrementIndex();
                FocusIndex();
            }

            int val;
            var isNumber = int.TryParse(itb.Text, out val);
            if (!isNumber || val > 255)
            {
                quartets[currentIndex].Clear();
            }

            IpAddress = GetIpAddress();
            FocusIndex();
        }

        public void OnFocus(object sender, RoutedEventArgs e)
        {
            var tb = (IntegerTextBox) sender;
            if (tb.Text == @"0") tb.Text = "";
        }

        public void FocusLost(object sender, RoutedEventArgs e)
        {
            var tb = (IntegerTextBox) sender;
            if (tb.Text.Length == 0) tb.Text = @"0";
        }

        private void HandleKeyJump(KeyEventArgs e)
        {
            if (e.Key == Key.Back && quartets[currentIndex].Text.Length == 0) DecrementIndex();
            else if (JumpLeftKeys.Contains(e.Key)) DecrementIndex();
            else if (JumpRightKeys.Contains(e.Key)) IncrementIndex();

            FocusIndex();
        }

        private void IncrementIndex()
        {
            currentIndex = RealMod(currentIndex + 1, 4);
        }

        private void DecrementIndex()
        {
            currentIndex = RealMod(currentIndex - 1, 4);
        }

        private int RealMod(int x, int m)
        {
            return (x%m + m)%m;
        }

        private static string GetText(TextBox tb)
        {
            return tb.Text.Length == 0 ? @"0" : tb.Text;
        }

        private void FocusIndex()
        {
            quartets[currentIndex].Focus();
            CaretPosition();
        }

        private void CaretPosition()
        {
            if (quartets[currentIndex].Text != @"")
            {
                quartets[currentIndex].CaretIndex = quartets[currentIndex].Text.Length;
            }
        }
    }
}