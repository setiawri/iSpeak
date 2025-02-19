﻿using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class VouchersModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };

        public bool Active { get; set; }
        public static ModelMember COL_Active = new ModelMember { Name = "Active", Display = "Active", LogDisplay = ActivityLogsController.editBooleanFormat("Active") };

        [Required]
        public string Code { get; set; }
        public static ModelMember COL_Code = new ModelMember { Name = "Code", Display = "Code", LogDisplay = ActivityLogsController.editStringFormat("Code") };

        [Required]
        public string Description { get; set; }
        public static ModelMember COL_Description = new ModelMember { Name = "Description", Display = "Description", LogDisplay = ActivityLogsController.editStringFormat("Description") };

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Amount { get; set; } = 0;
        public static ModelMember COL_Amount = new ModelMember { Name = "Amount", Display = "Amount", LogDisplay = ActivityLogsController.editIntFormat("Amount") };

        [Required]
        [Display(Name = "Franchise")]
        public Guid Franchises_Id { get; set; }
        public static ModelMember COL_Franchises_Id = new ModelMember { Name = "Franchises_Id", Display = "Franchise", LogDisplay = ActivityLogsController.editStringFormat("Franchise") };
        public string Franchises_Name { get; set; } = string.Empty;

        /******************************************************************************************************************************************************/

        public string DDLDescription { get; set; } = string.Empty;
        public static ModelMember COL_DDLDescription = new ModelMember { Name = "DDLDescription" };

    }
}