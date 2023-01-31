using iSpeakWebApp.Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using LIBUtil;
using System.Linq;
using System.IO;
using LIBWebMVC;

namespace iSpeakWebApp.Controllers
{
    public class LandingPageController : Controller
    {
        /* LANDING PAGE ***************************************************************************************************************************************/

        public ActionResult LandingPage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LandingPage(List<HttpPostedFileBase> AdvertisementBanner1, List<HttpPostedFileBase> ClubScheduleImage1, List<HttpPostedFileBase> ClubScheduleImage2, List<HttpPostedFileBase> ClubScheduleImage3)
        {
            if (UserAccountsController.getUserAccess(Session).LandingPageUpdate_Edit)
            {
                SettingsModel settings = SettingsController.get();
                bool executeUpdate = false;
                string filename = "";

                if ((filename = processFile(AdvertisementBanner1, SettingsModel.COL_AdvertisementBanner1.Id, settings.AdvertisementBanner1)) != null) { executeUpdate = true; settings.AdvertisementBanner1 = filename; }
                if ((filename = processFile(ClubScheduleImage1, SettingsModel.COL_ClubScheduleImage1.Id, settings.ClubScheduleImage1)) != null) { executeUpdate = true; settings.ClubScheduleImage1 = filename; }
                if ((filename = processFile(ClubScheduleImage2, SettingsModel.COL_ClubScheduleImage2.Id, settings.ClubScheduleImage2)) != null) { executeUpdate = true; settings.ClubScheduleImage2 = filename; }
                if ((filename = processFile(ClubScheduleImage3, SettingsModel.COL_ClubScheduleImage3.Id, settings.ClubScheduleImage3)) != null) { executeUpdate = true; settings.ClubScheduleImage3 = filename; }

                if (executeUpdate)
                    new SettingsController().update(settings);
            }

            return RedirectToAction(nameof(LandingPage));
        }

        /* METHODS ********************************************************************************************************************************************/

        private string processFile(List<HttpPostedFileBase> fileUpload, Guid Id, string previousFilename)
        {
            if (fileUpload != null && fileUpload[0] != null)
            {
                HttpPostedFileBase image = fileUpload[0];
                if (image.ContentLength > 0)
                {
                    string filename = string.Format("{0}-{1:yyyyMMdd}{2}", Id.ToString(), DateTime.Now, Path.GetExtension(image.FileName));

                    //delete original file
                    if (!string.IsNullOrEmpty(previousFilename) && System.IO.File.Exists(previousFilename))
                        System.IO.File.Delete(previousFilename);

                    //upload new file                    
                    image.SaveAs(System.IO.Path.Combine(Server.MapPath(Helper.IMAGEUPLOADFOLDER), filename));
                    return filename;
                }
            }

            return null;
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        /******************************************************************************************************************************************************/
    }
}