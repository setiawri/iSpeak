using iSpeakWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSpeakWebApp.Models
{
    public class ActivityLogsModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [DisplayFormat(DataFormatString = "{0:dd/MM/yy HH:mm}")]
        public DateTime Timestamp { get; set; }
        public static ModelMember COL_Timestamp = new ModelMember { Name = "Timestamp", Display = "Timestamp", LogDisplay = ActivityLogsController.editDateTimeFormat("Timestamp") };


        public Guid ReferenceId { get; set; }
        public static ModelMember COL_ReferenceId = new ModelMember { Name = "ReferenceId", Display = "Reference Id", LogDisplay = ActivityLogsController.editStringFormat("Reference Id") };


        public string Description { get; set; }
        public static ModelMember COL_Description = new ModelMember { Name = "Description", Display = "Description", LogDisplay = ActivityLogsController.editStringFormat("Description") };


        [Display(Name = "User")]
        public Guid UserAccounts_Id { get; set; }
        public static ModelMember COL_UserAccounts_Id = new ModelMember { Name = "UserAccounts_Id", Display = "User", LogDisplay = ActivityLogsController.editStringFormat("User") };
        public string UserAccounts_Fullname { get; set; } = null;
        public static ModelMember COL_UserAccounts_Fullname = new ModelMember { Name = "UserAccounts_Fullname", Display = "User Full Name", LogDisplay = ActivityLogsController.editStringFormat("User Full Name") };


        /******************************************************************************************************************************************************/


        [Display(Name = "Roles")]
        public string Roles { get; set; }
        public List<string> Roles_List { get; set; }

    }
}