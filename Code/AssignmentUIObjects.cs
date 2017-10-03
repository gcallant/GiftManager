// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssignmentUIObjects.cs" company="grantcallant.com">
// Author: Grant Callant  
// </copyright>
// <summary>
//   Defines the AssignmentUIObjects type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GiftManager.Code
{
   using System.Collections.ObjectModel;
   using System.ComponentModel;

   /// <summary>
   /// The assignment objects. Allows observing changes in the assignments collection
   /// </summary>
   internal class AssignmentUIObjects : ObservableCollection<Assignments>
   {
      /// <summary>
      /// The item end edit.
      /// </summary>
      public event Assignments.ItemEndEditEventHandler ItemEndEdit;

      /// <summary>
      /// The insert item.
      /// </summary>
      /// <param name="index">
      /// The index.
      /// </param>
      /// <param name="item">
      /// The item.
      /// </param>
      protected override void InsertItem(int index, Assignments item)
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