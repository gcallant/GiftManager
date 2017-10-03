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
   public partial class DisplayAssignments : Window
   {
      public DisplayAssignments()
      {
         this.InitializeComponent();
         this.dgUsers.ItemsSource = GiftManager.DisplayAssignments();
      }

      private void ReturnToDashboard(object sender, RoutedEventArgs e)
      {
         var dashboard = new AdminDashboard();
         dashboard.Show();
         this.Close();
      }
   }  
}