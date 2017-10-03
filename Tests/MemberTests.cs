// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberTests.cs" company="grantcallant.com">
// Author: Grant Callant  
// </copyright>
// <summary>
//   Defines the MemberTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GiftManager.Tests
{
   using GiftManager.Code;

   using Xunit;

   /// <summary>
   /// The member tests.
   /// </summary>
   public class MemberTests
   {
      /// <summary>
      /// The test new member properties.
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="username">
      /// The username.
      /// </param>
      /// <param name="id">
      /// The id.
      /// </param>
      /// <param name="isAdminUser">
      /// The is admin user.
      /// </param>
      [Theory]
      [InlineData("Josh", "jSmith", 300, false)]
      [InlineData("JOsH H. Smither-Zimmerman", "j333-Smit_h", int.MaxValue, true)]
      public void TestNewMemberProperties(string name, string username, long id, bool isAdminUser)
      {
         var member = new Member
         {
            MemberName = name,
            MemberUserName = username,
            MemberId = id,
            IsAdminUser = isAdminUser
         };
         Assert.Equal(member.MemberId, id);
         Assert.Equal(member.MemberName, name);
         Assert.Equal(member.MemberUserName, username);
         Assert.Equal(member.IsAdminUser, isAdminUser);
      }

      /// <summary>
      /// The test is valid password.
      /// </summary>
      /// <param name="password">
      /// The password.
      /// </param>
      /// <param name="result">
      /// The result.
      /// </param>
      [Theory]
      [InlineData("Pa1@a34", false)]
      [InlineData("nullNullNullnullnull", false)]
      [InlineData("kLPc0!$4", true)]
      [InlineData("i6!c%G7Nv&J2$a5x#G5u#SN4LDq*x5fu_4J-*TUe32Q7$56#_2ExmjPmENtk$t2N#!52NXb-", true)]
      [InlineData("i6!c%G7Nv&J2$a5x#G5u#SN4LDq*x5fu_4J-*TUe32Q7$56#_2ExmjPmENtk$t2N#!52NXb-3", false)]
      public void TestIsValidPassword(string password, bool result)
      {
         Assert.Equal(result, Member.IsValidPassword(password));
      }

      /// <summary>
      /// The test is valid name.
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="result">
      /// The result.
      /// </param>
      [Theory]
      [InlineData("Josephine Livingston", true)]
      [InlineData("J", true)]
      [InlineData("Josephine-Livingston", true)]
      [InlineData("Josephine H Livingston", true)]
      [InlineData("Josephine H. Livingston", true)]
      [InlineData("Josephine D'Livingston", true)]
      [InlineData("Jos3phine D'Livingston", false)]
      [InlineData("Josephine #'Livingston", false)]
      [InlineData("", false)]
      public void TestIsValidName(string name, bool result)
      {
         Assert.Equal(result, Member.IsValidName(name));
      }

      /// <summary>
      /// The test is valid user name.
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="result">
      /// The result.
      /// </param>
      [Theory]
      [InlineData("a", true)]
      [InlineData("edWard_Smith22", true)]
      [InlineData("a3asdlfkjasdlfj3294823094asdfkljasdfljsadf", true)]
      [InlineData("3", true)]
      [InlineData("3a", true)]
      [InlineData("3-a", true)]
      [InlineData("3-a-b", true)]
      [InlineData("3_a-b", true)]
      [InlineData("", false)]
      [InlineData("#asdflsdf3sdfks", false)]
      [InlineData("#", false)]
      [InlineData("-", false)]
      [InlineData("_ab", false)]
      [InlineData("3--b", false)]
      public void TestIsValidUserName(string name, bool result)
      {
         Assert.Equal(result, Member.IsValidUsername(name)[0]);
      }

      /// <summary>
      /// The test generate id.
      /// </summary>
      [Fact]
      public void TestGenerateId()
      {
         Assert.InRange(Member.GenerateMemberID(), 1, int.MaxValue);
      }
   }
}