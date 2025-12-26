using System;
using System.Collections.Generic;
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


namespace swas.BAL.Interfaces
{
    public interface IWatermarkRepository
    {
        void AddWatermark(PdfDocument pdf, string watermarkText);
    }

}
