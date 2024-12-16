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
        public async Task<IActionResult> UploadFiles(IFormFile[] files)//�������� ������ � ������
        {
            if (files == null || files.Length == 0)
                return BadRequest("�������� ����� ��� ��������.");
            if (files.Length > 16)
                return BadRequest("��������� ������� ����� ������.");

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            // ���������, ��� ���������� ����������
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var analysisResults = new List<AnalysisResult>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // ���������� ����� �� ������
                    var filePath = Path.Combine(uploadPath, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // ������ �����
                    string[] resultData = new string[5];
                    resultData = await AnalyzeFileAsync(filePath);

                    // �������� ������� ����������
                    var analysisResult = new AnalysisResult
                    {
                        FileName = file.FileName,
                        ResultData = resultData,
                        CreatedAt = DateTime.UtcNow
                    };

                    analysisResults.Add(analysisResult);
                }
            }

            // ���������� ���� ����������� � ��
            _context.AnalysisResults.AddRange(analysisResults);
            await _context.SaveChangesAsync();

            return RedirectToAction("Results");
        }


        /* ��������, ������������� ��� ������. ������� �������� 1 �����
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
            

            // ������ ����� (������: ������ ������� �����)
            var result = await AnalyzeFileAsync(filePath);

            // ���������� ���������� � ��
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

        private async Task<string[]> AnalyzeFileAsync(string filePath)// ���� ������ �����
        {


            string[] strings = new string[5];
            var lines = await System.IO.File.ReadAllLinesAsync(filePath);
            strings[0] = $"����� � �����: {lines.Length}";
            //strings[0] = await AnalisysMethods.Class1.T001(); //����� (����� ����� ������)
            //strings[1] = await AnalisysMethods.Class1.T002(); //����� (����� ����� ������)
            //strings[2] = await AnalisysMethods.Class1.T003(); //����� (����� ����� ������)
            //strings[3] = await AnalisysMethods.Class1.T004(); //����� (����� ����� ������)
            //strings[4] = await AnalisysMethods.Class1.T005(); //����� (����� ����� ������)
            //strings[5] = await AnalisysMethods.Class1.T006(); //����� (����� ����� ������)

            return strings; //TODO: ����������� ����������� ���������� ������ (����� ��� ������ �����)
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
