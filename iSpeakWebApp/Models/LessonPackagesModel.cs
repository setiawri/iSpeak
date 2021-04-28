using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class LessonPackagesModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Required]
        public string Name { get; set; }
        public static ModelMember COL_Name = new ModelMember { Name = "Name", Display = "Name", LogDisplay = ActivityLogsController.editStringFormat("Name") };


        [Required]
        public bool Active { get; set; } = true;
        public static ModelMember COL_Active = new ModelMember { Name = "Active", Display = "Active", LogDisplay = ActivityLogsController.editBooleanFormat("Active") };


        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };


        [Required]
        [Display(Name = "Language")]
        public Guid Languages_Id { get; set; }
        public static ModelMember COL_Languages_Id = new ModelMember { Name = "Languages_Id", Display = "Language", LogDisplay = ActivityLogsController.editStringFormat("Language") };
        public string Languages_Name { get; set; }


        [Required]
        [Display(Name = "Lesson Type")]
        public Guid LessonTypes_Id { get; set; }
        public static ModelMember COL_LessonTypes_Id = new ModelMember { Name = "LessonTypes_Id", Display = "Lesson Type", LogDisplay = ActivityLogsController.editStringFormat("Lesson Type") };
        public string LessonTypes_Name { get; set; }


        [Required]
        [Display(Name = "Hours")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal SessionHours { get; set; } = 0;
        public static ModelMember COL_SessionHours = new ModelMember { Name = "SessionHours", Display = "Hours", LogDisplay = ActivityLogsController.editDecimalFormat("Session Hours") };


        [Required]
        [Display(Name = "Expire")]
        public int ExpirationDay { get; set; } = 0;
        public static ModelMember COL_ExpirationDay = new ModelMember { Name = "ExpirationDay", Display = "Expire", LogDisplay = ActivityLogsController.editIntFormat("Expiration") };


        [Required]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Price { get; set; } = 0;
        public static ModelMember COL_Price = new ModelMember { Name = "Price", Display = "Price", LogDisplay = ActivityLogsController.editIntFormat("Price") };


        /******************************************************************************************************************************************************/

        public string DDLDescription { get; set; } = "";
        public static ModelMember COL_DDLDescription = new ModelMember { Name = "DDLDescription" };
    }
}