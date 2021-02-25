﻿using iSpeakWebApp.Models;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Web;
using System.Web.Mvc;
using LIBUtil;

/*
 * To add new user access:
 * - add items in UserAccountRolesModel
 * - add items to database table UserAccountRoles
 * - add items in Post UserAccountRolesController.Edit() 
 * - add items in UserAccountRolesController.getAccesses() 
 * - add items in UserAccountRoles > Edit.cshtml
 * - update views that use the items
 */

namespace iSpeakWebApp.Controllers
{
    public class UserAccountRolesController : Controller
    {
        public const string NAME = "UserAccountRoles";
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: UserAccountRoles
        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View(get(db));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: UserAccountRoles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserAccountRoles/Create
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

        // GET: UserAccountRoles/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var model = get(db, id);
            return View(model);
        }

        // POST: UserAccountRoles/Edit/{id}
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
                    log = Helper.append(log, originalModel.Name, model.Name, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Name.LogDisplay));

                    //Reminders
                    log = Helper.append(log, originalModel.Reminders_Notes, model.Reminders_Notes, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Reminders_Notes.LogDisplay));
                    log = Helper.append(log, originalModel.Reminders_Add, model.Reminders_Add, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Reminders_Add.LogDisplay));
                    log = Helper.append(log, originalModel.Reminders_Edit, model.Reminders_Edit, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Reminders_Edit.LogDisplay));
                    log = Helper.append(log, originalModel.Reminders_View, model.Reminders_View, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Reminders_View.LogDisplay));

                    //UserAccounts
                    log = Helper.append(log, originalModel.UserAccounts_Notes, model.UserAccounts_Notes, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_UserAccounts_Notes.LogDisplay));
                    log = Helper.append(log, originalModel.UserAccounts_Add, model.UserAccounts_Add, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_UserAccounts_Add.LogDisplay));
                    log = Helper.append(log, originalModel.UserAccounts_Edit, model.UserAccounts_Edit, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_UserAccounts_Edit.LogDisplay));
                    log = Helper.append(log, originalModel.UserAccounts_View, model.UserAccounts_View, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_UserAccounts_View.LogDisplay));

                    //User Account Roles
                    log = Helper.append(log, originalModel.UserAccountRoles_Notes, model.UserAccountRoles_Notes, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_UserAccountRoles_Notes.LogDisplay));
                    log = Helper.append(log, originalModel.UserAccountRoles_Add, model.UserAccountRoles_Add, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_UserAccountRoles_Add.LogDisplay));
                    log = Helper.append(log, originalModel.UserAccountRoles_Edit, model.UserAccountRoles_Edit, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_UserAccountRoles_Edit.LogDisplay));
                    log = Helper.append(log, originalModel.UserAccountRoles_View, model.UserAccountRoles_View, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_UserAccountRoles_View.LogDisplay));

                    //Settings
                    log = Helper.append(log, originalModel.Settings_Notes, model.Settings_Notes, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Settings_Notes.LogDisplay));
                    log = Helper.append(log, originalModel.Settings_Edit, model.Settings_Edit, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Settings_Edit.LogDisplay));
                    log = Helper.append(log, originalModel.Settings_View, model.Settings_View, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Settings_View.LogDisplay));

                    //Branches
                    log = Helper.append(log, originalModel.Branches_Notes, model.Branches_Notes, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Branches_Notes.LogDisplay));
                    log = Helper.append(log, originalModel.Branches_Add, model.Branches_Add, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Branches_Add.LogDisplay));
                    log = Helper.append(log, originalModel.Branches_Edit, model.Branches_Edit, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Branches_Edit.LogDisplay));
                    log = Helper.append(log, originalModel.Branches_View, model.Branches_View, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_Branches_View.LogDisplay));

                    //PromotionEvents
                    log = Helper.append(log, originalModel.PromotionEvents_Notes, model.PromotionEvents_Notes, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_PromotionEvents_Notes.LogDisplay));
                    log = Helper.append(log, originalModel.PromotionEvents_Add, model.PromotionEvents_Add, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_PromotionEvents_Add.LogDisplay));
                    log = Helper.append(log, originalModel.PromotionEvents_Edit, model.PromotionEvents_Edit, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_PromotionEvents_Edit.LogDisplay));
                    log = Helper.append(log, originalModel.PromotionEvents_View, model.PromotionEvents_View, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_PromotionEvents_View.LogDisplay));

                    //PettyCashRecordsCategories
                    log = Helper.append(log, originalModel.PettyCashRecordsCategories_Notes, model.PettyCashRecordsCategories_Notes, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_PettyCashRecordsCategories_Notes.LogDisplay));
                    log = Helper.append(log, originalModel.PettyCashRecordsCategories_Add, model.PettyCashRecordsCategories_Add, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_PettyCashRecordsCategories_Add.LogDisplay));
                    log = Helper.append(log, originalModel.PettyCashRecordsCategories_Edit, model.PettyCashRecordsCategories_Edit, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_PettyCashRecordsCategories_Edit.LogDisplay));
                    log = Helper.append(log, originalModel.PettyCashRecordsCategories_View, model.PettyCashRecordsCategories_View, ActivityLogsController.editStringFormat(UserAccountRolesModel.COL_PettyCashRecordsCategories_View.LogDisplay));

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
                //Reminders
                if (item.Reminders_Add) model.Reminders_Add = true;
                if (item.Reminders_Edit) model.Reminders_Edit = true;
                if (item.Reminders_View) model.Reminders_View = true;

                //UserAccounts
                if (item.UserAccounts_Add) model.UserAccounts_Add = true;
                if (item.UserAccounts_Edit) model.UserAccounts_Edit = true;
                if (item.UserAccounts_View) model.UserAccounts_View = true;

                //UserAccountRoles
                if (item.UserAccountRoles_Add) model.UserAccountRoles_Add = true;
                if (item.UserAccountRoles_Edit) model.UserAccountRoles_Edit = true;
                if (item.UserAccountRoles_View) model.UserAccountRoles_View = true;

                //Settings
                if (item.Settings_Edit) model.Settings_Edit = true;
                if (item.Settings_View) model.Settings_View = true;

                //Branches
                if (item.Branches_Add) model.Branches_Add = true;
                if (item.Branches_Edit) model.Branches_Edit = true;
                if (item.Branches_View) model.Branches_View = true;

                //PromotionEvents
                if (item.PromotionEvents_Add) model.PromotionEvents_Add = true;
                if (item.PromotionEvents_Edit) model.PromotionEvents_Edit = true;
                if (item.PromotionEvents_View) model.PromotionEvents_View = true;

                //PettyCashRecordsCategories
                if (item.PettyCashRecordsCategories_Add) model.PettyCashRecordsCategories_Add = true;
                if (item.PettyCashRecordsCategories_Edit) model.PettyCashRecordsCategories_Edit = true;
                if (item.PettyCashRecordsCategories_View) model.PettyCashRecordsCategories_View = true;

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