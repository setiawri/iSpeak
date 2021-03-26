using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class SaleInvoiceItemsModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };


        [Required]
        [Display(Name = "No")]
        public int RowNo { get; set; } = 0;
        public static ModelMember COL_RowNo = new ModelMember { Name = "RowNo", Display = "No", LogDisplay = ActivityLogsController.editIntFormat("Row No") };


        [Required]
        [Display(Name = "Invoice")]
        public Guid SaleInvoices_Id { get; set; }
        public static ModelMember COL_SaleInvoices_Id = new ModelMember { Name = "SaleInvoices_Id", Display = "Invoice", LogDisplay = ActivityLogsController.editStringFormat("Sale Invoice No") };
        public string SaleInvoices_No { get; set; }


        [Required]
        public string Description { get; set; }
        public static ModelMember COL_Description = new ModelMember { Name = "Description", Display = "Description", LogDisplay = ActivityLogsController.editStringFormat("Description") };


        [Required]
        [Display(Name = "Qty")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Qty { get; set; } = 0;
        public static ModelMember COL_Qty = new ModelMember { Name = "Qty", Display = "Qty", LogDisplay = ActivityLogsController.editIntFormat("Qty") };


        [Required]
        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Price { get; set; } = 0;
        public static ModelMember COL_Price = new ModelMember { Name = "Price", Display = "Price", LogDisplay = ActivityLogsController.editIntFormat("Price") };


        [Required]
        [Display(Name = "Discount")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int DiscountAmount { get; set; } = 0;
        public static ModelMember COL_DiscountAmount = new ModelMember { Name = "DiscountAmount", Display = "Discount", LogDisplay = ActivityLogsController.editIntFormat("Discount") };


        [Display(Name = "Vouchers")]
        public string Vouchers { get; set; }
        public static ModelMember COL_Vouchers = new ModelMember { Name = "Vouchers", Display = "Vouchers", LogDisplay = ActivityLogsController.editStringFormat("Vouchers") };


        [Display(Name = "Vouchers")]
        public string VouchersName { get; set; }
        public static ModelMember COL_VouchersName = new ModelMember { Name = "VouchersName", Display = "Vouchers", LogDisplay = ActivityLogsController.editStringFormat("Vouchers") };


        [Display(Name = "Vouchers Amount")]
        public int VouchersAmount { get; set; }
        public static ModelMember COL_VouchersAmount = new ModelMember { Name = "VouchersAmount", Display = "Vouchers Amount", LogDisplay = ActivityLogsController.editStringFormat("Vouchers Amount") };


        [Display(Name = "Product")]
        public Guid? Products_Id { get; set; }
        public static ModelMember COL_Products_Id = new ModelMember { Name = "Products_Id", Display = "Product", LogDisplay = ActivityLogsController.editStringFormat("Product") };
        public string Products_Name { get; set; }


        [Display(Name = "Service")]
        public Guid? Services_Id { get; set; }
        public static ModelMember COL_Services_Id = new ModelMember { Name = "Services_Id", Display = "Service", LogDisplay = ActivityLogsController.editStringFormat("Service") };
        public string Services_Name { get; set; }


        [Display(Name = "Lesson Package")]
        public Guid? LessonPackages_Id { get; set; }
        public static ModelMember COL_LessonPackages_Id = new ModelMember { Name = "LessonPackages_Id", Display = "Lesson Package", LogDisplay = ActivityLogsController.editStringFormat("Lesson Package") };
        public string LessonPackages_Name { get; set; }


        [Display(Name = "Hours")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal SessionHours { get; set; } = 0;
        public static ModelMember COL_SessionHours = new ModelMember { Name = "SessionHours", Display = "Hours", LogDisplay = ActivityLogsController.editDecimalFormat("Hours") };


        [Display(Name = "Remaining")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal SessionHours_Remaining { get; set; } = 0;
        public static ModelMember COL_SessionHours_Remaining = new ModelMember { Name = "SessionHours_Remaining", Display = "Remaining", LogDisplay = ActivityLogsController.editDecimalFormat("Remaining") };


        [Required]
        [Display(Name = "TravelCost")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TravelCost { get; set; } = 0;
        public static ModelMember COL_TravelCost = new ModelMember { Name = "TravelCost", Display = "Travel", LogDisplay = ActivityLogsController.editIntFormat("Travel Cost") };


        [Required]
        [Display(Name = "TutorTravelCost")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TutorTravelCost { get; set; } = 0;
        public static ModelMember COL_TutorTravelCost = new ModelMember { Name = "TutorTravelCost", Display = "Tutor Travel", LogDisplay = ActivityLogsController.editIntFormat("Tutor Travel Cost") };


        /******************************************************************************************************************************************************/

    }
}