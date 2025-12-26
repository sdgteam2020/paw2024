using swas.BAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Properties;
using Rectangle = iText.Kernel.Geom.Rectangle;


namespace swas.BAL.Repository
{

    public class WatermarkRepository : IWatermarkRepository
    {
        public void AddWatermark(PdfDocument pdf, string watermarkText)
        {
            PdfFont font_RULE = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            iText.Kernel.Colors.Color gray = new DeviceGray(0.50f);
            float fontSize = 50f;
            float opacity = 0.8f;
            float angleDegrees = 45f;

            Paragraph watermark = new Paragraph(watermarkText)
                .SetFont(font_RULE)
                .SetFontSize(fontSize)
                .SetFontColor(gray)
                .SetTextAlignment(TextAlignment.CENTER);

            for (int pageNum = 1; pageNum <= pdf.GetNumberOfPages(); pageNum++)
            {
                PdfPage page = pdf.GetPage(pageNum);
                Rectangle rect = page.GetPageSizeWithRotation();

                PdfCanvas pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdf);
                pdfCanvas.SaveState();
                pdfCanvas.SetExtGState(new iText.Kernel.Pdf.Extgstate.PdfExtGState().SetFillOpacity(opacity));

                Canvas canvas = new Canvas(pdfCanvas, rect);
                canvas
                    .ShowTextAligned(
                        watermark,
                        rect.GetWidth() / 2,
                        rect.GetHeight() / 2,
                        pageNum,
                        TextAlignment.CENTER,
                        VerticalAlignment.MIDDLE,
                        (float)(angleDegrees * Math.PI / 180)
                    )
                    .Close();

                pdfCanvas.RestoreState();
                pdfCanvas.Release();
            }
        }
    }

}
