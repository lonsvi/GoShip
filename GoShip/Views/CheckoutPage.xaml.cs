using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using GoShip.ViewModels;

namespace GoShip.Views
{
    public partial class CheckoutPage : Page
    {
        private readonly int userId;
        private readonly CartViewModel viewModel;

        public CheckoutPage(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            viewModel = new CartViewModel(userId);
            DataContext = viewModel;
        }

        private void Cart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage(userId));
        }

        private void Catalog_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ClientMainPage(userId));
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Переход в поддержку (в разработке)");
        }

        private async void ConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            string address = AddressTextBox.Text;
            string name = NameTextBox.Text;
            string email = EmailTextBox.Text;
            string deliveryTime = DeliveryTimeTextBox.Text;
            string cardNumber = CardNumberTextBox.Text;
            string cardDate = CardDateTextBox.Text;
            string cvv = CvvTextBox.Text;

            // Проверка на пустые поля (кроме комментария)
            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Пожалуйста, укажите адрес доставки!");
                return;
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Пожалуйста, укажите имя!");
                return;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Пожалуйста, укажите email!");
                return;
            }
            if (string.IsNullOrWhiteSpace(deliveryTime))
            {
                MessageBox.Show("Пожалуйста, укажите время доставки!");
                return;
            }
            if (string.IsNullOrWhiteSpace(cardNumber))
            {
                MessageBox.Show("Пожалуйста, укажите номер карты!");
                return;
            }
            if (string.IsNullOrWhiteSpace(cardDate))
            {
                MessageBox.Show("Пожалуйста, укажите дату/год карты!");
                return;
            }
            if (string.IsNullOrWhiteSpace(cvv))
            {
                MessageBox.Show("Пожалуйста, укажите CVV!");
                return;
            }

            // Сохраняем заказ
            viewModel.ConfirmOrder(address);

            // Обновляем данные во всплывающем окне
            AddressDisplay.Text = $"Адрес: {address}";
            DeliveryTimeDisplay.Text = $"Заказ будет доставлен к: {deliveryTime}";

            // Показываем всплывающее окно
            OverlayGrid.Visibility = Visibility.Visible;

            // Ждём 5 секунд
            await Task.Delay(5000);

            // Переходим на страницу каталога
            OverlayGrid.Visibility = Visibility.Collapsed;
            NavigationService.Navigate(new ClientMainPage(userId));
        }
    }
}