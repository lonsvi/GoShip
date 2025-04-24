using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GoShip.Services;
using GoShip.ViewModels;

namespace GoShip.Views
{
    public partial class CheckoutPage : Page
    {
        private readonly int userId;
        private readonly CartViewModel viewModel;
        private readonly DatabaseService db;

        public CheckoutPage(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            viewModel = new CartViewModel(userId);
            DataContext = viewModel;
            db = new DatabaseService();
            LoadUserDetails();
        }

        private void LoadUserDetails()
        {
            var (name, email, address) = db.GetUserDetails(userId);
            NameTextBox.Text = name;
            EmailTextBox.Text = email;
            AddressTextBox.Text = address;
        }

        private void SaveDetails_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string email = EmailTextBox.Text;
            string address = AddressTextBox.Text;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!");
                return;
            }

            db.SaveUserDetails(userId, name, email, address);
            MessageBox.Show("Данные сохранены!");
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
            string comment = CommentTextBox.Text; // Получаем комментарий

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

            // Сохраняем данные клиента
            db.SaveUserDetails(userId, name, email, address);

            // Сохраняем заказ, передаём комментарий
            viewModel.ConfirmOrder(address, comment);

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