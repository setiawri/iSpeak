using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSpeakWebApp.Models
{
    [Table("ActivityLogs")]
    public class ActivityLogsModel
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime Timestamp { get; set; }

        public Guid ReffId { get; set; }
        public static ModelMember COL_ReffId = new ModelMember { Name = "ReffId" };

        public string Description { get; set; }

        public Guid UserAccounts_Id { get; set; }
    }
}