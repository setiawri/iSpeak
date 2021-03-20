using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class SettingsController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* EDIT ***********************************************************************************************************************************************/

        // GET: Settings/Edit
        public ActionResult Edit()
        {
            SettingsModel model = get();
            setEditViewBags(model);
            return View(model);
        }

        // POST: Settings/Edit/{modifiedModel,uploadFiles}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SettingsModel modifiedModel, List<HttpPostedFileBase> uploadFiles)
        {
            if (ModelState.IsValid)
            {
                //Prepare changes
                SettingsModel originalModel = get();
                string log = string.Empty;

                log = addLog(log, SettingsModel.COL_AutoEntryForCashPayments.Id,
                    db.PettyCashRecordsCategories.Where(x=> x.Id == originalModel.AutoEntryForCashPayments).FirstOrDefault().Name,
                    db.PettyCashRecordsCategories.Where(x => x.Id == modifiedModel.AutoEntryForCashPayments).FirstOrDefault().Name, 
                    "Update: '{1}'");
                log = addLog(log, SettingsModel.COL_AutoEntryForCashPayments.Id, originalModel.AutoEntryForCashPayments_Notes, modifiedModel.AutoEntryForCashPayments_Notes, "Notes: '{1}'");

                log = addLog(log, SettingsModel.COL_UserSetRoleAllowed.Id,
                    db.UserAccountRoles.Where(x => x.Id == originalModel.UserSetRoleAllowed).FirstOrDefault().Name,
                    db.UserAccountRoles.Where(x => x.Id == modifiedModel.UserSetRoleAllowed).FirstOrDefault().Name,
                    "Update: '{1}'");
                log = addLog(log, SettingsModel.COL_UserSetRoleAllowed.Id, originalModel.UserSetRoleAllowed_Notes, modifiedModel.UserSetRoleAllowed_Notes, "Notes: '{1}'");

                log = addLog(log, SettingsModel.COL_ResetPassword.Id, originalModel.ResetPassword, modifiedModel.ResetPassword, "UPDATE: '{1}'");
                log = addLog(log, SettingsModel.COL_ResetPassword.Id, originalModel.ResetPassword_Notes, modifiedModel.ResetPassword_Notes, "Notes: '{1}'");

                log = addLogForList<UserAccountRolesModel>(log, SettingsModel.COL_RolesToSeeReminders.Id, originalModel.RolesToSeeReminders_List, modifiedModel.RolesToSeeReminders_List);
                log = addLog(log, SettingsModel.COL_RolesToSeeReminders.Id, originalModel.RolesToSeeReminders_Notes, modifiedModel.RolesToSeeReminders_Notes, "Notes: '{1}'");

                log = addLogForList<UserAccountRolesModel>(log, SettingsModel.COL_FullAccessForTutorSchedules.Id, originalModel.FullAccessForTutorSchedules_List, modifiedModel.FullAccessForTutorSchedules_List);
                log = addLog(log, SettingsModel.COL_FullAccessForTutorSchedules.Id, originalModel.FullAccessForTutorSchedules_Notes, modifiedModel.FullAccessForTutorSchedules_Notes, "Notes: '{1}'");

                log = addLogForList<UserAccountRolesModel>(log, SettingsModel.COL_ShowOnlyOwnUserData.Id, originalModel.ShowOnlyOwnUserData_List, modifiedModel.ShowOnlyOwnUserData_List);
                log = addLog(log, SettingsModel.COL_ShowOnlyOwnUserData.Id, originalModel.ShowOnlyOwnUserData_Notes, modifiedModel.ShowOnlyOwnUserData_Notes, "Notes: '{1}'");

                log = addLogForList<UserAccountRolesModel>(log, SettingsModel.COL_PayrollRatesRoles.Id, originalModel.PayrollRatesRoles_List, modifiedModel.PayrollRatesRoles_List);
                log = addLog(log, SettingsModel.COL_PayrollRatesRoles.Id, originalModel.PayrollRatesRoles_Notes, modifiedModel.PayrollRatesRoles_Notes, "Notes: '{1}'");

                //Update Database
                if (string.IsNullOrEmpty(log))
                    Util.setBootboxMessage(this, "No change to update");
                else
                {
                    update(modifiedModel); //update setting values
                    db.SaveChanges(); //insert activity logs
                    Util.setBootboxMessage(this, log);
                    return RedirectToAction(nameof(Edit));
                }
            }

            setEditViewBags(modifiedModel);
            return View(modifiedModel);
        }

        private void setEditViewBags(SettingsModel model)
        {
            ViewBag.ResetPassword = model.ResetPassword;
            ViewBag.PettyCashRecordsCategories = new SelectList(db.PettyCashRecordsCategories.OrderBy(x => x.Name).ToList(), PettyCashRecordsCategoriesModel.COL_Id.Name, UserAccountRolesModel.COL_Name.Name);
            UserAccountRolesController.setDropDownListViewBag(this);
        }

        /* METHODS ********************************************************************************************************************************************/
        #region METHODS

        public static SettingsModel get()
        {
            List<SettingsModel> models = new DBContext().Database.SqlQuery<SettingsModel>(@"
                SELECT
                    ISNULL(Settings_AutoEntryForCashPayments.Value_Guid,'') AS AutoEntryForCashPayments,
                    ISNULL(Settings_AutoEntryForCashPayments.Notes,'') AS AutoEntryForCashPayments_Notes,
                    ISNULL(Settings_UserSetRoleAllowed.Value_Guid,'') AS UserSetRoleAllowed,
                    ISNULL(Settings_UserSetRoleAllowed.Notes,'') AS UserSetRoleAllowed_Notes,
                    ISNULL(Settings_ResetPassword.Value_String,'') AS ResetPassword,
                    ISNULL(Settings_ResetPassword.Notes,'') AS ResetPassword_Notes,
                    ISNULL(Settings_RolesToSeeReminders.Value_String,'') AS RolesToSeeReminders,
                    ISNULL(Settings_RolesToSeeReminders.Notes,'') AS RolesToSeeReminders_Notes,
                    ISNULL(Settings_FullAccessForTutorSchedules.Value_String,'') AS FullAccessForTutorSchedules,
                    ISNULL(Settings_FullAccessForTutorSchedules.Notes,'') AS FullAccessForTutorSchedules_Notes,
                    ISNULL(Settings_ShowOnlyOwnUserData.Value_String,'') AS ShowOnlyOwnUserData,
                    ISNULL(Settings_ShowOnlyOwnUserData.Notes,'') AS ShowOnlyOwnUserData_Notes,
                    ISNULL(Settings_PayrollRatesRoles.Value_String,'') AS PayrollRatesRoles,
                    ISNULL(Settings_PayrollRatesRoles.Notes,'') AS PayrollRatesRoles_Notes
                FROM Settings Settings_AutoEntryForCashPayments
                    LEFT JOIN Settings Settings_UserSetRoleAllowed ON Settings_UserSetRoleAllowed.Id = @UserSetRoleAllowedId
                    LEFT JOIN Settings Settings_ResetPassword ON Settings_ResetPassword.Id = @ResetPasswordId
                    LEFT JOIN Settings Settings_RolesToSeeReminders ON Settings_RolesToSeeReminders.Id = @RolesToSeeRemindersId
                    LEFT JOIN Settings Settings_FullAccessForTutorSchedules ON Settings_FullAccessForTutorSchedules.Id = @FullAccessForTutorSchedulesId
                    LEFT JOIN Settings Settings_ShowOnlyOwnUserData ON Settings_ShowOnlyOwnUserData.Id = @ShowOnlyOwnUserDataId
                    LEFT JOIN Settings Settings_PayrollRatesRoles ON Settings_PayrollRatesRoles.Id = @PayrollRatesRolesId
                WHERE Settings_AutoEntryForCashPayments.Id = @AutoEntryForCashPaymentsId
                ",
                    DBConnection.getSqlParameter(SettingsModel.COL_AutoEntryForCashPayments.Name + "Id", SettingsModel.COL_AutoEntryForCashPayments.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_UserSetRoleAllowed.Name + "Id", SettingsModel.COL_UserSetRoleAllowed.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ResetPassword.Name + "Id", SettingsModel.COL_ResetPassword.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_RolesToSeeReminders.Name + "Id", SettingsModel.COL_RolesToSeeReminders.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_FullAccessForTutorSchedules.Name + "Id", SettingsModel.COL_FullAccessForTutorSchedules.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ShowOnlyOwnUserData.Name + "Id", SettingsModel.COL_ShowOnlyOwnUserData.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_PayrollRatesRoles.Name + "Id", SettingsModel.COL_PayrollRatesRoles.Id)
                ).ToList();

            foreach(SettingsModel model in models)
            {
                if(!string.IsNullOrEmpty(model.RolesToSeeReminders)) model.RolesToSeeReminders_List = model.RolesToSeeReminders.Split(',').ToList();
                if (!string.IsNullOrEmpty(model.FullAccessForTutorSchedules)) model.FullAccessForTutorSchedules_List = model.FullAccessForTutorSchedules.Split(',').ToList();
                if (!string.IsNullOrEmpty(model.ShowOnlyOwnUserData)) model.ShowOnlyOwnUserData_List = model.ShowOnlyOwnUserData.Split(',').ToList();
                if (!string.IsNullOrEmpty(model.PayrollRatesRoles)) model.PayrollRatesRoles_List = model.PayrollRatesRoles.Split(',').ToList();
            }

            return models.Count == 0 ? null : models[0];
        }

        private void update(SettingsModel modifiedModel)
        {
            if(modifiedModel.RolesToSeeReminders_List != null) modifiedModel.RolesToSeeReminders = string.Join(",", modifiedModel.RolesToSeeReminders_List.ToArray());
            if (modifiedModel.FullAccessForTutorSchedules_List != null) modifiedModel.FullAccessForTutorSchedules = string.Join(",", modifiedModel.FullAccessForTutorSchedules_List.ToArray());
            if (modifiedModel.ShowOnlyOwnUserData_List != null) modifiedModel.ShowOnlyOwnUserData = string.Join(",", modifiedModel.ShowOnlyOwnUserData_List.ToArray());
            if (modifiedModel.PayrollRatesRoles_List != null) modifiedModel.PayrollRatesRoles = string.Join(",", modifiedModel.PayrollRatesRoles_List.ToArray());

            db.Database.ExecuteSqlCommand(@"
                    UPDATE Settings SET Value_Guid=@AutoEntryForCashPayments, Notes=@AutoEntryForCashPayments_Notes WHERE Id=@AutoEntryForCashPaymentsId;
                    UPDATE Settings SET Value_Guid=@UserSetRoleAllowed, Notes=@UserSetRoleAllowed_Notes WHERE Id=@UserSetRoleAllowedId;
                    UPDATE Settings SET Value_String=@ResetPassword, Notes=@ResetPassword_Notes WHERE Id=@ResetPasswordId;
                    UPDATE Settings SET Value_String=@RolesToSeeReminders, Notes=@RolesToSeeReminders_Notes WHERE Id=@RolesToSeeRemindersId;
                    UPDATE Settings SET Value_String=@FullAccessForTutorSchedules, Notes=@FullAccessForTutorSchedules_Notes WHERE Id=@FullAccessForTutorSchedulesId;
                    UPDATE Settings SET Value_String=@ShowOnlyOwnUserData, Notes=@ShowOnlyOwnUserData_Notes WHERE Id=@ShowOnlyOwnUserDataId;
                    UPDATE Settings SET Value_String=@PayrollRatesRoles, Notes=@PayrollRatesRoles_Notes WHERE Id=@PayrollRatesRolesId;
                ",
                    DBConnection.getSqlParameter(SettingsModel.COL_AutoEntryForCashPayments.Name + "Id", SettingsModel.COL_AutoEntryForCashPayments.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_AutoEntryForCashPayments.Name, Util.wrapNullable(modifiedModel.AutoEntryForCashPayments)),
                    DBConnection.getSqlParameter(SettingsModel.COL_AutoEntryForCashPayments_Notes.Name, Util.wrapNullable(modifiedModel.AutoEntryForCashPayments_Notes)),
                    DBConnection.getSqlParameter(SettingsModel.COL_UserSetRoleAllowed.Name + "Id", SettingsModel.COL_UserSetRoleAllowed.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_UserSetRoleAllowed.Name, Util.wrapNullable(modifiedModel.UserSetRoleAllowed)),
                    DBConnection.getSqlParameter(SettingsModel.COL_UserSetRoleAllowed_Notes.Name, Util.wrapNullable(modifiedModel.UserSetRoleAllowed_Notes)),
                    DBConnection.getSqlParameter(SettingsModel.COL_ResetPassword.Name + "Id", SettingsModel.COL_ResetPassword.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ResetPassword.Name, Util.wrapNullable(modifiedModel.ResetPassword)),
                    DBConnection.getSqlParameter(SettingsModel.COL_ResetPassword_Notes.Name, Util.wrapNullable(modifiedModel.ResetPassword_Notes)),
                    DBConnection.getSqlParameter(SettingsModel.COL_RolesToSeeReminders.Name + "Id", SettingsModel.COL_RolesToSeeReminders.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_RolesToSeeReminders.Name, Util.wrapNullable(modifiedModel.RolesToSeeReminders)),
                    DBConnection.getSqlParameter(SettingsModel.COL_RolesToSeeReminders_Notes.Name, Util.wrapNullable(modifiedModel.RolesToSeeReminders_Notes)),
                    DBConnection.getSqlParameter(SettingsModel.COL_FullAccessForTutorSchedules.Name + "Id", SettingsModel.COL_FullAccessForTutorSchedules.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_FullAccessForTutorSchedules.Name, Util.wrapNullable(modifiedModel.FullAccessForTutorSchedules)),
                    DBConnection.getSqlParameter(SettingsModel.COL_FullAccessForTutorSchedules_Notes.Name, Util.wrapNullable(modifiedModel.FullAccessForTutorSchedules_Notes)),
                    DBConnection.getSqlParameter(SettingsModel.COL_ShowOnlyOwnUserData.Name + "Id", SettingsModel.COL_ShowOnlyOwnUserData.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ShowOnlyOwnUserData.Name, Util.wrapNullable(modifiedModel.ShowOnlyOwnUserData)),
                    DBConnection.getSqlParameter(SettingsModel.COL_ShowOnlyOwnUserData_Notes.Name, Util.wrapNullable(modifiedModel.ShowOnlyOwnUserData_Notes)),
                    DBConnection.getSqlParameter(SettingsModel.COL_PayrollRatesRoles.Name + "Id", SettingsModel.COL_PayrollRatesRoles.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_PayrollRatesRoles.Name, Util.wrapNullable(modifiedModel.PayrollRatesRoles)),
                    DBConnection.getSqlParameter(SettingsModel.COL_PayrollRatesRoles_Notes.Name, Util.wrapNullable(modifiedModel.PayrollRatesRoles_Notes))
            );
        }

        private string addLog(string log, Guid reffId, object oldValue, object newValue, string format)
        {
            string newlog = string.Empty;
            newlog = Util.appendChange(newlog, oldValue, newValue, format);
            if (string.IsNullOrEmpty(newlog))
                return log;
            else
            {
                ActivityLogsController.Add(db, Session, reffId, newlog);
                return Util.append(log, string.Format("UPDATE: {0} to {1}", oldValue, newValue), Environment.NewLine + Environment.NewLine);
            }
        }

        private string addLogForList<T>(string log, Guid reffId, List<string> oldValue, List<string> newValue)
        {
            string addedlog = string.Empty;

            if (newValue != null)
            {
                foreach (string value in newValue)
                {
                    if (oldValue != null && oldValue.Contains(value))
                        oldValue.Remove(value);
                    else
                        addedlog = append<T>(addedlog, value, ",");
                }
            }
            if (!string.IsNullOrEmpty(addedlog)) addedlog = Environment.NewLine + "Added: " + addedlog;

            string removedlog = string.Empty;
            if (oldValue != null)
            {
                foreach (string value in oldValue)
                    removedlog = append<T>(removedlog, value, ",");
            }
            if (!string.IsNullOrEmpty(removedlog)) removedlog = Environment.NewLine + "Removed: " + removedlog;

            if (!string.IsNullOrEmpty(removedlog) || !string.IsNullOrEmpty(addedlog))
            {
                string newlog = "UPDATE: " + removedlog + addedlog;
                ActivityLogsController.Add(db, Session, reffId, newlog);
                return Util.append(log, newlog, Environment.NewLine + Environment.NewLine);
            }
            else
                return log;
        }

        private string append<T>(string log, string value, string delimiter)
        {
            if (typeof(T) == typeof(UserAccountRolesModel))
                value = db.UserAccountRoles.Where(x => x.Id.ToString() == value).FirstOrDefault().Name;

            return Util.append(log, value, ",");
        }

        #endregion METHODS
        /******************************************************************************************************************************************************/
    }
}