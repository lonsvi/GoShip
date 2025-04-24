using System.Windows.Controls;

namespace GoShip.Views
{
    public partial class CheckoutPage : Page
    {
        private readonly int userId;

        public CheckoutPage(int userId)
        {
            InitializeComponent();
            this.userId = userId;
        }
    }
}