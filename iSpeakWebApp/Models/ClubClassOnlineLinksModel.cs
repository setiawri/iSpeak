using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class ClubClassOnlineLinksModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Required]
        [Display(Name = "Club Class")]
        public Guid ClubClasses_Id { get; set; }
        public static ModelMember COL_ClubClasses_Id = new ModelMember { Name = "ClubClasses_Id", Display = "Club Class", LogDisplay = ActivityLogsController.editStringFormat("Club Class") };
        public string ClubClasses_Name { get; set; }


        [Required]
        public string Name { get; set; } = String.Empty;
        public static ModelMember COL_Name = new ModelMember { Name = "Name", Display = "Name", LogDisplay = ActivityLogsController.editStringFormat("Name") };


        [Required]
        [Display(Name = "Online Link")]
        public string OnlineLink { get; set; } = String.Empty;
        public static ModelMember COL_OnlineLink = new ModelMember { Name = "OnlineLink", Display = "Online Link", LogDisplay = ActivityLogsController.editStringFormat("Online Link") };


        [Display(Name = "Week")]
        public byte WeekNo { get; set; } = 0;
        public static ModelMember COL_WeekNo = new ModelMember { Name = "WeekNo", Display = "Week No", LogDisplay = ActivityLogsController.editIntFormat("Week No") };


        [Display(Name = "Duration")]
        public byte DurationDays { get; set; } = 7;
        public static ModelMember COL_DurationDays = new ModelMember { Name = "DurationDays", Display = "Duration Days", LogDisplay = ActivityLogsController.editIntFormat("Duration Days") };


        [Display(Name = "Active")]
        public bool Active { get; set; } = true;
        public static ModelMember COL_Active = new ModelMember { Name = "Active", Display = "Active", LogDisplay = ActivityLogsController.editBooleanFormat("Active") };


        public string Notes { get; set; } = null;
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };


        /******************************************************************************************************************************************************/

        public DateTime? DisplayStartDate { get; set; }
        public static ModelMember COL_DisplayStartDate = new ModelMember { Name = "DisplayStartDate" };

        public DateTime? DisplayEndDate { get; set; }
        public static ModelMember COL_DisplayEndDate = new ModelMember { Name = "DisplayEndDate" };
    }
}