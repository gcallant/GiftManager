// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdminDashboardTest.cs" company="grantcallant.com">
// Author: Grant Callant  
// </copyright>
// <summary>
//   Defines the AdminDashboardTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GiftManager.Tests
{
   using TestStack.White;
   using TestStack.White.UIA;
   using TestStack.White.UIItems;
   using TestStack.White.UIItems.WindowItems;

   using Xunit;

   /// <summary>
   /// The admin dashboard UI test.
   /// </summary>
   [Collection("UI Tests")]
   public class AdminDashboardTest : UITestBase
   {
      [Theory]
      [InlineData("testadminuser", "T3stP@ssword!")]
      public void TestDashboardButtons(string username, string password)
      {
         using(Application giftApplication = Application.Launch(@"GiftManager.exe"))
         {
            this.DoLogin(username, password, giftApplication.GetWindow("Gift Manager"));
            using(Window adminWindow = giftApplication.GetWindow("Gift Manager"))
            {
               Assert.Equal("Welcome to the Admin Dashboard!", adminWindow.Get<Label>("adWelcomeLabel").Text);
               var button = adminWindow.Get<Button>("adManageUsers");
               Assert.Equal("List and Manage Users", button.Text);
               Assert.Equal(true, button.Enabled);
               button = adminWindow.Get<Button>("adAddUsers");
               Assert.Equal("Add Users", button.Text);
               Assert.Equal(true, button.Enabled);
               button = adminWindow.Get<Button>("adShowAssignments");
               Assert.Equal("Display Gift Assignments", button.Text);
               adminWindow.Mouse.Location = button.Bounds.Center();
               Assert.Equal(true, button.Enabled);
               button = adminWindow.Get<Button>("adAddGifts");
               Assert.Equal("Add Gifts", button.Text);
               Assert.Equal(true, button.Enabled);
               button = adminWindow.Get<Button>("adManageGifts");
               Assert.Equal("List and Manage Gifts", button.Text);
               adminWindow.Mouse.Location = button.Bounds.Center();
               Assert.Equal(true, button.Enabled);
               button = adminWindow.Get<Button>("adManageAssignments");
               Assert.Equal("Manage Assignments", button.Text);
               Assert.Equal(true, button.Enabled);
               adminWindow.Close();
            }
         }
      }
   }
}