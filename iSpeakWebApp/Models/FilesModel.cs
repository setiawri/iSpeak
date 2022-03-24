using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class FilesModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };

        public string OnlineFileId { get; set; } = null;
        public static ModelMember COL_OnlineFileId = new ModelMember { Name = "OnlineFileId" };


        [Required]
        public Guid ParentId { get; set; } = Guid.NewGuid();
        public static ModelMember COL_ParentId = new ModelMember { Name = "ParentId" };


        [Required]
        public Guid? Branches_Id { get; set; } = null;
        public static ModelMember COL_Branches_Id = new ModelMember { Name = "Branches_Id", LogDisplay = ActivityLogsController.editStringFormat("Branch") };


        [Required]
        [Display(Name = "Id")]
        public string No { get; set; } = String.Empty;
        public static ModelMember COL_No = new ModelMember { Name = "No", Display = "Id", LogDisplay = ActivityLogsController.editStringFormat("No") };


        public string Filename { get; set; } = null;
        public static ModelMember COL_Filename = new ModelMember { Name = "Filename", Display = "Filename", LogDisplay = ActivityLogsController.editStringFormat("Filename") };


        public string DirectoryName { get; set; } = null;
        public static ModelMember COL_DirectoryName = new ModelMember { Name = "DirectoryName", Display = "DirectoryName", LogDisplay = ActivityLogsController.editStringFormat("Directory Name") };


        public string Notes { get; set; } = null;
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };


        [Required]
        [Display(Name = "Created by")]
        public Guid UserAccounts_Id { get; set; }
        public static ModelMember COL_UserAccounts_Id = new ModelMember { Name = "UserAccounts_Id", Display = "Created by" };
        public string UserAccounts_Name { get; set; }
        public string UserAccounts_No { get; set; }


        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yy HH:mm}")]
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public static ModelMember COL_Timestamp = new ModelMember { Name = "Timestamp", Display = "Date", LogDisplay = ActivityLogsController.editDateTimeFormat("Timestamp") };


        [Required]
        public bool IsDeleted { get; set; } = false;
        public static ModelMember COL_IsDeleted = new ModelMember { Name = "IsDeleted", Display = "Deleted", LogDisplay = ActivityLogsController.editBooleanFormat("Deleted") };


        [Required]
        [Display(Name = "Approved")]
        public bool Approved { get; set; } = false;
        public static ModelMember COL_Approved = new ModelMember { Name = "Approved", Display = "Approved", LogDisplay = ActivityLogsController.editBooleanFormat("Approved") };

        /******************************************************************************************************************************************************/

        public bool isDirectory { get; set; } = false;
        public bool parentIsFile { get; set; } = false;
        public bool hasRevisions { get; set; } = false;
    }
}