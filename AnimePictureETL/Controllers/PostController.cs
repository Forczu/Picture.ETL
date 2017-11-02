using AnimePictureETL.Repositories;
using AnimePictureETL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnimePictureETL.Controllers
{
    public class PostController : Controller
    {
        private IAnimeWorksService _animeWorksService;

        public PostController(IAnimeWorksService animeWorksService)
        {
            _animeWorksService = animeWorksService;
        }

        public ActionResult Index(long id)
        {
            var pic = _animeWorksService.GetPictureForView(id);
            if (pic != null)
            {
                Session["Picture"] = pic;
                return View();
            }
            else
            {
                return View("NoPost");
            }
        }
    }
}