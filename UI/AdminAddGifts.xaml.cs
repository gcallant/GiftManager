namespace GiftManager.UI
{
    using System.Windows;
    using System.Windows.Forms;

    using GiftManager.Code;

    using MessageBox = System.Windows.MessageBox;
    using TextBox = System.Windows.Controls.TextBox;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AdminAddGifts : Window
    {
        public AdminAddGifts()
        {
            this.InitializeComponent();
        }

        private void ReturnToDashboard(object sender, RoutedEventArgs e)
        {
            var dashboard = new AdminDashboard();
            dashboard.Show();
            this.Close();
        }

        private void TextOnFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Text = string.Empty;
            textBox.GotFocus -= this.TextOnFocus;
        }

        private void ClearAllFields()
        {
            this.giftName.Text = string.Empty;
            this.giftPrice.Text = string.Empty;
            this.giftDescription.Text = string.Empty;
        }

        private static void ShowGiftAddedBalloonNotification(string name, string price, string description)
        {
            using (var notifyIcon = new NotifyIcon())
            {
                notifyIcon.Visible = true;
                notifyIcon.BalloonTipTitle = "Gift Added!";
                notifyIcon.BalloonTipText = $"{name}, for ${price} was created! \n{description}";
                notifyIcon.Icon = Properties.Resources.GiftManager;
                notifyIcon.ShowBalloonTip(30000);
            }
        }

        private void AddGift(object sender, RoutedEventArgs e)
        {
            string name = this.giftName?.Text;
            string price = this.giftPrice?.Text;
            string description = this.giftDescription?.Text;
            string errorMessage = GiftManager.HasInvalidGiftTextFields(name, price);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage);
                return;
            }
            bool added = GiftManager.AddGift(name, price, description);

            if (added)
            {
                ShowGiftAddedBalloonNotification(name, price, description);
                this.ClearAllFields();
            }
            else
            {
                MessageBox.Show($"An error occurred, and {name} could not be added", "Error adding Gift", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}