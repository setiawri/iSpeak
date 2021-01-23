using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSpeakWebApp.Models
{
    [Table("UserAccounts")]
    public class UserAccountsModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string Fullname { get; set; }
        public DateTime Birthday { get; set; }
        public Guid Default_Branches_Id { get; set; }
        public bool Active { get; set; }
        public bool ResetPassword { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Notes { get; set; }
        public string Interest { get; set; }
        public Guid? PromotionEvents_Id { get; set; }

    }
}