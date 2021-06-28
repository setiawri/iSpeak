using System;
using System.ComponentModel.DataAnnotations;

namespace iSpeakWebApp.Models
{
    public class PayrollPaymentItemsModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        public Guid? PayrollPayments_Id { get; set; } = null;
        public static ModelMember COL_PayrollPayments_Id = new ModelMember { Name = "PayrollPayments_Id" };


        [DisplayFormat(DataFormatString = "{0:dd/MM/yy HH:mm}")]
        public DateTime? Timestamp { get; set; }
        public static ModelMember COL_Timestamp = new ModelMember { Name = "Timestamp" };


        public string Description { get; set; }
        public static ModelMember COL_Description = new ModelMember { Name = "Description" };


        public decimal Hour { get; set; }
        public static ModelMember COL_Hour = new ModelMember { Name = "Hour" };


        public decimal HourlyRate { get; set; }
        public static ModelMember COL_HourlyRate = new ModelMember { Name = "HourlyRate" };


        public int TutorTravelCost { get; set; }
        public static ModelMember COL_TutorTravelCost = new ModelMember { Name = "TutorTravelCost" };


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Amount { get; set; }
        public static ModelMember COL_Amount = new ModelMember { Name = "Amount" };


        public Guid UserAccounts_Id { get; set; }
        public static ModelMember COL_UserAccounts_Id = new ModelMember { Name = "UserAccounts_Id" };
        public string UserAccounts_Fullname { get; set; }


        public string CancelNotes { get; set; } = string.Empty;
        public static ModelMember COL_CancelNotes = new ModelMember { Name = "CancelNotes" };


        public Guid? Branches_Id { get; set; }
        public static ModelMember COL_Branches_Id = new ModelMember { Name = "Branches_Id" };


        public bool IsFullTime { get; set; } = false;
        public static ModelMember COL_IsFullTime = new ModelMember { Name = "IsFullTime" };


        /******************************************************************************************************************************************************/

        public string Student_UserAccounts_FirstName { get; set; }
        public string Student_UserAccounts_FullName { get; set; }

        public long InitialRowNumber { get; set; } = 0;
    }

}