using System.Windows;
using System.Windows.Controls;
using GoShip.Services;

namespace GoShip.Views
{
    public partial class EmployeeLoginPage : Page
    {
        public EmployeeLoginPage()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text;
            string password = txtPassword.Password;
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            var db = new DatabaseService();
            var result = db.Authenticate(login, password);
            if (result.HasValue)
            {
                int userId = result.Value.userId;
                string role = result.Value.role;
                if (role == "Employee" || role == "Admin")
                {
                    NavigationService.Navigate(new EmployeeMainPage(userId));
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль для сотрудника!");
                }
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль для сотрудника!");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}