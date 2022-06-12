using System;
using System.ComponentModel.DataAnnotations;

namespace iSpeakWebApp.Models
{
    public class IncomeStatementModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [DisplayFormat(DataFormatString = "{0:MM/yy}")]
        public string MonthYear { get; set; } = String.Empty;
        public static ModelMember COL_MonthYear = new ModelMember { Name = "MonthYear" };


        public int Month { get; set; } = 0;
        public static ModelMember COL_Month = new ModelMember { Name = "Month" };


        public string Year { get; set; } = String.Empty;
        public static ModelMember COL_Year = new ModelMember { Name = "Year" };


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long Revenue { get; set; } = 0;
        public static ModelMember COL_Revenue = new ModelMember { Name = "Revenue" };


        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Expenses { get; set; } = 0;
        public static ModelMember COL_Expenses = new ModelMember { Name = "Expenses" };


        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Profit { get; set; } = 0;
        public static ModelMember COL_Profit = new ModelMember { Name = "Profit" };


        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ProfitPercent { get; set; } = 0;
        public static ModelMember COL_ProfitPercent = new ModelMember { Name = "ProfitPercent" };

        /******************************************************************************************************************************************************/

    }

}