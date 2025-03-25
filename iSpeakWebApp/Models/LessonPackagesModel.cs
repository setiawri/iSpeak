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
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal SessionHours { get; set; } = 0;
        public static ModelMember COL_SessionHours = new ModelMember { Name = "SessionHours", Display = "Hours", LogDisplay = ActivityLogsController.editDecimalFormat("Session Hours") };


        [Required]
        [Display(Name = "Month Expire")]
        public byte ExpirationMonth { get; set; } = 0;
        public static ModelMember COL_ExpirationMonth = new ModelMember { Name = "ExpirationMonth", Display = "Month Expire", LogDisplay = ActivityLogsController.editIntFormat("Expiration Month") };


        [Required]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Price { get; set; } = 0;
        public static ModelMember COL_Price = new ModelMember { Name = "Price", Display = "Price", LogDisplay = ActivityLogsController.editIntFormat("Price") };


        [Display(Name = "Club Subscription")]
        public bool IsClubSubscription { get; set; } = false;
        public static ModelMember COL_IsClubSubscription = new ModelMember { Name = "IsClubSubscription", Display = "Club", LogDisplay = ActivityLogsController.editBooleanFormat("Club Subscription") };


        [Display(Name = "Franchise")]
        public Guid Franchises_Id { get; set; }
        public static ModelMember COL_Franchises_Id = new ModelMember { Name = "Franchises_Id", Display = "Franchise", LogDisplay = ActivityLogsController.editStringFormat("Franchise") };
        public string Franchises_Name { get; set; } = string.Empty;

        /******************************************************************************************************************************************************/

        public string DDLDescription { get; set; } = "";
        public static ModelMember COL_DDLDescription = new ModelMember { Name = "DDLDescription" };
    }
}