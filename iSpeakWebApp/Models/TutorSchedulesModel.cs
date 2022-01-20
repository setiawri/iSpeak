using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class TutorSchedulesModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Required]
        [Display(Name = "Tutor")]
        public Guid Tutor_UserAccounts_Id { get; set; }
        public static ModelMember COL_Tutor_UserAccounts_Id = new ModelMember { Name = "Tutor_UserAccounts_Id" };
        public string Tutor_UserAccounts_Name { get; set; }


        [Required]
        [Display(Name = "Day")]
        public DayOfWeekEnum DayOfWeek { get; set; }
        public static ModelMember COL_DayOfWeek = new ModelMember { Name = "DayOfWeek", Display = "Day of Week", LogDisplay = ActivityLogsController.editStringFormat("Day of Week") };


        [Required]
        [Display(Name = "Start")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public DateTime StartTime { get; set; }
        public static ModelMember COL_StartTime = new ModelMember { Name = "StartTime", Display = "Start Time", LogDisplay = ActivityLogsController.editTimeFormat("Start Time") };


        [Required]
        [Display(Name = "End")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public DateTime EndTime { get; set; }
        public static ModelMember COL_EndTime = new ModelMember { Name = "EndTime", Display = "End Time", LogDisplay = ActivityLogsController.editTimeFormat("End Time") };


        public bool Active { get; set; }
        public static ModelMember COL_Active = new ModelMember { Name = "Active", Display = "Active", LogDisplay = ActivityLogsController.editBooleanFormat("Active") };


        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };


        /******************************************************************************************************************************************************/

    }
}