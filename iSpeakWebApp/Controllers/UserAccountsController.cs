﻿using iSpeakWebApp.Models;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using LIBUtil;

using System.Web.Mvc;

namespace iSpeakWebApp.Controllers
{
    public class UserAccountsController : Controller
    {
        public const string ACTIONNAME_Login = "Login";
        public const string CONTROLLERNAME = "UserAccounts";
        
        public const string SESSION_UserAccount = "UserAccount";
        public const string SESSION_UserAccountAccess = "UserAccountAccess";
        public const string SESSION_ActiveBranches_Id = "ActiveBranches_Id";


        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
            ViewBag.FILTER_Languages_Id = FILTER_Languages_Id;
            UserAccountRolesController.setDropDownListViewBag(this);
            BranchesController.setDropDownListViewBag(this);
            LanguagesController.setDropDownListViewBag(this);
            PromotionEventsController.setDropDownListViewBag(this);
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: UserAccounts
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
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

        // POST: UserAccounts
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id);
            return View(get(FILTER_Keyword, FILTER_Active, FILTER_Languages_Id));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: UserAccounts/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id)
        {
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
                if (isExists(model.Fullname, model.Birthday))
                    ModelState.AddModelError(UserAccountsModel.COL_Fullname.Name, $"{model.Fullname} dengan tanggal lahir yang sama sudah terdaftar");
                else
                {
                    model.Password = HashPassword(SettingsController.get().ResetPassword);
                    model.Branches_Id = Helper.getActiveBranchId(Session);
                    model.Username = generateUsername(model.Fullname, model.Birthday);

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

        public JsonResult ResetPassword(Guid UserAccounts_Id)
        {
            UserAccountsModel model = new UserAccountsModel();
            model.Id = UserAccounts_Id;
            model.Password = HashPassword(SettingsController.get().ResetPassword);
            model.ResetPassword = true;
            updatePassword(model, "Password reset by admin");

            return Json(new { Error = "" });
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
        public ActionResult Login(UserAccountsModel model, string returnUrl)
        {
            //bypass login
            if (Server.MachineName == Helper.DEVCOMPUTERNAME)
            {
                if(string.IsNullOrEmpty(model.Username))
                    model.Username = "ricky";
                if (string.IsNullOrEmpty(model.Password))
                    model.Password = "qwerty";
            }
            
            string hashedPassword = HashPassword(model.Password);
            UserAccountsModel userAccount = get(model.Username, hashedPassword);

            if (userAccount == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                ViewBag.Username = model.Username;
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
            else
            {
                if (userAccount.ResetPassword)
                {
                    TempData["UserAccountsModel"] = userAccount;
                    return RedirectToAction(nameof(UserAccountsController.ChangePassword), CONTROLLERNAME, new { returnUrl = returnUrl });
                }
                else
                {
                    setLoginSession(Session, userAccount);
                    return RedirectToLocal(returnUrl);
                }
            }
        }

        /* CHANGE PASSWORD PAGE *******************************************************************************************************************************/

        public ActionResult ChangePassword(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            UserAccountsModel model = (UserAccountsModel)TempData["UserAccountsModel"];
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
                setLoginSession(Session, model);

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

            List<UserAccountsModel> models = getBirthdays(Helper.getActiveBranchId(Session), null, DateTime.Now.Month);

            ViewBag.BirthdayCount = models.Count;
            UserAccountRolesController.setDropDownListViewBag(this);

            return PartialView("BirthdaysPartial");
        }

        public JsonResult GetBirthdayData(int? month, Guid? UserAccountRoles_Id)
        {
            List<UserAccountsModel> models = getBirthdays(Helper.getActiveBranchId(Session), UserAccountRoles_Id, (int)month);
            
            return Json(new { result = models}, JsonRequestBehavior.AllowGet);
        }

        /* METHODS ********************************************************************************************************************************************/

        public ActionResult UpdateActiveBranch(Guid BranchId, string returnUrl)
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

        public static UserAccountsModel getUserAccount(HttpSessionStateBase Session)
        {
            if (Session[SESSION_UserAccount] == null)
                return null;
            else
                return (UserAccountsModel)Session[SESSION_UserAccount];
        }

        public static UserAccountRolesModel getUserAccess(HttpSessionStateBase Session)
        {
            return (UserAccountRolesModel)Session[SESSION_UserAccountAccess];
        }

        public static bool isLoggedIn(HttpSessionStateBase Session)
        {
            return getUserAccount(Session) != null;
        }

        public static bool isChangePassword(HttpSessionStateBase Session)
        {
            return getUserAccount(Session).ResetPassword;
        }

        public static void setLoginSession(HttpSessionStateBase Session, UserAccountsModel model)
        {
            if (model != null)
            {
                Session[SESSION_UserAccount] = model;
                Session[SESSION_ActiveBranches_Id] = model.Branches_Id;
                Session[SESSION_UserAccountAccess] = UserAccountRolesController.getAccesses(model.Id);
            }
        }

        public static void updateLoginSession(HttpSessionStateBase Session)
        {
            setLoginSession(Session, (UserAccountsModel)Session[SESSION_UserAccount]);
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

            List<string> name = Fullname.Split().ToList();
            if (name.Count == 1)
                name.Add(name[0]); //name must consist of 2 words

            Username = generateUsername(name, Birthday, charCount);

            //verify username doesn't exist
            while (isExists(null, Username) && charCount <= name[0].Length)
                Username = generateUsername(name, Birthday, charCount++);

            return Username;
        }

        public string generateUsername(List<string> nameArray, DateTime Birthday, int charCount)
        {
            return string.Format("{0}{1}{2:##00}{3:##00}",
                nameArray[0].Substring(0, charCount),
                nameArray[nameArray.Count - 1].Substring(0, charCount),
                Birthday.Day,
                Birthday.Month);
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

        public UserAccountsModel get(string Username, string Password) { return get(null, null, Username, Password, null, null, null, null, null).FirstOrDefault(); }
        public List<UserAccountsModel> getBirthdays(Guid Branches_Id, Guid? UserAccountRoles_Id, int BirthdayListMonth) { return get(Branches_Id, null, null, null, 1, UserAccountRoles_Id, BirthdayListMonth, null, null); }
        public List<UserAccountsModel> get(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Languages_Id) { return get(null, null, null, null, FILTER_Active, null, null, FILTER_Keyword, FILTER_Languages_Id); }
        public UserAccountsModel get(Guid Id) { return get(null, Id, null, null, null, null, null, null, null).FirstOrDefault(); }
        public List<UserAccountsModel> get(Guid? Branches_Id, Guid? Id, string Username, string Password, int? Active, Guid? UserAccountRoles_Id, int? BirthdayListMonth, string FILTER_Keyword, Guid? Language_Id)
        {
            if (Branches_Id == null && Helper.isActiveBranchAvailable(Session))
                Helper.getActiveBranchId(Session);

            List<UserAccountsModel> models = db.Database.SqlQuery<UserAccountsModel>(@"
                        SELECT UserAccounts.*
                        FROM UserAccounts
                        WHERE 1=1
							AND (@Id IS NULL OR UserAccounts.Id = @Id)
							AND (@Username IS NULL OR UserAccounts.Username = @Username)
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
							AND (@FILTER_Keyword IS NULL OR (UserAccounts.Fullname LIKE '%'+@FILTER_Keyword+'%' OR UserAccounts.Username LIKE '%'+@FILTER_Keyword+'%'))
						ORDER BY UserAccounts.Fullname ASC
                    ",
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Username.Name, Username),
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Password.Name, Password),
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Active.Name, Active),
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Branches_Id.Name, Branches_Id),
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Roles.Name, Branches_Id),
                    DBConnection.getSqlParameter("BirthdayListMonth", BirthdayListMonth),
                    DBConnection.getSqlParameter("UserAccountRoles_Id", UserAccountRoles_Id),
                    DBConnection.getSqlParameter("Languages_Id", Language_Id),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();

            foreach (UserAccountsModel model in models)
            {
                if(!string.IsNullOrEmpty(model.Roles)) model.Roles_List = model.Roles.Split(',').ToList();
                if (!string.IsNullOrEmpty(model.Interest)) model.Interest_List = model.Interest.Split(',').ToList();
            }

            return models;
        }

        public void updatePassword(UserAccountsModel model, string log)
        {
            db.Database.ExecuteSqlCommand(@"
                UPDATE UserAccounts 
                SET
                    Password = @Password,
                    ResetPassword = @ResetPassword
                WHERE UserAccounts.Id = @Id;                
            ",
                DBConnection.getSqlParameter(UserAccountsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Password.Name, model.Password),
                DBConnection.getSqlParameter(UserAccountsModel.COL_ResetPassword.Name, model.ResetPassword)
            );

            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        public void update(UserAccountsModel model, string log)
        {
            if(model.Roles_List != null) model.Roles = string.Join(",", model.Roles_List.ToArray());
            if(model.Interest_List != null) model.Interest = string.Join(",", model.Interest_List.ToArray());

            db.Database.ExecuteSqlCommand(@"
                UPDATE UserAccounts 
                SET
                    Username = @Username,
                    Password = @Password,
                    Fullname = @Fullname,
                    Birthday = @Birthday,
                    Branches_Id = @Branches_Id,
                    Active = @Active,
                    ResetPassword = @ResetPassword,
                    Email = @Email,
                    Address = @Address,
                    Phone1 = @Phone1,
                    Phone2 = @Phone2,
                    Notes = @Notes,
                    Interest = @Interest,
                    PromotionEvents_Id = @PromotionEvents_Id,
                    Roles = @Roles
                WHERE UserAccounts.Id = @Id;                
            ",
                DBConnection.getSqlParameter(UserAccountsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Username.Name, model.Username),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Password.Name, model.Password),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Fullname.Name, model.Fullname),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Birthday.Name, model.Birthday),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Branches_Id.Name, model.Branches_Id),
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

            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        public void add(UserAccountsModel model)
        {
            db.Database.ExecuteSqlCommand(@"
                INSERT INTO UserAccounts (Id,Fullname,Username,Password,Birthday,Branches_Id,ResetPassword,Active) 
                    VALUES(@Id,@Fullname,@Username,@Password,@Birthday,@Branches_Id,@ResetPassword,@Active);
            ",
                DBConnection.getSqlParameter(UserAccountsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Username.Name, model.Username),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Password.Name, model.Password),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Fullname.Name, model.Fullname),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Birthday.Name, model.Birthday),
                DBConnection.getSqlParameter(UserAccountsModel.COL_ResetPassword.Name, model.ResetPassword),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(UserAccountsModel.COL_Branches_Id.Name, model.Branches_Id)
            );

            ActivityLogsController.AddCreateLog(db, Session, model.Id);
        }

        /******************************************************************************************************************************************************/
    }
}