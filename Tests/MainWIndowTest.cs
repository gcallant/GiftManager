// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWIndowTest.cs" company="grantcallant.com">
// Author: Grant Callant  
// </copyright>
// <summary>
//   Defines the MainWindowTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GiftManager.Tests
{
   using System;

   using TestStack.White;
   using TestStack.White.UIItems;
   using TestStack.White.UIItems.WindowItems;

   using Xunit;

   /// <summary>
   /// The main window UI test.
   /// </summary>
   [Collection("UI Tests")]
   public class MainWindowTest : UITestBase
   {
      /// <summary>
      /// The create user test.
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="password">
      /// The password.
      /// </param>
      [Theory]
      [InlineData("George", "T3stP@ssword!")]
      private void CreateUserTest(string name, string password)
      {
         using(Application giftApplication = Application.Launch(@"GiftManager.exe"))
         {
            using(Window mainWindow = giftApplication.GetWindow("Gift Manager"))
            {
               var button = mainWindow.Get<Button>("mwCreateNewUserButton");
               Assert.Equal("Create new user", button.Text);
               button.Click();
            }

            using(Window window = giftApplication.GetWindow("Gift Manager"))
            {
               window.Get<TextBox>("cuwNameField").Text = name;
               window.Get<TextBox>("cuwUsernameField").Text = new Random().Next().ToString();
               window.Get<TextBox>("cuwPasswordBox").Text = password;
               window.Get<Button>("createUserButton").Click();
            }

            using(Window adWindow = giftApplication.GetWindow("Gift Manager"))
            {
               var button = adWindow.Get<Button>("dashLogout");
               Assert.Equal("Logout", button.Text);
               button.Click();
            }

            using(Window mWindow = giftApplication.GetWindow("Gift Manager"))
            {
               mWindow.Close();
            }
         }
      }
   }
}