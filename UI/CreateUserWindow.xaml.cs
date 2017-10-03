namespace GiftManager.UI
{
    using System.Windows;
    using System.Windows.Input;

    using GiftManager.Code;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CreateUserWindow : Window
    {
        public CreateUserWindow()
        {
            this.InitializeComponent();
        }

        private void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void CreateUser(object sender, RoutedEventArgs e)
        {
            string name = this.cuwNameField?.Text;
            string username = this.cuwUsernameField?.Text;
            string password = this.cuwPasswordBox?.Password;
            string errorMessage = GiftManager.HasInvalidMemberTextFields(name, username, password);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage);
                return;
            }
            bool[] creationInfo = GiftManager.CreateUser(name, username, password, false);
            bool created = creationInfo[0];
            bool isAdminUser = creationInfo[1];

            if (created)
            {
                if (isAdminUser)
                {
                    var dashboard = new AdminDashboard();
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    var dashboard = new Dashboard();
                    dashboard.Show();
                    this.Close();
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.CreateUser(sender, (RoutedEventArgs)e);
                e.Handled = true;
            }
        }
    }
}