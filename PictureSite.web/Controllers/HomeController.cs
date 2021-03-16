using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PictureSite.data;
using PictureSite.web.Models;


namespace PictureSite.web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString =
           @"Data Source=.\sqlexpress;Initial Catalog=PictureSite;Integrated Security=true;";

        private readonly IWebHostEnvironment _environment;

        public HomeController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile imageFile, string password)
        {
            Guid guid = Guid.NewGuid();
            string uniqueFileName = $"{guid}-{imageFile.FileName}";
            string fileNamePath = Path.Combine(_environment.WebRootPath, "Uploads", uniqueFileName);
            using var fileStream = new FileStream(fileNamePath, FileMode.CreateNew);
            imageFile.CopyTo(fileStream);
            DbManager db = new DbManager(_connectionString);
            int id = db.AddImage(fileNamePath, password);
            ShareImageVM vm = new ShareImageVM
            {
                Image = db.GetImage(id)
            };
            return View(vm);
        }
        public IActionResult ViewImage(int imageId, string password, bool enteredPassword)
        {
            ViewImageViewModel vm = new ViewImageViewModel();
            List<int> ids = HttpContext.Session.Get<List<int>>("imageIds");
            bool contains = false;
            if(ids != null)
            {
                var currentId = ids.FirstOrDefault(i => i == imageId);
                if (currentId > 0)
                {
                    password = "";
                    enteredPassword = true;
                    contains = true;
                }
            }
         
            if (password == null)
            {
                vm.imageId = imageId;
                vm.EnteredPassword = enteredPassword;

                return View(vm);
            }

            DbManager db = new DbManager(_connectionString);
            bool correct = db.CorrectPassword(imageId, password);

            if (correct || contains)
            {
                db.UpdateViews(imageId);
                Image image = db.GetImage(imageId);
                vm.CorrectPassword = true;
                vm.EnteredPassword = true;
                if (HttpContext.Session.Get<List<int>>("imageIds") == null)
                {
                    List<int> imageIds = new List<int>();
                    HttpContext.Session.Set("imageIds", imageIds);
                }

                List<int> newList = HttpContext.Session.Get<List<int>>("imageIds");
                newList.Add(image.ID);
                HttpContext.Session.Set("imageIds", newList);
                vm.Image = image;
            }
            else if (!correct)
            {
                return Redirect($"/home/viewImage?imageId={imageId}&enteredPassword=true");
            }
            return View(vm);
        }

    }
}
