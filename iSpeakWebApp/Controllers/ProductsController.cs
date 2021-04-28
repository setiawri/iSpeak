using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: Products
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Products_View)
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

        // POST: Products
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active)
        {
            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: Products/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Products_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(new ProductsModel());
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductsModel model, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name))
                    ModelState.AddModelError(ProductsModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
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

        // GET: Products/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Products_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get((Guid)id));
        }

        // POST: Products/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductsModel modifiedModel, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(ProductsModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    ProductsModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, ProductsModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, ProductsModel.COL_Active.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, ProductsModel.COL_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Description, modifiedModel.Description, ProductsModel.COL_Description.LogDisplay);
                    log = Helper.append(log, originalModel.Barcode, modifiedModel.Barcode, ProductsModel.COL_Barcode.LogDisplay);
                    log = Helper.append<UnitsModel>(log, originalModel.Units_Id, modifiedModel.Units_Id, ProductsModel.COL_Units_Id.LogDisplay);
                    log = Helper.append(log, originalModel.BuyPrice, modifiedModel.BuyPrice, ProductsModel.COL_BuyPrice.LogDisplay);
                    log = Helper.append(log, originalModel.SellPrice, modifiedModel.SellPrice, ProductsModel.COL_SellPrice.LogDisplay);
                    log = Helper.append(log, originalModel.ForSale, modifiedModel.ForSale, ProductsModel.COL_ForSale.LogDisplay);

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

        public static void setDropDownListViewBag(ControllerBase controller)
        {
            controller.ViewBag.Products = new SelectList(get(1, 1).OrderBy(x => x.Description), ProductsModel.COL_Id.Name, ProductsModel.COL_DDLDescription.Name);
        }

        public static void setViewBag(ControllerBase controller)
        {
            controller.ViewBag.ProductsModels = get(1, 1);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<ProductsModel>(@"
                        SELECT Products.*,
                            NULL AS Units_Name
                        FROM Products
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR Products.Name = @Name)
							AND (@Id IS NULL OR (Products.Name = @Name AND Products.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(ProductsModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ProductsModel.COL_Name.Name, Name)
                ).Count() > 0;
        }

        public List<ProductsModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(null, FILTER_Active, null, FILTER_Keyword); }
        public ProductsModel get(Guid Id) { return get(Id, null, null, null).FirstOrDefault(); }
        public static List<ProductsModel> get(int? Active, int? ForSale) { return get(null, Active, ForSale, null); }
        public static List<ProductsModel> get() { return get(null, null, null, null); }
        public static List<ProductsModel> get(Guid? Id, int? Active, int? ForSale, string FILTER_Keyword)
        {
            return new DBContext().Database.SqlQuery<ProductsModel>(@"
                    SELECT Products.*,
                        Units.Name AS Units_Name,
                        ISNULL(InventoryCount.AvailableQty,0) AS AvailableQty,
                        Products.Name + ' (Available: ' + FORMAT(ISNULL(InventoryCount.AvailableQty,0),'N0') + ') ' + FORMAT(Products.SellPrice,'N0') AS DDLDescription
                    FROM Products
                        LEFT JOIN Units ON Units.Id = Products.Units_Id
                        LEFT JOIN (
                                SELECT Inventory.Products_Id, SUM(Inventory.AvailableQty) AS AvailableQty
                                FROM Inventory
                                GROUP BY Products_Id
                            ) InventoryCount ON InventoryCount.Products_Id = Products.Id
                    WHERE 1=1
						AND (@Id IS NULL OR Products.Id = @Id)
						AND (@Id IS NOT NULL OR (
                            (@Active IS NULL OR Products.Active = @Active)
    						AND (@FILTER_Keyword IS NULL OR (Products.Name LIKE '%'+@FILTER_Keyword+'%'))
                            AND (@ForSale IS NULL OR Products.ForSale = @ForSale)
                        ))
					ORDER BY Products.Name ASC
                ",
                DBConnection.getSqlParameter(ProductsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(ProductsModel.COL_Active.Name, Active),
                DBConnection.getSqlParameter(ProductsModel.COL_ForSale.Name, ForSale),
                DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
            ).ToList();
        }

        public void update(ProductsModel model, string log)
        {
            db.Database.ExecuteSqlCommand(@"
                UPDATE Products 
                SET
                    Name = @Name,
                    Active = @Active,
                    Notes = @Notes,
                    Description = @Description,
                    Barcode = @Barcode,
                    Units_Id = @Units_Id,
                    ForSale = @ForSale,
                    BuyPrice = @BuyPrice,
                    SellPrice = @SellPrice
                WHERE Products.Id = @Id;                
            ",
                DBConnection.getSqlParameter(ProductsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(ProductsModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(ProductsModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(ProductsModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(ProductsModel.COL_Description.Name, model.Description),
                DBConnection.getSqlParameter(ProductsModel.COL_Barcode.Name, model.Barcode),
                DBConnection.getSqlParameter(ProductsModel.COL_Units_Id.Name, model.Units_Id),
                DBConnection.getSqlParameter(ProductsModel.COL_ForSale.Name, model.ForSale),
                DBConnection.getSqlParameter(ProductsModel.COL_BuyPrice.Name, model.BuyPrice),
                DBConnection.getSqlParameter(ProductsModel.COL_SellPrice.Name, model.SellPrice)
            );

            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        public void add(ProductsModel model)
        {
            db.Database.ExecuteSqlCommand(@"
                INSERT INTO Products   (Id, Name, Active, Notes, Description, Barcode, Units_Id, ForSale, BuyPrice, SellPrice) 
                                VALUES(@Id,@Name,@Active,@Notes,@Description,@Barcode,@Units_Id,@ForSale,@BuyPrice,@SellPrice);
            ",
                DBConnection.getSqlParameter(ProductsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(ProductsModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(ProductsModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(ProductsModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(ProductsModel.COL_Description.Name, model.Description),
                DBConnection.getSqlParameter(ProductsModel.COL_Barcode.Name, model.Barcode),
                DBConnection.getSqlParameter(ProductsModel.COL_Units_Id.Name, model.Units_Id),
                DBConnection.getSqlParameter(ProductsModel.COL_ForSale.Name, model.ForSale),
                DBConnection.getSqlParameter(ProductsModel.COL_BuyPrice.Name, model.BuyPrice),
                DBConnection.getSqlParameter(ProductsModel.COL_SellPrice.Name, model.SellPrice)
            );

            ActivityLogsController.AddCreateLog(db, Session, model.Id);
            db.SaveChanges();
        }

        /******************************************************************************************************************************************************/
    }
}