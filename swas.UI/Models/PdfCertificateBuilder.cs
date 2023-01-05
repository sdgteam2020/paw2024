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

        //public byte[] BuildCertificate(CertificateDataDTO data, string ip, string remarks, string watermark, Login logins, int substage)
        //{
        //	using (var ms = new MemoryStream())
        //	{
        //		// PDF Setup
        //		PdfWriter writer = new PdfWriter(ms);
        //		PdfDocument pdf = new PdfDocument(writer);
        //		Document document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);
        //		document.SetMargins(25, 25, 25, 25);

        //		PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        //		document.SetFont(font);

        //		// ---------------- HEADER ----------------
        //		Table header = new Table(new float[] { 80, 20 }).UseAllAvailableWidth();
        //		DeviceRgb headerColor = new DeviceRgb(230, 230, 230);

        //		ImageData img = ImageDataFactory.Create("wwwroot/assets/images/CertifiedCertificate.png");
        //		header.AddCell(new Cell()
        //			.Add(new Image(img).SetWidth(95).SetHeight(85))
        //			.SetBorder(Border.NO_BORDER)
        //			.SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //			.SetBackgroundColor(headerColor)
        //			.SetPadding(4));

        //		Paragraph heading = new Paragraph($"{data.CertificateName} Certificate")
        //			.SetFontSize(25)
        //			.SetBold()
        //			.SetTextAlignment(TextAlignment.LEFT)
        //			.SetMargin(0);

        //		header.AddCell(new Cell()
        //			.Add(heading)
        //			.SetBorder(Border.NO_BORDER)
        //			.SetBackgroundColor(headerColor)
        //			.SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //			.SetTextAlignment(TextAlignment.CENTER)
        //			.SetPadding(6));

        //		document.Add(header);
        //		document.Add(new Paragraph("").SetMargin(4));
        //		document.Add(new LineSeparator(new SolidLine(0.7f)));
        //		document.Add(new Paragraph("").SetMargin(4));

        //		// ---------------- INTRO ----------------
        //		document.Add(new Paragraph(
        //			$"It is certified that {data.CertificateName} has been Approved for ibid project. Details are as under:")
        //			.SetFontSize(12)
        //			.SetBold()
        //			.SetMarginBottom(10));

        //		// ---------------- TABLE ----------------
        //		Table table = new Table(new float[] { 35, 5, 60 })
        //			.UseAllAvailableWidth()
        //			.SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 0.8f))
        //			.SetMarginBottom(15);

        //		AddRow(table, "Project Name", data.ProjName);
        //		AddRow(table, "Approved Date (PAW)", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
        //		AddRow(table, "Approved Remarks", remarks);

        //		// Remarks Title (full width)
        //		//table.AddCell(new Cell(1, 3)
        //		//	.Add(new Paragraph("Remarks→").SetFontSize(11).SetBold())
        //		//	.SetBorder(Border.NO_BORDER)
        //		//	.SetPadding(4));
        //		//table.AddCell(new Cell(1, 3)
        //		//	.Add(new Paragraph(""))
        //		//	.SetBorder(Border.NO_BORDER));

        //		// ---------------- CONDITIONAL ROWS BASED ON SUBSTAGE ----------------
        //		if (substage == 27)
        //		{
        //			AddRow(table, "Scope of Appl Audit",
        //				"The Scope of Cyber Security audit was to perform Vulnerability Assessment and Penetration Testing (VAPT) to identify vulnerable area/gaps in the website/application platform.");

        //			AddRow(table, "Validity of Clearance",
        //				"The validity of the Cyber Security Audit will lapse in case of any changes in structure/source code or deployment scenario or three years from the issue date.");
        //		}
        //		else if (substage == 24)
        //		{
        //			AddRow(table, "Arch Guidline",
        //				"Layout And architecture vetting of ibid website is hereby accorded as per GIGW guidelines.");
        //		}
        //		else if (substage == 29)
        //		{


        //		table.AddCell(new Cell(1, 3)
        //			.Add(new Paragraph("Std Remarks from Issuing Authority").SetFontSize(11).SetBold())
        //			.SetBorder(Border.NO_BORDER)
        //			.SetPadding(4));
        //			table.AddCell(new Cell(1, 3)
        //				.Add(new Paragraph(""))
        //				.SetBorder(Border.NO_BORDER));


        //			document.Add(new Paragraph(
        //				"The Software appl '" + data.ProjName + "' is hereby approved as whitelisted for use in IA as per fwg details:-")
        //				.SetFontSize(12)
        //				.SetMarginBottom(10));

        //			AddRow(table, "Details of Sponsor", data.Sponsor);

        //			// Ensure RemoteTestNext3Years exists
        //			string validity = data.RemoteTestNext3Years != DateTime.MinValue
        //				? data.RemoteTestNext3Years.ToString()
        //				: "N/A";

        //			AddRow(table, "Validity of Cert", validity);





        //		}

        //		AddRow(table, "Deploy Scenario", data.HostType);
        //		AddRow(table, "Security Cl of data", "Restricted");
        //		AddRow(table, "Cert Generation Date", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));

        //		if (substage == 29)
        //		{


        //			// Remarks Title (full width)
        //			table.AddCell(new Cell(1, 3)
        //			.Add(new Paragraph("The above software will be used 'As-is' basis only. In case of further customisation by user, all applicable vetting/clearances from all stake holders as per SOP on the subject must be obtained again prior to dply of the customised version of the appl/sw.").SetFontSize(11))
        //			.SetBorder(Border.NO_BORDER));
        //			table.AddCell(new Cell(1, 3)
        //				.Add(new Paragraph(""))
        //				.SetBorder(Border.NO_BORDER));
        //		}

        //		document.Add(table);

        //		// ---------------- OFFICER INFO & VERIFIED TICK ----------------
        //		//AddOfficerInfoOverlayTick(document, pdf, logins);

        //		// ---------------- FOOTER ----------------
        //		document.Add(new Paragraph("This certificate is auto-generated and does not require Ink signature.")
        //			.SetFontSize(9)
        //			.SetTextAlignment(TextAlignment.CENTER)
        //			.SetOpacity(0.7f)
        //			.SetMarginBottom(10));

        //		document.Add(new LineSeparator(new SolidLine(0.5f)));

        //		string user = Helper.LoginDetails(logins);
        //		document.Add(new Paragraph($"Generated by [{user}] | IP: {ip}")
        //			.SetFontSize(9)
        //			.SetTextAlignment(TextAlignment.CENTER)
        //			.SetOpacity(0.6f));

        //		// ---------------- ADD BACKGROUND SHIELD ----------------
        //		AddBackgroundShield(pdf);

        //		// ---------------- ADD TEXT WATERMARK ----------------
        //		//_watermarkRepo.AddWatermark(pdf, watermark); // Uncomment if needed

        //		document.Close();
        //		return ms.ToArray();
        //	}
        //}

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
                // ---------------- PDF SETUP ----------------
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf, PageSize.A4);
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
                    .SetBackgroundColor(headerColor)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE));

                header.AddCell(new Cell()
                    .Add(new Paragraph($"{data.CertificateName ?? string.Empty} Certificate")
                        .SetFontSize(25)
                        .SetBold())
                    .SetBorder(Border.NO_BORDER)
                    .SetBackgroundColor(headerColor)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE));

                document.Add(header);
                document.Add(new LineSeparator(new SolidLine(0.7f)));

                // ---------------- INTRO ----------------
                document.Add(new Paragraph(
                    $"It is certified that {data.CertificateName ?? string.Empty} has been approved for ibid project. Details are as under:")
                    .SetFontSize(12)
                    .SetBold()
                    .SetMarginBottom(10));

                // ---------------- BASE TABLE ----------------
                Table table = new Table(new float[] { 35, 5, 60 })
                    .UseAllAvailableWidth()
                    .SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 0.8f))
                    .SetMarginBottom(15);

                AddRow(table, "Project Name", data.ProjName ?? "N/A");
                AddRow(table, "Approved Date (PAW)", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                AddRow(table, "Approved Remarks", remarks ?? "N/A");

                // ---------------- DYNAMIC CONTENT FROM DB ----------------
                var contents = context.tbl_mCertificateContent
                    .Where(x=>x.SubStage == substage
                             && x.IsActive)
                    .OrderBy(x => x.DisplayOrder)
                    .ToList();
    //            var footerContent = contents
    //.FirstOrDefault(x => x.ContentId == 7);

                var mainContents = contents
                    .Where(x => x.ContentId != 7)
                    .ToList();


                foreach (var item in mainContents)
                {
                    if (item.ContentType == "Header" && item.ContentId == 3)
                        continue;
                    string text = item.ContentText?
                        .Replace("{ProjName}", data.ProjName)
                        .Replace("{Sponsor}", data.Sponsor)
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

                // ---------------- COMMON FOOTER ROWS ----------------
                AddRow(table, "Deploy Scenario", data.HostType ?? "N/A");
                AddRow(table, "Security Cl of data", "Restricted");
                AddRow(table, "Cert Generation Date", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                // Get label (Std Remarks) and text (As-is basis)
                var stdRemarkLabel = contents.FirstOrDefault(x => x.ContentId == 3);
                var stdRemarkText = contents.FirstOrDefault(x => x.ContentId == 7);

                if (stdRemarkLabel != null || stdRemarkText != null)
                {
                    // LABEL FROM DB (ContentId = 3)
                    if (stdRemarkLabel != null)
                    {
                        table.AddCell(new Cell(1, 3)
                            .Add(new Paragraph(stdRemarkLabel.ContentTitle ?? stdRemarkLabel.ContentText)
                                .SetFontSize(11)
                                .SetBold())
                            .SetPadding(6)
                             .SetBorder(Border.NO_BORDER));
                           
                    }

                    // REMARK TEXT FROM DB (ContentId = 7)
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



                document.Add(table);

                // ---------------- FOOTER ----------------
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

                // ---------------- BACKGROUND & WATERMARK ----------------
                AddBackgroundShield(pdf);
                // _watermarkRepo.AddWatermark(pdf, watermark);

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

                // Get actual scaled dimensions
                float actualWidth = img.GetImageScaledWidth();
                float actualHeight = img.GetImageScaledHeight();

                // Calculate position to center the image
                float x = (pageSize.GetWidth() - actualWidth) / 2;
                float y = (pageSize.GetHeight() - actualHeight) / 2;

                img.SetFixedPosition(x, y);

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

            document.Add(new Paragraph(logins.Appontment.Trim())
                   .SetFontSize(14)
                   .SetFixedPosition(textX, textY - 40, 200));


            document.Add(new Paragraph(logins.UserName.Trim())
                .SetFontSize(14)
                .SetFixedPosition(textX, textY - 60, 200));
        }



    }
}
