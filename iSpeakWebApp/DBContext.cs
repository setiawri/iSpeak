using System.Data.Entity;
using iSpeakWebApp.Models;

namespace iSpeakWebApp
{
    public class DBContext : DbContext
    {
        public DBContext()
        {
            this.Database.Connection.ConnectionString = Helper.ConnectionString;
        }

        public DbSet<SettingsModel> Settings { get; set; }

        /* USER ACCOUNTS **************************************************************************************************************************************/


        /******************************************************************************************************************************************************/
    }
}