using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class LessonTypesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: LessonTypes
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).LessonTypes_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                setViewBag(FILTER_Keyword, FILTER_Active);
                return View(get(FILTER_Keyword, FILTER_Active));
            }
        }

        // POST: LessonTypes
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active)
        {
            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: LessonTypes/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).LessonTypes_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(new LessonTypesModel());
        }

        // POST: LessonTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LessonTypesModel model, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name))
                    ModelState.AddModelError(LessonTypesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    model.Active = true;
                    db.LessonTypes.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges(); 
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: LessonTypes/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).LessonTypes_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get((Guid)id));
        }

        // POST: LessonTypes/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LessonTypesModel modifiedModel, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(LessonTypesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    LessonTypesModel originalModel = db.LessonTypes.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, LessonTypesModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, LessonTypesModel.COL_Active.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                    {
                        db.Entry(modifiedModel).State = EntityState.Modified;
                        ActivityLogsController.AddEditLog(db, Session, modifiedModel.Id, log);
                        db.SaveChanges();
                    }

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public static void setDropDownListViewBag(ControllerBase controller)
        {
            controller.ViewBag.LessonTypes = new SelectList(get(), LessonTypesModel.COL_Id.Name, LessonTypesModel.COL_Name.Name);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<LessonTypesModel>(@"
                        SELECT LessonTypes.*
                        FROM LessonTypes
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR LessonTypes.Name = @Name)
							AND (@Id IS NULL OR (LessonTypes.Name = @Name AND LessonTypes.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(LessonTypesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(LessonTypesModel.COL_Name.Name, Name)
                ).Count() > 0;
        }

        public List<LessonTypesModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(null, FILTER_Active, FILTER_Keyword); }
        public LessonTypesModel get(Guid Id) { return get(Id, null, null).FirstOrDefault(); }
        public static List<LessonTypesModel> get() { return get(null, null, null); }
        public static List<LessonTypesModel> get(Guid? Id, int? FILTER_Active, string FILTER_Keyword)
        {
            return new DBContext().Database.SqlQuery<LessonTypesModel>(@"
                        SELECT LessonTypes.*
                        FROM LessonTypes
                        WHERE 1=1
							AND (@Id IS NULL OR LessonTypes.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR LessonTypes.Active = @Active)
    							AND (@FILTER_Keyword IS NULL OR (LessonTypes.Name LIKE '%'+@FILTER_Keyword+'%'))
                            ))
						ORDER BY LessonTypes.Name ASC
                    ",
                    DBConnection.getSqlParameter(LessonTypesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(LessonTypesModel.COL_Active.Name, FILTER_Active),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();
        }

        /******************************************************************************************************************************************************/
    }
}