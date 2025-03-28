﻿using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    /*
     * LessonPackages is separated by franchise. 
     */

    public class LessonPackagesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id, Guid? FILTER_LessonTypes_Id)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
            ViewBag.FILTER_Languages_Id = FILTER_Languages_Id;
            ViewBag.FILTER_LessonTypes_Id = FILTER_LessonTypes_Id;
            LanguagesController.setDropDownListViewBag(this);
            LessonTypesController.setDropDownListViewBag(this);
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: LessonPackages
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id, Guid? FILTER_LessonTypes_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).LessonPackages_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id, FILTER_LessonTypes_Id);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id, FILTER_LessonTypes_Id));
            }
        }

        // POST: LessonPackages
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id, Guid? FILTER_LessonTypes_Id)
        {
            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id, FILTER_LessonTypes_Id);
            return View(get(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id, FILTER_LessonTypes_Id));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: LessonPackages/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id, Guid? FILTER_LessonTypes_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).LessonPackages_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id, FILTER_LessonTypes_Id);
            return View(new LessonPackagesModel());
        }

        // POST: LessonPackages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LessonPackagesModel model, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id, Guid? FILTER_LessonTypes_Id)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name))
                    ModelState.AddModelError(LessonPackagesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Franchises_Id = Helper.getActiveFranchiseId(Session);
                    add(model);
                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active, FILTER_Languages_Id = FILTER_Languages_Id, FILTER_LessonTypes_Id = FILTER_LessonTypes_Id });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id, FILTER_LessonTypes_Id);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: LessonPackages/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id, Guid? FILTER_LessonTypes_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).LessonPackages_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id, FILTER_LessonTypes_Id);
            return View(get(Session, (Guid)id));
        }

        // POST: LessonPackages/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LessonPackagesModel modifiedModel, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id, Guid? FILTER_LessonTypes_Id)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(LessonPackagesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    LessonPackagesModel originalModel = get(Session, modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, LessonPackagesModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, LessonPackagesModel.COL_Active.LogDisplay);
                    log = Helper.append<LanguagesModel>(log, originalModel.Languages_Id, modifiedModel.Languages_Id, LessonPackagesModel.COL_Languages_Id.LogDisplay);
                    log = Helper.append<LessonTypesModel>(log, originalModel.LessonTypes_Id, modifiedModel.LessonTypes_Id, LessonPackagesModel.COL_LessonTypes_Id.LogDisplay);
                    log = Helper.append(log, originalModel.SessionHours, modifiedModel.SessionHours, LessonPackagesModel.COL_SessionHours.LogDisplay);
                    log = Helper.append(log, originalModel.ExpirationMonth, modifiedModel.ExpirationMonth, LessonPackagesModel.COL_ExpirationMonth.LogDisplay);
                    log = Helper.append(log, originalModel.Price, modifiedModel.Price, LessonPackagesModel.COL_Price.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, LessonPackagesModel.COL_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.IsClubSubscription, modifiedModel.IsClubSubscription, LessonPackagesModel.COL_IsClubSubscription.LogDisplay);
                    log = Helper.append<FranchisesModel>(log, originalModel.Franchises_Id, modifiedModel.Franchises_Id, LessonPackagesModel.COL_Franchises_Id.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                        update(modifiedModel, log);

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active, FILTER_Languages_Id = FILTER_Languages_Id, FILTER_LessonTypes_Id = FILTER_LessonTypes_Id });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id, FILTER_LessonTypes_Id);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public static void setDropDownListViewBag(Controller controller)
        {
            controller.ViewBag.LessonPackages = new SelectList(get(controller.Session, 1).OrderBy(x => x.DDLDescription), LessonPackagesModel.COL_Id.Name, LessonPackagesModel.COL_DDLDescription.Name);
        }

        public static void setViewBag(Controller controller)
        {
            controller.ViewBag.LessonPackagesModels = get(controller.Session, 1);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<LessonPackagesModel>(@"
                        SELECT LessonPackages.*,
                            NULL AS Languages_Name,
                            NULL AS LessonTypes_Name,
                            NULL AS Franchises_Name,
                            '' AS DDLDescription
                        FROM LessonPackages
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR LessonPackages.Name = @Name)
							AND (@Id IS NULL OR (LessonPackages.Name = @Name AND LessonPackages.Id <> @Id))
                            AND LessonPackages.Franchises_Id = @Franchises_Id
                    ",
                    DBConnection.getSqlParameter(LessonPackagesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(LessonPackagesModel.COL_Name.Name, Name),
                    DBConnection.getSqlParameter(LessonPackagesModel.COL_Franchises_Id.Name, Helper.getActiveFranchiseId(Session))
                ).Count() > 0;
        }

        public List<LessonPackagesModel> get(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id, Guid? FILTER_LessonTypes_Id) { return get(Session, null, FILTER_Active, FILTER_Keyword, FILTER_Languages_Id, FILTER_LessonTypes_Id); }
        public static LessonPackagesModel get(HttpSessionStateBase Session, Guid Id) { return get(Session, Id, null, null, null, null).FirstOrDefault(); }
        public static List<LessonPackagesModel> get(HttpSessionStateBase Session, int? Active) { return get(Session, null, Active, null, null, null); }
        public static List<LessonPackagesModel> get(HttpSessionStateBase Session, Guid? Id, int? Active, string FILTER_Keyword, Guid? Languages_Id, Guid? LessonTypes_Id)
        {
            return new DBContext().Database.SqlQuery<LessonPackagesModel>(@"
                        SELECT LessonPackages.*,
                            Languages.Name AS Languages_Name,
                            LessonTypes.Name AS LessonTypes_Name,
                            Franchises.Name AS Franchises_Name,
                            '['+Languages.Name+': '+LessonTypes.Name+'] '+LessonPackages.Name+' (' + 
                                CASE WHEN LessonPackages.IsClubSubscription = 0 
                                    THEN FORMAT(LessonPackages.SessionHours,'N2') + ' hours' 
                                    ELSE FORMAT(LessonPackages.ExpirationMonth,'N0') + ' months' 
                                END
                                + ') '+FORMAT(LessonPackages.Price,'N0') AS DDLDescription
                        FROM LessonPackages
                            LEFT JOIN Languages ON Languages.Id = LessonPackages.Languages_Id
                            LEFT JOIN LessonTypes ON LessonTypes.Id = LessonPackages.LessonTypes_Id
                            LEFT JOIN Franchises ON Franchises.Id = LessonPackages.Franchises_Id
                        WHERE 1=1
							AND (@Id IS NULL OR LessonPackages.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR LessonPackages.Active = @Active)
    							AND (@FILTER_Keyword IS NULL OR (LessonPackages.Name LIKE '%'+@FILTER_Keyword+'%'))
                                AND (@Languages_Id IS NULL OR LessonPackages.Languages_Id = @Languages_Id)
                                AND (@LessonTypes_Id IS NULL OR LessonPackages.LessonTypes_Id = @LessonTypes_Id)
                            ))
                            AND LessonPackages.Franchises_Id = @Franchises_Id
						ORDER BY LessonPackages.Name ASC
                    ",
                    DBConnection.getSqlParameter(LessonPackagesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(LessonPackagesModel.COL_Active.Name, Active),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                    DBConnection.getSqlParameter(LessonPackagesModel.COL_Languages_Id.Name, Languages_Id),
                    DBConnection.getSqlParameter(LessonPackagesModel.COL_LessonTypes_Id.Name, LessonTypes_Id),
                    DBConnection.getSqlParameter(LessonPackagesModel.COL_Franchises_Id.Name, Helper.getActiveFranchiseId(Session))
                ).ToList();
        }

        public void update(LessonPackagesModel model, string log)
        {
            LIBWebMVC.WebDBConnection.Update(db.Database, "LessonPackages",
                DBConnection.getSqlParameter(LessonPackagesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_Languages_Id.Name, model.Languages_Id),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_LessonTypes_Id.Name, model.LessonTypes_Id),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_SessionHours.Name, model.SessionHours),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_ExpirationMonth.Name, model.ExpirationMonth),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_Price.Name, model.Price),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_IsClubSubscription.Name, model.IsClubSubscription),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_Franchises_Id.Name, model.Franchises_Id)
            );

            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
        }

        public void add(LessonPackagesModel model)
        {
            LIBWebMVC.WebDBConnection.Insert(db.Database, "LessonPackages",
                DBConnection.getSqlParameter(LessonPackagesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_Languages_Id.Name, model.Languages_Id),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_LessonTypes_Id.Name, model.LessonTypes_Id),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_SessionHours.Name, model.SessionHours),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_ExpirationMonth.Name, model.ExpirationMonth),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_Price.Name, model.Price),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_IsClubSubscription.Name, model.IsClubSubscription),
                DBConnection.getSqlParameter(LessonPackagesModel.COL_Franchises_Id.Name, model.Franchises_Id)
            );

            ActivityLogsController.AddCreateLog(db, Session, model.Id);
        }

        /******************************************************************************************************************************************************/
    }
}