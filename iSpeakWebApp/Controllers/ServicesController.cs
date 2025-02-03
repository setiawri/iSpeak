using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    /*
     * Services is NOT controller by franchise. Can only be changed by central
     */

    public class ServicesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: Services
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Services_View)
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

        // POST: Services
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active)
        {
            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: Services/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Services_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(new ServicesModel());
        }

        // POST: Services/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ServicesModel model, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name))
                    ModelState.AddModelError(ServicesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    add(model);
                    return RedirectToAction(nameof(Edit), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: Services/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Services_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get((Guid)id));
        }

        // POST: Services/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ServicesModel modifiedModel, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(ServicesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    ServicesModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, ServicesModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, ServicesModel.COL_Active.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, ServicesModel.COL_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Description, modifiedModel.Description, ServicesModel.COL_Description.LogDisplay);
                    log = Helper.append<UnitsModel>(log, originalModel.Units_Id, modifiedModel.Units_Id, ServicesModel.COL_Units_Id.LogDisplay);
                    log = Helper.append(log, originalModel.ForSale, modifiedModel.ForSale, ServicesModel.COL_ForSale.LogDisplay);
                    log = Helper.append(log, originalModel.SellPrice, modifiedModel.SellPrice, ServicesModel.COL_SellPrice.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                        update(modifiedModel, log);

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
            UnitsController.setDropDownListViewBag(this);
        }

        public static void setDropDownListViewBag(Controller controller)
        {
            controller.ViewBag.Services = new SelectList(get(1, 1), ServicesModel.COL_Id.Name, ServicesModel.COL_DDLDescription.Name);
        }

        public static void setViewBag(Controller controller)
        {
            controller.ViewBag.ServicesModels = get(1, 1);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<ServicesModel>(@"
                        SELECT Services.*,
                            NULL AS Units_Name
                        FROM Services
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR Services.Name = @Name)
							AND (@Id IS NULL OR (Services.Name = @Name AND Services.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(ServicesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ServicesModel.COL_Name.Name, Name)
                ).Count() > 0;
        }

        public List<ServicesModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(null, FILTER_Active, null, FILTER_Keyword); }
        public ServicesModel get(Guid Id) { return get(Id, null, null, null).FirstOrDefault(); }
        public static List<ServicesModel> get(int? Active, int? ForSale) { return get(null, Active, ForSale, null); }
        public static List<ServicesModel> get() { return get(null, null, null, null); }
        public static List<ServicesModel> get(Guid? Id, int? Active, int? ForSale, string FILTER_Keyword)
        {
            return new DBContext().Database.SqlQuery<ServicesModel>(@"
                    SELECT Services.*,
                        Units.Name AS Units_Name,
                        Services.Name + ' (' + FORMAT(Services.SellPrice,'N0') + ')' AS DDLDescription
                    FROM Services
                        LEFT JOIN Units ON Units.Id = Services.Units_Id
                    WHERE 1=1
						AND (@Id IS NULL OR Services.Id = @Id)
						AND (@Id IS NOT NULL OR (
                            (@Active IS NULL OR Services.Active = @Active)
    						AND (@FILTER_Keyword IS NULL OR (Services.Name LIKE '%'+@FILTER_Keyword+'%'))
                            AND (@ForSale IS NULL OR Services.ForSale = @ForSale)
                        ))
					ORDER BY Services.Name ASC
                ",
                DBConnection.getSqlParameter(ServicesModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(ServicesModel.COL_Active.Name, Active),
                DBConnection.getSqlParameter(ServicesModel.COL_ForSale.Name, ForSale),
                DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
            ).ToList();
        }

        public void update(ServicesModel model, string log)
        {
            LIBWebMVC.WebDBConnection.Update(db.Database, "Services",
                DBConnection.getSqlParameter(ServicesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(ServicesModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(ServicesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(ServicesModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(ServicesModel.COL_Description.Name, model.Description),
                DBConnection.getSqlParameter(ServicesModel.COL_Units_Id.Name, model.Units_Id),
                DBConnection.getSqlParameter(ServicesModel.COL_ForSale.Name, model.ForSale),
                DBConnection.getSqlParameter(ServicesModel.COL_SellPrice.Name, model.SellPrice)
            );

            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
        }

        public void add(ServicesModel model)
        {
            LIBWebMVC.WebDBConnection.Insert(db.Database, "Services",
                DBConnection.getSqlParameter(ServicesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(ServicesModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(ServicesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(ServicesModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(ServicesModel.COL_Description.Name, model.Description),
                DBConnection.getSqlParameter(ServicesModel.COL_Units_Id.Name, model.Units_Id),
                DBConnection.getSqlParameter(ServicesModel.COL_ForSale.Name, model.ForSale),
                DBConnection.getSqlParameter(ServicesModel.COL_SellPrice.Name, model.SellPrice)
            );

            ActivityLogsController.AddCreateLog(db, Session, model.Id);
        }

        /******************************************************************************************************************************************************/
    }
}