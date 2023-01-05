using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
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

        public CertificateController(ICertificateService certService, PdfCertificateBuilder builder, ApplicationDbContext context, IWatermarkRepository watermarkRepo, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _watermarkRepo = watermarkRepo;
            _dbContext = context;
            _certificateService = certService;
            _pdfBuilder = builder;
        }

        [HttpGet]
        public IActionResult GeneratePDF()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdfDoc = new PdfDocument(writer);
                Document doc = new Document(pdfDoc);

                // Title
                Paragraph title = new Paragraph("RESTRICTED")
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(24)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(30);
                doc.Add(title);

                // Body
                doc.Add(new Paragraph("This certificate is proudly awarded to:")
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.LEFT));

                doc.Add(new Paragraph("RFP Vetting")
                    .SetFontSize(20)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));

                doc.Add(new Paragraph("For outstanding performance and dedication.")
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER));

                // Name and Rank
                doc.Add(new Paragraph("(" + Logins.Offr_Name.Trim() + ")")
                    .SetFontSize(14)
                    .SetMargin(0)
                    .SetMultipliedLeading(0.9f)
                    .SetTextAlignment(TextAlignment.LEFT));

                doc.Add(new Paragraph(Logins.Rank.Trim())
                    .SetFontSize(14)
                    .SetMargin(0)
                    .SetMultipliedLeading(0.9f)
                    .SetTextAlignment(TextAlignment.LEFT));

                // Username with inline green tick
                string tickPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets/images/check-mark.png"); // physical path
                ImageData tickData = ImageDataFactory.Create(tickPath);
                iText.Layout.Element.Image tickImage = new iText.Layout.Element.Image(tickData)
                    .ScaleToFit(20, 20);

                Paragraph userDetails = new Paragraph()
                    .SetFontSize(14)
                    .SetMargin(0)
                    .SetMultipliedLeading(0.9f)
                    .SetTextAlignment(TextAlignment.LEFT);

                userDetails.Add(new Text(Logins.UserName.Trim() + " "));
                userDetails.Add(tickImage); // inline tick

                doc.Add(userDetails);

                doc.Close();

                string base64 = Convert.ToBase64String(ms.ToArray());
                return Json(base64);
            }





        }
        [HttpGet]
        public IActionResult GenerateCertificate(int substage, string ddlaction, string ddlRemarks, int Projid)
        {

            var data = _certificateService.GetCertificateData(Projid, substage);

            string ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            string watermark = $"{ip} {DateTime.Now:dd-MM-yyyy HH:mm:ss}";
            Login login = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            byte[] pdfBytes = _pdfBuilder.BuildCertificate(data, ip, ddlRemarks, watermark, login, substage);
            HttpContext.Session.Set("UnsignedPDF", pdfBytes);

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

            // Optional: store if needed later
            session.Set("SignedPDF", signedPdf);

            // 🔥 RETURN PDF BYTES
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

            // optional: store path in Session for signing
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
