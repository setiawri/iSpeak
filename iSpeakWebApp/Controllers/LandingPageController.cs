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
        public ActionResult LandingPage(List<HttpPostedFileBase> uploadFileBanner1, List<HttpPostedFileBase> uploadFileClubSchedule1, List<HttpPostedFileBase> uploadFileClubSchedule2, List<HttpPostedFileBase> uploadFileClubSchedule3)
        {
            SettingsModel settings = SettingsController.get();
            bool executeUpdate = false;
            if (uploadFileClubSchedule1 != null)
            {
                if (uploadFileClubSchedule1[0] != null)
                {
                    HttpPostedFileBase image = uploadFileClubSchedule1[0];
                    if (image.ContentLength > 0)
                    {
                        executeUpdate = true;

                        string filename = string.Format("{0}-{1:yyyyMMdd}{2}", SettingsModel.COL_ClubScheduleImage1.Id.ToString(), DateTime.Now, Path.GetExtension(image.FileName));

                        //delete original file
                        if (!string.IsNullOrEmpty(settings.ClubScheduleImage1) && System.IO.File.Exists(settings.ClubScheduleImage1))
                            System.IO.File.Delete(settings.ClubScheduleImage1);

                        //upload new file
                        settings.ClubScheduleImage1 = filename;
                        image.SaveAs(System.IO.Path.Combine(Server.MapPath(Helper.IMAGEUPLOADFOLDER), settings.ClubScheduleImage1));
                    }
                }
            }

            if(executeUpdate)
                new SettingsController().update(settings);

            return RedirectToAction(nameof(LandingPage));
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        /******************************************************************************************************************************************************/
    }
}