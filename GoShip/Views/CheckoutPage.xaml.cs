using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using QRCoder;
using GoShip.Services;
using GoShip.ViewModels;
using System.Linq;
using System;
using System.Windows.Input;

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

        private void SaveDetails_Click(object sender, RoutedEventArgs e)
        {
            string address = AddressTextBox.Text;
            string name = NameTextBox.Text;
            string email = EmailTextBox.Text;

            if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Пожалуйста, заполните все поля: адрес, имя и email!");
                return;
            }

            db.SaveUserDetails(userId, name, email, address);
            MessageBox.Show("Данные успешно сохранены!");
        }

        private void SaveCardData_Click(object sender, RoutedEventArgs e)
        {
            string cardNumber = CardNumberTextBox.Text;
            string cardDate = CardDateTextBox.Text;
            string cvv = CvvTextBox.Text;
            string comment = CommentTextBox.Text;

            if (string.IsNullOrWhiteSpace(cardNumber) || string.IsNullOrWhiteSpace(cardDate) || string.IsNullOrWhiteSpace(cvv))
            {
                MessageBox.Show("Пожалуйста, заполните все поля: номер карты, дата/год и CVV!");
                return;
            }

            //// Здесь можно сохранить данные карты в базу (например, в таблицу UserCard)
            //db.GetCartItems(userId, cardNumber, cardDate, cvv, comment);
            //MessageBox.Show("Данные карты успешно сохранены!");
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
            string comment = CommentTextBox.Text;

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

            // Логика оформления заказа
            var items = viewModel.CartItems.ToList();
            if (!items.Any())
            {
                MessageBox.Show("Корзина пуста!");
                return;
            }

            decimal total = items.Sum(item => item.Product.Price * item.Quantity);
            viewModel.LastOrderTotal = total;
            string orderDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            int orderId = db.CreateOrder(userId, address, total, orderDate, comment, deliveryTime);

            foreach (var item in items)
            {
                db.AddOrderItem(orderId, item.ProductId, item.Product.Name, item.Product.Price, item.Quantity);
            }

            db.ClearCart(userId);
            viewModel.LoadCart();
            viewModel.LoadPastOrders();
            MessageBox.Show("Заказ успешно оформлен!");

            // Формируем строку для QR-кода
            string qrContent = $"Адрес: {address}\nВремя доставки: {deliveryTime}\nСумма: {viewModel.LastOrderTotal} руб.";
            // Генерируем QR-код
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20); // 20 — размер пикселя для модуля QR-кода

            // Преобразуем байты в изображение для WPF
            using (MemoryStream memoryStream = new MemoryStream(qrCodeBytes))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                // Устанавливаем изображение в Image
                QrCodeImage.Source = bitmapImage;
            }

            // Обновляем данные во всплывающем окне
            AddressDisplay.Text = $"Адрес: {address}";
            DeliveryTimeDisplay.Text = $"Заказ будет доставлен к: {deliveryTime}";

            // Показываем всплывающее окно
            OverlayGrid.Visibility = Visibility.Visible;

            // Ждём 10 секунд
            await Task.Delay(10000);

            // Переходим на страницу каталога
            OverlayGrid.Visibility = Visibility.Collapsed;
            NavigationService.Navigate(new ClientMainPage(userId));
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                Window.GetWindow(this)?.DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }
    }

}