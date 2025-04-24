using System.Windows.Controls;

namespace GoShip.Views
{
    public partial class EmployeeMainPage : Page
    {
        private readonly int userId;
        public EmployeeMainPage(int userId)
        {
            InitializeComponent();
            this.userId = userId;
        }
    }
}