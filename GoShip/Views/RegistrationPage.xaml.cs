using System.Windows;
using System.Windows.Controls;
using GoShip.Services;

namespace GoShip.Views
{
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text;
            string password = txtPassword.Password;
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            var db = new DatabaseService();
            bool success = db.RegisterUser(login, password, "Client");
            if (success)
            {
                MessageBox.Show("Регистрация успешна!");
                NavigationService.Navigate(new LoginPage());
            }
            else
            {
                MessageBox.Show("Логин уже занят!");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }

        private void EmployeeLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EmployeeLoginPage());
        }
    }
}