using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;

using LIBUtil;
using LIBWebMVC;

using Google.Apis.Auth.OAuth2.Web;

namespace iSpeakWebApp.Controllers
{
    public class FilesController : Controller
    {
        private readonly DBContext db = new DBContext();
        public static Guid RootId = new Guid("0A9F488B-FB9C-4F09-BDF0-7143462B0E2E");
        public static string BUTTONVALUE_UPLOADFILE = "UploadFile";
        public static string BUTTONVALUE_UPLOADREVISION = "UploadRevision";
        public static string BUTTONVALUE_CREATEDIRECTORY = "CreateDirectory";
        public static string BUTTONVALUE_UPDATEDIRECTORY = "UpdateDirectory";

        /* INDEX **********************************************************************************************************************************************/

        // GET: Files
        public ActionResult Index(int? rss, Guid? ParentId, int? IsDeleted, bool? ViewTrashBin, string FILTER_Keyword)
        {
            if (!UserAccountsController.getUserAccess(Session).Files_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(ParentId, IsDeleted, ViewTrashBin, FILTER_Keyword);
            if (rss != null)
                ViewBag.RemoveDatatablesStateSave = rss;

            List<FilesModel> models = get(ParentId, IsDeleted, ViewTrashBin, FILTER_Keyword);
            return View(models);
        }

        // POST: Files
        [HttpPost]
        public ActionResult Index(string submitButton, Guid? ParentId, bool? IsDeleted, bool? ViewTrashBin, string FILTER_Keyword, 
            HttpPostedFileBase File, Guid? FileId, string FileDescription, bool? IsGlobalFile, 
            Guid? DirectoryId, string DirectoryName, bool? IsGlobalDirectory)
        {
            ActionResult actionResult = null;
            if (!string.IsNullOrWhiteSpace(submitButton))
                actionResult = Create(submitButton, ParentId, File, FileId, FileDescription, IsGlobalFile, DirectoryId, DirectoryName, IsGlobalDirectory);
            if (actionResult != null)
                return actionResult;

            return RedirectToAction(nameof(Index), new { ParentId = ParentId, FILTER_Keyword = FILTER_Keyword });
        }

        //This method is required by Google Drive API
        public ActionResult IndexAsync(CancellationToken cancellationToken)
        {
            AuthorizationCodeWebApp.AuthResult authorization = GoogleDriveAPIHelper.getAuthorization(this); 
            if (authorization.Credential == null)
                return new RedirectResult(authorization.RedirectUri);
            else
                return RedirectToAction(nameof(Index));
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(Guid? ParentId, int? IsDeleted, bool? ViewTrashBin, string FILTER_Keyword)
        {
            ViewBag.ParentId = ParentId;
            ViewBag.IsDeleted = IsDeleted;
            ViewBag.FILTER_Keyword = FILTER_Keyword;

            ViewBag.DirectoryTree = getDirectoryTree(ParentId, ViewTrashBin);
        }

        public JsonResult Ajax_Update_Approved(Guid id, bool value)
        {
            update_Approved(id, value);
            return Json(new { Message = "" });
        }

        public JsonResult Ajax_Update_IsDeleted(Guid id, bool value)
        {
            update_IsDeleted(id, value);
            return Json(new { Message = "" });
        }

        public ActionResult Create(string submitButton, Guid? ParentId, HttpPostedFileBase File, Guid? FileId, string FileDescription, bool? IsGlobalFile, Guid? DirectoryId, string DirectoryName, bool? IsGlobalDirectory)
        {
            FilesModel model = new FilesModel();
            if (ParentId == null)
                model.ParentId = RootId;
            else
                model.ParentId = (Guid)ParentId;

            if (IsGlobalFile == true || IsGlobalDirectory == true)
                model.Branches_Id = null;
            else
                model.Branches_Id = Helper.getActiveBranchId(Session);

            if (submitButton == BUTTONVALUE_UPLOADFILE || submitButton == BUTTONVALUE_UPLOADREVISION)
            {
                if (File == null || File.ContentLength == 0)
                    UtilWebMVC.setBootboxMessage(this, "Invalid File");
                else
                {
                    AuthorizationCodeWebApp.AuthResult authorization = GoogleDriveAPIHelper.getAuthorization(this);
                    if (authorization.Credential == null)
                    {
                        UtilWebMVC.setBootboxMessage(this, "Google Login was required. Please navigate to the folder and try to upload the file again.");
                        return RedirectToAction(nameof(IndexAsync));
                    }
                    else
                    {
                        authorization = GoogleDriveAPIHelper.checkExpiration(this, authorization);

                        string OnlineFileId = GoogleDriveAPIHelper.UploadToDrive(GoogleDriveAPIHelper.GetServiceForWebApplication(authorization), File);
                        if (GoogleDriveAPIHelper.isFileUploadSuccessful(this, OnlineFileId))
                        {

                            model.Filename = File.FileName;
                            model.OnlineFileId = OnlineFileId;
                            model.Notes = FileDescription;

                            if (submitButton == BUTTONVALUE_UPLOADFILE)
                                add(model);
                            else
                            {
                                FilesModel originalModel = get((Guid)FileId);
                                model.ParentId = originalModel.ParentId;
                                add(model);
                                originalModel.ParentId = model.Id;
                                update(originalModel);
                            }
                        }
                    }
                }
            }
            else if (submitButton == BUTTONVALUE_CREATEDIRECTORY || submitButton == BUTTONVALUE_UPDATEDIRECTORY)
            {
                if (string.IsNullOrWhiteSpace(DirectoryName))
                    UtilWebMVC.setBootboxMessage(this, "Invalid Name");
                else
                {
                    model.DirectoryName = DirectoryName;
                    if (submitButton == BUTTONVALUE_CREATEDIRECTORY)
                        add(model);
                    else
                    {
                        model.Id = (Guid)DirectoryId;
                        update(model);
                    }
                }
            }

            return null;
        }

        public void update(FilesModel modifiedModel)
        {
            FilesModel originalModel = get(modifiedModel.Id);

            string log = string.Empty;
            log = Helper.append<BranchesModel>(log, originalModel.Branches_Id, modifiedModel.Branches_Id, FilesModel.COL_Branches_Id.LogDisplay);
            log = Helper.append(log, originalModel.DirectoryName, modifiedModel.DirectoryName, FilesModel.COL_DirectoryName.LogDisplay);
            log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, FilesModel.COL_Notes.LogDisplay);

            if (originalModel.ParentId != modifiedModel.ParentId)
                log = Util.append(log, "Replaced with a new version", Environment.NewLine);

            if (!string.IsNullOrEmpty(log))
            {
                update(modifiedModel, log);
            }
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public List<FilesModel> get(Guid? ParentId, int? IsDeleted, bool? ViewTrashBin, string FILTER_Keyword) { return get(Session, null, ParentId, IsDeleted, ViewTrashBin, FILTER_Keyword); }
        public FilesModel get(Guid Id) { return get(Session, Id, null, null, null, null).FirstOrDefault(); }
        public static List<FilesModel> get(HttpSessionStateBase Session, Guid? Id, Guid? ParentId, int? IsDeleted, bool? ViewTrashBin, string FILTER_Keyword)
        {
            Guid Branches_Id = Helper.getActiveBranchId(Session);

            if (ParentId == null && ViewTrashBin == null)
                ParentId = RootId;
            else if (ViewTrashBin != null && (bool)ViewTrashBin)
                IsDeleted = 1;

            return new DBContext().Database.SqlQuery<FilesModel>(@"
                    SELECT FilesTable.*,
                        Branches.Name AS Branches_Name,
                        UserAccounts.Fullname AS UserAccounts_Name,
                        UserAccounts.No AS UserAccounts_No,
                        CAST(IIF(DirectoryName IS NULL, 0, 1) AS BIT) AS IsDirectory,
                        CAST(IIF((SELECT COUNT(Id) FROM Files WHERE ParentId = FilesTable.Id) > 0, 1, 0) AS BIT) AS hasRevisions,
                        CAST(IIF((SELECT Filename FROM Files WHERE Id = FilesTable.ParentId) IS NULL, 0, 1) AS BIT) AS parentIsFile
                    FROM Files FilesTable
                        LEFT JOIN Branches ON Branches.Id = FilesTable.Branches_Id
                        LEFT JOIN UserAccounts ON UserAccounts.Id = FilesTable.UserAccounts_Id
                    WHERE 1=1
						AND (@Id IS NULL OR FilesTable.Id = @Id)
						AND (@Id IS NOT NULL OR (
    						(@FILTER_Keyword IS NULL OR (
                                    FilesTable.No LIKE '%'+@FILTER_Keyword+'%'
                                    OR FilesTable.Filename LIKE '%'+@FILTER_Keyword+'%'
                                ))
                            AND (@ParentId IS NULL OR FilesTable.ParentId = @ParentId)
                            AND (@Branches_Id IS NULL OR (FilesTable.Branches_Id = @Branches_Id OR FilesTable.Branches_Id IS NULL))
                            AND (@IsDeleted IS NULL OR FilesTable.IsDeleted = @IsDeleted)
                        ))
					ORDER BY FilesTable.Filename ASC, DirectoryName ASC
                ",
                DBConnection.getSqlParameter(FilesModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(FilesModel.COL_Branches_Id.Name, Branches_Id),
                DBConnection.getSqlParameter(FilesModel.COL_ParentId.Name, ParentId),
                DBConnection.getSqlParameter(FilesModel.COL_IsDeleted.Name, IsDeleted),
                DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
            ).ToList();
        }

        public MvcHtmlString getDirectoryTree(Guid? ParentId, bool? ViewTrashBin)
        {
            List<FilesModel> models = new DBContext().Database.SqlQuery<FilesModel>(@"
                    ;WITH DirectoryTree AS (

                       SELECT Files.*
                       FROM Files
                       WHERE Id = @ParentId -- Recursion starting point

                       UNION ALL

                       SELECT Files.*
                       FROM Files 
                         JOIN DirectoryTree on DirectoryTree.ParentId = Files.Id -- Recursion while ParentId IS NOT NULL
                          AND Files.Id <> Files.ParentId -- Exit condition

                    ) SELECT DirectoryTree.* FROM DirectoryTree
                ",
                DBConnection.getSqlParameter(FilesModel.COL_ParentId.Name, ParentId)
            ).ToList();

            string directoryTree = "";
            foreach (FilesModel model in models)
                directoryTree = string.Format("<a href='{0}'>{1}</a> <i class='icon-arrow-right22'></i> {2}",
                    Url.Action("Index", "Files", new { No = model.No, ParentId = model.Id }),
                    model.DirectoryName == null ? model.Filename : model.DirectoryName,
                    directoryTree);
            directoryTree = string.Format("<a href='{0}'>ROOT</a> <i class='icon-arrow-right22'></i> {1}",
                Url.Action("Index", "Files", new { ParentId = RootId }),
                directoryTree);
            
            if (ViewTrashBin != null && (bool)ViewTrashBin)
                directoryTree += " TRASH BIN";

            return new MvcHtmlString(directoryTree);
        }

        public void update(FilesModel model, string log)
        {
            WebDBConnection.Update(db.Database, "Files",
                    DBConnection.getSqlParameter(FilesModel.COL_Id.Name, model.Id),
                    DBConnection.getSqlParameter(FilesModel.COL_Branches_Id.Name, model.Branches_Id),
                    DBConnection.getSqlParameter(FilesModel.COL_DirectoryName.Name, model.DirectoryName),
                    DBConnection.getSqlParameter(FilesModel.COL_ParentId.Name, model.ParentId),
                    DBConnection.getSqlParameter(FilesModel.COL_Notes.Name, model.Notes)
                );

            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        public void update_Approved(Guid Id, bool value)
        {
            WebDBConnection.Update(db.Database, "Files",
                    DBConnection.getSqlParameter(FilesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(FilesModel.COL_Approved.Name, value)
                );
            ActivityLogsController.AddEditLog(db, Session, Id, string.Format(FilesModel.COL_Approved.LogDisplay, null, value));
            db.SaveChanges();
        }

        public void update_IsDeleted(Guid Id, bool value)
        {
            WebDBConnection.Update(db.Database, "Files",
                    DBConnection.getSqlParameter(FilesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(FilesModel.COL_IsDeleted.Name, value)
                );
            ActivityLogsController.AddEditLog(db, Session, Id, string.Format(FilesModel.COL_IsDeleted.LogDisplay, null, value));
            db.SaveChanges();
        }

        public void add(FilesModel model)
        {
            model.UserAccounts_Id = (Guid)UserAccountsController.getUserId(Session);
            model.No = WebDBConnection.GetNextHex(db.Database, "Files", "No");

            WebDBConnection.Insert(db.Database, "Files",
                DBConnection.getSqlParameter(FilesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(FilesModel.COL_OnlineFileId.Name, model.OnlineFileId),
                DBConnection.getSqlParameter(FilesModel.COL_ParentId.Name, model.ParentId),
                DBConnection.getSqlParameter(FilesModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(FilesModel.COL_No.Name, model.No),
                DBConnection.getSqlParameter(FilesModel.COL_Filename.Name, model.Filename),
                DBConnection.getSqlParameter(FilesModel.COL_DirectoryName.Name, model.DirectoryName),
                DBConnection.getSqlParameter(FilesModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(FilesModel.COL_UserAccounts_Id.Name, model.UserAccounts_Id),
                DBConnection.getSqlParameter(FilesModel.COL_Timestamp.Name, model.Timestamp),
                DBConnection.getSqlParameter(FilesModel.COL_IsDeleted.Name, model.IsDeleted),
                DBConnection.getSqlParameter(FilesModel.COL_Approved.Name, model.Approved)
            );
        }

        /******************************************************************************************************************************************************/
    }
}