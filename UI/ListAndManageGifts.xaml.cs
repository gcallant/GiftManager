using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GiftManager.UI
{
    using System.Drawing;
    using System.Windows.Forms;

    using GiftManager.Code;

    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
    using MessageBox = System.Windows.MessageBox;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ListAndManageGifts : Window
    {
        public ListAndManageGifts()
        {
            this.InitializeComponent();
            this.dgUsers.ItemsSource = GiftManager.ListAllGifts();
        }

        private void ReturnToDashboard(object sender, RoutedEventArgs e)
        {
            var dashboard = new AdminDashboard();
            dashboard.Show();
            this.Close();
        }

       private void AddGifts(object sender, RoutedEventArgs e)
       {
          var addGiftsWindow = new AdminAddGifts();
          addGiftsWindow.Show();
          this.Close();
       }
    }
}