using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Web;
using System.IO;
using System.Threading;
using System.Collections.Generic;

public class GoogleDriveAPIHelper
{
    static string[] Scopes = { DriveService.Scope.Drive };
    static string ApplicationName = "iSpeakWebApp";
    static string credentialFilename_Localhost = "credential-localhost.json";
    static string credentialFilename_Live = "credential-live.json";
    static string GoogleDrivePath = "~/Common/GoogleDrive";
    static string JsonToken = "token.json";
    static string SharedFolderId = "1mG5TZOl6y8TXJtJh-WhtyoHsFwF3k9jp"; //Folder which has been set to be shared to anyone with link

    public static string message = "";

    //file Upload to the Google Drive root folder.
    public static string UploadToDrive(HttpServerUtilityBase Server, HttpPostedFileBase file)
    {
        if (file != null && file.ContentLength > 0)
        {
            DriveService service = GetService(Server);

            //upload to server
            string path = Path.Combine(HttpContext.Current.Server.MapPath(GoogleDrivePath), Path.GetFileName(file.FileName));
            file.SaveAs(path);

            var FileMetaData = new Google.Apis.Drive.v3.Data.File();
            FileMetaData.Name = Path.GetFileName(file.FileName);
            FileMetaData.MimeType = MimeMapping.GetMimeMapping(path);
            FileMetaData.Parents = new List<string> { SharedFolderId };

            //upload to drive
            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(path, System.IO.FileMode.Open))
            {
                request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                request.Fields = "id";
                request.Upload();
            }

            ////set permission
            //Google.Apis.Drive.v3.Data.Permission permission = new Google.Apis.Drive.v3.Data.Permission();
            //permission.Type = "anyone";
            ////permission.EmailAddress = "anotheruser@gmail.com";
            //permission.Role = "owner";
            //service.Permissions.Create(permission, request.ResponseBody.Id).Execute();

            //delete file on server
            File.Delete(path);

            return request.ResponseBody.Id;
        }

        return null;
    }

    public static void GetFileList(HttpServerUtilityBase Server)
    {
        DriveService service = GetService(Server);

        FilesResource.ListRequest listRequest = service.Files.List();
        listRequest.PageSize = 10;
        listRequest.Fields = "nextPageToken, files(id, name)";

        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;
        if (files == null || files.Count == 0)
            message += " :: No files found.";
        else
        {
            message = "";
            foreach (var file in files)
            {
                message = LIBUtil.Util.append(message, string.Format("{0} ({1})", file.Name, file.Id), ", ");
            }
            message = "Files: " + message;
        }

        return;
    }

    public static DriveService GetService(HttpServerUtilityBase Server)
    {
        UserCredential credential;

        var CSPath = System.Web.Hosting.HostingEnvironment.MapPath(GoogleDrivePath);
        string path = Path.Combine(CSPath, Server.MachineName == iSpeakWebApp.Helper.DEVCOMPUTERNAME ? credentialFilename_Localhost : credentialFilename_Live);
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            string FolderPath = System.Web.Hosting.HostingEnvironment.MapPath(GoogleDrivePath);
            string FilePath = Path.Combine(FolderPath, JsonToken);
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                ApplicationName,
                CancellationToken.None,
                new FileDataStore(FilePath, true)).Result;
        }

        return new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

}