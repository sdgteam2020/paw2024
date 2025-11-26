using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Layout.Element;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;

using iText.Layout.Properties;
using iText.Layout;

namespace swas.BAL.Interfaces
{
    public interface IWatermarkRepository
    {
        void AddWatermark(PdfDocument pdf, string watermarkText);
    }
}
