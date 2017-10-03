// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberUIObjects.cs" company="grantcallant.com">
// Author: Grant Callant  
// </copyright>
// <summary>
//   Defines the MemberUIObjects type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GiftManager.Code
{
   using System.Collections.ObjectModel;
   using System.ComponentModel;

   /// <summary>
   /// The member ui objects.
   /// </summary>
   internal class MemberUIObjects : ObservableCollection<Member>
   {
      public event Member.ItemEndEditEventHandler ItemEndEdit;

      /// <summary>
      /// The insert item.
      /// </summary>
      /// <param name="index">
      /// The index.
      /// </param>
      /// <param name="item">
      /// The item.
      /// </param>
      protected override void InsertItem(int index, Member item)
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