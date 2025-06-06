﻿using System.Data.Entity;
using iSpeakWebApp.Models;

namespace iSpeakWebApp
{
    public class DBContext : DbContext
    {
        public DBContext() { this.Database.Connection.ConnectionString = Helper.ConnectionString; }

        /******************************************************************************************************************************************************/

        /* USER ACCOUNTS **************************************************************************************************************************************/

        public DbSet<UserAccountRolesModel> UserAccountRoles { get; set; }

        /******************************************************************************************************************************************************/

        public DbSet<FranchisesModel> Franchises { get; set; }
        public DbSet<RemindersModel> Reminders { get; set; }
        public DbSet<PettyCashRecordsCategoriesModel> PettyCashRecordsCategories { get; set; }
        public DbSet<PromotionEventsModel> PromotionEvents { get; set; }
        public DbSet<LanguagesModel> Languages { get; set; }
        public DbSet<LessonTypesModel> LessonTypes { get; set; }
        public DbSet<VouchersModel> Vouchers { get; set; }
        public DbSet<SuppliersModel> Suppliers { get; set; }
        public DbSet<UnitsModel> Units { get; set; }
        public DbSet<ExpenseCategoriesModel> ExpenseCategories { get; set; }
        public DbSet<PaymentsModel> Payments { get; set; }
        public DbSet<PayrollPaymentItemsModel> PayrollPaymentItems { get; set; }

        /******************************************************************************************************************************************************/

    }
}