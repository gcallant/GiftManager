// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Assignments.cs" company="grantcallant.com">
// Author: Grant Callant  
// </copyright>
// <summary>
//   Defines the Assignments type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GiftManager.Code
{
   using System.ComponentModel;

   /// <summary>
   /// The random assignments of members and optionally gifts
   /// </summary>
   public class Assignments : IEditableObject
   {
      /// <summary>
      /// The item end edit event handler.
      /// </summary>
      /// <param name="sender">
      /// The sender.
      /// </param>
      public delegate void ItemEndEditEventHandler(IEditableObject sender);

      /// <summary>
      /// The item end edit.
      /// </summary>
      public event ItemEndEditEventHandler ItemEndEdit;

      /// <summary>
      ///    Gets or sets the assigned gift id. This is the id of the gift this member is assigned to get for their assigned
      ///    member.
      /// </summary>
      [DB]
      public long AssignedGiftId { get; set; }

      /// <summary>
      ///    Gets or sets the assigned member. This is for storing the name.
      /// </summary>
      public string AssignedMember { get; set; }

      /// <summary>
      ///    Gets or sets the assigned member id. This is the id of the member this person is assigned to.
      /// </summary>
      [DB]
      public long AssignedMemberId { get; set; }

      /// <summary>
      ///    Gets or sets the gift description. Optional field.
      /// </summary>
      public string GiftDescription { get; set; }

      /// <summary>
      ///    Gets or sets the gift name.
      /// </summary>
      public string GiftName { get; set; }

      /// <summary>
      ///    Gets or sets the gift price.
      /// </summary>
      public long GiftPrice { get; set; }

      /// <summary>
      ///    Gets or sets the member id for this assignment.
      /// </summary>
      [DB(PrimaryKey = true)]
      public long MemberId { get; set; }

      /// <summary>
      ///    Gets or sets the membername of this person's assignment.
      /// </summary>
      public string Membername { get; set; }

      /// <summary>
      ///    The begin edit.
      /// </summary>
      public void BeginEdit() {}

      /// <summary>
      ///    The cancel edit.
      /// </summary>
      public void CancelEdit() {}

      /// <summary>
      ///    The end edit. Notifies to the observable notifier when this item is done being edited.
      /// </summary>
      public void EndEdit()
      {
         if(this.ItemEndEdit != null)
         {
            this.ItemEndEdit(this);
         }
      }
   }
}