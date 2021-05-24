﻿using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class PettyCashRecordsModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        public Guid Branches_Id { get; set; }
        public static ModelMember COL_Branches_Id = new ModelMember { Name = "Branches_Id" };

        public Guid? RefId { get; set; } = null;
        public static ModelMember COL_RefId = new ModelMember { Name = "RefId" };


        public string No { get; set; } = string.Empty;
        public static ModelMember COL_No = new ModelMember { Name = "No" };


        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime Timestamp { get; set; }
        public static ModelMember COL_Timestamp = new ModelMember { Name = "Timestamp" };


        [Display(Name = "Category")]
        public Guid PettyCashRecordsCategories_Id { get; set; }
        public static ModelMember COL_PettyCashRecordsCategories_Id = new ModelMember { Name = "PettyCashRecordsCategories_Id" };
        public string PettyCashRecordsCategories_Name { get; set; }


        [Required]
        [Display(Name = "Description")]
        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes" };


        [Required]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Amount { get; set; } = 0;
        public static ModelMember COL_Amount = new ModelMember { Name = "Amount" };


        public bool IsChecked { get; set; } = false;
        public static ModelMember COL_IsChecked = new ModelMember { Name = "IsChecked", Display = "IsChecked", LogDisplay = ActivityLogsController.editBooleanFormat("Approval") };


        public string UserAccounts_Id { get; set; }


        [Display(Name = "Name")]
        public Guid? UserAccounts_Id_TEMP { get; set; }
        public static ModelMember COL_UserAccounts_Id_TEMP = new ModelMember { Name = "UserAccounts_Id_TEMP" };
        public string UserAccounts_Firstname { get; set; }


        [Display(Name = "Expense Category")]
        public Guid? ExpenseCategories_Id { get; set; } = null;
        public static ModelMember COL_ExpenseCategories_Id = new ModelMember { Name = "ExpenseCategories_Id" };

        /******************************************************************************************************************************************************/

        [Display(Name = "Balance")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Balance { get; set; } = 0;

    }
}