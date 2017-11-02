using AnimePictureETL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnimePictureETL.Controllers
{
    public class BrowseController : Controller
    {
        private IAnimeWorksService _animeWorksService;

        public BrowseController(IAnimeWorksService animeWorksService)
        {
            _animeWorksService = animeWorksService;
        }
        
        public ActionResult Index()
        {
            var picturesFirstPage = _animeWorksService.GetLastestPictures();
            return View(picturesFirstPage);
        }

        public ActionResult Search(string name, string familyName, string series, string artist)
        {
            var pictures = _animeWorksService.SearchPictures(name, familyName, series, artist);
            return View("Index", pictures);
        }
    }
}