﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    /*
     * PettyCashRecordsCategories is filtered by Franchise. 
     */

    public class PettyCashRecordsCategoriesController : Controller
    {
        private readonly DBContext db = new DBContext();

        public static string CASHPAYMENT_Name = "CASH PAYMENT";
        public static Guid CASHPAYMENT_Id = new Guid("8781106D-4DC7-4C39-90E5-02111EB1D144");

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: PettyCashRecordsCategories
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).PettyCashRecordsCategories_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword, FILTER_Active));
            }
        }

        // POST: PettyCashRecordsCategories
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active)
        {
            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PettyCashRecordsCategories/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).PettyCashRecordsCategories_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(new PettyCashRecordsCategoriesModel());
        }

        // POST: PettyCashRecordsCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PettyCashRecordsCategoriesModel model, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name))
                    ModelState.AddModelError(PettyCashRecordsCategoriesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    model.Active = true;
                    model.Franchises_Id = (Guid)Helper.getActiveFranchiseId(this.Session);
                    db.PettyCashRecordsCategories.Add(model);
                    db.SaveChanges();
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PettyCashRecordsCategories/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).PettyCashRecordsCategories_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get((Guid)id));
        }

        // POST: PettyCashRecordsCategories/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PettyCashRecordsCategoriesModel modifiedModel, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(PettyCashRecordsCategoriesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    PettyCashRecordsCategoriesModel originalModel = db.PettyCashRecordsCategories.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, PettyCashRecordsCategoriesModel.COL_Name.LogDisplay);
                    log = Helper.append<FranchisesModel>(log, originalModel.Franchises_Id, modifiedModel.Franchises_Id, UserAccountsModel.COL_Franchises_Id.LogDisplay);
                    log = Helper.append(log, originalModel.Default_row, modifiedModel.Default_row, PettyCashRecordsCategoriesModel.COL_Default_row.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, PettyCashRecordsCategoriesModel.COL_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, PettyCashRecordsCategoriesModel.COL_Active.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                    {
                        db.Entry(modifiedModel).State = EntityState.Modified;
                        db.SaveChanges();
                        ActivityLogsController.AddEditLog(db, Session, modifiedModel.Id, log);
                    }

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public static void setDropDownListViewBag(Controller controller)
        {
            List<PettyCashRecordsCategoriesModel> items = get(controller.Session);
            PettyCashRecordsCategoriesModel cashPayment = new PettyCashRecordsCategoriesModel() { Id = PettyCashRecordsCategoriesController.CASHPAYMENT_Id, Name = PettyCashRecordsCategoriesController.CASHPAYMENT_Name };
            items.Insert(0,cashPayment);

            controller.ViewBag.PettyCashRecordsCategories = new SelectList(items, PettyCashRecordsCategoriesModel.COL_Id.Name, PettyCashRecordsCategoriesModel.COL_Name.Name, cashPayment.Id);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<PettyCashRecordsCategoriesModel>(@"
                        SELECT PettyCashRecordsCategories.*
                        FROM PettyCashRecordsCategories
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR PettyCashRecordsCategories.Name = @Name AND PettyCashRecordsCategories.Franchises_Id = @Franchises_Id)
							AND (@Id IS NULL OR (PettyCashRecordsCategories.Name = @Name AND PettyCashRecordsCategories.Id <> @Id AND PettyCashRecordsCategories.Franchises_Id = @Franchises_Id))
                    ",
                    DBConnection.getSqlParameter(PettyCashRecordsCategoriesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(PettyCashRecordsCategoriesModel.COL_Name.Name, Name),
                    DBConnection.getSqlParameter("Franchises_Id", Helper.getActiveFranchiseId(Session))
                ).Count() > 0;
        }

        public List<PettyCashRecordsCategoriesModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(Session, null, FILTER_Active, FILTER_Keyword); }
        public PettyCashRecordsCategoriesModel get(Guid Id) { return get(Session, Id, null, null).FirstOrDefault(); }
        public static List<PettyCashRecordsCategoriesModel> get(HttpSessionStateBase Session) { return get(Session, null, null, null); }
        public static List<PettyCashRecordsCategoriesModel> get(HttpSessionStateBase Session, Guid? Id, int? FILTER_Active, string FILTER_Keyword)
        {
            return new DBContext().Database.SqlQuery<PettyCashRecordsCategoriesModel>(@"
                        SELECT PettyCashRecordsCategories.*
                        FROM PettyCashRecordsCategories
                        WHERE 1=1
							AND (@Id IS NULL OR PettyCashRecordsCategories.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR PettyCashRecordsCategories.Active = @Active)
    							AND (@FILTER_Keyword IS NULL OR (PettyCashRecordsCategories.Name LIKE '%'+@FILTER_Keyword+'%'))
                            ))
							AND (@Franchises_Id IS NULL OR PettyCashRecordsCategories.Franchises_Id = @Franchises_Id)
						ORDER BY PettyCashRecordsCategories.Name ASC
                    ",
                    DBConnection.getSqlParameter(PettyCashRecordsCategoriesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(PettyCashRecordsCategoriesModel.COL_Active.Name, FILTER_Active),
                    DBConnection.getSqlParameter(PettyCashRecordsCategoriesModel.COL_Franchises_Id.Name, Helper.getActiveFranchiseId(Session)), 
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();
        }

        /******************************************************************************************************************************************************/
    }
}