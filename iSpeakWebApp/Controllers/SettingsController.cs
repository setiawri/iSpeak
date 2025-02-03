using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;
using LIBWebMVC;

namespace iSpeakWebApp.Controllers
{
    /*
     * Settings is GLOBAL and is NOT filtered by Franchise. Changes must be made by superuser.
     */

    public class SettingsController : Controller
    {
        private readonly DBContext db = new DBContext();

        private void setViewBag(SettingsModel model)
        {
            ViewBag.ResetPassword = model.ResetPassword;
            ViewBag.PettyCashRecordsCategories = new SelectList(db.PettyCashRecordsCategories.OrderBy(x => x.Name).ToList(), PettyCashRecordsCategoriesModel.COL_Id.Name, UserAccountRolesModel.COL_Name.Name);
            UserAccountRolesController.setDropDownListViewBag(this);
            LessonPackagesController.setDropDownListViewBag(this);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: Settings/Edit
        public ActionResult Edit()
        {
            if (!UserAccountsController.getUserAccess(Session).Settings_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            SettingsModel model = get();
            setViewBag(model);
            return View(model);
        }

        // POST: Settings/Edit/{modifiedModel,uploadFiles}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SettingsModel modifiedModel, List<HttpPostedFileBase> uploadFiles)
        {
            if (!UserAccountsController.getUserAccess(Session).Settings_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (ModelState.IsValid)
            {
                //Prepare changes
                SettingsModel originalModel = get();
                string log = string.Empty;

                log = addLog(log, SettingsModel.COL_StudentRole.Id, SettingsModel.COL_StudentRole.Display,
                    originalModel.StudentRole == null ? "" : db.UserAccountRoles.Where(x => x.Id == originalModel.StudentRole).FirstOrDefault().Name,
                    modifiedModel.StudentRole == null ? "" : db.UserAccountRoles.Where(x => x.Id == modifiedModel.StudentRole).FirstOrDefault().Name,
                    "Update: '{1}'");
                log = addLog(log, SettingsModel.COL_StudentRole.Id, SettingsModel.COL_StudentRole.Display, originalModel.StudentRole_Notes, modifiedModel.StudentRole_Notes, "Notes: '{1}'");

                log = addLog(log, SettingsModel.COL_TutorRole.Id, SettingsModel.COL_TutorRole.Display,
                    originalModel.TutorRole == null ? "" : db.UserAccountRoles.Where(x => x.Id == originalModel.TutorRole).FirstOrDefault().Name,
                    modifiedModel.TutorRole == null ? "" : db.UserAccountRoles.Where(x => x.Id == modifiedModel.TutorRole).FirstOrDefault().Name,
                    "Update: '{1}'");
                log = addLog(log, SettingsModel.COL_TutorRole.Id, SettingsModel.COL_TutorRole.Display, originalModel.TutorRole_Notes, modifiedModel.TutorRole_Notes, "Notes: '{1}'");

                log = addLog(log, SettingsModel.COL_ResetPassword.Id, SettingsModel.COL_ResetPassword.Display, originalModel.ResetPassword, modifiedModel.ResetPassword, "UPDATE: '{1}'");
                log = addLog(log, SettingsModel.COL_ResetPassword.Id, SettingsModel.COL_ResetPassword.Display, originalModel.ResetPassword_Notes, modifiedModel.ResetPassword_Notes, "Notes: '{1}'");

                log = addLogForList<UserAccountRolesModel>(log, SettingsModel.COL_FullAccessForTutorSchedules.Id, SettingsModel.COL_FullAccessForTutorSchedules.Display, originalModel.FullAccessForTutorSchedules_List, modifiedModel.FullAccessForTutorSchedules_List);
                log = addLog(log, SettingsModel.COL_FullAccessForTutorSchedules.Id, SettingsModel.COL_FullAccessForTutorSchedules.Display, originalModel.FullAccessForTutorSchedules_Notes, modifiedModel.FullAccessForTutorSchedules_Notes, "Notes: '{1}'");

                log = addLogForList<UserAccountRolesModel>(log, SettingsModel.COL_ShowOnlyOwnUserData.Id, SettingsModel.COL_ShowOnlyOwnUserData.Display, originalModel.ShowOnlyOwnUserData_List, modifiedModel.ShowOnlyOwnUserData_List);
                log = addLog(log, SettingsModel.COL_ShowOnlyOwnUserData.Id, SettingsModel.COL_ShowOnlyOwnUserData.Display, originalModel.ShowOnlyOwnUserData_Notes, modifiedModel.ShowOnlyOwnUserData_Notes, "Notes: '{1}'");

                log = addLogForList<UserAccountRolesModel>(log, SettingsModel.COL_PayrollRatesRoles.Id, SettingsModel.COL_PayrollRatesRoles.Display, originalModel.PayrollRatesRoles_List, modifiedModel.PayrollRatesRoles_List);
                log = addLog(log, SettingsModel.COL_PayrollRatesRoles.Id, SettingsModel.COL_PayrollRatesRoles.Display, originalModel.PayrollRatesRoles_Notes, modifiedModel.PayrollRatesRoles_Notes, "Notes: '{1}'");

                //Update Database
                if (string.IsNullOrEmpty(log))
                    UtilWebMVC.setBootboxMessage(this, "No change to update");
                else
                {
                    update(modifiedModel); //update setting values
                    UtilWebMVC.setBootboxMessage(this, log);
                    return RedirectToAction(nameof(Edit));
                }
            }

            setViewBag(modifiedModel);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/
        #region METHODS

        public JsonResult Ajax_Update_ClubClassroomLink(string value)
        {
            db.Database.ExecuteSqlCommand(@"
                    IF (SELECT COUNT(Id) FROM Settings WHERE Id = @ClubClassroomLinkId) = 0 INSERT INTO Settings (Id) VALUES(@ClubClassroomLinkId);
                    UPDATE Settings SET Value_String=@ClubClassroomLink WHERE Id=@ClubClassroomLinkId;
                ",
                    DBConnection.getSqlParameter(SettingsModel.COL_ClubClassroomLink.Name + "Id", SettingsModel.COL_ClubClassroomLink.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ClubClassroomLink.Name, Util.wrapNullable(value))
            );
            ActivityLogsController.Add(db, Session, SettingsModel.COL_ClubClassroomLink.Id, $"Updated to {value}");

            return Json(new { Message = "" });
        }

        private string addLog(string log, Guid ReferenceId, string Name, object oldValue, object newValue, string format)
        {
            string newlog = string.Empty;
            newlog = Util.appendChange(newlog, oldValue, newValue, format);
            if (string.IsNullOrEmpty(newlog))
                return log;
            else
            {
                ActivityLogsController.Add(db, Session, ReferenceId, newlog);
                return Util.append(log, string.Format(Name + " UPDATE: {0} to {1}", oldValue, newValue), Environment.NewLine + Environment.NewLine);
            }
        }

        private string addLogForList<T>(string log, Guid ReferenceId, string Name, List<string> oldValue, List<string> newValue)
        {
            if (newValue != null)
                newValue = newValue.ConvertAll(d => d.ToUpper());

            if (oldValue != null)
                oldValue = oldValue.ConvertAll(d => d.ToUpper());

            string addedlog = string.Empty;
            if (newValue != null)
            {
                foreach (string value in newValue)
                {
                    if (oldValue != null && oldValue.Contains(value))
                        oldValue.Remove(value);
                    else
                        addedlog = append<T>(addedlog, value, ", ");
                }
            }
            if (!string.IsNullOrEmpty(addedlog)) addedlog = Environment.NewLine + "Added: " + addedlog;

            string removedlog = string.Empty;
            if (oldValue != null)
            {
                foreach (string value in oldValue)
                    removedlog = append<T>(removedlog, value, ", ");
            }
            if (!string.IsNullOrEmpty(removedlog)) removedlog = Environment.NewLine + "Removed: " + removedlog;

            if (!string.IsNullOrEmpty(removedlog) || !string.IsNullOrEmpty(addedlog))
            {
                string newlog = Name + " UPDATE: " + removedlog + addedlog;
                ActivityLogsController.Add(db, Session, ReferenceId, newlog);
                return Util.append(log, newlog, Environment.NewLine + Environment.NewLine);
            }
            else
                return log;
        }

        private string append<T>(string log, string value, string delimiter)
        {
            if (typeof(T) == typeof(UserAccountRolesModel))
                value = db.UserAccountRoles.Where(x => x.Id.ToString() == value).FirstOrDefault().Name;
            else if (typeof(T) == typeof(LessonPackagesModel))
                value = LessonPackagesController.get(new Guid(value)).Name;

            return Util.append(log, value, ", ");
        }

        public static bool ShowOnlyOwnUserData(HttpSessionStateBase Session) { return ShowOnlyOwnUserData(UserAccountsController.getUserAccount(Session).Roles_List); }
        public static bool ShowOnlyOwnUserData(List<string> UserRoles_List)
        {
            List<string> ShowOnlyOwnUserData_List = get().ShowOnlyOwnUserData_List;
            foreach (string Role in UserRoles_List)
                if (!ShowOnlyOwnUserData_List.Contains(Role))
                    return false;
            return true;
        }

        #endregion METHODS
        /* DATABASE METHODS ***********************************************************************************************************************************/
        #region DATABASE METHODS

        public static SettingsModel get()
        {
            List<SettingsModel> models = new DBContext().Database.SqlQuery<SettingsModel>(@"
                SELECT
                    ISNULL(Settings_StudentRole.Value_Guid,NULL) AS StudentRole,
                    ISNULL(Settings_StudentRole.Notes,'') AS StudentRole_Notes,
                    ISNULL(Settings_TutorRole.Value_Guid,NULL) AS TutorRole,
                    ISNULL(Settings_TutorRole.Notes,'') AS TutorRole_Notes,
                    ISNULL(Settings_ResetPassword.Value_String,'') AS ResetPassword,
                    ISNULL(Settings_ResetPassword.Notes,'') AS ResetPassword_Notes,
                    ISNULL(Settings_FullAccessForTutorSchedules.Value_String,'') AS FullAccessForTutorSchedules,
                    ISNULL(Settings_FullAccessForTutorSchedules.Notes,'') AS FullAccessForTutorSchedules_Notes,
                    ISNULL(Settings_ShowOnlyOwnUserData.Value_String,'') AS ShowOnlyOwnUserData,
                    ISNULL(Settings_ShowOnlyOwnUserData.Notes,'') AS ShowOnlyOwnUserData_Notes,
                    ISNULL(Settings_PayrollRatesRoles.Value_String,'') AS PayrollRatesRoles,
                    ISNULL(Settings_PayrollRatesRoles.Notes,'') AS PayrollRatesRoles_Notes,
                    ISNULL(Settings_ClubClassroomLink.Value_String,'') AS ClubClassroomLink,
                    ISNULL(Settings_AdvertisementBanner1.Value_String,'') AS AdvertisementBanner1,
                    ISNULL(Settings_ClubScheduleImage1.Value_String,'') AS ClubScheduleImage1,
                    ISNULL(Settings_ClubScheduleImage2.Value_String,'') AS ClubScheduleImage2,
                    ISNULL(Settings_ClubScheduleImage3.Value_String,'') AS ClubScheduleImage3
                FROM Settings Settings_StudentRole
                    LEFT JOIN Settings Settings_TutorRole ON Settings_TutorRole.Id = @TutorRoleId
                    LEFT JOIN Settings Settings_ResetPassword ON Settings_ResetPassword.Id = @ResetPasswordId
                    LEFT JOIN Settings Settings_FullAccessForTutorSchedules ON Settings_FullAccessForTutorSchedules.Id = @FullAccessForTutorSchedulesId
                    LEFT JOIN Settings Settings_ShowOnlyOwnUserData ON Settings_ShowOnlyOwnUserData.Id = @ShowOnlyOwnUserDataId
                    LEFT JOIN Settings Settings_PayrollRatesRoles ON Settings_PayrollRatesRoles.Id = @PayrollRatesRolesId
                    LEFT JOIN Settings Settings_ClubClassroomLink ON Settings_ClubClassroomLink.Id = @ClubClassroomLinkId
                    LEFT JOIN Settings Settings_AdvertisementBanner1 ON Settings_AdvertisementBanner1.Id = @AdvertisementBanner1Id
                    LEFT JOIN Settings Settings_ClubScheduleImage1 ON Settings_ClubScheduleImage1.Id = @ClubScheduleImage1Id
                    LEFT JOIN Settings Settings_ClubScheduleImage2 ON Settings_ClubScheduleImage2.Id = @ClubScheduleImage2Id
                    LEFT JOIN Settings Settings_ClubScheduleImage3 ON Settings_ClubScheduleImage3.Id = @ClubScheduleImage3Id
                WHERE Settings_StudentRole.Id = @StudentRoleId
                ",
                    DBConnection.getSqlParameter(SettingsModel.COL_StudentRole.Name + "Id", SettingsModel.COL_StudentRole.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_TutorRole.Name + "Id", SettingsModel.COL_TutorRole.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ResetPassword.Name + "Id", SettingsModel.COL_ResetPassword.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_FullAccessForTutorSchedules.Name + "Id", SettingsModel.COL_FullAccessForTutorSchedules.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ShowOnlyOwnUserData.Name + "Id", SettingsModel.COL_ShowOnlyOwnUserData.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_PayrollRatesRoles.Name + "Id", SettingsModel.COL_PayrollRatesRoles.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ClubClassroomLink.Name + "Id", SettingsModel.COL_ClubClassroomLink.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_AdvertisementBanner1.Name + "Id", SettingsModel.COL_AdvertisementBanner1.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ClubScheduleImage1.Name + "Id", SettingsModel.COL_ClubScheduleImage1.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ClubScheduleImage2.Name + "Id", SettingsModel.COL_ClubScheduleImage2.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ClubScheduleImage3.Name + "Id", SettingsModel.COL_ClubScheduleImage3.Id)
                ).ToList();

            foreach(SettingsModel model in models)
            {
                if (!string.IsNullOrEmpty(model.FullAccessForTutorSchedules)) model.FullAccessForTutorSchedules_List = model.FullAccessForTutorSchedules.Split(',').ToList();
                if (!string.IsNullOrEmpty(model.ShowOnlyOwnUserData)) model.ShowOnlyOwnUserData_List = model.ShowOnlyOwnUserData.Split(',').ToList();
                if (!string.IsNullOrEmpty(model.PayrollRatesRoles)) model.PayrollRatesRoles_List = model.PayrollRatesRoles.Split(',').ToList();
            }

            return models.Count == 0 ? null : models[0];
        }

        public void update(SettingsModel modifiedModel)
        {
            if (modifiedModel.FullAccessForTutorSchedules_List != null) modifiedModel.FullAccessForTutorSchedules = string.Join(",", modifiedModel.FullAccessForTutorSchedules_List.ToArray());
            if (modifiedModel.ShowOnlyOwnUserData_List != null) modifiedModel.ShowOnlyOwnUserData = string.Join(",", modifiedModel.ShowOnlyOwnUserData_List.ToArray());
            if (modifiedModel.PayrollRatesRoles_List != null) modifiedModel.PayrollRatesRoles = string.Join(",", modifiedModel.PayrollRatesRoles_List.ToArray());

            db.Database.ExecuteSqlCommand(@"
                    IF (SELECT COUNT(Id) FROM Settings WHERE Id = @StudentRoleId) = 0 INSERT INTO Settings (Id) VALUES(@StudentRoleId);
                    UPDATE Settings SET Value_Guid=@StudentRole, Notes=@StudentRole_Notes WHERE Id=@StudentRoleId;

                    IF (SELECT COUNT(Id) FROM Settings WHERE Id = @TutorRoleId) = 0 INSERT INTO Settings (Id) VALUES(@TutorRoleId);
                    UPDATE Settings SET Value_Guid=@TutorRole, Notes=@TutorRole_Notes WHERE Id=@TutorRoleId;

                    IF (SELECT COUNT(Id) FROM Settings WHERE Id = @ResetPasswordId) = 0 INSERT INTO Settings (Id) VALUES(@ResetPasswordId);
                    UPDATE Settings SET Value_String=@ResetPassword, Notes=@ResetPassword_Notes WHERE Id=@ResetPasswordId;

                    IF (SELECT COUNT(Id) FROM Settings WHERE Id = @FullAccessForTutorSchedulesId) = 0 INSERT INTO Settings (Id) VALUES(@FullAccessForTutorSchedulesId);
                    UPDATE Settings SET Value_String=@FullAccessForTutorSchedules, Notes=@FullAccessForTutorSchedules_Notes WHERE Id=@FullAccessForTutorSchedulesId;

                    IF (SELECT COUNT(Id) FROM Settings WHERE Id = @ShowOnlyOwnUserDataId) = 0 INSERT INTO Settings (Id) VALUES(@ShowOnlyOwnUserDataId);
                    UPDATE Settings SET Value_String=@ShowOnlyOwnUserData, Notes=@ShowOnlyOwnUserData_Notes WHERE Id=@ShowOnlyOwnUserDataId;

                    IF (SELECT COUNT(Id) FROM Settings WHERE Id = @PayrollRatesRolesId) = 0 INSERT INTO Settings (Id) VALUES(@PayrollRatesRolesId);
                    UPDATE Settings SET Value_String=@PayrollRatesRoles, Notes=@PayrollRatesRoles_Notes WHERE Id=@PayrollRatesRolesId;

                    IF (SELECT COUNT(Id) FROM Settings WHERE Id = @AdvertisementBanner1Id) = 0 INSERT INTO Settings (Id) VALUES(@AdvertisementBanner1Id);
                    UPDATE Settings SET Value_String=@AdvertisementBanner1, Notes=NULL WHERE Id=@AdvertisementBanner1Id;

                    IF (SELECT COUNT(Id) FROM Settings WHERE Id = @ClubScheduleImage1Id) = 0 INSERT INTO Settings (Id) VALUES(@ClubScheduleImage1Id);
                    UPDATE Settings SET Value_String=@ClubScheduleImage1, Notes=NULL WHERE Id=@ClubScheduleImage1Id;

                    IF (SELECT COUNT(Id) FROM Settings WHERE Id = @ClubScheduleImage2Id) = 0 INSERT INTO Settings (Id) VALUES(@ClubScheduleImage2Id);
                    UPDATE Settings SET Value_String=@ClubScheduleImage2, Notes=NULL WHERE Id=@ClubScheduleImage2Id;

                    IF (SELECT COUNT(Id) FROM Settings WHERE Id = @ClubScheduleImage3Id) = 0 INSERT INTO Settings (Id) VALUES(@ClubScheduleImage3Id);
                    UPDATE Settings SET Value_String=@ClubScheduleImage3, Notes=NULL WHERE Id=@ClubScheduleImage3Id;
                ",
                    DBConnection.getSqlParameter(SettingsModel.COL_StudentRole.Name + "Id", SettingsModel.COL_StudentRole.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_StudentRole.Name, Util.wrapNullable(modifiedModel.StudentRole)),
                    DBConnection.getSqlParameter(SettingsModel.COL_StudentRole_Notes.Name, Util.wrapNullable(modifiedModel.StudentRole_Notes)),
                    DBConnection.getSqlParameter(SettingsModel.COL_TutorRole.Name + "Id", SettingsModel.COL_TutorRole.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_TutorRole.Name, Util.wrapNullable(modifiedModel.TutorRole)),
                    DBConnection.getSqlParameter(SettingsModel.COL_TutorRole_Notes.Name, Util.wrapNullable(modifiedModel.TutorRole_Notes)),
                    DBConnection.getSqlParameter(SettingsModel.COL_ResetPassword.Name + "Id", SettingsModel.COL_ResetPassword.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ResetPassword.Name, Util.wrapNullable(modifiedModel.ResetPassword)),
                    DBConnection.getSqlParameter(SettingsModel.COL_ResetPassword_Notes.Name, Util.wrapNullable(modifiedModel.ResetPassword_Notes)),
                    DBConnection.getSqlParameter(SettingsModel.COL_FullAccessForTutorSchedules.Name + "Id", SettingsModel.COL_FullAccessForTutorSchedules.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_FullAccessForTutorSchedules.Name, Util.wrapNullable(modifiedModel.FullAccessForTutorSchedules)),
                    DBConnection.getSqlParameter(SettingsModel.COL_FullAccessForTutorSchedules_Notes.Name, Util.wrapNullable(modifiedModel.FullAccessForTutorSchedules_Notes)),
                    DBConnection.getSqlParameter(SettingsModel.COL_ShowOnlyOwnUserData.Name + "Id", SettingsModel.COL_ShowOnlyOwnUserData.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ShowOnlyOwnUserData.Name, Util.wrapNullable(modifiedModel.ShowOnlyOwnUserData)),
                    DBConnection.getSqlParameter(SettingsModel.COL_ShowOnlyOwnUserData_Notes.Name, Util.wrapNullable(modifiedModel.ShowOnlyOwnUserData_Notes)),
                    DBConnection.getSqlParameter(SettingsModel.COL_PayrollRatesRoles.Name + "Id", SettingsModel.COL_PayrollRatesRoles.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_PayrollRatesRoles.Name, Util.wrapNullable(modifiedModel.PayrollRatesRoles)),
                    DBConnection.getSqlParameter(SettingsModel.COL_PayrollRatesRoles_Notes.Name, Util.wrapNullable(modifiedModel.PayrollRatesRoles_Notes)),
                    DBConnection.getSqlParameter(SettingsModel.COL_AdvertisementBanner1.Name + "Id", SettingsModel.COL_AdvertisementBanner1.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_AdvertisementBanner1.Name, Util.wrapNullable(modifiedModel.AdvertisementBanner1)),
                    DBConnection.getSqlParameter(SettingsModel.COL_ClubScheduleImage1.Name + "Id", SettingsModel.COL_ClubScheduleImage1.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ClubScheduleImage1.Name, Util.wrapNullable(modifiedModel.ClubScheduleImage1)),
                    DBConnection.getSqlParameter(SettingsModel.COL_ClubScheduleImage2.Name + "Id", SettingsModel.COL_ClubScheduleImage2.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ClubScheduleImage2.Name, Util.wrapNullable(modifiedModel.ClubScheduleImage2)),
                    DBConnection.getSqlParameter(SettingsModel.COL_ClubScheduleImage3.Name + "Id", SettingsModel.COL_ClubScheduleImage3.Id),
                    DBConnection.getSqlParameter(SettingsModel.COL_ClubScheduleImage3.Name, Util.wrapNullable(modifiedModel.ClubScheduleImage3))
            );
        }

        #endregion
        /******************************************************************************************************************************************************/
    }
}