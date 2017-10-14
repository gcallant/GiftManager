namespace GiftManager.UI
{
   using System.Threading;
   using System.Windows;
   using System.Windows.Controls;
   using System.Windows.Input;
   using System.Windows.Media;

   using GiftManager.Code;

   /// <summary>
   ///    Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class Dashboard : Window
   {
      public Dashboard()
      {
         this.InitializeComponent();
      }

      public string CurrentUsername { get; set; }

      private void ListGifts(object sender, RoutedEventArgs e)
      {
         var gifts = new ListGifts() {CurrentUsername = this.CurrentUsername};
         gifts.Show();
         this.Close();
      }

      private void ShowAssignment(object sender, RoutedEventArgs e)
      {
         var assignment = new DisplayCurrentUserAssignment(this.CurrentUsername) {CurrentUsername = this.CurrentUsername};
         assignment.Show();
         this.Close();
      }

      private void ShowAssignments_MouseEnter(object sender, MouseEventArgs e)
      {
         var button = (Button)sender;

         if(GiftManager.DisplayCurrentUserAssignment(this.CurrentUsername).Count == 0)
         {
            button.IsEnabled = false;
            button.Background = Brushes.Crimson;
            button.ToolTip = "You haven't been assigned yet! \nAsk your administrator about this";
            Thread.Sleep(50);
         }

         e.Handled = true;
      }

      private void DashMouseLeave(object sender, MouseEventArgs e)
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

      private void Logout(object sender, RoutedEventArgs e)
      {
         var window = new MainWindow();
         window.Show();
         this.Close();
      }
   }
}