namespace GiftManager.UI
{
   using System.Threading;
   using System.Windows;
   using System.Windows.Controls;
   using System.Windows.Input;
   using System.Windows.Media;

   using GiftManager.Code;

   /// <summary>
   ///    Interaction logic for MainWindow
   /// </summary>
   public partial class AdminDashboard : Window
   {
      public AdminDashboard()
      {
         this.InitializeComponent();
      }

      public string CurrentUsername { get; set; }

      private void Logout(object sender, RoutedEventArgs e)
      {
         var window = new MainWindow();
         window.Show();
         this.Close();
      }

      private void AddUsers(object sender, RoutedEventArgs e)
      {
         var newUserWindow = new AdminCreateUser();
         newUserWindow.Show();
         this.Close();
      }

      private void ListAndManagerUsers(object sender, RoutedEventArgs e)
      {
         var listUsersWindow = new ListAndManageUsers();
         listUsersWindow.Show();
         this.Close();
      }

      private void AddGifts(object sender, RoutedEventArgs e)
      {
         var giftsWindows = new AdminAddGifts();
         giftsWindows.Show();
         this.Close();
      }

      private void ListandManageGifts(object sender, RoutedEventArgs e)
      {
         var giftsDashboard = new ListAndManageGifts();
         giftsDashboard.Show();
         this.Close();
      }

      private void ManageAssignments(object sender, RoutedEventArgs e)
      {
         var manageAssignmentsWindow = new ManageAssignments();
         manageAssignmentsWindow.Show();
         this.Close();
      }

      private void DisplayAssignments(object sender, RoutedEventArgs e)
      {
         var button = (Button)sender;
         if(GiftManager.ListAssignments().Count == 0)
         {
            this.adShowAssignments.IsEnabled = false;
            button.Background = Brushes.Crimson;
            MessageBox.Show(
               "No assignments are set yet",
               "No Assignments to Display!",
               MessageBoxButton.OK,
               MessageBoxImage.Hand);
         }
         else
         {
            var display = new DisplayAssignments();
            display.Show();
            this.Close();
         }
         e.Handled = true;
      }

      private void adShowAssignments_MouseEnter(object sender, MouseEventArgs e)
      {
         var button = (Button)sender;

         if(GiftManager.ListAssignments().Count == 0)
         {
            button.IsEnabled = false;
            button.Background = Brushes.Crimson;
            button.ToolTip = "No Assignments are set yet";
            Thread.Sleep(50);
         }

         e.Handled = true;
      }

      private void ADMouseLeave(object sender, MouseEventArgs e)
      {
         var button = (Button)sender;

         if(!button.IsMouseOver)
         {
            button.IsEnabled = true;
            var bc = new BrushConverter();
            button.Background = (Brush)bc.ConvertFrom("#FFAA8D39");
         }
         e.Handled = true;
      }

      private void AdManageGifts_OnMouseEnter(object sender, MouseEventArgs e)
      {
         var button = (Button)sender;

         if(GiftManager.ListAllGifts().Count == 0)
         {
            button.IsEnabled = false;
            button.Background = Brushes.Crimson;
            button.ToolTip = "No gifts are added yet";
            Thread.Sleep(50);
         }

         e.Handled = true;
      }

      private void AdManageUsers_OnMouseEnter(object sender, MouseEventArgs e)
      {
         var button = (Button)sender;

         if(GiftManager.ListAllMembers().Count == 0)
         {
            button.IsEnabled = false;
            button.Background = Brushes.Crimson;
            button.ToolTip = "No users are added yet";
            Thread.Sleep(50);
         }

         e.Handled = true;
      }
   }
}