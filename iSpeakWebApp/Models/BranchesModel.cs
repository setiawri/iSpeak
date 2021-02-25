﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    [Table("Branches")]
    public class BranchesModel
    {
        [Key]
		public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };

        [Required]
        public string Name { get; set; }
        public static ModelMember COL_Name = new ModelMember { Name = "Name", Display = "Name", LogDisplay = ActivityLogsController.editStringFormat("Name") };

        [Required]
        public string Address { get; set; }
        public static ModelMember COL_Address = new ModelMember { Name = "Address", Display = "Address", LogDisplay = ActivityLogsController.editStringFormat("Address") };

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public static ModelMember COL_PhoneNumber = new ModelMember { Name = "PhoneNumber", Display = "Phone Number", LogDisplay = ActivityLogsController.editStringFormat("Phone Number") };

        [Display(Name = "Invoice Header")]
        public string InvoiceHeaderText { get; set; }
        public static ModelMember COL_InvoiceHeaderText = new ModelMember { Name = "InvoiceHeaderText", Display = "Invoice Header Text", LogDisplay = ActivityLogsController.editStringFormat("Invoice Header Text") };

        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };

        public bool Active { get; set; }
        public static ModelMember COL_Active = new ModelMember { Name = "Active", Display = "Active", LogDisplay = ActivityLogsController.editBooleanFormat("Active") };

    }
}