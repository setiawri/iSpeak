using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSpeakWebApp.Models
{
    [Table("Settings")]
    public class SettingsModel
    {
        [Key]
        public Guid Id { get; set; }
        public int? Value_Int { get; set; }
        public string Value_String { get; set; }
        public Guid? Value_Guid { get; set; }
        public string Notes { get; set; }
    }
}