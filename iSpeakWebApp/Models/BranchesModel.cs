using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSpeakWebApp.Models
{
    [Table("Branches")]
    public class BranchesModel
    {
        [Key]
		public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

		public string Address { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Invoice Header Text")]
        public string InvoiceHeaderText { get; set; }

        public bool Active { get; set; }

	}
}