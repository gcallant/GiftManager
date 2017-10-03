// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GiftUIObjects.cs" company="grantcallant.com">
// Author: Grant Callant  
// </copyright>
// <summary>
//   Defines the GiftUIObjects type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GiftManager.Code
{
   using System.Collections.ObjectModel;
   using System.ComponentModel;

   /// <summary>
   /// The gift ui objects.
   /// </summary>
   internal class GiftUIObjects : ObservableCollection<Gift>
   {
      /// <summary>
      /// The item end edit.
      /// </summary>
      public event Gift.ItemEndEditEventHandler ItemEndEdit;

      /// <summary>
      /// The insert item.
      /// </summary>
      /// <param name="index">
      /// The index.
      /// </param>
      /// <param name="item">
      /// The item.
      /// </param>
      protected override void InsertItem(int index, Gift item)
      {
         base.InsertItem(index, item);
         item.ItemEndEdit += this.ItemEndEditHandler;
      }

      /// <summary>
      /// The item end edit handler.
      /// </summary>
      /// <param name="sender">
      /// The sender.
      /// </param>
      private void ItemEndEditHandler(IEditableObject sender)
      {
         if(this.ItemEndEdit != null)
         {
            this.ItemEndEdit(sender);
         }
      }
   }
}