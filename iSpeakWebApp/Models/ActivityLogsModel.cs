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


        [DisplayFormat(DataFormatString = "{0:dd/MM/yy HH:mm}")]
        public DateTime Timestamp { get; set; }


        public Guid ReferenceId { get; set; }
        public static ModelMember COL_ReferenceId = new ModelMember { Name = "ReferenceId" };


        public string Description { get; set; }


        [Display(Name = "User")]
        public Guid UserAccounts_Id { get; set; }


        //this column is added to avoid error using DBContext linq to add/update. 
        //In ActivityLogsController.get(), fullname is fetched again using UserAccounts_Id. This field is not used.
        public string UserAccounts_Fullname { get; set; } = null;
    }
}