// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GiftManager.cs" company="grantcallant.com">
// Author: Grant Callant  
// </copyright>
// <summary>
//   A factory facade controller class, allowing for interaction between datastore, objects, and view
//   Author: Grant Callant
//   Email: grant@grantcallant.com
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GiftManager.Code
{
   using System;
   using System.Collections.Generic;
   using System.Collections.ObjectModel;
   using System.Collections.Specialized;
   using System.ComponentModel;
   using System.Linq;
   using System.Runtime.InteropServices;
   using System.Security;
   using System.Security.Cryptography;

   /// <summary>
   /// A factory facade controller class, allowing for interaction between datastore, objects, and view
   /// Author: Grant Callant
   /// Email: grant@grantcallant.com
   /// </summary>
   public static class GiftManager
   {
      /// <summary>
      /// Gets the hash iteration, set to 1000
      /// </summary>
      private static int HashIteration { get; } = 1000;

      /// <summary>
      /// Adds a gift to the database from incoming arguments
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="price">
      /// The price.
      /// </param>
      /// <param name="description">
      /// The description.
      /// </param>
      /// <returns>
      /// The <see cref="bool"/>.
      /// </returns>
      public static bool AddGift(string name, string price, string description)
      {
         if(Gift.IsValidName(name) && Gift.IsValidPrice(price))
         {
            var gift = new Gift {GiftName = name, GiftPrice = long.Parse(price), GiftDescription = description};
            return new GiftDataOperations().AddRecord(gift) != 0;
         }
         return false;
      }

      /// <summary>
      /// Creates and user and then adds it
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="username">
      /// The username.
      /// </param>
      /// <param name="password">
      /// The password.
      /// </param>
      /// <param name="isAdminUser">
      /// The is admin user.
      /// </param>
      /// <returns>
      /// The <see cref="bool[]"/>.
      /// </returns>
      public static bool[] CreateUser(string name, string username, string password, bool isAdminUser)
      {
         var member = new Member {MemberName = name, MemberUserName = username, PlainTextPassword = password};
         Collection<Member> members = ListAllMembers();
         if(members.Count == 0)
         {
            isAdminUser = true;
         }

         member.IsAdminUser = isAdminUser;
         bool added = AddMember(member);
         return new[] {added, isAdminUser};
      }

      /// <summary>
      /// Matches up the details from the assignments tables to names and prices from members and gifts
      /// </summary>
      /// <returns>
      /// The <see cref="List"/>.
      /// </returns>
      public static List<Assignments> DisplayAssignments()
      {
         List<Assignments> assignments = new AssignmentDataOperations().GetAll().ToList();
         var memberData = new MemberDataOperations();
         var giftData = new GiftDataOperations();

         foreach(Assignments assignment in assignments)
         {
            Member assignee = memberData.GetById(assignment.MemberId);
            Member assignedMember = memberData.GetById(assignment.AssignedMemberId);
            assignment.Membername = assignee.MemberName;
            assignment.AssignedMember = assignedMember.MemberName;
            if(assignment.AssignedGiftId != 0)
            {
               Gift gift = giftData.GetById(assignment.AssignedGiftId);
               assignment.GiftName = gift.GiftName;
               assignment.GiftPrice = gift.GiftPrice;
               assignment.GiftDescription = gift.GiftDescription;
            }
         }

         return assignments;
      }

      /// <summary>
      /// Displays the matched up assignment details to the current (non-admin) user
      /// </summary>
      /// <param name="currentUsername">
      /// The current username.
      /// </param>
      /// <returns>
      /// The <see cref="List"/>.
      /// </returns>
      public static List<Assignments> DisplayCurrentUserAssignment(string currentUsername)
      {
         var assignments = new List<Assignments>();
         var memberData = new MemberDataOperations();
         var giftData = new GiftDataOperations();
         var filter = new Filter<Member>();

         filter.Add(mem => mem.MemberUserName, currentUsername);
         Member assignee = new MemberDataOperations().Find(filter).ToList()[0];
         Assignments assignment = new AssignmentDataOperations().GetById(assignee.MemberId);
         Member assignedMember = memberData.GetById(assignment.AssignedMemberId);
         assignment.Membername = assignee.MemberName;
         assignment.AssignedMember = assignedMember.MemberName;
         if(assignment.AssignedGiftId != 0)
         {
            Gift gift = giftData.GetById(assignment.AssignedGiftId);
            assignment.GiftName = gift.GiftName;
            assignment.GiftPrice = gift.GiftPrice;
            assignment.GiftDescription = gift.GiftDescription;
         }

         if(assignment.MemberId != 0)
         {
            assignments.Add(assignment);
         }

         return assignments;
      }

      /// <summary>
      /// Generates the actual random assignments from the composition of members and gifts. 
      /// </summary>
      /// <param name="assignGifts">
      /// Whether to include gifts in the the random assignment
      /// </param>
      /// <returns>
      /// The <see cref="string"/>.
      /// </returns>
      public static string GenerateAssignments(bool assignGifts)
      {
         var rand = new Random();
         List<Member> members = new MemberDataOperations().GetAll().ToList();
         if(members.Count <= 1)
         {
            return "There needs to be more than 1 member added to assign";
         }

         List<Gift> gifts = new GiftDataOperations().GetAll().ToList();
         if(assignGifts && gifts.Count < 1)
         {
            return "There are no gifts to assign";
         }

         var assignments = new Collection<Assignments>();
         while(members.Count > 0)
         {
            var assignment1 = new Assignments();
            var assignment2 = new Assignments();
            Assignments assignment3 = null;
            int index = rand.Next(members.Count);
            Member member1 = members[index];
            members.RemoveAt(index);

            index = rand.Next(members.Count);
            Member member2 = members[index];
            members.RemoveAt(index);
            assignment1.MemberId = member1.MemberId;
            assignment1.AssignedMemberId = member2.MemberId;
            assignment2.MemberId = member2.MemberId;
            assignment2.AssignedMemberId = member1.MemberId;
            Member member3 = null;

            // Handle the edge case where we have 3 members
            if(members.Count == 1)
            {
               index = rand.Next(members.Count);
               member3 = members[index];
               members.RemoveAt(index);
               assignment3 = new Assignments();
               assignment3.MemberId = member3.MemberId;
            }

            if(assignGifts)
            {
               Gift gift1 = gifts[rand.Next(gifts.Count)];
               Gift gift2 = gifts[rand.Next(gifts.Count)];
               assignment1.AssignedGiftId = gift1.GiftId;
               assignment2.AssignedGiftId = gift2.GiftId;
               if(member3 != null)
               {
                  Gift gift3 = gifts[rand.Next(gifts.Count)];
                  assignment3.AssignedGiftId = gift3.GiftId;
                  assignment2.AssignedMemberId = member3.MemberId;
                  assignment3.AssignedMemberId = member1.MemberId;
                  assignments.Add(assignment3);
               }

               assignments.Add(assignment1);
               assignments.Add(assignment2);
            }
            else
            {
               if(member3 != null)
               {
                  assignment2.AssignedMemberId = member3.MemberId;
                  assignment3.AssignedMemberId = member1.MemberId;
                  assignments.Add(assignment3);
               }

               assignments.Add(assignment1);
               assignments.Add(assignment2);
            }
         }

         // We can drop here because we're just going to regenerate from existing data
         new AssignmentDataOperations().DropAssignmentTable();
         if(new AssignmentDataOperations().AddMultipleRecords(assignments) < 2)
         {
            return "Assignment failed!";
         }

         return null;
      }

      /// <summary>
      /// Returns an error message to the view class if any UI fields are invalid
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="priceString">
      /// The price string.
      /// </param>
      /// <returns>
      /// The <see cref="string"/>.
      /// </returns>
      public static string HasInvalidGiftTextFields(string name, string priceString)
      {
         if(string.IsNullOrEmpty(name))
         {
            return "Gift name is required";
         }
         if(string.IsNullOrEmpty(priceString))
         {
            return "Price is required";
         }

         if(!Gift.IsValidName(name))
         {
            return "Gift names should have only letters, numbers, dashes and underscores in between characters";
         }

         if(!Gift.IsValidPrice(priceString))
         {
            return "Price should be in US dollars (no $ sign) optional commas (all or none) and optional cents"
                   + "\nExamples:" + "\n1234.34" + "\n123434" + "\n1,234.34" + "\n1,23434";
         }

         return Gift.CatchIntParseOverflow(priceString);
      }

      /// <summary>
      /// Returns an error message to the view class if any UI fields are invalid
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="username">
      /// The username.
      /// </param>
      /// <param name="password">
      /// The password.
      /// </param>
      /// <returns>
      /// The <see cref="string"/>.
      /// </returns>
      public static string HasInvalidMemberTextFields(string name, string username, string password)
      {
         if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
         {
            return "All fields are required";
         }

         if(!Member.IsValidName(name))
         {
            return
               $"Sorry, {name} is not a valid name. Names may only contain \nletters \ndashes \napostraphes \nperiods";
         }

         bool[] usernameinfo = Member.IsValidUsername(username);
         bool valid = usernameinfo[0];
         bool unique = usernameinfo[1];
         if(!valid)
         {
            return
               $"Sorry, {username} is not valid, only letters and numbers are allowed with 1 dash or underscore in between";
         }

         if(!unique)
         {
            return $"Sorry, {username}, is already taken, please select a different username";
         }

         if(!Member.IsValidPassword(password))
         {
            return $"{password} is not valid. Valid passwords contain at least 8 characters, contained of at least:"
                   + "\n1 Uppercase" + "\n1 lowercase" + "\n1 digit" + "\n1 special character (#?!@$%^&*-)";
         }

         return null;
      }

      /// <summary>
      /// Gets an editable gift collection
      /// </summary>
      /// <returns>
      /// The <see cref="ObservableCollection"/>.
      /// </returns>
      public static ObservableCollection<Gift> ListAllGifts()
      {
         var observableGifts = new GiftUIObjects();
         List<Gift> gifts = new GiftDataOperations().GetAll().ToList();
         foreach(Gift gift in gifts)
         {
            observableGifts.Add(gift);
         }

         observableGifts.ItemEndEdit += GiftsItemEndEdit;
         observableGifts.CollectionChanged += GiftsCollectionChanged;
         return observableGifts;
      }

      /// <summary>
      /// Gets an editable member collection
      /// </summary>
      /// <returns>
      /// The <see cref="ObservableCollection"/>.
      /// </returns>
      public static ObservableCollection<Member> ListAllMembers()
      {
         var observableMembers = new MemberUIObjects();
         List<Member> members = new MemberDataOperations().GetAll().ToList();
         foreach(Member member in members)
         {
            observableMembers.Add(member);
         }

         observableMembers.ItemEndEdit += MembersItemEndEdit;
         observableMembers.CollectionChanged += MembersCollectionChanged;
         return observableMembers;
      }

      /// <summary>
      /// Gets an editable assignment collection
      /// </summary>
      /// <returns>
      /// The <see cref="ObservableCollection"/>.
      /// </returns>
      public static ObservableCollection<Assignments> ListAssignments()
      {
         var observableAssignments = new AssignmentUIObjects();
         List<Assignments> assignments = new AssignmentDataOperations().GetAll().ToList();
         foreach(Assignments assignment in assignments)
         {
            observableAssignments.Add(assignment);
         }

         observableAssignments.ItemEndEdit += AssignmentsItemEndEdit;
         observableAssignments.CollectionChanged += AssignmentsCollectionChanged;
         return observableAssignments;
      }

      /// <summary>
      /// Takes username and password from UI, rehashes, and compares with hash in database
      /// If there is a match, then the user is logged in- an additional boolean bit is sent to indicate
      /// if the user is an admin to switch between user and admin dashboards respectively
      /// </summary>
      /// <param name="username">
      /// The username.
      /// </param>
      /// <param name="password">
      /// The password.
      /// </param>
      /// <returns>
      /// The <see cref="bool[]"/>.
      /// </returns>
      public static bool[] Login(string username, SecureString password)
      {
         List<Member> members = GetUsernamesFromDatabase(username);
         if(members.Count > 0)
         {
            foreach(Member member in members)
            {
               if(member.MemberUserName.Equals(username))
               {
                  string plainTextPassword = UnsecurePassword(password);
                  string passwordHash = HashPassword(plainTextPassword, member.Salt);
                  if(passwordHash.Equals(member.PasswordHash))
                  {
                     bool IsAdminUser = member.IsAdminUser;
                     return new[] {true, IsAdminUser};
                  }
               }
            }
         }

         return new[] {false, false};
      }

      /// <summary>
      /// Inserts a member in the database
      /// </summary>
      /// <param name="member">
      /// The member.
      /// </param>
      /// <returns>
      /// The <see cref="bool"/>.
      /// </returns>
      private static bool AddMember(Member member)
      {
         string salt = member.Salt = GenerateSalt();
         string password = member.PlainTextPassword;
         member.PasswordHash = HashPassword(password, salt);
         return new MemberDataOperations().AddRecord(member) != 0;
      }

      /// <summary>
      /// The assignments collection changed handler. Detects if a record is deleted
      /// </summary>
      /// <param name="sender">
      /// The sender.
      /// </param>
      /// <param name="e">
      /// The e.
      /// </param>
      private static void AssignmentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if(e.Action == NotifyCollectionChangedAction.Remove)
         {
            foreach(object item in e.OldItems)
            {
               new AssignmentDataOperations().DeleteRecord((Assignments)item);
            }
         }
      }

      /// <summary>
      /// The assignments item end edit. Detects if item is edited
      /// </summary>
      /// <param name="sender">
      /// The sender.
      /// </param>
      private static void AssignmentsItemEndEdit(IEditableObject sender)
      {
         new AssignmentDataOperations().UpdateRecord((Assignments)sender);
      }

      /// <summary>
      /// Generates salt to hash password before storing
      /// </summary>
      /// <returns>
      /// The <see cref="string"/>.
      /// </returns>
      private static string GenerateSalt()
      {
         byte[] salt;
         using(var saltProvider = new RNGCryptoServiceProvider())
         {
            salt = new byte[256];
            saltProvider.GetBytes(salt);
         }

         return Convert.ToBase64String(salt);
      }

      /// <summary>
      /// Gets all users matched based on their username
      /// </summary>
      /// <param name="username">
      /// The username.
      /// </param>
      /// <returns>
      /// The <see cref="List"/>.
      /// </returns>
      private static List<Member> GetUsernamesFromDatabase(string username)
      {
         var filter = new Filter<Member>();

         filter.Add(member => member.MemberUserName, username);

         List<Member> members = new MemberDataOperations().Find(filter).ToList();
         return members;
      }

      /// <summary>
      /// The assignments collection changed handler. Detects if a record is deleted
      /// </summary>
      /// <param name="sender">
      /// The sender.
      /// </param>
      /// <param name="e">
      /// The e.
      /// </param>
      private static void GiftsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if(e.Action == NotifyCollectionChangedAction.Remove)
         {
            foreach(object item in e.OldItems)
            {
               new GiftDataOperations().DeleteRecord((Gift)item);
            }
         }
      }

      /// <summary>
      /// The gifts item end edit. Detects if item is edited
      /// </summary>
      /// <param name="sender">
      /// The sender.
      /// </param>
      private static void GiftsItemEndEdit(IEditableObject sender)
      {
         new GiftDataOperations().UpdateRecord((Gift)sender);
      }

      /// <summary>
      /// Hashes a plaintext password before storing
      /// </summary>
      /// <param name="plainTextPassword">
      /// The plain text password.
      /// </param>
      /// <param name="saltString">
      /// The salt string.
      /// </param>
      /// <returns>
      /// The <see cref="string"/>.
      /// </returns>
      private static string HashPassword(string plainTextPassword, string saltString)
      {
         byte[] salt = Convert.FromBase64String(saltString);
         using(var hBytes = new Rfc2898DeriveBytes(plainTextPassword, salt))
         {
            hBytes.IterationCount = HashIteration;
            byte[] hash = hBytes.GetBytes(256);
            plainTextPassword = string.Empty;
            return Convert.ToBase64String(hash);
         }
      }

      /// <summary>
      /// The assignments collection changed handler. Detects if a record is deleted
      /// </summary>
      /// <param name="sender">
      /// The sender.
      /// </param>
      /// <param name="e">
      /// The e.
      /// </param>
      private static void MembersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if(e.Action == NotifyCollectionChangedAction.Remove)
         {
            foreach(object item in e.OldItems)
            {
               new MemberDataOperations().DeleteRecord((Member)item);
            }
         }
      }

      /// <summary>
      /// The members item end edit. Detects if item is edited
      /// </summary>
      /// <param name="sender">
      /// The sender.
      /// </param>
      private static void MembersItemEndEdit(IEditableObject sender)
      {
         new MemberDataOperations().UpdateRecord((Member)sender);
      }

      /// <summary>
      /// Best practice way of getting the plaintext password from a SecureString- required from a passwordbox
      /// </summary>
      /// <param name="password">
      /// The password.
      /// </param>
      /// <returns>
      /// The <see cref="string"/>.
      /// </returns>
      private static string UnsecurePassword(SecureString password)
      {
         IntPtr unmanagedString = IntPtr.Zero;
         try
         {
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(password);
            return Marshal.PtrToStringUni(unmanagedString);
         }
         finally
         {
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
         }
      }
   }
}