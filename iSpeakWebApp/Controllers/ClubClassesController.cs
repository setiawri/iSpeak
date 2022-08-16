using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class ClubClassesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubClasses_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id));
            }
        }

        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id);
            return View(get(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id));
        }

        /* CREATE *********************************************************************************************************************************************/

        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubClasses_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id);
            return View(new ClubClassesModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClubClassesModel model, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name))
                    ModelState.AddModelError(ClubClassesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    add(model);
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active, FILTER_Languages_Id = FILTER_Languages_Id });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubClasses_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id);
            return View(get((Guid)id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClubClassesModel modifiedModel, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(ClubClassesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    ClubClassesModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, ClubClassesModel.COL_Name.LogDisplay);
                    log = Helper.append<LanguagesModel>(log, originalModel.Languages_Id, modifiedModel.Languages_Id, ClubClassesModel.COL_Languages_Id.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, ClubClassesModel.COL_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, ClubClassesModel.COL_Active.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                        update(modifiedModel, log);

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active, FILTER_Languages_Id = FILTER_Languages_Id });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
            ViewBag.FILTER_Languages_Id = FILTER_Languages_Id;
            LanguagesController.setDropDownListViewBag(this);
        }

        public static void setDropDownListViewBag(Controller controller)
        {
            controller.ViewBag.ClubClasses = new SelectList(get(), ClubClassesModel.COL_Id.Name, ClubClassesModel.COL_Name.Name);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<ClubClassesModel>(@"
                        SELECT ClubClasses.*
                        FROM ClubClasses
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR ClubClasses.Name = @Name)
							AND (@Id IS NULL OR (ClubClasses.Name = @Name AND ClubClasses.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(ClubClassesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ClubClassesModel.COL_Name.Name, Name)
                ).Count() > 0;
        }

        public static List<ClubClassesModel> get(int? FILTER_Active) { return get(null, FILTER_Active, null, null); }
        public List<ClubClassesModel> get(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id) { return get(null, FILTER_Active, FILTER_Keyword, FILTER_Languages_Id); }
        public static ClubClassesModel get(Guid Id) { return get(Id, null, null, null).FirstOrDefault(); }
        public static List<ClubClassesModel> get() { return get(null, null, null, null); }
        public static List<ClubClassesModel> get(Guid? Id, int? FILTER_Active, string FILTER_Keyword, Guid? FILTER_Languages_Id)
        {
            string sql = string.Format(@"
                        SELECT ClubClasses.*,
                            Languages.Name AS Languages_Name
                        FROM ClubClasses
                            LEFT JOIN Languages ON Languages.Id = ClubClasses.Languages_Id
                        WHERE 1=1
							AND (@Id IS NULL OR ClubClasses.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR ClubClasses.Active = @Active)
    							AND (@FILTER_Keyword IS NULL OR (ClubClasses.Name LIKE '%'+@FILTER_Keyword+'%'))
                                AND (@FILTER_Languages_Id IS NULL OR ClubClasses.Languages_Id = @FILTER_Languages_Id)
                            ))
						ORDER BY Languages.Name ASC, ClubClasses.Name ASC
                    ");

            return new DBContext().Database.SqlQuery<ClubClassesModel>(sql,
                    DBConnection.getSqlParameter(ClubClassesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ClubClassesModel.COL_Active.Name, FILTER_Active),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                    DBConnection.getSqlParameter("FILTER_Languages_Id", FILTER_Keyword)
                ).ToList();
        }

        public void add(ClubClassesModel model)
        {
            LIBWebMVC.WebDBConnection.Insert(db.Database, "ClubClasses",
                DBConnection.getSqlParameter(ClubClassesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(ClubClassesModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(ClubClassesModel.COL_Languages_Id.Name, model.Languages_Id),
                DBConnection.getSqlParameter(ClubClassesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(ClubClassesModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddCreateLog(db, Session, model.Id);
            db.SaveChanges();
        }

        public void update(ClubClassesModel model, string log)
        {
            LIBWebMVC.WebDBConnection.Update(db.Database, "ClubClasses",
                DBConnection.getSqlParameter(ClubClassesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(ClubClassesModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(ClubClassesModel.COL_Languages_Id.Name, model.Languages_Id),
                DBConnection.getSqlParameter(ClubClassesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(ClubClassesModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        /******************************************************************************************************************************************************/
    }
}