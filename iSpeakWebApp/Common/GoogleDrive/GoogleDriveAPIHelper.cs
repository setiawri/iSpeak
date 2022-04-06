using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Json;

using LIBWebMVC;
using LIBUtil;
using iSpeakWebApp.Models;
using iSpeakWebApp.Controllers;

public class GoogleDriveAPIHelper
{
    public static string[] _Scopes = { DriveService.Scope.Drive };
    public static string _GoogleSharedFolderId = "19M82j8f1MQnxL4EE6eA-9EGSQHU9iXfo"; //Folder in Google Drive which has been set to be shared to anyone with link

    public static string _GoogleDriveVirtualPath = "~/Common/GoogleDrive";
    public static string _credentialFilename_Localhost = "credential-localhost.json";
    public static string _credentialFilename_Live = "credential-live.json";
    public static string _JsonToken = "token.json";

    public static string _GoogleDrivePhysicalPath = System.Web.Hosting.HostingEnvironment.MapPath(_GoogleDriveVirtualPath);
    public static string _CredentialPhysicalPath = Path.Combine(_GoogleDrivePhysicalPath, HttpContext.Current.Server.MachineName == iSpeakWebApp.Helper.DEVCOMPUTERNAME ? _credentialFilename_Localhost : _credentialFilename_Live);
    public static string _JsonTokenPhysicalPath = Path.Combine(_GoogleDrivePhysicalPath, _JsonToken);
    public static string _ApplicationName = JObject.Parse(File.ReadAllText(_CredentialPhysicalPath))["web"]["project_id"].ToString();

    public static string _ClientId = JObject.Parse(File.ReadAllText(_CredentialPhysicalPath))["web"]["client_id"].ToString();
    public static ClientSecrets ClientSecret
    {
        get
        {
            using (var stream = new FileStream(_CredentialPhysicalPath, FileMode.Open, FileAccess.Read))
            {
                return GoogleClientSecrets.FromStream(stream).Secrets;
            };
        }
    }

    //file Upload to the Google Drive root folder.
    public static string UploadToDrive(DriveService driveService, HttpPostedFileBase file)
    {
        if (file != null && file.ContentLength > 0)
        {
            //upload to server
            string path = Path.Combine(_GoogleDrivePhysicalPath, Path.GetFileName(file.FileName));
            file.SaveAs(path);

            var FileMetaData = new Google.Apis.Drive.v3.Data.File();
            FileMetaData.Name = Path.GetFileName(file.FileName);
            FileMetaData.MimeType = MimeMapping.GetMimeMapping(path);
            FileMetaData.Parents = new List<string> { _GoogleSharedFolderId };

            //upload to drive
            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(path, FileMode.Open))
            {
                request = driveService.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                request.Fields = "id";
                request.Upload();
            }

            //delete file on server
            File.Delete(path);

            return request.ResponseBody.Id;
        }

        return null;
    }

    public static bool isFileUploadSuccessful(Controller controller, string OnlineFileId)
    {
        if (!string.IsNullOrEmpty(OnlineFileId))
            return true;
        else
        {
            LIBWebMVC.UtilWebMVC.setBootboxMessage(controller, "Error occurred. Please contact IT.");
            return false;
        }
    }

    public static List<Google.Apis.Drive.v3.Data.File> GetFileList(DriveService driveService)
    {
        return driveService.Files.List().Execute().Files as List<Google.Apis.Drive.v3.Data.File>;
    }

    public static AuthorizationCodeWebApp.AuthResult getAuthorization(Controller controller)
    {
        return new AuthorizationCodeMvcApp(controller, new AppFlowMetadata()).AuthorizeAsync(CancellationToken.None).Result;
    }

    public static AuthorizationCodeWebApp.AuthResult checkExpiration(Controller controller, AuthorizationCodeWebApp.AuthResult authorization)
    {
        if (authorization.Credential.Token.IsExpired(Google.Apis.Util.SystemClock.Default))
        {
            Google.Apis.Auth.OAuth2.Responses.TokenResponse token = new Google.Apis.Auth.OAuth2.Responses.TokenResponse();
            //recreate the token
            token = authorization.Credential.Flow.RefreshTokenAsync(
                    UserAccountsController.getUserId(controller.Session).ToString(), 
                    authorization.Credential.Token.RefreshToken, 
                    CancellationToken.None
                ).Result;

            authorization = getAuthorization(controller);
        }
        return authorization;
    }

    public static DriveService GetServiceForWebApplication(AuthorizationCodeWebApp.AuthResult authorization)
    {
        if (authorization.Credential != null)
        {
            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = authorization.Credential,
                ApplicationName = _ApplicationName
            });
        }
        return null;
    }

    /*
     * add to credential file 
     *      "access_type": "offline",
     *      "response_type": "code"
     * 
     * add http://127.0.0.1/authorize/ to Google Console credential's Authorized Redirect URI
     * 
     */
    public static bool GetServiceForDesktopApplication(ref DriveService driveService)
    {
        UserCredential credential;

        using (var stream = new FileStream(_CredentialPhysicalPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                ClientSecret,
                _Scopes,
                _ApplicationName,
                CancellationToken.None,
                new FileDataStore(_JsonTokenPhysicalPath, true)).Result;
        }

        driveService = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = _ApplicationName,
        });

        return true;
    }

}


