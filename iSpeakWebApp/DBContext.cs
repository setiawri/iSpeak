using System.Data.Entity;
using iSpeakWebApp.Models;

namespace iSpeakWebApp
{
    public class DBContext : DbContext
    {
        public DBContext() { this.Database.Connection.ConnectionString = Helper.ConnectionString; }

        /******************************************************************************************************************************************************/

        public DbSet<ActivityLogsModel> ActivityLogs { get; set; }

        /* USER ACCOUNTS **************************************************************************************************************************************/

        public DbSet<UserAccountRolesModel> UserAccountRoles { get; set; }

        /******************************************************************************************************************************************************/

        public DbSet<BranchesModel> Branches { get; set; }
        public DbSet<RemindersModel> Reminders { get; set; }
        public DbSet<PettyCashRecordsCategoriesModel> PettyCashRecordsCategories { get; set; }
        public DbSet<PromotionEventsModel> PromotionEvents { get; set; }
        public DbSet<LanguagesModel> Languages { get; set; }
        public DbSet<LessonTypesModel> LessonTypes { get; set; }

        /******************************************************************************************************************************************************/

    }
}