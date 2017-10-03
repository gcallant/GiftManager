namespace GiftManager.UI
{
   using System.Security;
   using System.Windows;
   using System.Windows.Controls;
   using System.Windows.Input;

   using GiftManager.Code;

   /// <summary>
   ///    Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         this.InitializeComponent();
      }

      private void TextBox_OnGotFocus(object sender, RoutedEventArgs e)
      {
         var textBox = (TextBox)sender;
         textBox.Text = string.Empty;
         textBox.GotFocus -= this.TextBox_OnGotFocus;
      }

      private void CreateNewUserButton(object sender, RoutedEventArgs e)
      {
         var userWindow = new CreateUserWindow();
         userWindow.Show();
         this.Close();
      }

      private void Login(object sender, RoutedEventArgs e)
      {
         string username = this.mwUsernameField?.Text;
         SecureString password = this.mwPasswordBox?.SecurePassword;
         bool[] login = GiftManager.Login(username, password);
         bool isLoggedIn = login[0];
         bool isAdminUser = login[1];

         if(isLoggedIn)
         {
            if(isAdminUser)
            {
               var dashboard = new AdminDashboard {CurrentUsername = username};
               dashboard.Show();
               this.Close();
            }
            else
            {
               var dashboard = new Dashboard {CurrentUsername = username};
               dashboard.Show();
               this.Close();
            }
         }
         else
         {
            MessageBox.Show("Username or password was not correct", "Login Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         }
      }

      private void OnKeyDown(object sender, KeyEventArgs e)
      {
         if(e.Key == Key.Return)
         {
            this.Login(sender, e);
            e.Handled = true;
         }
      }
   }
}