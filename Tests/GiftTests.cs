// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GiftTests.cs" company="grantcallant.com">
// Author: Grant Callant  
// </copyright>
// <summary>
//   Defines the GiftTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GiftManager.Tests
{
    using GiftManager.Code;

    using Xunit;

   /// <summary>
   /// The gift tests.
   /// </summary>
   public class GiftTests
    {
       /// <summary>
       /// The testlength for generate giftid
       /// </summary>
       private const int TESTLENGTH = 50;

       /// <summary>
       /// The test catch int parse overflow.
       /// </summary>
       /// <param name="price">
       /// The price.
       /// </param>
       /// <param name="result">
       /// The expected result.
       /// </param>
       [Theory]
      [InlineData("0", null)]
      [InlineData("9223372036854775807", null)]
      [InlineData("-1", "Price should be between 0 (free) and 9223372036854775807")]
      [InlineData("9223372036854775808", "Price should be between 0 (free) and 9223372036854775807")]
      public void TestCatchIntParseOverflow(string price, string result)
      {
         Assert.Equal(Gift.CatchIntParseOverflow(price), result);
      }

       /// <summary>
       /// The test is valid gift name.
       /// </summary>
       /// <param name="name">
       /// The name.
       /// </param>
       /// <param name="result">
       /// The expected result.
       /// </param>
       [Theory]
      [InlineData("a", true)]
      [InlineData("Flower Pot", true)]
      [InlineData("3", true)]
      [InlineData("3 Dog Knight", true)]
      [InlineData("mp3-cd", true)]
      [InlineData("", false)]
      [InlineData("#asdflsdf3sdfks", false)]
      [InlineData("#", false)]
      [InlineData("-", false)]
      [InlineData("_ab", false)]
      [InlineData("3--b", false)]
      public void TestIsValidGiftName(string name, bool result)
      {
         Assert.Equal(Gift.IsValidName(name), result);
      }

       /// <summary>
       /// The test is valid price.
       /// </summary>
       /// <param name="price">
       /// The price.
       /// </param>
       /// <param name="result">
       /// The expected result.
       /// </param>
       [Theory]
      [InlineData("0.01", true)]
      [InlineData("0.10", true)]
      [InlineData("9223372036854775807", true)]
      [InlineData("9223372036854775807.99", true)]
      [InlineData("9,223,372,036,854,775,807", true)]
      [InlineData("9,223,372,036,854,775,807.11", true)]
      [InlineData("0.1", false)]
      [InlineData(".1", false)]
      [InlineData("9223,372,036,854,775,807.11", false)]
      [InlineData(".1abcde", false)]
      [InlineData("Word", false)]
      [InlineData("32340234#$asdfasdf324320948", false)]

      public void TestIsValidPrice(string price, bool result)
      {
         Assert.Equal(Gift.IsValidPrice(price), result);
      }

       /// <summary>
       /// The test generate gift id range.
       /// </summary>
       [Fact]
        public void TestGenerateGiftIdRange()
        {
           for(int i = 0; i < TESTLENGTH; i++)
           {
              Assert.InRange(Gift.GenerateGiftID(), 1, long.MaxValue);
              Assert.NotInRange(Gift.GenerateGiftID(), long.MinValue, 0);
           }
        }
    }
}
