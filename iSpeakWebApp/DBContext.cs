using System.Data.Entity;
using iSpeakWebApp.Models;

namespace iSpeakWebApp
{
    public class DBContext : DbContext
    {
        public DBContext() { this.Database.Connection.ConnectionString = Helper.ConnectionString; }

        public DbSet<SettingsModel> Settings { get; set; }

        /* USER ACCOUNTS **************************************************************************************************************************************/
        public DbSet<UserAccountsModel> UserAccounts { get; set; }

        /******************************************************************************************************************************************************/

        public DbSet<BranchesModel> Branches { get; set; }
        public DbSet<RemindersModel> Reminders { get; set; }
    }
}