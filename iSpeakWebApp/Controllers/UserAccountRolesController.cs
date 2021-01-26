using iSpeakWebApp.Models;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Web;
using LIBUtil;

using System.Web.Mvc;

namespace iSpeakWebApp.Controllers
{
    public class UserAccountRolesController : Controller
    {
        public const string NAME = "UserAccountRoles";
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PAYROLL/UserAccountRoles
        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View(get(db));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PAYROLL/UserAccountRoles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PAYROLL/UserAccountRoles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserAccountRolesModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Create, null, model.Name))
                    ModelState.AddModelError(UserAccountRolesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    db.UserAccountRoles.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Edit), new { id = model.Id });
                }
            }

            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PAYROLL/UserAccountRoles/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var model = get(db, id);
            return View(model);
        }

        // POST: PAYROLL/UserAccountRoles/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserAccountRolesModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Edit, model.Id, model.Name))
                    ModelState.AddModelError(UserAccountRolesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    UserAccountRolesModel originalModel = get(db, model.Id);

                    string log = string.Empty;
                    log = Util.webAppendChange(log, originalModel.Name, model.Name, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Name.LogDisplay));

                    log = Util.webAppendChange(log, originalModel.Reminders_Add, model.Reminders_Add, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Reminders_Add.LogDisplay));
                    log = Util.webAppendChange(log, originalModel.Reminders_Edit, model.Reminders_Edit, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Reminders_Edit.LogDisplay));
                    log = Util.webAppendChange(log, originalModel.Reminders_Notes, model.Reminders_Notes, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Reminders_Notes.LogDisplay));
                    log = Util.webAppendChange(log, originalModel.Reminders_View, model.Reminders_View, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Reminders_View.LogDisplay));

                    if (!string.IsNullOrEmpty(log))
                    {
                        db.Entry(model).State = EntityState.Modified;
                        ActivityLogsController.AddEditLog(db, Session, model.Id, log);
                        db.SaveChanges();
                    }

                    UserAccountsController.updateLoginSession(Session);

                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        /* METHODS ********************************************************************************************************************************************/

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller)
        {
            controller.ViewBag.UserAccountRoles = new SelectList(get(db), UserAccountRolesModel.COL_Id.Name, UserAccountRolesModel.COL_Name.Name);
        }

        public static UserAccountRolesModel getAccesses(DBContext db, Guid? UserAccounts_Id) 
        {
            UserAccountRolesModel model = new UserAccountRolesModel();
            foreach( UserAccountRolesModel item in get(db, null, UserAccounts_Id))
            {
                if (item.Reminders_Add) model.Reminders_Add = true;
                if (item.Reminders_Edit) model.Reminders_Edit = true;
                if (item.Reminders_View) model.Reminders_View = true;
            }

            return model;
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(EnumActions action, Guid? id, object value)
        {
            var result = action == EnumActions.Create
                ? db.UserAccountRoles.AsNoTracking().Where(x => x.Name.ToLower() == value.ToString().ToLower()).FirstOrDefault()
                : db.UserAccountRoles.AsNoTracking().Where(x => x.Name.ToLower() == value.ToString().ToLower() && x.Id != id).FirstOrDefault();
            return result != null;
        }

        public static List<UserAccountRolesModel> get(DBContext db) { return get(db, null, null); }
        public static UserAccountRolesModel get(DBContext db, Guid? Id) { return get(db, Id, null).FirstOrDefault(); }
        public static List<UserAccountRolesModel> get(DBContext db, Guid? Id, Guid? UserAccounts_Id)
        {
            List<UserAccountRolesModel> models = db.Database.SqlQuery<UserAccountRolesModel>(@"
                        SELECT UserAccountRoles.*
                        FROM UserAccountRoles
                        WHERE 1=1
							AND (@Id IS NULL OR UserAccountRoles.Id = @Id)
							AND (@UserAccounts_Id IS NULL OR UserAccountRoles.Id IN (
									SELECT DISTINCT(UserAccountRoleAssignments.UserAccountRoles_Id) 
									FROM UserAccountRoleAssignments 
									WHERE UserAccountRoleAssignments.UserAccounts_Id=@UserAccounts_Id
								)
							)
						ORDER BY UserAccountRoles.Name ASC
                    ",
                    DBConnection.getSqlParameter(UserAccountRolesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(UserAccountRoleAssignmentsModel.COL_UserAccounts_Id.Name, UserAccounts_Id)
                ).ToList();

            return models;
        }

        /******************************************************************************************************************************************************/
    }
}