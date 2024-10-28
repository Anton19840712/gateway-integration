using Microsoft.AspNetCore.Mvc;


namespace websocket_client_web.Controllers
{

    public class PagesController : Controller
    {
        [Route("mainpage")]
        public ActionResult Index()
        {
            return View("socketpage");
        }
    }
}
