﻿using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class PayrollsModel
    {
        [Display(Name = "Employee")]
        public Guid Tutor_UserAccounts_Id { get; set; }
        public static ModelMember COL_Tutor_UserAccounts_Id = new ModelMember { Name = "Tutor_UserAccounts_Id", Display = "Employee", LogDisplay = ActivityLogsController.editStringFormat("Employee") };
        public string Tutor_UserAccounts_FullName { get; set; }


        [Display(Name = "Hours")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal TotalHours { get; set; }

        [Display(Name = "Payable")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal PayableAmount { get; set; }

        [Display(Name = "Due")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal DueAmount { get; set; }
    }
}