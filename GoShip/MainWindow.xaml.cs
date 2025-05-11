using GoShip.Views;
using System.Windows;
using System.Windows.Input;

namespace GoShip
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new LoginPage());
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Window.GetWindow(this).DragMove();
        }
    }
}