// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssignmentDataOperations.cs" company="grantcallant.com">
//  Author: Grant Callant 
// </copyright>
// <summary>
//   Generic Data class for Assignments
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GiftManager.Code
{
   /// <summary>
   /// Generic Data class for Assignments
   /// </summary>
   public class AssignmentDataOperations : GenericDataOperations<Assignments>
   {
      /// <summary>
      /// Necessary to drop the table in between generations
      /// </summary>
      public void DropAssignmentTable()
      {
         this.DropTable("Assignments");
      }
   }
}