using iSpeakWebApp.Models;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace iSpeakWebApp.Controllers
{
    public static class BranchSelectLists
    {
        static SelectList BranchList;

        public static SelectList get()
        {
            if (BranchList == null)
                update();

            return BranchList;
        }

        //IMPROVEMENT: need to call this method when branches table change
        public static void update()
        {
            DBContext db = new DBContext();
            BranchList = new SelectList(BranchesController.get(true).Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name.ToString() }), "Value", "Text");
        }
    }

    public class BranchesController : Controller
    {
        /******************************************************************************************************************************************************/

        public static List<BranchesModel> get(bool isActiveOnly)
        {
            return new DBContext().Branches.AsNoTracking()
                .Where(x => x.Active == isActiveOnly)
                .OrderBy(x => x.Name)
                .ToList();
        }

        /******************************************************************************************************************************************************/
    }
}