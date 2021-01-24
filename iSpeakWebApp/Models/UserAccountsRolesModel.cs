using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSpeakWebApp.Models
{
    [Table("UserAccountRoles")]
    public class UserAccountRolesModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id", Display = "" };

        public string Name { get; set; }
        public static ModelMember COL_Name = new ModelMember { Name = "Name", Display = "" };

        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "" };

    }
}