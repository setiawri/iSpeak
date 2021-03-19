using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class LanguagesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: Languages
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Languages_View)
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

        // POST: Languages
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active)
        {
            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: Languages/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Languages_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(new LanguagesModel());
        }

        // POST: Languages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LanguagesModel model, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name))
                    ModelState.AddModelError(LanguagesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    model.Active = true;
                    db.Languages.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges(); 
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: Languages/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Languages_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get((Guid)id));
        }

        // POST: Languages/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LanguagesModel modifiedModel, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(LanguagesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    LanguagesModel originalModel = db.Languages.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, LanguagesModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, LanguagesModel.COL_Active.LogDisplay);

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
            controller.ViewBag.Languages = new SelectList(get(), LanguagesModel.COL_Id.Name, LanguagesModel.COL_Name.Name);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<LanguagesModel>(@"
                        SELECT Languages.*
                        FROM Languages
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR Languages.Name = @Name)
							AND (@Id IS NULL OR (Languages.Name = @Name AND Languages.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(LanguagesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(LanguagesModel.COL_Name.Name, Name)
                ).Count() > 0;
        }

        public List<LanguagesModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(null, FILTER_Active, FILTER_Keyword); }
        public LanguagesModel get(Guid Id) { return get(Id, null, null).FirstOrDefault(); }
        public static List<LanguagesModel> get() { return get(null, null, null); }
        public static List<LanguagesModel> get(Guid? Id, int? FILTER_Active, string FILTER_Keyword)
        {
            return new DBContext().Database.SqlQuery<LanguagesModel>(@"
                        SELECT Languages.*
                        FROM Languages
                        WHERE 1=1
							AND (@Id IS NULL OR Languages.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR Languages.Active = @Active)
    							AND (@FILTER_Keyword IS NULL OR (Languages.Name LIKE '%'+@FILTER_Keyword+'%'))
                            ))
						ORDER BY Languages.Name ASC
                    ",
                    DBConnection.getSqlParameter(LanguagesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(LanguagesModel.COL_Active.Name, FILTER_Active),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();
        }

        /******************************************************************************************************************************************************/
    }
}