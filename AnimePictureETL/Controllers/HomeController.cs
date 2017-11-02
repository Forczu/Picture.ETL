using AnimePictureETL.ExtractedData;
using AnimePictureETL.Models;
using AnimePictureETL.Scrappers;
using AnimePictureETL.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AnimePictureETL.Extensions;
using AnimePictureETL.ExtractedData.Danbooru;
using System.Web.Routing;

namespace AnimePictureETL.Controllers
{
    public class HomeController : Controller
    {
        private IDanbooruService _danbooruService;

        private IAnimeWorksService _animeWorksService;

        public HomeController(IDanbooruService danbooruService, IAnimeWorksService animeWorksService)
        {
            _danbooruService = danbooruService;
            _animeWorksService = animeWorksService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public void DanbooruExtractTag(string tag)
        {
            string fileLink = Server.MapPath("~/Images");
            string configFilePath = Server.MapPath("~/Images/Config/danbooru_config.ini");
            ETL.DanbooruEtlProcess etl = new ETL.DanbooruEtlProcess(configFilePath, tag, fileLink);
            etl.Run();
        }

        public JsonResult DanbooruExtract(string id)
        {
            long danbooruId = Convert.ToInt64(id);
            var post = _danbooruService.ExtractPost(danbooruId);
            var postWithChanges = _danbooruService.GetSuggestedValuesForPost(post);

            string fileDest = Server.MapPath("~/Images/Tmp");
            string dest = fileDest + @"\" + postWithChanges.FileName;
            if (!System.IO.File.Exists(dest))
            {
                string source = DanbooruScrapper.DOWNLOAD_DEST + @"\" + postWithChanges.FileName;
                System.IO.File.Move(source, dest);
            }
            postWithChanges.FileName = string.Format("Images/Tmp/{0}", postWithChanges.FileName);
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = postWithChanges
            };
        }

        public void SavePostWithChanges(PostChangesView data)
        {
            data.FileName = Path.GetFileName(data.FileName);
            string fileDest = Server.MapPath("~/Images");
            string tempLocation = Server.MapPath("~/Images/Tmp");
            string source = tempLocation + @"\" + data.FileName;
            string dest = fileDest + @"\" + data.FileName;
            if (!System.IO.File.Exists(dest))
            {
                System.IO.File.Move(source, dest);
            }
            data.FileName = string.Format("/Images/{0}", data.FileName); ;
            _danbooruService.Save(data);
        }
    }
}