namespace GiftManager.UI
{
    using System.Windows;

    using GiftManager.Code;

    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
    using MessageBox = System.Windows.MessageBox;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ListAndManageUsers : Window
    {
        public ListAndManageUsers()
        {
            this.InitializeComponent();
            this.dgUsers.ItemsSource = GiftManager.ListAllMembers();
        }

        private void ReturnToDashboard(object sender, RoutedEventArgs e)
        {
            var dashboard = new AdminDashboard();
            dashboard.Show();
            this.Close();
        }

       private void CreateUser(object sender, RoutedEventArgs e)
       {
          var newUserWindow = new AdminCreateUser();
          newUserWindow.Show();
          this.Close();
       }
   }
}