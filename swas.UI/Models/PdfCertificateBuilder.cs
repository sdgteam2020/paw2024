using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.UI.Helpers;
using iText.Kernel.Geom;
using swas.DAL;
using System.Reflection;

namespace swas.UI.Models
{
    public class PdfCertificateBuilder
    {
        private readonly IWatermarkRepository _watermarkRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext context;

        public PdfCertificateBuilder(IWatermarkRepository watermarkRepo, IWebHostEnvironment webHostEnvironment,ApplicationDbContext context)
        {
            _watermarkRepo = watermarkRepo;
            _webHostEnvironment = webHostEnvironment;
            this.context = context;
        }

        #region
        public byte[] BuildCertificate(
    CertificateDataDTO data,
    string ip,
    string remarks,
    string watermark,
    Login logins,
    int substage)
        {
            using (var ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf, PageSize.A4);
                document.SetMargins(25, 25, 25, 25);

                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                document.SetFont(font);
                Table header = new Table(new float[] { 1, 4, 2 }).UseAllAvailableWidth();

                DeviceRgb headerColor = new DeviceRgb(230, 230, 230);
                ImageData img = ImageDataFactory.Create("wwwroot/assets/images/CertifiedSheild2.png");
                Image logo = new Image(img).SetHeight(55).SetAutoScaleWidth(true); // Auto scale width
                header.AddCell(new Cell()
                    .Add(logo)
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                    .SetTextAlignment(TextAlignment.CENTER));

                header.AddCell(new Cell()
                    .Add(new Paragraph($"{data.CertificateName ?? string.Empty} Certificate")
                        .SetFontSize(25)
                        .SetBold())
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                    .SetTextAlignment(TextAlignment.CENTER));

                header.AddCell(new Cell().SetWidth(90).SetBorder(Border.NO_BORDER));

                document.Add(header);
                document.Add(new LineSeparator(new SolidLine(0.7f)));
                document.Add(new Paragraph(
                    $"It is certified that {data.CertificateName ?? string.Empty} has been approved for ibid project. Details are as under:")
                    .SetFontSize(12)
                    .SetBold()
                    .SetMarginBottom(10));
                Table table = new Table(new float[] { 35, 5, 60 })
                    .UseAllAvailableWidth()
                    .SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 0.8f))
                    .SetMarginBottom(15);

