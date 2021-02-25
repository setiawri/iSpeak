using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSpeakWebApp.Models
{
    [Table("UserAccountRoleAssignments")]
    public class UserAccountRoleAssignmentsModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserAccounts_Id { get; set; }
        public static ModelMember COL_UserAccounts_Id = new ModelMember { Name = "UserAccounts_Id", Display = "", LogDisplay = "" };
        public Guid UserAccountRoles_Id { get; set; }
        public static ModelMember COL_UserAccountRoles_Id = new ModelMember { Name = "UserAccountRoles_Id", Display = "", LogDisplay = "" };

    }
}