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
    public class PromotionEventsController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: PromotionEvents
        public ActionResult Index(int? rss, string FILTER_Keyword)
        {
            if (!UserAccountsController.getUserAccess(Session).PromotionEvents_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword));
            }
        }

        // POST: PromotionEvents
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword)
        {
            setViewBag(FILTER_Keyword);
            return View(get(FILTER_Keyword));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PromotionEvents/Create
        public ActionResult Create(string FILTER_Keyword)
        {
            if (!UserAccountsController.getUserAccess(Session).PromotionEvents_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword);
            return View(new PromotionEventsModel());
        }

        // POST: PromotionEvents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PromotionEventsModel model, string FILTER_Keyword)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name))
                    ModelState.AddModelError(PromotionEventsModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    model.Branches_Id = Helper.getActiveBranchId(Session);
                    db.PromotionEvents.Add(model);
                    db.SaveChanges();
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword });
                }
            }

            setViewBag(FILTER_Keyword);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PromotionEvents/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword)
        {
            if (!UserAccountsController.getUserAccess(Session).PromotionEvents_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword);
            return View(get((Guid)id));
        }

        // POST: PromotionEvents/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PromotionEventsModel modifiedModel, string FILTER_Keyword)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(PromotionEventsModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    PromotionEventsModel originalModel = db.PromotionEvents.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Helper.append<BranchesModel>(log, originalModel.Branches_Id, modifiedModel.Branches_Id, PromotionEventsModel.COL_Branches_Id.LogDisplay);
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, PromotionEventsModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.Location, modifiedModel.Location, PromotionEventsModel.COL_Location.LogDisplay);
                    log = Helper.append(log, originalModel.TotalDays, modifiedModel.TotalDays, PromotionEventsModel.COL_TotalDays.LogDisplay);
                    log = Helper.append(log, originalModel.EventFee, modifiedModel.EventFee, PromotionEventsModel.COL_EventFee.LogDisplay);
                    log = Helper.append(log, originalModel.PersonnelCost, modifiedModel.PersonnelCost, PromotionEventsModel.COL_PersonnelCost.LogDisplay);
                    log = Helper.append(log, originalModel.AdditionalCost, modifiedModel.AdditionalCost, PromotionEventsModel.COL_AdditionalCost.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, PromotionEventsModel.COL_Notes.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                    {
                        db.Entry(modifiedModel).State = EntityState.Modified;
                        db.SaveChanges();
                        ActivityLogsController.AddEditLog(db, Session, modifiedModel.Id, log);
                    }

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword });
                }
            }

            setViewBag(FILTER_Keyword);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public static void setDropDownListViewBag(Controller controller)
        {
            controller.ViewBag.PromotionEvents = new SelectList(PromotionEventsController.get(controller), PromotionEventsModel.COL_Id.Name, PromotionEventsModel.COL_Name.Name);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<PromotionEventsModel>(@"
                        SELECT PromotionEvents.*
                        FROM PromotionEvents
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR PromotionEvents.Name = @Name)
							AND (@Id IS NULL OR (PromotionEvents.Name = @Name AND PromotionEvents.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(PromotionEventsModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(PromotionEventsModel.COL_Name.Name, Name)
                ).Count() > 0;
        }

        public PromotionEventsModel get(Guid Id) { return get(this, Id, null, null).FirstOrDefault(); }
        public List<PromotionEventsModel> get(string FILTER_Keyword) { return get(this, null, FILTER_Keyword, null); }
        public static List<PromotionEventsModel> get(Controller controller) { return new PromotionEventsController().get(controller, null, null, null); }
        public List<PromotionEventsModel> get(Controller controller, Guid? Id, string FILTER_Keyword, Guid? Branches_Id)
        {
            return new DBContext().Database.SqlQuery<PromotionEventsModel>(@"
                        SELECT PromotionEvents.*
                        FROM PromotionEvents
                            LEFT JOIN Branches ON Branches.Id = PromotionEvents.Branches_Id
                        WHERE 1=1
							AND (@Id IS NULL OR PromotionEvents.Id = @Id)
							AND (@Id IS NOT NULL OR (
    							(@FILTER_Keyword IS NULL OR (PromotionEvents.Name LIKE '%'+@FILTER_Keyword+'%'))
                            ))
							AND (@Branches_Id IS NULL OR PromotionEvents.Branches_Id = @Branches_Id)
							AND (@Franchises_Id IS NULL OR Branches.Franchises_Id = @Franchises_Id)
						ORDER BY PromotionEvents.Name ASC
                    ",
                    DBConnection.getSqlParameter(PromotionEventsModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(PromotionEventsModel.COL_Branches_Id.Name, Branches_Id),
                    DBConnection.getSqlParameter("Franchises_Id", Helper.getActiveFranchiseId(controller.Session)),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();
        }

        /******************************************************************************************************************************************************/
    }
}