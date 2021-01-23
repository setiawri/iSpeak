﻿using System;
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

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Timestamp { get; set; }

        [Required]
        public string Description { get; set; }

        public EnumReminderStatuses Status_enumid { get; set; }
    }
}