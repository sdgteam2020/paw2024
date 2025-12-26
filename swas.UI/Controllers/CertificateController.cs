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

			return Ok(new
			{
				tempPath = fullPath
			});
		}
		public IActionResult ViewSignedCertificate()
		{
			byte[] signedPdf = HttpContext.Session.Get("SignedPDF");
			return File(signedPdf, "application/pdf");
		}
		//   [HttpGet]
		//   public IActionResult GenerateCertificate(int substage, string ddlaction, string ddlRemarks, int Projid)
		//   {
		//       try
		//       {



		//           var query =
		//     (from a in _dbContext.Projects
		//      join b in _dbContext.mHostType
		//          on a.HostTypeID equals b.HostTypeID into hostGrp
		//      from b in hostGrp.DefaultIfEmpty()
		//      where a.ProjId == Projid
		//      select new
		//      {
		//          Project = a,
		//          HostTypeName = b != null ? b.HostingDesc : "N/A"
		//      }).FirstOrDefault();


		//           using (var ms = new MemoryStream())
		//           {
		//               PdfWriter writer = new PdfWriter(ms);
		//               PdfDocument pdf = new PdfDocument(writer);
		//               Document document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);
		//               document.SetMargins(40, 35, 40, 35);

		//               // PREPARE WATERMARK FIRST
		//               string ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
		//               string watermark = $"{ip}   {DateTime.Now:dd-MM-yyyy HH:mm:ss}";
		//               //_watermarkRepo.AddWatermark(pdf, watermark);

		//               // Default font
		//               PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
		//               document.SetFont(font);

		//               // PREMIUM HEADER BAR
		//               Table header = new Table(new float[] { 85, 15 }).UseAllAvailableWidth();

		//               DeviceRgb headerColor = new DeviceRgb(230, 230, 230);

		//               ImageData img = ImageDataFactory.Create("wwwroot/assets/images/Certificate1.png");
		//               header.AddCell(new Cell()
		//    .Add(new Image(img)
		//        .SetWidth(80)
		//        .SetHeight(80)
		//        .SetRotationAngle(-12 * Math.PI / 180)  // rotate 15° left
		//    )
		//    .SetBorder(Border.NO_BORDER)
		//    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
		//    .SetBackgroundColor(headerColor));



		//               var CertName = _dbContext.tbl_mCertificate.Where(x => x.Statusid == substage).FirstOrDefault();

		//               Paragraph heading = new Paragraph(CertName?.CertificateName + " Certificate")
		//.SetFontSize(24)                     // Bigger font for main heading
		//.SetBold()
		//.SetVerticalAlignment(VerticalAlignment.MIDDLE)
		//.SetFontColor(ColorConstants.BLACK)
		//.SetTextAlignment(TextAlignment.CENTER)
		//.SetMarginBottom(4);                 // Space before subtitle

		//               //// Subtitle: Project Name
		//               //Paragraph subtitle = new Paragraph("ProjectName")
		//               //    .SetFontSize(18)                     // Slightly smaller for subtitle
		//               //    .SetFontColor(ColorConstants.DARK_GRAY)
		//               //    .SetTextAlignment(TextAlignment.LEFT);                        // Optional styling

		//               // Add to the header cell
		//               header.AddCell(
		//                   new Cell()
		//                       .Add(heading)
		//                       //.Add(subtitle)
		//                       .SetBorder(Border.NO_BORDER)
		//                       .SetBackgroundColor(headerColor)
		//                       .SetVerticalAlignment(VerticalAlignment.MIDDLE)
		//                       .SetTextAlignment(TextAlignment.CENTER)
		//                       .SetPaddingTop(8)
		//                       .SetPaddingBottom(8)

		//               );

		//               document.Add(header);

		//               // Soft separator
		//               document.Add(new Paragraph("\n"));
		//               document.Add(new LineSeparator(new SolidLine(1f)).SetStrokeColor(ColorConstants.DARK_GRAY));
		//               document.Add(new Paragraph("\n"));

		//               // Intro text
		//               document.Add(new Paragraph("It is Certified that IPA has been granted by Steering Tech Committee for ibid project. Details are as under:")
		//                   .SetFontSize(14)
		//                   .SetTextAlignment(TextAlignment.LEFT)
		//                   .SetBold()
		//                   .SetMarginBottom(20));

		//               // TABLE SECTION — PREMIUM LOOK
		//               DeviceRgb bg = new DeviceRgb(245, 245, 245);

		//               // Use 3 columns because each row contains: Label | Colon | Value
		//               Table table = new Table(new float[] { 40, 5, 55 })
		//                   .UseAllAvailableWidth()
		//                   .SetPadding(10)
		//                   .SetBackgroundColor(bg)
		//                   .SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1.0f))
		//                   .SetMarginBottom(30);

		//               // Project Name


		//               table.AddCell(CreateCell("Project Name", true).SetWidth(160));
		//               table.AddCell(CreateCell(":", true));
		//               table.AddCell(CreateCell("ProjectName", true));

		//               // Approved Date
		//               table.AddCell(CreateCell("Approved Date (PAW)", true).SetWidth(160));
		//               table.AddCell(CreateCell(":", true));
		//               //table.AddCell(CreateCell(approvedDate.ToString("dd-MM-yyyy HH:mm:ss"), true));
		//               table.AddCell(CreateCell(DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), true));

		//               // Remarks
		//               table.AddCell(CreateCell("Approved Remarks", true).SetWidth(160));
		//               table.AddCell(CreateCell(":", true));
		//               table.AddCell(CreateCell(ddlRemarks, true));

		//               table.AddCell(CreateCell("Additional Remarks-->", true).SetWidth(160));
		//               table.AddCell(CreateCell("", true));
		//               table.AddCell(CreateCell("", true));




		//               table.AddCell(CreateCell("1. Scope of Appl Audit", true).SetWidth(160));
		//               table.AddCell(CreateCell(":", true));
		//               table.AddCell(CreateCell("The scope of Cyber Securtiy audit was to perform Vulnerability Assessment and Penetration Testing (VAPT) to identify aspects of the web apl do not come under the purview of the Cyber Security audit process and the same is to be ensured by the sponsor.", true));


		//               table.AddCell(CreateCell("2. Dely SCenario", true).SetWidth(160));
		//               table.AddCell(CreateCell(":", true));
		//               table.AddCell(CreateCell(query.HostTypeName, true));


		//               table.AddCell(CreateCell("3. Security Cl of data to be handled by Web appl.", true).SetWidth(160));
		//               table.AddCell(CreateCell(":", true));
		//               table.AddCell(CreateCell("Restricted", true));

		//               table.AddCell(CreateCell("4. Validity of Clearance", true).SetWidth(160));
		//               table.AddCell(CreateCell(":", true));
		//               table.AddCell(CreateCell("The validity of the Cyber Security Audit will lapse in case if abt of any changes in structure/source code or dply scenario (less contents) of the website or three yrs from the date of issue of this letter. However, continuous upgradation of the library dependencies must be ensured.", true));


		//               // Date of Generation
		//               table.AddCell(CreateCell("Date of Cert Generation", true).SetWidth(160));
		//               table.AddCell(CreateCell(":", true));
		//               table.AddCell(CreateCell(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), true));

		//               table.AddCell(CreateCell(".........is hearby accordance/Clearance", true).SetWidth(250));



		//               // Add to document
		//               document.Add(table);
		//               // Note
		//               document.Add(new Paragraph("This certificate is auto-generated based on electronic data and does not require an ink signature.")
		//                   .SetFontSize(10)
		//                   .SetTextAlignment(TextAlignment.CENTER)
		//                   .SetOpacity(0.7f)
		//                   .SetMarginBottom(20));

		//               // Footer Line
		//               document.Add(new LineSeparator(new SolidLine(0.5f)).SetStrokeColor(ColorConstants.GRAY));

		//               Login login = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
		//               var user = Helper.LoginDetails(login);

		//               document.Add(new Paragraph($"Generated by [{user}]  |  IP: {ip}")
		//                   .SetFontSize(10)
		//                   .SetTextAlignment(TextAlignment.CENTER)
		//                   .SetOpacity(0.6f)
		//               );


		//               _watermarkRepo.AddWatermark(pdf, watermark);
		//               document.Close();



		//               string base64 = Convert.ToBase64String(ms.ToArray());
		//               return Json(base64);
		//               // Return PDF to open in a new tab
		//               //byte[] fileBytes = ms.ToArray();
		//               //Response.Headers["Content-Disposition"] = "inline; filename=Certificate.pdf";
		//               //Response.Headers["Content-Type"] = "application/pdf";
		//               //return File(fileBytes, "application/pdf");
		//           }
		//       }
		//       catch (Exception ex)
		//       {
		//           // Log the exception (use your preferred logging mechanism)
		//           return StatusCode(500, $"An error occurred while generating the PDF: {ex.Message}");
		//       }
		//   }

		//   // Helper method to create table cells
		//   private Cell CreateCell(string text, bool isBold)
		//   {
		//       Paragraph p = new Paragraph(text)
		//           .SetFontSize(12)
		//           .SetTextAlignment(TextAlignment.LEFT);
		//       if (isBold)
		//       {
		//           p.SetBold();
		//       }
		//       return new Cell()
		//           .Add(p)
		//           .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
		//           .SetPadding(6);
		//   }
	}

}
