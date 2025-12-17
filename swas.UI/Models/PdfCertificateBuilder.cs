using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.UI.Helpers;
using swas.BAL.Helpers;
using Microsoft.AspNetCore.Hosting;
using Org.BouncyCastle.Utilities;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Extgstate;

namespace swas.UI.Models
{
    public class PdfCertificateBuilder
    {
        private readonly IWatermarkRepository _watermarkRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PdfCertificateBuilder(IWatermarkRepository watermarkRepo, IWebHostEnvironment webHostEnvironment)
        {
            _watermarkRepo = watermarkRepo;
            _webHostEnvironment = webHostEnvironment;

        }
        public byte[] BuildCertificate(CertificateDataDTO data, string ip, string remarks, string watermark, Login logins)
        {
            using (var ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);

                document.SetMargins(25, 25, 25, 25);

                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                document.SetFont(font);

               

                // ---------------- HEADER ----------------
                Table header = new Table(new float[] { 80, 20 }).UseAllAvailableWidth();
                DeviceRgb headerColor = new DeviceRgb(230, 230, 230);

                ImageData img = ImageDataFactory.Create("wwwroot/assets/images/CertifiedCertificate.png");
                header.AddCell(new Cell()
                    .Add(new Image(img).SetWidth(95).SetHeight(85))
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                    .SetBackgroundColor(headerColor)
                    .SetPadding(4));

                Paragraph heading = new Paragraph(data.CertificateName + " Certificate")
                    .SetFontSize(25)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetMargin(0);

                header.AddCell(new Cell()
                    .Add(heading)
                    .SetBorder(Border.NO_BORDER)
                    .SetBackgroundColor(headerColor)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(6));

                document.Add(header);
                document.Add(new Paragraph("").SetMargin(4));
                document.Add(new LineSeparator(new SolidLine(0.7f)));
                document.Add(new Paragraph("").SetMargin(4));

                // ---------------- INTRO ----------------
                document.Add(new Paragraph(
                    $"It is certified that {data.CertificateName} has been granted for ibid project. Details are as under:")
                    .SetFontSize(12)
                    .SetBold()
                    .SetMarginBottom(10));

                // ---------------- TABLE ----------------
                Table table = new Table(new float[] { 35, 5, 60 })
                    .UseAllAvailableWidth()
                    //.SetBackgroundColor(new DeviceRgb(248, 248, 248))
                    .SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 0.8f))
                    .SetMarginBottom(15);

                AddRow(table, "Project Name", data.ProjName);
                AddRow(table, "Approved Date (PAW)", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                AddRow(table, "Approved Remarks", remarks);

                // Remarks Title (full width)
                table.AddCell(new Cell(1, 3)
                    .Add(new Paragraph("Remarks→")
                    .SetFontSize(11)
                    .SetBold())
                    .SetBorder(Border.NO_BORDER)
                    .SetPadding(4));

                table.AddCell(new Cell(1, 3)
                    .Add(new Paragraph(""))
                    .SetBorder(Border.NO_BORDER));

                // Additional data rows
                AddRow(table, "1. Scope of Appl Audit",
                    "The Scope of Cyber Security audit was to perform Vulnerability Assessment and Penetration Testing (VAPT) to identify vulnerable area/gaps in the website/application platform.");

                AddRow(table, "2. Deploy Scenario", data.HostType);
                AddRow(table, "3. Security Cl of data", "Restricted");

                AddRow(table, "4. Validity of Clearance",
                    "The validity of the Cyber Security Audit will lapse in case of any changes in structure/source code or deployment scenario or three years from the issue date.");

                //AddRow(table, "Cert Generation Date", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));

                document.Add(table);

                //float tickX = 25; // from left
                //float tickY = 140; // from bottom (you may need to tweak)
                AddOfficerInfoOverlayTick(document, pdf, logins);
                //         AddVerifiedTickBehindName(pdf, tickX, tickY);

                //         // ---------------- FOOTER ----------------
                //         document.Add(new Paragraph("(" + logins.Offr_Name.Trim() + ")")
                //.SetFontSize(14).SetTextAlignment(TextAlignment.LEFT));
                //         document.Add(new Paragraph(logins.Rank.Trim())
                //             .SetFontSize(14).SetTextAlignment(TextAlignment.LEFT));
                //         document.Add(new Paragraph(logins.UserName.Trim())
                //             .SetFontSize(14).SetTextAlignment(TextAlignment.LEFT));



                document.Add(new Paragraph("This certificate is auto-generated and does not require Ink signature.")
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetOpacity(0.7f)
                    .SetMarginBottom(10));

                document.Add(new LineSeparator(new SolidLine(0.5f)));

                string user = Helper.LoginDetails(logins);
                document.Add(new Paragraph($"Generated by [{user}] | IP: {ip}")
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetOpacity(0.6f));


                // -------------------------------------------------------
                // 1️⃣ NOW pages exist → SAFE to add shield background
                // -------------------------------------------------------
                AddBackgroundShield(pdf);

                // -------------------------------------------------------
                // 2️⃣ Add text watermark on top
                // -------------------------------------------------------

                // -------------------------------------------------------
                // ADD TEXT WATERMARK ON TOP OF PAGE CONTENT
                // -------------------------------------------------------
                //_watermarkRepo.AddWatermark(pdf, watermark);

                document.Close();
                return ms.ToArray();
            }
        }

        private void AddRow(Table table, string label, string value)
        {
            table.AddCell(CreateCell(label, true).SetPadding(4).SetWidth(150));
            table.AddCell(CreateCell(":", true).SetPadding(4));
            table.AddCell(CreateCell(value ?? "N/A", false).SetPadding(4));
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

                float imgWidth = pageSize.GetWidth() * 0.95f;
                float imgHeight = pageSize.GetHeight() * 0.55f;

                float x = (pageSize.GetWidth() - imgWidth) / 2;
                float y = (pageSize.GetHeight() - imgHeight) / 2;

                PdfCanvas canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdf);
                Canvas pdfCanvas = new Canvas(canvas, page.GetPageSize());

                Image img = new Image(wmImage)
                    .ScaleToFit(imgWidth, imgHeight)
                    .SetFixedPosition(x, y)
                    .SetOpacity(0.4f); // faint watermark

                pdfCanvas.Add(img);
            }
        }


        private void AddOfficerInfoOverlayTick(Document document, PdfDocument pdf, Login logins, string tickImagePath = "wwwroot/assets/images/verified_tick.png")
        {
            PdfPage page = pdf.GetPage(1);

            // 1️⃣ Draw tick as background
            PdfCanvas canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdf);

            ImageData tickData = ImageDataFactory.Create(tickImagePath);
            Image tick = new Image(tickData)
                .ScaleToFit(100, 100)        // adjust size as needed
                .SetFixedPosition(50, 100);  // X,Y from bottom-left, adjust for placement
              /*  .SetOpacity(0.2f);    */       // faint, watermark style

            new Canvas(canvas, page.GetPageSize()).Add(tick);

            // 2️⃣ Overlay officer info on top of tick
            float textX = 60;  // adjust horizontal position relative to tick
            float textY = 150; // starting vertical position
            document.Add(new Paragraph("(" + logins.Offr_Name.Trim() + ")")
              .SetFontSize(14)
              .SetFixedPosition(textX, textY - 20, 200));
            document.Add(new Paragraph(logins.Rank.Trim())
                .SetFontSize(14)
                .SetFixedPosition(textX, textY, 200)); // width of text area

          

            document.Add(new Paragraph(logins.UserName.Trim())
                .SetFontSize(14)
                .SetFixedPosition(textX, textY - 40, 200));
        }



    }
}
