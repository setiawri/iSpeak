using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    [Table("Reminders")]
    public class RemindersModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };

        public Guid Branches_Id { get; set; }
        public static ModelMember COL_Branches_Id = new ModelMember { Name = "Branches_Id", Display = "Branch", LogDisplay = ActivityLogsController.editStringFormat("Branch") };

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yy}")]
        public DateTime Timestamp { get; set; }
        public static ModelMember COL_Timestamp = new ModelMember { Name = "Timestamp", Display = "Date", LogDisplay = ActivityLogsController.editDateFormat("Date") };

        [Required]
        public string Description { get; set; }
        public static ModelMember COL_Description = new ModelMember { Name = "Description", Display = "Description", LogDisplay = ActivityLogsController.editStringFormat("Description") };

        [Display(Name = "Status")]
        public EnumReminderStatuses Status_enumid { get; set; }
        public static ModelMember COL_Status_enumid = new ModelMember { Name = "Status_enumid", Display = "Status", LogDisplay = ActivityLogsController.editStringFormat("Status") };

    }
}