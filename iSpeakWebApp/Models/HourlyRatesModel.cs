using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    [Table("HourlyRates")]
    public class HourlyRatesModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Display(Name = "Branch")]
        public Guid? Branches_Id { get; set; }
        
        
        [Display(Name = "Lesson Package")]
        public Guid? LessonPackages_Id { get; set; }


        public string UserAccounts_Id { get; set; }

        [Required]
        [Display(Name = "User")]
        public Guid UserAccounts_Id_TEMP { get; set; }


        public decimal Rate { get; set; }
        [Display(Name = "Full Time Payrate")]
        public static ModelMember COL_Rate = new ModelMember { Name = "Rate", Display = "Rate", LogDisplay = ActivityLogsController.editIntFormat("Rate") };


        public decimal FullTimeTutorPayrate { get; set; }
        
        
        public string Notes { get; set; }

        /******************************************************************************************************************************************************/
    }
}