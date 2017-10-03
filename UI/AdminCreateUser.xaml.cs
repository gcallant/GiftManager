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
    public partial class AdminCreateUser : Window
    {
        public AdminCreateUser()
        {
            this.InitializeComponent();
        }

        private void ReturnToDashboard(object sender, RoutedEventArgs e)
        {
            var dashboard = new AdminDashboard();
            dashboard.Show();
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
            string comboText = this.comboBox.Text;
            bool isAdminUser = comboText.Equals("Yes") ? true : false;
            bool[] creationInfo = GiftManager.CreateUser(name, username, password, isAdminUser);
            bool created = creationInfo[0];

            if (created)
            {
                ShowUserCreatedBalloonNotification(name, username);
                ClearAllFields();
            }
            else
            {
                MessageBox.Show($"An error occurred, and {username} could not be added", $"Error adding {name}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearAllFields()
        {
            this.cuwNameField.Text = string.Empty;
            this.cuwUsernameField.Text = string.Empty;
            this.cuwPasswordBox.Password = string.Empty;
            this.comboBox.SelectedIndex = 1;
        }

        private static void ShowUserCreatedBalloonNotification(string name, string username)
        {
            using (var notifyIcon = new NotifyIcon())
            {
                notifyIcon.Visible = true;
                notifyIcon.BalloonTipTitle = "User Created!";
                notifyIcon.BalloonTipText = $"{name}, with username {username} was created";
                notifyIcon.Icon = Properties.Resources.GiftManager;
                notifyIcon.ShowBalloonTip(30000);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CreateUser(sender, (RoutedEventArgs)e);
                e.Handled = true;
            }
        }
    }
}