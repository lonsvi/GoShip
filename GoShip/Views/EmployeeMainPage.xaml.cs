using System.Windows;
using System.Windows.Controls;
using GoShip.ViewModels;

namespace GoShip.Views
{
    public partial class EmployeeMainPage : Page
    {
        private readonly EmployeeMainViewModel viewModel;
        private readonly int userId; // Добавляем поле для хранения userId

        public EmployeeMainPage(int userId) // Добавляем конструктор с параметром
        {
            InitializeComponent();
            this.userId = userId; // Сохраняем userId
            viewModel = new EmployeeMainViewModel();
            DataContext = viewModel;
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Переход в поддержку (в разработке)");
        }

        private void MarkAsActive_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is int orderId)
            {
                viewModel.MarkAsActive(orderId);
                MessageBox.Show($"Заказ №{orderId} отмечен как активный.");
            }
        }

        private void MarkAsCompleted_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is int orderId)
            {
                viewModel.MarkAsCompleted(orderId);
                MessageBox.Show($"Заказ №{orderId} отмечен как завершённый.");
            }
        }
    }
}