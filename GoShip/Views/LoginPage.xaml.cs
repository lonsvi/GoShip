using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GoShip.Services;

namespace GoShip.Views
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Window.GetWindow(this).DragMove();
        }
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            // Скрываем форму, показываем спиннер
            LoginFormPanel.Visibility = Visibility.Collapsed;
            LoaderSpinner.Visibility = Visibility.Visible;

            string login = txtLogin.Text;
            string password = txtPassword.Password;
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                LoaderSpinner.Visibility = Visibility.Collapsed;
                LoginFormPanel.Visibility = Visibility.Visible;
                MessageBox.Show("Заполните все поля!");
                return;
            }

            // Имитация задержки (например, запрос к БД)
            await Task.Delay(1000);

            var db = new DatabaseService();
            var result = db.Authenticate(login, password);
            LoaderSpinner.Visibility = Visibility.Collapsed;
            LoginFormPanel.Visibility = Visibility.Visible;

            if (result.HasValue)
            {
                int userId = result.Value.userId;
                string role = result.Value.role;
                if (role == "Client")
                {
                    NavigationService.Navigate(new ClientMainPage(userId));
                }
                else if (role == "Employee" || role == "Admin")
                {
                    NavigationService.Navigate(new EmployeeLoginPage());
                }
                else
                {
                    MessageBox.Show("Неизвестная роль пользователя!");
                }
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!");
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }

        private void EmployeeLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EmployeeLoginPage());
        }

    }
}