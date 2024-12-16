using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WEBka.Models;
using AnalisysMethods;

namespace WEBka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult TestDbContext()
        {
            var exists = _context != null;
            return Content($"DbContext is {(exists ? "available" : "null")}");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UploadFiles(IFormFile[] files)//загрузка файлов и анализ
        {
            if (files == null || files.Length == 0)
                return BadRequest("Выберите файлы для загрузки.");
            if (files.Length > 16)
                return BadRequest("Загружено слишком много файлов.");

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            // Убедитесь, что директория существует
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var analysisResults = new List<AnalysisResult>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Сохранение файла на сервер
                    var filePath = Path.Combine(uploadPath, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Анализ файла
                    string[] resultData = new string[5];
                    resultData = await AnalyzeFileAsync(filePath);

                    // Создание объекта результата
                    var analysisResult = new AnalysisResult
                    {
                        FileName = file.FileName,
                        ResultData = resultData,
                        CreatedAt = DateTime.UtcNow
                    };

                    analysisResults.Add(analysisResult);
                }
            }

            // Сохранение всех результатов в БД
            _context.AnalysisResults.AddRange(analysisResults);
            await _context.SaveChangesAsync();

            return RedirectToAction("Results");
        }


        /* РУДИМЕНТ, ИСПОЛЬЗОВАЛСЯ ДЛЯ ТЕСТОВ. функция загрузки 1 файла
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not uploaded.");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            

            // Анализ файла (пример: просто подсчет строк)
            var result = await AnalyzeFileAsync(filePath);

            // Сохранение результата в БД
            var analysisResult = new AnalysisResult
            {
                FileName = file.FileName,
                ResultData = result,
                CreatedAt = DateTime.UtcNow
            };

            _context.AnalysisResults.Add(analysisResult);
            await _context.SaveChangesAsync();

            return RedirectToAction("Results");
        }*/

        private async Task<string[]> AnalyzeFileAsync(string filePath)// тутй анализ всего
        {


            string[] strings = new string[5];
            var lines = await System.IO.File.ReadAllLinesAsync(filePath);
            strings[0] = $"Строк в файле: {lines.Length}";
            //strings[0] = await AnalisysMethods.Class1.T001(); //тесты (когда будут готовы)
            //strings[1] = await AnalisysMethods.Class1.T002(); //тесты (когда будут готовы)
            //strings[2] = await AnalisysMethods.Class1.T003(); //тесты (когда будут готовы)
            //strings[3] = await AnalisysMethods.Class1.T004(); //тесты (когда будут готовы)
            //strings[4] = await AnalisysMethods.Class1.T005(); //тесты (когда будут готовы)
            //strings[5] = await AnalisysMethods.Class1.T006(); //тесты (когда будут готовы)

            return strings; //TODO: реализовать отображение результата тестов (когда они готовы будут)
        }

        public async Task<IActionResult> Results()
        {
            var results = await _context.AnalysisResults.ToListAsync();
            return View(results);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