public class AppFlowMetadata : FlowMetadata
{
    private static readonly IAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer {
        ClientSecrets = new ClientSecrets
        {
            ClientId = GoogleDriveAPIHelper._ClientId,
            ClientSecret = GoogleDriveAPIHelper.ClientSecret.ClientSecret
        },
        Scopes = GoogleDriveAPIHelper._Scopes,
        DataStore = new DBDataStore() //FileDataStore("Drive.Api.Auth.Store")
    });

    public override string GetUserId(Controller controller)
    {
        return UserAccountsController.getUserId(controller.Session).ToString();
    }

    public override IAuthorizationCodeFlow Flow
    {
        get { return flow; }
    }
}


public class DBDataStore : IDataStore
{
    private readonly iSpeakWebApp.DBContext db = new iSpeakWebApp.DBContext();
    private const string TABLENAME = "GoogleTokens";
    private const string COL_DB_Id = "Id";
    private const string COL_DB_Token = "Token";

    public DBDataStore() { }

    public Task<T> GetAsync<T>(string key)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key MUST have a value");

        TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();

        string token = WebDBConnection.GetValues(db.Database, string.Format(@"
                SELECT @{0} = {3} 
                FROM {1} 
                WHERE [{2}]='{4}'
            ", WebDBConnection.PARAM_RETURNVALUE, TABLENAME, COL_DB_Id, COL_DB_Token, key)
            );

        if (string.IsNullOrWhiteSpace(token))
            taskCompletionSource.SetResult(default(T));
        else
        {
            try
            {
                taskCompletionSource.SetResult(NewtonsoftJsonSerializer.Instance.Deserialize<T>(token));
            }
            catch (Exception ex) { taskCompletionSource.SetException(ex); }
        }

        return taskCompletionSource.Task;
    }

    public Task StoreAsync<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key MUST have a value");

        var serialized = NewtonsoftJsonSerializer.Instance.Serialize(value);
        WebDBConnection.Execute(db.Database, string.Format(@"
                IF NOT EXISTS (SELECT * FROM {0} WHERE [{1}] = @{1})
                    INSERT INTO {0} ([{1}],{2}) VALUES (@{1},@{2})
                ELSE
                    UPDATE {0} SET {2} = @{2} WHERE [{1}] = @{1}
            ", TABLENAME, COL_DB_Id, COL_DB_Token),
            DBConnection.getSqlParameter(COL_DB_Id, key),
            DBConnection.getUnsanitizedSqlParameter(COL_DB_Token, serialized)
        );
        return Task.Delay(0);
    }

    public Task ClearAsync()
    {
        WebDBConnection.DeleteAllRows(db.Database, TABLENAME);
        return Task.Delay(0);
    }

    public Task DeleteAsync<T>(string key)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key MUST have a value");

        WebDBConnection.Delete(db.Database, TABLENAME,
                DBConnection.getSqlParameter(COL_DB_Id, key)
            );

        return Task.Delay(0);
    }

}


namespace iSpeakWebApp.Controllers
{
    public class AuthCallbackController : Google.Apis.Auth.OAuth2.Mvc.Controllers.AuthCallbackController
    {
        protected override FlowMetadata FlowData
        {
            get { return new AppFlowMetadata(); }
        }
    }

}
