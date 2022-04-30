using System.Configuration;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.DAL;
using swas.UI.Models;

namespace swas.UI.Controllers
{
    public class CertificateController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IWatermarkRepository _watermarkRepo;
        private readonly ApplicationDbContext _dbContext;
        private readonly ICertificateService _certificateService;
        private readonly PdfCertificateBuilder _pdfBuilder;
        private readonly IConfiguration _configuration;

        public CertificateController(ICertificateService certService, PdfCertificateBuilder builder, IConfiguration configuration, ApplicationDbContext context, IWatermarkRepository watermarkRepo, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _watermarkRepo = watermarkRepo;
            _dbContext = context;
            _certificateService = certService;
            _pdfBuilder = builder;
            _configuration = configuration;
        }

      
        [HttpGet]
        public IActionResult GenerateCertificate(string encryptdata)
        {



            if (string.IsNullOrWhiteSpace(encryptdata))
            {
                return BadRequest(new { success = false, message = "Invalid request." });
            }

            int projid = 0;
         
            int substage = 0;
            var ddlaction = "";
            var ddlRemarks = "";
            string cryptoKey = _configuration["CryptoSettings:LoginKey"] ?? string.Empty;

            try
            {
                // Decrypt
                string decryptedJson = CryptoHelper.SafeDecrypt(encryptdata, cryptoKey);
                if (string.IsNullOrWhiteSpace(decryptedJson))
                {
                 
                    return BadRequest(new { success = false, message = "Invalid request data." });
                }

                // Parse JSON
                var obj = JsonConvert.DeserializeObject<dynamic>(decryptedJson.Trim('"'));
               ddlaction = obj.ddlaction;
               ddlRemarks = obj.ddlRemarks;
                if (obj == null ||
                    !int.TryParse((string?)obj.substage, out substage) ||
                    !int.TryParse((string?)obj.Projid, out projid ))
                {
                 
                    return BadRequest(new { success = false, message = "Invalid identifier." });
                }
            }
            catch (Exception ex)
            {
 
                return StatusCode(500, new { success = false, message = "Error processing request." });
            }

            var data = _certificateService.GetCertificateData(projid, substage);

            string ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            string watermark = $"{ip} {DateTime.Now:dd-MM-yyyy HH:mm:ss}";
            Login login = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            byte[] pdfBytes = _pdfBuilder.BuildCertificate(data, ip, ddlRemarks, watermark, login, substage);
            HttpContext.Session.Set("UnsignedPDF", pdfBytes);
            //return File(pdfBytes, "application/pdf");
            return Content(Convert.ToBase64String(pdfBytes), "text/plain");
        }



        [HttpPost]
        public IActionResult SignCertificate()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
                return BadRequest("Session not available");

            Login? login = SessionHelper.GetObjectFromJson<Login>(session, "User");
            if (login == null)
                return BadRequest("User not found");

            byte[]? unsignedPdf = session.Get("UnsignedPDF");
            if (unsignedPdf == null)
                return BadRequest();


            byte[] signedPdf = _pdfBuilder.SignPdf(unsignedPdf, login);
            session.Set("SignedPDF", signedPdf);
            return File(signedPdf, "application/pdf");
        }

        [HttpPost]
        public IActionResult SaveTempPdf(IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
                return BadRequest("No file");

            string tempFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "temp"
            );

            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            string fileName = $"{Guid.NewGuid()}.pdf";
            string fullPath = Path.Combine(tempFolder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                pdfFile.CopyTo(stream);
            }
            HttpContext.Session.SetString("TempPdfPath", fullPath);
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            string pdfUrl = $"{baseUrl}/temp/{fileName}";

            return Ok(new
            {
                tempPath = pdfUrl
            });

        }
        public IActionResult ViewSignedCertificate()
        {
            byte[] signedPdf = HttpContext.Session.Get("SignedPDF");
            return File(signedPdf, "application/pdf");
        }

    }

}
