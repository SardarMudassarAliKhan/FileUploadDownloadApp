using FileUploadDownloadApp.Model;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadDownloadApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        public HomeController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: File/Upload
        public IActionResult Upload()
        {
            return View();
        }

        // POST: File/Upload
        [HttpPost]
        public async Task<IActionResult> Upload(FileUploadViewModel model)
        {
            if (model.File != null && model.File.Length > 0)
            {
                // Define the upload folder
                string uploadPath = Path.Combine(_environment.WebRootPath, "uploads");

                // Create the folder if it doesn't exist
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Generate the file path
                string filePath = Path.Combine(uploadPath, model.File.FileName);

                // Save the file to the specified location
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                ViewBag.Message = "File uploaded successfully!";
            }

            return View();
        }

        // GET: File/ListFiles
        public IActionResult ListFiles()
        {
            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadPath))
            {
                return View(new List<string>());
            }

            var files = Directory.GetFiles(uploadPath).Select(Path.GetFileName).ToList();
            return View(files);
        }

        // GET: File/Download?filename=example.txt
        public IActionResult Download(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return Content("Filename is not provided.");
            }

            string filePath = Path.Combine(_environment.WebRootPath, "uploads", filename);

            if (!System.IO.File.Exists(filePath))
            {
                return Content("File not found.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", filename);
        }
    }
}

