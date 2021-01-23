using System.Web;
using System.Linq;
using System.Web.Mvc;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp
{
    public static class SelectLists
    {
        static SelectList BranchList;

        public static SelectList getBranchList(HttpSessionStateBase Session)
        {
            if (BranchList == null)
                updateBranchList();

            return BranchList;
        }
        public static void updateBranchList()
        {
            DBContext db = new DBContext();

            BranchList = new SelectList(db.Branches
                .AsNoTracking()
                .Where(x => x.Active == true)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name.ToString() }), "Value", "Text");
        }

    }

}