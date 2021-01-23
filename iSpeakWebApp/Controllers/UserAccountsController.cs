using iSpeakWebApp.Models;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System;
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
                    return RedirectToAction(nameof(UserAccountsController.ResetPassword), CONTROLLERNAME, new { returnUrl = returnUrl });
                }
                else
                {
                    setLoginSession(Session, userAccount);
                    return RedirectToLocal(returnUrl);
                }
            }
        }


        /* RESET PASSWORD PAGE ********************************************************************************************************************************/

        public ActionResult ResetPassword(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            UserAccountsModel model = (UserAccountsModel)TempData["UserAccountsModel"];
            return View(model);
        }

        [HttpPost]
        public ActionResult ResetPassword(Guid Id, string NewPassword, string ConfirmPassword, string returnUrl)
        {
            UserAccountsModel model = db.UserAccounts.Where(x => x.Id == Id).FirstOrDefault();

            string SanitizedNewPassword = Util.sanitizeString(NewPassword);
            if (string.IsNullOrEmpty(SanitizedNewPassword) || SanitizedNewPassword.Length < 6)
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

        public static int getUserId(HttpSessionStateBase Session)
        {
            return int.Parse(Session[SESSION_UserAccounts_Id].ToString());
        }

        public static bool isLoggedIn(HttpSessionStateBase Session)
        {
            return Session[SESSION_UserAccounts_Id] != null;
        }

        public static bool isResetPassword(HttpSessionStateBase Session)
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

                AccessList accessList = new AccessList();
                //accessList.populate(result.OperatorPrivilegePayrollModel);

                //Session[SESSION_OperatorPrivilegePayrollModel_PayrollApproval] = accessList.OperatorPrivilegePayrollModel.Approval;
                //Session[SESSION_OperatorPrivilegePayrollModel_ReimbursementApproval] = accessList.OperatorPrivilegePayrollModel.ReimbursementApproval;
            }
        }

        /******************************************************************************************************************************************************/
    }
}