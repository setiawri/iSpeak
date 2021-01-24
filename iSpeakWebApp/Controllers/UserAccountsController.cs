﻿using iSpeakWebApp.Models;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using LIBUtil;

using System.Web.Mvc;

namespace iSpeakWebApp.Controllers
{
    public class AccessList
    {
        //public OperatorPrivilegePayrollModel OperatorPrivilegePayrollModel;

        public AccessList() { }

        //public void populate(OperatorPrivilegePayrollModel operatorPrivilegePayrollModel)
        //{
        //    OperatorPrivilegePayrollModel = operatorPrivilegePayrollModel;
        //}
    }

    public class UserAccountsController : Controller
    {
        public const string ACTIONNAME_Login = "Login";
        public const string CONTROLLERNAME = "UserAccounts";

        public const string SESSION_UserAccounts_Id = "UserAccounts_Id";
        public const string SESSION_UserAccounts_Username = "UserAccounts_Username";
        public const string SESSION_UserAccounts_ResetPassword = "UserAccounts_ResetPassword";
        public const string SESSION_ActiveBranches_Id = "Branches_Id";

        public const string SESSION_OperatorPrivilegePayrollModel_PayrollApproval = "OperatorPrivilegePayrollModel_PayrollApproval";
        public const string SESSION_OperatorPrivilegePayrollModel_ReimbursementApproval = "OperatorPrivilegePayrollModel_ReimbursementApproval";

        private readonly DBContext db = new DBContext();

        /* LOGIN PAGE *****************************************************************************************************************************************/

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
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

            UserAccountsModel userAccount = db.UserAccounts.Where(x => x.Username.ToLower() == model.Username.ToLower() && x.Password == hashedPassword).FirstOrDefault();

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
                    model = db.UserAccounts.Where(x => x.Id == (Guid)Id).FirstOrDefault();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(Guid Id, string CurrentPassword, string NewPassword, string ConfirmPassword, string returnUrl)
        {
            UserAccountsModel model = db.UserAccounts.Where(x => x.Id == Id).FirstOrDefault();
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
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                setLoginSession(Session, model);
                return RedirectToLocal(returnUrl);
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.NewPassword = NewPassword;
            ViewBag.ConfirmPassword = ConfirmPassword;
            return View(model);
        }

        /* METHODS ********************************************************************************************************************************************/

        public ActionResult UpdateActiveBranch(Guid BranchId, string returnUrl)
        {
            Session[SESSION_ActiveBranches_Id] = BranchId;

            return RedirectToLocal(returnUrl);
        }

        public ActionResult LogOff()
        {
            Session[SESSION_UserAccounts_Id] = null;
            return RedirectToAction(nameof(Login));
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
            return Session[SESSION_UserAccounts_Id];
        }

        public static bool isLoggedIn(HttpSessionStateBase Session)
        {
            return Session[SESSION_UserAccounts_Id] != null;
        }

        public static bool isChangePassword(HttpSessionStateBase Session)
        {
            return (bool)Session[SESSION_UserAccounts_ResetPassword];
        }

        private static void setLoginSession(HttpSessionStateBase Session, UserAccountsModel model)
        {
            if (model != null)
            {
                Session[SESSION_UserAccounts_Id] = model.Id;
                Session[SESSION_UserAccounts_Username] = model.Username;
                Session[SESSION_UserAccounts_ResetPassword] = model.ResetPassword;
                Session[SESSION_ActiveBranches_Id] = model.Branches_Id;

                AccessList accessList = new AccessList();
                //accessList.populate(result.OperatorPrivilegePayrollModel);

                //Session[SESSION_OperatorPrivilegePayrollModel_PayrollApproval] = accessList.OperatorPrivilegePayrollModel.Approval;
                //Session[SESSION_OperatorPrivilegePayrollModel_ReimbursementApproval] = accessList.OperatorPrivilegePayrollModel.ReimbursementApproval;
            }
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public List<UserAccountsModel> getBirthdays(Guid Branches_Id, Guid? UserAccountRoles_Id, int? BirthdayListMonth) { return get(Branches_Id, null, true, UserAccountRoles_Id, BirthdayListMonth); }
        public UserAccountsModel get(Guid Id) { return get(null, Id, false, null, null).FirstOrDefault(); }
        public List<UserAccountsModel> get(Guid? Branches_Id, Guid? Id, bool? isActiveOnly, Guid? UserAccountRoles_Id, int? BirthdayListMonth)
        {
            if (Branches_Id == null)
                Helper.getActiveBranchId(Session);

            List<UserAccountsModel> models = db.Database.SqlQuery<UserAccountsModel>(@"
                        SELECT UserAccounts.*
                        FROM UserAccounts
                        WHERE 1=1
							AND (@Id IS NULL OR UserAccounts.Id = @Id)
							AND (@Branches_Id IS NULL OR UserAccounts.Branches_Id = @Branches_Id)
							AND (@isActiveOnly = 0 OR UserAccounts.Active = 1)
							AND (@UserAccountRoles_Id IS NULL OR UserAccounts.Id IN (
								SELECT UserAccounts_Id 
								FROM UserAccountRoleAssignments 
								WHERE UserAccountRoles_Id = @UserAccountRoles_Id
							))
							AND (@BirthdayListMonth IS NULL OR (
									MONTH(UserAccounts.Birthday) = @BirthdayListMonth
									AND (MONTH(GETDATE()) <> @BirthdayListMonth OR DAY(UserAccounts.Birthday) >= DAY(GETDATE()))
								)
							)
						ORDER BY UserAccounts.Fullname ASC
                    ",
                    DBConnection.getSqlParameter(UserAccountsModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter("UserAccountRoles_Id", UserAccountRoles_Id),
                    DBConnection.getSqlParameter("Branches_Id", Branches_Id),
                    DBConnection.getSqlParameter("isActiveOnly", isActiveOnly),
                    DBConnection.getSqlParameter("BirthdayListMonth", BirthdayListMonth)
                ).ToList();

            return models;
        }

        /******************************************************************************************************************************************************/
    }
}