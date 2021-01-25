using iSpeakWebApp.Models;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using LIBUtil;

using System.Web.Mvc;

namespace iSpeakWebApp.Controllers
{
    public class UserAccountRolesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /******************************************************************************************************************************************************/

        public static List<UserAccountRolesModel> get(DBContext db)
        {
            return db.UserAccountRoles.AsNoTracking().ToList();
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller)
        {
            List<UserAccountRolesModel> models = get(db);
            controller.ViewBag.UserAccountRoles = new SelectList(models, UserAccountRolesModel.COL_Id.Name, UserAccountRolesModel.COL_Name.Name);
        }

        /******************************************************************************************************************************************************/
    }
}