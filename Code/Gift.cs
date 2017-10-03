// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Gift.cs" company="grantcallant.com">
// Author: Grant Callant  
// </copyright>
// <summary>
//   Defines the Gift type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GiftManager.Code
{
   using System;
   using System.ComponentModel;
   using System.Text.RegularExpressions;

   /// <summary>
   /// The gift.
   /// </summary>
   public class Gift : IDataErrorInfo, INotifyPropertyChanged, IEditableObject
   {
      /// <summary>
      /// The optional gift description.
      /// </summary>
      private string giftDescription;

      /// <summary>
      /// The gift name.
      /// </summary>
      private string giftName;

      /// <summary>
      /// The gift price.
      /// </summary>
      private long giftPrice;

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
      /// The property changed event.
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged;

      /// <summary>
      /// The error, required by IDataErrorInfo.
      /// </summary>
      public string Error => null;

      /// <summary>
      /// Gets or sets the gift description. Also implements OnPropertyChanged
      /// </summary>
      [DB]
      public string GiftDescription
      {
         get
         {
            return this.giftDescription;
         }

         set
         {
            this.giftDescription = value;
            this.OnPropertyChanged("GiftDescription");
         }
      }

      /// <summary>
      /// Gets or sets the gift id.
      /// </summary>
      [DB(PrimaryKey = true)]
      public long GiftId { get; set; } = GenerateGiftID();

      /// <summary>
      /// Gets or sets the gift name.
      /// </summary>
      [DB]
      public string GiftName
      {
         get
         {
            return this.giftName;
         }

         set
         {
            this.giftName = value;
            this.OnPropertyChanged("GiftName");
         }
      }

      /// <summary>
      /// Gets or sets the gift price.
      /// </summary>
      [DB]
      public long GiftPrice
      {
         get
         {
            return this.giftPrice;
         }

         set
         {
            this.giftPrice = value;
            this.OnPropertyChanged("GiftPrice");
         }
      }

      /// <summary>
      /// Indexer for Gift, required by INotifyPropertyChanged
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
            if(columnName == "GiftName")
            {
               if(string.IsNullOrEmpty(this.GiftName))
               {
                  return "Valid name is required";
               }

               if(!IsValidName(this.GiftName))
               {
                  return "Gift names should have only letters, numbers, dashes and underscores in between characters";
               }
            }
            else if(columnName == "GiftPrice")
            {
               string error = CatchIntParseOverflow(this.GiftPrice.ToString());
               if(!string.IsNullOrEmpty(error))
               {
                  return error;
               }

               if(!IsValidPrice(this.GiftPrice.ToString()))
               {
                  return "Price should be in US dollars (no $ sign) optional commas (all or none) and optional cents"
                         + "\nExamples:" + "\n1234.34" + "\n123434" + "\n1,234.34" + "\n1,23434";
               }
            }

            return null;
         }
      }

      /// <summary>
      ///    //Verify long value does not overflow or underflow
      /// </summary>
      /// <param name="priceString">The dirty string containing the price to convert to a long</param>
      /// <returns>An error message string if number is less than 0, or an overflow occurs, null otherwise</returns>
      public static string CatchIntParseOverflow(string priceString)
      {
         try
         {
            long price = long.Parse(priceString);
            if(price < 0)
            {
               return $"Price should be between 0 (free) and {long.MaxValue}";
            }
         }
         catch(OverflowException)
         {
            return $"Price should be between 0 (free) and {long.MaxValue}";
         }

         return null;
      }

      /// <summary>
      /// Generates the primary key of each gift
      /// </summary>
      /// <returns>
      /// The <see cref="long"/>.
      /// </returns>
      public static long GenerateGiftID()
      {
         var random = new Random();
         return random.Next(1, int.MaxValue);
      }

      /// <summary>
      /// Checks for valid gift name, regex allows for letters and numbers at the beginning and one special underscore, 
      /// apostrophe, period or dash in between another sequence of letters or numbers
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <returns>
      /// The <see cref="bool"/>.
      /// </returns>
      public static bool IsValidName(string name)
      {
         var regex = new Regex(@"^[a-zA-Z0-9]+([_\s'.-]?[a-zA-Z0-9])*$");
         if(regex.IsMatch(name))
         {
            return true;
         }

         return false;
      }

      /// <summary>
      /// Checks for valid price, only allows positive numbers, optional commas (all or none), and two (optional) digits
      /// </summary>
      /// <param name="price">
      /// The price.
      /// </param>
      /// <returns>
      /// The <see cref="bool"/>.
      /// </returns>
      public static bool IsValidPrice(string price)
      {
         var regex = new Regex(@"^[+]?[0-9]{1,3}(?:(,[0-9]{3})*|([0-9]{3})*)(?:\.[0-9]{2})?$");
         if(regex.IsMatch(price))
         {
            return true;
         }

         return false;
      }

      /// <summary>
      /// The begin edit.
      /// </summary>
      public void BeginEdit() {}

      /// <summary>
      /// The cancel edit.
      /// </summary>
      public void CancelEdit() {}

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
      /// The on property changed method handler
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
   }
}