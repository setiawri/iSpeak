using System.Web.Mvc;

namespace iSpeakWebApp.Controllers
{
    /*
     * Articles are NOT filtered by Franchise. If it is needed, will implement in the future. It is currently only for landing page and there is only one
     */

    public class ArticlesController : Controller
    {
        public const string CONTROLLERNAME = "Articles";
        
        public PartialViewResult Article2022060101() { return PartialView("Article2022060101"); }

        /* METHODS ********************************************************************************************************************************************/

        /* DATABASE METHODS ***********************************************************************************************************************************/

        /******************************************************************************************************************************************************/
    }
}