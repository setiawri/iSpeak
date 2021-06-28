using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class HourlyRatesModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Display(Name = "Branch")]
        public Guid? Branches_Id { get; set; }
        public static ModelMember COL_Branches_Id = new ModelMember { Name = "Branches_Id", Display = "Branch", LogDisplay = ActivityLogsController.editStringFormat("Branch") };
        public string Branches_Name { get; set; }


        [Display(Name = "Lesson Package")]
        public Guid? LessonPackages_Id { get; set; }
        public static ModelMember COL_LessonPackages_Id = new ModelMember { Name = "LessonPackages_Id", Display = "Lesson Package", LogDisplay = ActivityLogsController.editStringFormat("Lesson Package") };
        public string LessonPackages_Name { get; set; }


        [Required]
        [Display(Name = "Employee")]
        public Guid UserAccounts_Id { get; set; }
        public static ModelMember COL_UserAccounts_Id = new ModelMember { Name = "UserAccounts_Id", Display = "Employee", LogDisplay = ActivityLogsController.editStringFormat("Employee") };
        public string UserAccounts_Fullname { get; set; }


        [Display(Name = "Hourly Rate")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Rate { get; set; }
        [Display(Name = "Full Time Payrate")]
        public static ModelMember COL_Rate = new ModelMember { Name = "Rate", Display = "Rate", LogDisplay = ActivityLogsController.editIntFormat("Rate") };


        [Display(Name = "Fulltime Rate")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal FullTimeTutorPayrate { get; set; }
        public static ModelMember COL_FullTimeTutorPayrate = new ModelMember { Name = "FullTimeTutorPayrate", Display = "Full Time Payrate", LogDisplay = ActivityLogsController.editDecimalFormat("Full Time Payrate") };


        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };

        /******************************************************************************************************************************************************/
    }
}