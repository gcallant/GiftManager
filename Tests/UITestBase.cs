// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UITestBase.cs" company="grantcallant.com">
// Author: Grant Callant  
// </copyright>
// <summary>
//   Defines the UITestBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GiftManager.Tests
{
   using TestStack.White.UIItems;
   using TestStack.White.UIItems.WindowItems;

   using Xunit;

   /// <summary>
   /// The ui test base. Allows inheriting classes to process login
   /// </summary>
   [Collection("UI Tests")]
   public class UITestBase
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="UITestBase"/> class. Prevents public instantiation.
      /// </summary>
      protected UITestBase() {}

      /// <summary>
      /// Login to UI
      /// </summary>
      /// <param name="username">
      /// The username.
      /// </param>
      /// <param name="password">
      /// The password.
      /// </param>
      /// <param name="window">
      /// The main window.
      /// </param>
      protected void DoLogin(string username, string password, Window window)
      {
         var usernameField = window.Get<TextBox>("mwUsernameField");
         var passwordField = window.Get<TextBox>("mwPasswordBox");
         var loginButton = window.Get<Button>("mwLoginButton");
         usernameField.Text = username;
         passwordField.Text = password;
         loginButton.Click();
      }
   }
}