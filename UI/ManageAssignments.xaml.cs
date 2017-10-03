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
   public partial class ManageAssignments : Window
   {
      public ManageAssignments()
      {
         this.InitializeComponent();
         this.maRegenerateAssignments.DataContext = this;
         this.UpdateList();
      }

      private void UpdateList()
      {
         ObservableCollection<Assignments> assignments = GiftManager.ListAssignments();
         this.dgUsers.ItemsSource = assignments;
         this.blurGrid.Visibility = assignments.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
         this.maRegenerateAssignments.Visibility = this.blurGrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
         this.checkBox.Visibility = this.blurGrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
      }

      private void ReturnToDashboard(object sender, RoutedEventArgs e)
      {
         var dashboard = new AdminDashboard();
         dashboard.Show();
         this.Close();
      }

      private void GenerateAssignments(object sender, RoutedEventArgs e)
      {
         bool assignGifts = this.comboBox.Text.Equals("Yes") ? true : false;
         if(this.checkBox.IsVisible)
         {
            assignGifts = this.checkBox.IsChecked.Value;
         }
         string errorMessage = GiftManager.GenerateAssignments(assignGifts);
         if(!string.IsNullOrEmpty(errorMessage))
         {
            MessageBox.Show(errorMessage);
            return;
         }
         this.UpdateList();
      }
   }  
}