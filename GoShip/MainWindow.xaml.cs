using GoShip.Views;
using System.Windows;

namespace GoShip
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new LoginPage());
        }
    }
}