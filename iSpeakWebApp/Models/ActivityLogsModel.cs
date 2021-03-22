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

        //this column is added to avoid error using DBContext linq to add/update. 
        //In ActivityLogsController.get(), fullname is fetched again using UserAccounts_Id. This field is not used.
        public string UserAccounts_Fullname { get; set; } = null;
    }
}