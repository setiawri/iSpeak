using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSpeakWebApp.Models
{
    [Table("PettyCashRecordsCategories")]
    public class PettyCashRecordsCategoriesModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id", Display = "Id", LogDisplay = "" };

        public string Name { get; set; }
        public static ModelMember COL_Name = new ModelMember { Name = "Name", Display = "Name", LogDisplay = "Name" };

        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = "Notes" };

        [Display(Name = "Default")]
        public bool Default_row { get; set; }
        public static ModelMember COL_Default_row = new ModelMember { Name = "Default_row", Display = "Default Row", LogDisplay = "Default Row" };

        public bool Active { get; set; }
        public static ModelMember COL_Active = new ModelMember { Name = "Active", Display = "Active", LogDisplay = "Active" };
    }
}