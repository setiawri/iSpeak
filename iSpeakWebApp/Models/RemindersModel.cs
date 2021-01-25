using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSpeakWebApp.Models
{
    [Table("Reminders")]
    public class RemindersModel
    {
        [Key]
        public Guid Id { get; set; }

        public Guid Branches_Id { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Timestamp { get; set; }

        [Required]
        public string Description { get; set; }

        public EnumReminderStatuses Status_enumid { get; set; }

    }
}