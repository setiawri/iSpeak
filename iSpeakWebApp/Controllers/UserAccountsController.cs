using iSpeakWebApp.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System;
using System.Collections.Generic;

using LIBUtil;
using LIBWebMVC;

namespace iSpeakWebApp.Controllers
{
    public class UserAccountsController : Controller
    {
        public const string ACTIONNAME_Login = "Login";
        public const string CONTROLLERNAME = "UserAccounts";
        
        public const string SESSION_UserAccount = "UserAccount";
        public const string SESSION_UserAccountAccess = "UserAccountAccess";
        public const string SESSION_ActiveBranches_Id = "ActiveBranches_Id";
        public const string SESSION_OnlineToken = "OnlineToken";
        public const string SESSION_Branches_Models = "Branches_Models";
        public const string SESSION_ShowOnlyUserData = "ShowOnlyUserData";
        public const string SESSION_ConnectToLiveDatabase = "ConnectToLiveDatabase";


        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
            ViewBag.FILTER_Languages_Id = FILTER_Languages_Id;
            UserAccountRolesController.setDropDownListViewBag(this, getUserAccess(Session).UserAccounts_EditRoles);
            BranchesController.setDropDownListViewBag(this);
            LanguagesController.setDropDownListViewBag(this);
            PromotionEventsController.setDropDownListViewBag(this);
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: UserAccounts
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).UserAccounts_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                if (UtilWebMVC.hasNoFilter(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id))
                    return View();
                else
                {
                    string roles = "";
                    if (!getUserAccess(Session).UserAccounts_EditRoles)
                        roles = SettingsController.get().StudentRole.ToString();
                    return View(get(null, null, FILTER_Keyword, FILTER_Active, FILTER_Languages_Id, roles, false));
                }
            }
        }

        // POST: UserAccounts
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id);

            string roles = "";
            if (!getUserAccess(Session).UserAccounts_ViewAllRoles)
                roles = SettingsController.get().StudentRole.ToString();
            return View(get(null, null, FILTER_Keyword, FILTER_Active, FILTER_Languages_Id, roles, false));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: UserAccounts/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).UserAccounts_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id);
            return View(new UserAccountsModel());
        }

        // POST: UserAccounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserAccountsModel model, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            if (ModelState.IsValid)
            {
                model.Fullname = model.Fullname.Replace("  ", " ").Replace("  ", " ");
                if (isExists(model.Fullname, model.Birthday))
                    ModelState.AddModelError(UserAccountsModel.COL_Fullname.Name, $"{model.Fullname} dengan tanggal lahir yang sama sudah terdaftar");
                else
                {
                    model.Password = HashPassword(SettingsController.get().ResetPassword);
                    model.Branches_Id = Helper.getActiveBranchId(Session);
                    model.Branches = model.Branches_Id.ToString();
                    model.Username = generateUsername(model.Fullname, model.Birthday);
                    if (!getUserAccess(Session).UserAccounts_EditRoles)
                        model.Roles = SettingsController.get().StudentRole.ToString();

                    add(model);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Edit), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active, FILTER_Languages_Id = FILTER_Languages_Id });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: UserAccounts/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).UserAccounts_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id);
            return View(get((Guid)id));
        }

        // POST: UserAccounts/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserAccountsModel modifiedModel, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            if (ModelState.IsValid)
            {
                modifiedModel.Fullname = modifiedModel.Fullname.Replace("  ", " ").Replace("  ", " ");
                if (isExists(modifiedModel.Id, modifiedModel.Username))
                    ModelState.AddModelError(UserAccountsModel.COL_Username.Name, $"{modifiedModel.Username} sudah terdaftar");
                else
                {
                    UserAccountsModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Username, modifiedModel.Username, UserAccountsModel.COL_Username.LogDisplay);
                    log = Helper.append(log, originalModel.Fullname, modifiedModel.Fullname, UserAccountsModel.COL_Fullname.LogDisplay);
                    log = Helper.append(log, originalModel.Birthday, modifiedModel.Birthday, UserAccountsModel.COL_Birthday.LogDisplay);
                    log = Helper.append<BranchesModel>(log, originalModel.Branches_Id, modifiedModel.Branches_Id, UserAccountsModel.COL_Branches_Id.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, UserAccountsModel.COL_Active.LogDisplay);
                    log = Helper.append(log, originalModel.ResetPassword, modifiedModel.ResetPassword, UserAccountsModel.COL_ResetPassword.LogDisplay);
                    log = Helper.append(log, originalModel.Email, modifiedModel.Email, UserAccountsModel.COL_Email.LogDisplay);
                    log = Helper.append(log, originalModel.Address, modifiedModel.Address, UserAccountsModel.COL_Address.LogDisplay);
                    log = Helper.append(log, originalModel.Phone1, modifiedModel.Phone1, UserAccountsModel.COL_Phone1.LogDisplay);
                    log = Helper.append(log, originalModel.Phone2, modifiedModel.Phone2, UserAccountsModel.COL_Phone2.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, UserAccountsModel.COL_Notes.LogDisplay);
                    log = Helper.append<PromotionEventsModel>(log, originalModel.PromotionEvents_Id, modifiedModel.PromotionEvents_Id, UserAccountsModel.COL_PromotionEvents_Id.LogDisplay);

                    log = Helper.addLogForList<BranchesModel>(log, originalModel.Branches_List, modifiedModel.Branches_List, UserAccountsModel.COL_Branches.LogDisplay);
                    log = Helper.addLogForList<UserAccountRolesModel>(log, originalModel.Roles_List, modifiedModel.Roles_List, UserAccountsModel.COL_Roles.LogDisplay);
                    log = Helper.addLogForList<LanguagesModel>(log, originalModel.Interest_List, modifiedModel.Interest_List, UserAccountsModel.COL_Interest.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                        update(modifiedModel, log);

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active, FILTER_Languages_Id = FILTER_Languages_Id });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id);
            return View(modifiedModel);
        }

        /* RESET PASSWORD *************************************************************************************************************************************/

        public JsonResult Ajax_ResetPassword(Guid id)
        {
            UserAccountsModel model = new UserAccountsModel();
            model.Id = id;
            model.Password = HashPassword(SettingsController.get().ResetPassword);
            model.ResetPassword = true;
            updatePassword(model, "Password reset by admin");

            return Json(new { Message = "Password has been reset" });
        }

        /* LOGIN PAGE *****************************************************************************************************************************************/

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserAccountsModel model, bool ConnectToLiveDatabase, string returnUrl)
        {
            //bypass login
            if (Server.MachineName == Helper.DEVCOMPUTERNAME)
            {
                if (string.IsNullOrEmpty(model.Username))
                    model.Username = "ricky";
                if (string.IsNullOrEmpty(model.Password))
                    model.Password = "A2cdefGH";
            }

            string hashedPassword = HashPassword(model.Password);
            UserAccountsModel userAccount = get(model.Username, hashedPassword);

            if (userAccount == null || string.IsNullOrWhiteSpace(hashedPassword))
            {
                ModelState.AddModelError("", "Invalid username or password");
                ViewBag.Username = model.Username;
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
            else
            {
                setLoginSession(Session, userAccount, ConnectToLiveDatabase);
                if (!userAccount.ResetPassword)
                    return RedirectToLocal(returnUrl);
                else
                {
                    TempData["UserAccountsModel"] = userAccount;
                    return RedirectToAction(nameof(UserAccountsController.ChangePassword), CONTROLLERNAME, new { returnUrl = returnUrl });
                }
            }
        }

        /* CHANGE PASSWORD PAGE *******************************************************************************************************************************/

        public ActionResult ChangePassword(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            UserAccountsModel model = (UserAccountsModel)TempData["UserAccountsModel"];
            if (model != null && model.ResetPassword)
                ViewBag.CurrentPassword = SettingsController.get().ResetPassword;
            object Id = getUserId(Session);
            if (model == null)
            {
                if(Id == null)
                    return RedirectToAction(nameof(Login));
                else
                    model = get((Guid)Id);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(Guid Id, string CurrentPassword, string NewPassword, string ConfirmPassword, string returnUrl)
        {
            UserAccountsModel model = get((Guid)Id);
            string SanitizedNewPassword = Util.sanitizeString(NewPassword);

            if (HashPassword(CurrentPassword) != model.Password)
                ModelState.AddModelError("", "Invalid current password");
            else if (string.IsNullOrEmpty(SanitizedNewPassword) || SanitizedNewPassword.Length < 6)
                ModelState.AddModelError("", "Invalid new password. Must be at least 6 characters");
            else if(string.IsNullOrEmpty(ConfirmPassword) || SanitizedNewPassword != ConfirmPassword)
                ModelState.AddModelError("", "Invalid confirm password");
            else
            {
                model.Password = HashPassword(SanitizedNewPassword);
                model.ResetPassword = false;
                updatePassword(model, "Password changed");

                return RedirectToLocal(returnUrl);
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.NewPassword = NewPassword;
            ViewBag.ConfirmPassword = ConfirmPassword;
            return View(model);
        }

        /* BIRTHDAYS ******************************************************************************************************************************************/

        public PartialViewResult BirthdaysPartial(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            List<UserAccountsModel> models = getBirthdays(Helper.getActiveBranchId(Session), null, Helper.getCurrentDateTime().Month);

            ViewBag.BirthdayCount = models.Count;
            UserAccountRolesController.setDropDownListViewBag(this);

            return PartialView("BirthdaysPartial");
        }

        public JsonResult Ajax_GetBirthdayData(int? month, Guid? UserAccountRoles_Id)
        {
            List<UserAccountsModel> models = getBirthdays(Helper.getActiveBranchId(Session), UserAccountRoles_Id, (int)month);
            
            return Json(new { result = models}, JsonRequestBehavior.AllowGet);
        }

        /* METHODS ********************************************************************************************************************************************/

        public ActionResult Ajax_UpdateActiveBranch(Guid BranchId, string returnUrl)
        {
            Session[SESSION_ActiveBranches_Id] = BranchId;

            return RedirectToLocal(returnUrl);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public static string HashPassword(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            else
            {
                var hash = new System.Security.Cryptography.SHA1Managed().ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                return string.Concat(hash.Select(b => b.ToString("x2")));
            }
        }

        public static object getUserId(HttpSessionStateBase Session)
        {
            if (getUserAccount(Session) == null)
                return null;
            else
                return getUserAccount(Session).Id;
        }

        public static object get_OnlineToken(HttpSessionStateBase Session)
        {
            if (getUserAccount(Session) == null)
                return null;
            else
                return getUserAccount(Session).Id;
        }

        public static bool getShowOnlyUserData(HttpSessionStateBase Session)
        {
            return (bool)Session[SESSION_ShowOnlyUserData];
        }

        public static UserAccountsModel getUserAccount(HttpSessionStateBase Session)
        {
            if (Session == null || Session[SESSION_UserAccount] == null)
                return null;
            else
                return (UserAccountsModel)Session[SESSION_UserAccount];
        }

        public static UserAccountRolesModel getUserAccess(HttpSessionStateBase Session)
        {
            return (UserAccountRolesModel)Session[SESSION_UserAccountAccess];
        }

        public static SelectList getDDLBranches(HttpSessionStateBase Session)
        {
            return new SelectList((List<BranchesModel>)Session[SESSION_Branches_Models], BranchesModel.COL_Id.Name, BranchesModel.COL_Name.Name, Helper.getActiveBranchId(Session).ToString());
        }

        public static bool isLoggedIn(HttpSessionStateBase Session)
        {
            return getUserAccount(Session) != null;
        }

        public static bool isChangePassword(HttpSessionStateBase Session)
        {
            return getUserAccount(Session).ResetPassword;
        }

        public static void setLoginSession(HttpSessionStateBase Session, UserAccountsModel model) { setLoginSession(Session, model, null); }
        public static void setLoginSession(HttpSessionStateBase Session, UserAccountsModel model, bool? ConnectToLiveDatabase)
        {
            if (model != null)
            {
                Session[SESSION_UserAccount] = model;
                Session[SESSION_ActiveBranches_Id] = model.Branches_Id;
                Session[SESSION_Branches_Models] = BranchesController.get(1, model.Branches);
                Session[SESSION_UserAccountAccess] = UserAccountRolesController.getAccesses(model);
                if (ConnectToLiveDatabase != null) Session[SESSION_ConnectToLiveDatabase] = ConnectToLiveDatabase;

                Session[SESSION_ShowOnlyUserData] = true;
                string ShowOnlyOwnUserData = SettingsController.get().ShowOnlyOwnUserData;
                string Roles = model.Roles;
                foreach(string role in Roles.Split(','))
                {
                    if(!ShowOnlyOwnUserData.Contains(role))
                    {
                        Session[SESSION_ShowOnlyUserData] = false;
                        break;
                    }
                }
            }
        }

        public static void updateLoginSession(HttpSessionStateBase Session)
        {
            setLoginSession(Session, new UserAccountsController().get(getUserAccount(Session).Id));
        }

        public ActionResult LogOff()
        {
            Session[SESSION_UserAccount] = null;
            return RedirectToAction(nameof(Login));
        }

        public string generateUsername(string Fullname, DateTime Birthday)
        {
            string Username;
            int charCount = 3;

            List<string> nameArray = Fullname.Split().ToList();
            if (nameArray.Count == 1)
                nameArray.Add(nameArray[0]); //name must consist of 2 words. duplicate first name as last name

            Username = generateUsername(nameArray, Birthday, charCount);

            //verify username doesn't exist
            while (isExists(null, Username) && charCount <= nameArray[0].Length)
            {
                Username = generateUsername(nameArray, Birthday, charCount++);
            }

            return Username;
        }

        public string generateUsername(List<string> nameArray, DateTime Birthday, int charCount)
        {
            string first;
            if (nameArray[0].Length < charCount)
                first = nameArray[0];
            else
                first = nameArray[0].Substring(0, charCount);

            string last;
            if (nameArray[nameArray.Count - 1].Length < charCount)
                last = nameArray[nameArray.Count - 1];
            else
                last = nameArray[nameArray.Count - 1].Substring(0, charCount);

            return string.Format("{0}{1}{2:##00}{3:##00}", first, last, Birthday.Day, Birthday.Month);
        }

        public JsonResult Ajax_GetDDLItems(string keyword, int page, int take, string key)
        {
            int skip = take * (page - 1);
            List<UserAccountsModel> models = get(skip, take, keyword, 1, null, key, SettingsController.ShowOnlyOwnUserData(Session));

            List<Select2Pagination.Select2Results> results = new List<Select2Pagination.Select2Results>();
            results.AddRange(models.Select(model => new Select2Pagination.Select2Results
            {
                id = model.Id,
                text = model.Fullname
            }));

            Select2Pagination.Select2Page pagination = new Select2Pagination.Select2Page
            {
                more = results.Count() == take ? true : false
            };

            return Json(new { results, pagination }, JsonRequestBehavior.AllowGet);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Username)
        {
            return db.Database.SqlQuery<UserAccountsModel>(@"
                        SELECT UserAccounts.*
                        FROM UserAccounts
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR UserAccounts.Username = @Username)
							AND (@Id IS NULL OR (UserAccounts.Username = @Username AND UserAccounts.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Username.Name, Username)
                ).Count() > 0;
        }

        public bool isExists(string Fullname, DateTime Birthday) 
        {
            return db.Database.SqlQuery<UserAccountsModel>(@"
                        SELECT UserAccounts.*
                        FROM UserAccounts
                        WHERE UserAccounts.Fullname = @Fullname
							AND UserAccounts.Birthday = @Birthday
                    ",
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Fullname.Name, Fullname),
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Birthday.Name, Birthday)
                ).Count() > 0;
        }

        public UserAccountsModel get(string Username, string Password) { return get(null, null, null, true, null, Username, Password, null, null, null, null, null, null, false).FirstOrDefault(); }
        public List<UserAccountsModel> getBirthdays(Guid Branches_Id, Guid? UserAccountRoles_Id, int BirthdayListMonth) { return get(null, null, Branches_Id, false, null, null, null, 1, UserAccountRoles_Id, BirthdayListMonth, null, null, null, false); }
        public List<UserAccountsModel> get(int? skip, int? take, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id, string Role, bool ShowOnlyOwnUserData) { return get(skip, take, null, false, null, null, null, FILTER_Active, null, null, FILTER_Keyword, FILTER_Languages_Id, Role, ShowOnlyOwnUserData); }
        public UserAccountsModel get(Guid Id) { return get(null, null, null, true, Id, null, null, null, null, null, null, null, null, false).FirstOrDefault(); }
        public List<UserAccountsModel> get(int? skip, int? take, Guid? Default_Branches_Id, bool showAllBranches, Guid? Id, string Username, string Password, int? Active, Guid? UserAccountRoles_Id, int? BirthdayListMonth, string FILTER_Keyword, Guid? Language_Id, string Role, bool ShowOnlyOwnUserData)
        {
            UserAccountsModel UserAccount = getUserAccount(Session);
            Guid? UserAccountId = null;
            if (UserAccount != null)
                UserAccountId = UserAccount.Id;

            string BranchClause = null;
            if (!showAllBranches)
                BranchClause = string.Format(" AND UserAccounts.Branches LIKE '%{0}%' ", Helper.getActiveBranchId(Session));

            string RoleClause = null;
            if (!string.IsNullOrEmpty(Role))
                RoleClause = string.Format(" AND UserAccounts.Roles LIKE '%{0}%' ", Role);

            string sql = string.Format(@"
                        SELECT UserAccounts.*,
                            COALESCE(LessonPackages.ActiveLessonPackages,0) AS ActiveLessonPackages,
                            COALESCE(ClubSubscription.RemainingClubSubscriptionDays,0) AS RemainingClubSubscriptionDays
                        FROM UserAccounts
							LEFT JOIN (
									SELECT SaleInvoices.Customer_UserAccounts_Id, COUNT(SaleInvoiceItems.Id) AS ActiveLessonPackages
									FROM SaleInvoiceItems
										LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
									WHERE SaleInvoices.Due = 0 
                                        AND CURRENT_TIMESTAMP > SaleInvoiceItems.StartingDate 
                                        AND SaleInvoiceItems.SessionHours_Remaining > 0 
                                        AND SaleInvoices.Cancelled = 0
									GROUP BY SaleInvoices.Customer_UserAccounts_Id
								) LessonPackages ON LessonPackages.Customer_UserAccounts_Id = UserAccounts.Id
							LEFT JOIN (
									SELECT SaleInvoices.Customer_UserAccounts_Id, MAX(DATEDIFF(DAY, CURRENT_TIMESTAMP, DATEADD(MONTH, SaleInvoiceItems.ExpirationMonth, SaleInvoiceItems.StartingDate))) AS RemainingClubSubscriptionDays
									FROM SaleInvoiceItems
										LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
									WHERE SaleInvoices.Due = 0 
                                        AND SaleInvoiceItems.IsClubSubscription = 1
                                        AND CURRENT_TIMESTAMP > SaleInvoiceItems.StartingDate 
                                        AND DATEDIFF(DAY, CURRENT_TIMESTAMP, DATEADD(MONTH, SaleInvoiceItems.ExpirationMonth, SaleInvoiceItems.StartingDate)) > 0
									GROUP BY SaleInvoices.Customer_UserAccounts_Id
								) ClubSubscription ON ClubSubscription.Customer_UserAccounts_Id = UserAccounts.Id
                        WHERE 1=1
							AND (@Id IS NULL OR UserAccounts.Id = @Id)
                            AND (@Id IS NOT NULL OR (
                                @Username IS NULL OR UserAccounts.Username = @Username)
							    AND (@Password IS NULL OR UserAccounts.Password = @Password)
							    AND (@Branches_Id IS NULL OR UserAccounts.Branches_Id = @Branches_Id)
							    AND (@Languages_Id IS NULL OR UserAccounts.Interest LIKE '%'+CONVERT(varchar(MAX),@Languages_Id)+'%')
							    AND (@Active IS NULL OR UserAccounts.Active = @Active)
							    AND (@UserAccountRoles_Id IS NULL OR UserAccounts.Roles LIKE '%'+CONVERT(varchar(MAX),@UserAccountRoles_Id)+'%')
							    AND (@BirthdayListMonth IS NULL OR (
									    MONTH(UserAccounts.Birthday) = @BirthdayListMonth
									    AND (MONTH(GETDATE()) <> @BirthdayListMonth OR DAY(UserAccounts.Birthday) >= DAY(GETDATE()))
								    )
							    )
							    AND (@FILTER_Keyword IS NULL OR (
                                        UserAccounts.Fullname LIKE '%'+@FILTER_Keyword+'%' 
                                        OR UserAccounts.Username LIKE '%'+@FILTER_Keyword+'%'
                                        OR UserAccounts.No LIKE '%'+@FILTER_Keyword+'%'
                                    ))
                                AND (@ShowOnlyOwnUserData = 0 OR (UserAccounts.Id = @UserAccounts_Id))
                                {0}{1}
                            )
						ORDER BY UserAccounts.Fullname ASC
                        OFFSET COALESCE(@SKIP,0) ROWS
                        FETCH NEXT COALESCE(CONVERT(int, @TAKE),0x7ffffff) ROWS ONLY
                    ", BranchClause, RoleClause
                );

            List<UserAccountsModel> models = db.Database.SqlQuery<UserAccountsModel>(sql,
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Username.Name, Username),
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Password.Name, Password),
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Active.Name, Active),
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Branches_Id.Name, Default_Branches_Id),
                    DBConnection.getSqlParameter("BirthdayListMonth", BirthdayListMonth),
                    DBConnection.getSqlParameter("UserAccountRoles_Id", UserAccountRoles_Id),
                    DBConnection.getSqlParameter("Languages_Id", Language_Id),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                    DBConnection.getSqlParameter("SKIP", skip),
                    DBConnection.getSqlParameter("TAKE", take),
                    DBConnection.getSqlParameter("ShowOnlyOwnUserData", ShowOnlyOwnUserData),
                    DBConnection.getSqlParameter("UserAccounts_Id", UserAccountId)
                ).ToList();

            foreach (UserAccountsModel model in models)
            {
                if (!string.IsNullOrEmpty(model.Roles)) model.Roles_List = model.Roles.Split(',').ToList();
                if (!string.IsNullOrEmpty(model.Interest)) model.Interest_List = model.Interest.Split(',').ToList();
                if (!string.IsNullOrEmpty(model.Branches)) model.Branches_List = model.Branches.Split(',').ToList();
            }

            return models;
        }

        public void updatePassword(UserAccountsModel model, string log)
        {
            WebDBConnection.Update(db.Database, "UserAccounts",
                DBConnection.getSqlParameter(UserAccountsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Password.Name, model.Password),
                DBConnection.getSqlParameter(UserAccountsModel.COL_ResetPassword.Name, model.ResetPassword)
            );

            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        public static void update_OnlineToken(DBContext db, string Id, string OnlineToken)
        {
            WebDBConnection.Update(db.Database, "UserAccounts",
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Id.Name, new Guid(Id)),
                    DBConnection.getSqlParameter(UserAccountsModel.COL_OnlineToken.Name, OnlineToken)
                );
        }

        public void update(UserAccountsModel model, string log)
        {
            if(model.Roles_List != null) model.Roles = string.Join(",", model.Roles_List.ToArray());
            if(model.Interest_List != null) model.Interest = string.Join(",", model.Interest_List.ToArray());
            if (model.Branches_List != null) model.Branches = string.Join(",", model.Branches_List.ToArray());

            WebDBConnection.Update(db.Database, "UserAccounts",
                DBConnection.getSqlParameter(UserAccountsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Username.Name, model.Username),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Fullname.Name, model.Fullname),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Birthday.Name, model.Birthday),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Branches.Name, model.Branches),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(UserAccountsModel.COL_ResetPassword.Name, model.ResetPassword),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Email.Name, model.Email),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Address.Name, model.Address),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Phone1.Name, model.Phone1),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Phone2.Name, model.Phone2),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Interest.Name, model.Interest),
                DBConnection.getSqlParameter(UserAccountsModel.COL_PromotionEvents_Id.Name, model.PromotionEvents_Id),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Roles.Name, model.Roles)
            );

            updateLoginSession(Session);

            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        public void add(UserAccountsModel model)
        {
            db.Database.ExecuteSqlCommand(@"

	            -- INCREMENT LAST HEX NUMBER
	            DECLARE @HexLength int = 5, @LastHex_String varchar(5), @NewNo varchar(5)
	            SELECT @LastHex_String = ISNULL(MAX(No),'') From UserAccounts	
	            DECLARE @LastHex_Int int
	            SELECT @LastHex_Int = CONVERT(INT, CONVERT(VARBINARY, REPLICATE('0', LEN(@LastHex_String)%2) + @LastHex_String, 2)) --@LastHex_String length must be even number of digits to convert to int
	            SET @NewNo = RIGHT(CONVERT(NVARCHAR(10), CONVERT(VARBINARY(8), @LastHex_Int + 1), 1),@HexLength)

                INSERT INTO UserAccounts (Id, No,    Fullname, Username, Password, Birthday, Branches_Id, ResetPassword, Active, Roles, Branches) 
                                VALUES(  @Id,@NewNo,@Fullname,@Username,@Password,@Birthday,@Branches_Id,@ResetPassword,@Active,@Roles,@Branches);
            ",
                DBConnection.getSqlParameter(UserAccountsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Username.Name, model.Username),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Password.Name, model.Password),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Fullname.Name, model.Fullname),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Birthday.Name, model.Birthday),
                DBConnection.getSqlParameter(UserAccountsModel.COL_ResetPassword.Name, model.ResetPassword),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Roles.Name, model.Roles),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Branches.Name, model.Branches)
            );

            ActivityLogsController.AddCreateLog(db, Session, model.Id);
        }

        /******************************************************************************************************************************************************/
    }
}