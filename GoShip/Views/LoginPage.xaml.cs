﻿using System.Windows;
using System.Windows.Controls;
using GoShip.Services;

namespace GoShip.Views
{
    public partial class LoginPage : Page
    {
        public LoginPage()
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