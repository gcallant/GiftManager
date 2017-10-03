namespace GiftManager.Tests
{
   using GiftManager.Code;

   using Xunit;

   /// <summary>
   ///    The gift manager tests.
   /// </summary>
   public class GiftManagerTests
   {
      /// <summary>
      ///    The test add gift.
      /// </summary>
      /// <param name="name">
      ///    The name.
      /// </param>
      /// <param name="price">
      ///    The price.
      /// </param>
      /// <param name="description">
      ///    The description.
      /// </param>
      /// <param name="result">
      ///    The expected result.
      /// </param>
      [Theory]
      [InlineData("Flower Pot", 30, "A vase for storing dirt and flowers", true)]
      [InlineData("iPod", 303, "", true)]
      [InlineData("", 30, "A vase for storing dirt and flowers", false)]
      [InlineData("Flower pot", -30, "A vase for storing dirt and flowers", false)]
      public void TestAddGift(string name, string price, string description, bool result)
      {
         Assert.Equal(result, GiftManager.AddGift(name, price, description));
      }

      /// <summary>
      ///    The test has invalid gift text fields.
      /// </summary>
      /// <param name="name">
      ///    The name.
      /// </param>
      /// <param name="price">
      ///    The price.
      /// </param>
      /// <param name="result">
      ///    The expected result.
      /// </param>
      [Theory]
      [InlineData(null, null, "Gift name is required")]
      [InlineData("", "", "Gift name is required")]
      [InlineData("Apricot", null, "Price is required")]
      [InlineData("Apricot", "", "Price is required")]
      [InlineData(
         "-Xyl-ephone-",
         20,
         "Gift names should have only letters, numbers, dashes and underscores in between characters")]
      [InlineData(
         "Flute",
         "1,1,11,111.11",
         "Price should be in US dollars (no $ sign) optional commas (all or none) and optional cents" + "\nExamples:"
         + "\n1234.34" + "\n123434" + "\n1,234.34" + "\n1,23434")]
      public void TestHasInvalidGiftTextFields(string name, string price, string result)
      {
         Assert.Equal(result, GiftManager.HasInvalidGiftTextFields(name, price));
      }

      /// <summary>
      ///    The test has invalid member text field.
      /// </summary>
      /// <param name="name">
      ///    The name.
      /// </param>
      /// <param name="username">
      ///    The username.
      /// </param>
      /// <param name="password">
      ///    The password.
      /// </param>
      /// <param name="result">
      ///    The expected result.
      /// </param>
      [Theory]
      [InlineData(null, null, null, "All fields are required")]
      [InlineData("", "", "", "All fields are required")]
      public void TestHasInvalidMemberTextField(string name, string username, string password, string result)
      {
         Assert.Equal(result, GiftManager.HasInvalidMemberTextFields(name, username, password));
      }
   }
}