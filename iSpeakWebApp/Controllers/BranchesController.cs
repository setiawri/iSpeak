using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public static class BranchSelectLists
    {
        static SelectList BranchList; //for global branch dropdownlist

        public static SelectList get()
        {
            if (BranchList == null)
                update();

            return BranchList;
        }

        //always call this method when branches table change so global branch ddl is updated
        public static void update()
        {
            BranchList = new SelectList(BranchesController.get(null, 1, null).Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name.ToString() }), "Value", "Text");
        }
    }

    public class BranchesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setFilterViewBags(string FILTER_Keyword, int? FILTER_Active)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: Branches
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active)
        {
            if(rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                setFilterViewBags(FILTER_Keyword, FILTER_Active);
                return View(get(FILTER_Keyword, FILTER_Active));
            }
        }

        // POST: Branches
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active)
        {
            setFilterViewBags(FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: Branches/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active)
        {
            setCreateViewBags(FILTER_Keyword, FILTER_Active);
            return View();
        }

        // POST: Branches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BranchesModel model, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name))
                    ModelState.AddModelError(BranchesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    model.Active = true;
                    db.Branches.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    BranchSelectLists.update();
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setCreateViewBags(FILTER_Keyword, FILTER_Active);
            return View(model);
        }

        private void setCreateViewBags(string FILTER_Keyword, int? FILTER_Active)
        {
            setFilterViewBags(FILTER_Keyword, FILTER_Active);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: Branches/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            setEditViewBags(FILTER_Keyword, FILTER_Active);
            return View(get((Guid)id));
        }

        // POST: Branches/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BranchesModel modifiedModel, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(BranchesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    BranchesModel originalModel = db.Branches.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, BranchesModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.Address, modifiedModel.Address, BranchesModel.COL_Address.LogDisplay);
                    log = Helper.append(log, originalModel.PhoneNumber, modifiedModel.PhoneNumber, BranchesModel.COL_PhoneNumber.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, BranchesModel.COL_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.InvoiceHeaderText, modifiedModel.InvoiceHeaderText, BranchesModel.COL_InvoiceHeaderText.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, BranchesModel.COL_Active.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                    {
                        db.Entry(modifiedModel).State = EntityState.Modified;
                        ActivityLogsController.AddEditLog(db, Session, modifiedModel.Id, log);
                        db.SaveChanges();
                        BranchSelectLists.update();
                    }

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setEditViewBags(FILTER_Keyword, FILTER_Active);
            return View(modifiedModel);
        }

        private void setEditViewBags(string FILTER_Keyword, int? FILTER_Active)
        {
            setFilterViewBags(FILTER_Keyword, FILTER_Active);
        }

        /* METHODS ********************************************************************************************************************************************/

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<BranchesModel>(@"
                        SELECT Branches.*
                        FROM Branches
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR Branches.Name = @Name)
							AND (@Id IS NULL OR (Branches.Name = @Name AND Branches.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(BranchesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(BranchesModel.COL_Name.Name, Name)
                ).Count() > 0;
        }

        public List<BranchesModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(null, FILTER_Active, FILTER_Keyword); }
        public BranchesModel get(Guid Id) { return get(Id, null, null).FirstOrDefault(); }
        public static List<BranchesModel> get(Guid? Id, int? FILTER_Active, string FILTER_Keyword)
        {
            List<BranchesModel> models = new DBContext().Database.SqlQuery<BranchesModel>(@"
                        SELECT Branches.*
                        FROM Branches
                        WHERE 1=1
							AND (@Id IS NULL OR Branches.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR Branches.Active = @Active)
    							AND (@FILTER_Keyword IS NULL OR (Branches.Name LIKE '%'+@FILTER_Keyword+'%'))
                            ))
						ORDER BY Branches.Name ASC
                    ",
                    DBConnection.getSqlParameter(BranchesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(BranchesModel.COL_Active.Name, FILTER_Active),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();

            return models;
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller)
        {
            controller.ViewBag.Branches = new SelectList(db.Branches.AsNoTracking().OrderBy(x => x.Name).ToList(), BranchesModel.COL_Id.Name, BranchesModel.COL_Name.Name);
        }

        /******************************************************************************************************************************************************/
    }
}