using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class ClubClassOnlineLinksController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClassess_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubClassOnlineLinks_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_ClubClassess_Id);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword, FILTER_Active, FILTER_ClubClassess_Id));
            }
        }

        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClassess_Id)
        {
            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_ClubClassess_Id);
            return View(get(FILTER_Keyword, FILTER_Active, FILTER_ClubClassess_Id));
        }

        /* CREATE *********************************************************************************************************************************************/

        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClassess_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubClassOnlineLinks_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_ClubClassess_Id);
            return View(new ClubClassOnlineLinksModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClubClassOnlineLinksModel model, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClassess_Id)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name))
                    ModelState.AddModelError(ClubClassOnlineLinksModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    add(model);
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active, FILTER_ClubClassess_Id = FILTER_ClubClassess_Id });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_ClubClassess_Id);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClassess_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubClassOnlineLinks_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_ClubClassess_Id);
            return View(get((Guid)id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClubClassOnlineLinksModel modifiedModel, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClassess_Id)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(ClubClassOnlineLinksModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    ClubClassOnlineLinksModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append<ClubClassesModel>(log, originalModel.ClubClasses_Id, modifiedModel.ClubClasses_Id, ClubClassOnlineLinksModel.COL_ClubClasses_Id.LogDisplay);
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, ClubClassOnlineLinksModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.OnlineLink, modifiedModel.OnlineLink, ClubClassOnlineLinksModel.COL_OnlineLink.LogDisplay);
                    log = Helper.append(log, originalModel.WeekNo, modifiedModel.WeekNo, ClubClassOnlineLinksModel.COL_WeekNo.LogDisplay);
                    log = Helper.append(log, originalModel.DurationDays, modifiedModel.DurationDays, ClubClassOnlineLinksModel.COL_DurationDays.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, ClubClassOnlineLinksModel.COL_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, ClubClassOnlineLinksModel.COL_Active.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                        update(modifiedModel, log);

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active, FILTER_ClubClassess_Id = FILTER_ClubClassess_Id });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_ClubClassess_Id);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClassess_Id)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
            ViewBag.FILTER_ClubClassess_Id = FILTER_ClubClassess_Id;
            ClubClassesController.setDropDownListViewBag(this);
        }

        public static void setDropDownListViewBag(Controller controller)
        {
            controller.ViewBag.ClubClassOnlineLinks = new SelectList(get(), ClubClassOnlineLinksModel.COL_Id.Name, ClubClassOnlineLinksModel.COL_Name.Name);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<ClubClassOnlineLinksModel>(@"
                        SELECT ClubClassOnlineLinks.*
                        FROM ClubClassOnlineLinks
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR ClubClassOnlineLinks.Name = @Name)
							AND (@Id IS NULL OR (ClubClassOnlineLinks.Name = @Name AND ClubClassOnlineLinks.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Name.Name, Name)
                ).Count() > 0;
        }

        public static List<ClubClassOnlineLinksModel> get(int? FILTER_Active) { return get(null, FILTER_Active, null, null); }
        public List<ClubClassOnlineLinksModel> get(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClassess_Id) { return get(null, FILTER_Active, FILTER_Keyword, FILTER_ClubClassess_Id); }
        public static ClubClassOnlineLinksModel get(Guid Id) { return get(Id, null, null, null).FirstOrDefault(); }
        public static List<ClubClassOnlineLinksModel> get() { return get(null, null, null, null); }
        public static List<ClubClassOnlineLinksModel> get(Guid? Id, int? FILTER_Active, string FILTER_Keyword, Guid? FILTER_ClubClassess_Id)
        {
            string sql = string.Format(@"
                        SELECT ClubClassOnlineLinks.*,
                            ClubClasses.Name AS ClubClasses_Name
                        FROM ClubClassOnlineLinks
                            LEFT JOIN ClubClasses ON ClubClasses.Id = ClubClassOnlineLinks.ClubClasses_Id
                        WHERE 1=1
							AND (@Id IS NULL OR ClubClassOnlineLinks.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR ClubClassOnlineLinks.Active = @Active)
    							AND (@FILTER_Keyword IS NULL OR (ClubClassOnlineLinks.Name LIKE '%'+@FILTER_Keyword+'%'))
                                AND (@FILTER_ClubClassess_Id IS NULL OR ClubClassOnlineLinks.ClubClasses_Id = @FILTER_ClubClassess_Id)
                            ))
						ORDER BY ClubClasses.Name ASC, ClubClassOnlineLinks.WeekNo ASC, ClubClassOnlineLinks.Name ASC
                    ");

            return new DBContext().Database.SqlQuery<ClubClassOnlineLinksModel>(sql,
                    DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Active.Name, FILTER_Active),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                    DBConnection.getSqlParameter("FILTER_ClubClassess_Id", FILTER_ClubClassess_Id)
                ).ToList();
        }

        public void add(ClubClassOnlineLinksModel model)
        {
            LIBWebMVC.WebDBConnection.Insert(db.Database, "ClubClassOnlineLinks",
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_OnlineLink.Name, model.OnlineLink),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_ClubClasses_Id.Name, model.ClubClasses_Id),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_WeekNo.Name, model.WeekNo),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_DurationDays.Name, model.DurationDays),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddCreateLog(db, Session, model.Id);
            db.SaveChanges();
        }

        public void update(ClubClassOnlineLinksModel model, string log)
        {
            LIBWebMVC.WebDBConnection.Update(db.Database, "ClubClassOnlineLinks",
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_OnlineLink.Name, model.OnlineLink),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_ClubClasses_Id.Name, model.ClubClasses_Id),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_WeekNo.Name, model.WeekNo),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_DurationDays.Name, model.DurationDays),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        /******************************************************************************************************************************************************/
    }
}