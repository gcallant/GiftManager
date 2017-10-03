namespace GiftManager.UI
{
   using System;
   using System.Collections.ObjectModel;
   using System.Globalization;
   using System.Windows;
   using System.Windows.Data;

   using GiftManager.Code;

   /// <summary>
   ///    Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class DisplayCurrentUserAssignment : Window
   {
      public string CurrentUsername {get; set;}

      public DisplayCurrentUserAssignment(string currentUsername)
      {
         this.InitializeComponent();
         this.dgUsers.ItemsSource = GiftManager.DisplayCurrentUserAssignment(currentUsername);
      }

      private void ReturnToDashboard(object sender, RoutedEventArgs e)
      {
         var dashboard = new Dashboard {CurrentUsername = this.CurrentUsername};
         dashboard.Show();
         this.Close();
      }
   }  
}