                AddRow(table, "Project Name", data.ProjName ?? "N/A");
                AddRow(table, "Certificate Generated Date", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                AddRow(table, "Approver Remarks", remarks ?? "N/A");
                AddRow(table, "Deploy Scenario", data.HostType ?? "N/A");
                AddRow(table, "Security Cl of data", data.Security_Classification??"");
                AddRow(table, "Details of Sponsor", data.Sponsor);
               

                if (substage==25)
                {
                    substage = 27;
                }
                var contents = context.tbl_mCertificateContent
                    .Where(x=>x.SubStage == substage
                             && x.IsActive)
                    .OrderBy(x => x.DisplayOrder )
                    .ToList();

                var mainContents = contents
                    .Where(x => x.ContentId != 7 && x.ContentId != 8)
                    .ToList();
                if(substage !=28 && substage !=26)
                {
                    table.AddCell(new Cell(1, 3)
                   .Add(new Paragraph("Remarks from Issuing Authority")
                       .SetFontSize(11)
                       .SetBold())
                   .SetPadding(6)
                    .SetBorder(Border.NO_BORDER));
                }
               
                AddRow(table, " Issuing/Authority", logins.Unit ?? "N/A");

                foreach (var item in mainContents)
                {
                    if (item.ContentType == "Header" && item.ContentId == 3)
                        continue;
                    string text = item.ContentText?
                        .Replace("{ProjName}", data.ProjName)
                       
                        .Replace("{RemoteTestNext3Years}",
                            data.RemoteTestNext3Years != DateTime.MinValue
                                ? data.RemoteTestNext3Years.Value.ToString("dd-MM-yyyy")
                                : "N/A");

                    switch (item.ContentType)
                    {
                        case "TableRow":
                            AddRow(table, item.ContentTitle ?? "", text ?? "");

                            break;

                        case "Header":
                            table.AddCell(new Cell(1, 3)
                                .Add(new Paragraph(item.ContentTitle)
                                .SetBold()
                                .SetFontSize(11))
                                .SetBorder(Border.NO_BORDER));
                            break;

                        case "Paragraph":
                            document.Add(new Paragraph(text)
                                .SetFontSize(12)
                                .SetMarginBottom(10));
                            break;
                    }
                }

                var stdRemarkLabel = contents.FirstOrDefault(x => x.ContentId == 3);
                var stdRemarkText = contents.FirstOrDefault(x => x.ContentId == 7);


                var archrevmarks = contents.FirstOrDefault(x => x.ContentId == 8);

                if (stdRemarkLabel != null || stdRemarkText != null)
                {
                    if (stdRemarkText != null)
                    {
                        string footerText = stdRemarkText.ContentText!
                            .Replace("{ProjName}", data.ProjName);

                        table.AddCell(new Cell(1, 3)
                            .Add(new Paragraph(footerText)
                                .SetFontSize(11))
                            .SetPadding(8)
                            .SetBorder(Border.NO_BORDER));
                    }
                }
                if (archrevmarks != null)
                {

                    string footerText = archrevmarks.ContentText!
                        .Replace("{ProjName}", data.ProjName);

                    table.AddCell(new Cell(1, 3)
                        .Add(new Paragraph(footerText)
                            .SetFontSize(11))
                        .SetPadding(8)
                        .SetBorder(Border.NO_BORDER));

                }



                document.Add(table);
                document.Add(new Paragraph("This certificate is auto-generated and does not require Ink signature.")
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetOpacity(0.7f));

                document.Add(new LineSeparator(new SolidLine(0.5f)));

                string user = Helper.LoginDetails(logins);
                document.Add(new Paragraph($"Generated by [{user}] | IP: {ip}")
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetOpacity(0.6f));
                //AddBackgroundShield(pdf);

                document.Close();
                return ms.ToArray();
            }
        }

        


        public byte[] SignPdf(byte[] pdfBytes, Login logins)
		{
			using var ms = new MemoryStream();
			using var reader = new PdfReader(new MemoryStream(pdfBytes));
			using var writer = new PdfWriter(ms);

			PdfDocument pdf = new PdfDocument(reader, writer);
			Document document = new Document(pdf);

			AddOfficerInfoOverlayTick(document, pdf, logins);

			document.Close();
			return ms.ToArray();
		}

        private void AddRow(Table table, string label, string value)
        {
            table.AddCell(CreateCell(label, true).SetPadding(4).SetWidth(150));  // Label cell width fixed
            table.AddCell(CreateCell(":", true).SetPadding(4).SetWidth(15));     // Colon cell narrow width
            table.AddCell(CreateCell(value ?? "N/A", false).SetPadding(4));      // Value cell fills rest
        }


        private Cell CreateCell(string text, bool bold)
        {
            Paragraph p = new Paragraph(text)
                .SetFontSize(11)  // reduced font size
                .SetTextAlignment(TextAlignment.LEFT)
                .SetMargin(0);

            if (bold) p.SetBold();

            return new Cell()
                .Add(p)
                .SetBorder(Border.NO_BORDER);
        }

        private void AddBackgroundShield(PdfDocument pdf)
        {
            string wmPath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "assets/images/CertifiedSheild2.png");
            ImageData wmImage = ImageDataFactory.Create(wmPath);

            int pageCount = pdf.GetNumberOfPages();

            for (int pageNum = 1; pageNum <= pageCount; pageNum++)
            {
                PdfPage page = pdf.GetPage(pageNum);
                Rectangle pageSize = page.GetPageSize();

                float imgMaxWidth = pageSize.GetWidth() * 0.95f;
                float imgMaxHeight = pageSize.GetHeight() * 0.55f;

                PdfCanvas canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdf);
                Canvas pdfCanvas = new Canvas(canvas, page.GetPageSize());

                Image img = new Image(wmImage)
                    .ScaleToFit(imgMaxWidth, imgMaxHeight)
                    .SetOpacity(0.4f);
                float actualWidth = img.GetImageScaledWidth();
                float actualHeight = img.GetImageScaledHeight();
                float x = (pageSize.GetWidth() - actualWidth) / 2;
                float y = (pageSize.GetHeight() - actualHeight) / 2;

                img.SetFixedPosition(x, y);

                pdfCanvas.Add(img);
            }
        }



        private void AddOfficerInfoOverlayTick(Document document, PdfDocument pdf, Login logins, string tickImagePath = "wwwroot/assets/images/verified_tick.png")
        {
            PdfPage page = pdf.GetPage(1);
            PdfCanvas canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdf);

            // Get the page height to position near top
            float pageHeight = page.GetPageSize().GetHeight();

            // Y coordinate near the top of the page (adjust as needed)
            float headerY = pageHeight - 70;

            // Load and scale tick image
            ImageData tickData = ImageDataFactory.Create(tickImagePath);
            Image tick = new Image(tickData)
                .ScaleToFit(70, 70)                  // Adjust size to avoid overlap
                .SetFixedPosition(490, headerY - 40); // Position tick near top-left (adjust X, Y)

            // Add tick image to the canvas
            new Canvas(canvas, page.GetPageSize()).Add(tick);

            // Position officer info text just right of the tick image with some padding (say 10 pts)
            float textX = 415 + 50 + 10;  // 550
            float textY = headerY +15+10;

            document.Add(new Paragraph("(" + logins.Offr_Name.Trim() + ")")
                .SetFontSize(10)
                .SetMargin(0)
                .SetFixedPosition(textX, textY,150));

            document.Add(new Paragraph(logins.Rank.Trim())
                .SetFontSize(10)
                .SetMargin(0)
                .SetFixedPosition(textX, textY - 20, 150));

            document.Add(new Paragraph(logins.Appontment.Trim())
                .SetFontSize(10)
                .SetMargin(0)
                .SetFixedPosition(textX, textY - 40, 150));

            document.Add(new Paragraph(logins.UserName.Trim())
                .SetFontSize(10)
                .SetMargin(0)
                .SetFixedPosition(textX, textY - 60, 100));
        }

    }
}
#endregion