// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Member.cs" company="grantcallant.com">
// Author: Grant Callant  
// </copyright>
// <summary>
//   Defines the Member type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GiftManager.Code
{
   using System;
   using System.ComponentModel;
   using System.Linq;
   using System.Text.RegularExpressions;

   /// <summary>
   /// The member for this Gift Manager
   /// </summary>
   public class Member : IDataErrorInfo, INotifyPropertyChanged, IEditableObject
   {
      /// <summary>
      /// The member name.
      /// </summary>
      private string memberName;

      /// <summary>
      /// The member user name.
      /// </summary>
      private string memberUserName;

      /// <summary>
      /// The plain text password.
      /// </summary>
      private string plainTextPassword;

      /// <summary>
      /// The property changed event.
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged;

      /// <summary>
      /// The item end edit event handler.
      /// </summary>
      /// <param name="sender">
      /// The sender.
      /// </param>
      public delegate void ItemEndEditEventHandler(IEditableObject sender);

      /// <summary>
      /// The item end edit event.
      /// </summary>
      public event ItemEndEditEventHandler ItemEndEdit;

      /// <summary>
      /// The error. Required field of IDataErrorInfo
      /// </summary>
      public string Error => null;

      /// <summary>
      /// Gets or sets the member name.
      /// </summary>
      [DB]
      public string MemberName
      {
         get
         {
            return this.memberName;
         }

         set
         {
            this.memberName = value;
            this.OnPropertyChanged("MemberName");
         }
      }

      /// <summary>
      /// Gets or sets the member id.
      /// </summary>
      [DB(PrimaryKey = true)]
      public long MemberId { get; set; } = GenerateMemberID();

      /// <summary>
      /// Gets or sets the member user name.
      /// </summary>
      [DB]
      public string MemberUserName
      {
         get
         {
            return this.memberUserName;
         }
         set
         {
            this.memberUserName = value;
            this.OnPropertyChanged("MemberUserName");
         }
      }

      /// <summary>
      ///    Gets or sets a user's plaintext password and listens for changes
      /// </summary>
      public string PlainTextPassword
      {
         get
         {
            return this.plainTextPassword;
         }
         set
         {
            this.plainTextPassword = value;
            this.OnPropertyChanged("PlainTextPassword");
         }
      }

      /// <summary>
      ///    Gets or sets the admin user flag
      /// </summary>
      [DB]
      public bool IsAdminUser { get; set; } = false;

      /// <summary>
      /// Gets or sets the password hash.
      /// </summary>
      [DB]
      public string PasswordHash { get; set; }

      /// <summary>
      /// Gets or sets the salt.
      /// </summary>
      [DB]
      public string Salt { get; set; }

      /// <summary>
      /// The on property changed.
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      private void OnPropertyChanged(string name)
      {
         PropertyChangedEventHandler handler = this.PropertyChanged;
         if(handler != null)
         {
            handler(this, new PropertyChangedEventArgs(name));
         }
      }

      /// <summary>
      /// Generates the primary key for member
      /// </summary>
      /// <returns>
      /// The <see cref="long"/>.
      /// </returns>
      public static long GenerateMemberID()
      {
         var random = new Random();
         return Convert.ToInt64(random.Next(1, int.MaxValue));
      }

      /// <summary>
      /// Enforces each username to be unique by checking if username exists in database
      /// </summary>
      /// <param name="username">
      /// The username.
      /// </param>
      /// <returns>
      /// The <see cref="bool"/>.
      /// </returns>
      private static bool IsUniqueUsername(string username)
      {
         var filter = new Filter<Member>();
         filter.Add(member => member.MemberUserName, username);

         return new MemberDataOperations().Find(filter).ToList().Count == 0;
      }

      /// <summary>
      /// Member indexer provides feedback and error messages to UI
      /// </summary>
      /// <param name="columnName">
      /// The column name.
      /// </param>
      /// <returns>
      /// The <see cref="string"/>.
      /// </returns>
      public string this[string columnName]
      {
         get
         {
            if(columnName == "MemberName")
            {
               if(string.IsNullOrEmpty(this.MemberName))
               {
                  return "Valid name is required";
               }

               if(!IsValidName(this.MemberName))
               {
                  return "Only Alphanumeric Characters are allowed";
               }
            }
            else if(columnName == "MemberUserName")
            {
               if(string.IsNullOrEmpty(this.MemberUserName))
               {
                  return "Valid username is required";
               }
               bool[] usernameInfo = IsValidUsername(this.MemberUserName);
               bool valid = usernameInfo[0];
               bool unique = usernameInfo[1];
               if(!valid)
               {
                  return
                     "Valid usernames must start with a letter or a number, and may contain only 1 dash or underscore in the middle"
                     + "\nValid Examples: Sally_Struthers08" + "\n08-2859sally" + "\nsstruthers";
               }
               if(!unique)
               {
                  return $"Sorry, {this.MemberUserName} is already chosen. Please select a different username";
               }
            }
            else if(columnName == "PlainTextPassword")
            {
               if(string.IsNullOrEmpty(this.PlainTextPassword))
               {
                  return
                     "Password requires at least 8 characters: 1 uppercase, 1 lowercase, 1 digit, and one special character";
               }
               if(!IsValidPassword(this.PlainTextPassword))
               {
                  return "Valid password contains at least 8 characters, contained of at least:" + "\n1 Uppercase"
                         + "\n1 lowercase" + "\n1 digit" + "\n1 special character (#?!@$%^&*-)";
               }
            }
            return null;
         }
      }

      /// <summary>
      /// Checks for valid password, between 8-72 characters, 1 upper, 1 lower, 1 number and 1 special
      /// </summary>
      /// <param name="password">
      /// The password.
      /// </param>
      /// <returns>
      /// The <see cref="bool"/>.
      /// </returns>
      public static bool IsValidPassword(string password)
      {
         var regex = new Regex(@"^(?=[ -~]*?[A-Z])(?=[ -~]*?[a-z])(?=[ -~]*?[0-9])(?=[ -~]*?[#?!@$%^&*-])[ -~]{8,72}$");
         if(regex.IsMatch(password))
         {
            return true;
         }
         return false;
      }

      /// <summary>
      /// Checks for valid name. Letters, dashes, apostrophes and periods (in between letters)
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <returns>
      /// The <see cref="bool"/>.
      /// </returns>
      public static bool IsValidName(string name)
      {
         var regex = new Regex(@"^[\p{L}\s'.-]+$");
         if(regex.IsMatch(name))
         {
            return true;
         }
         return false;
      }

      /// <summary>
      /// Checks if username is valid, letters and numbers with one dash or underscore followed by letters and numbers. 
      /// Also checks for username uniqueness. Non-uniqueness is not valid.
      /// </summary>
      /// <param name="username">
      /// The username.
      /// </param>
      /// <returns>
      /// The <see cref="bool[]"/>.
      /// </returns>
      public static bool[] IsValidUsername(string username)
      {
         var regex = new Regex(@"^[a-zA-Z0-9]+([_ -]?[a-zA-Z0-9])*$");
         if(regex.IsMatch(username))
         {
            return new[] {true, IsUniqueUsername(username)};
         }
         return new[] {false, false};
      }

      /// <summary>
      /// The begin edit.
      /// </summary>
      public void BeginEdit() {}

      /// <summary>
      /// The end edit.
      /// </summary>
      public void EndEdit()
      {
         if(this.ItemEndEdit != null)
         {
            this.ItemEndEdit(this);
         }
      }

      /// <summary>
      /// The cancel edit.
      /// </summary>
      public void CancelEdit() {}
   }
}