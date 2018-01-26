using System.IO;
using System.Web;
using EvoPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using Sunnet.Framework.Log;
using Color = System.Drawing.Color;

namespace Sunnet.Framework.PDF
{
    /// <summary>
    /// 生成PDF类型：决定使用哪个PdfConverter
    /// </summary>
    public enum PdfType
    {
        /// <summary>
        /// 普通PDF
        /// </summary>
        General,

        /// <summary>
        /// Assessment 的报表
        /// </summary>
        Assessment_Landscape,

        /// <summary>
        /// Assessment 的报表
        /// </summary>
        Assessment_Portrait,

        COT_Portrait,

        COT_Landscape,

        COT_Landscape_No_Pager,

        Observable,
        ParentPinPage
    }

    public class PdfProvider
    {
        private ISunnetLog _logger;
        private PdfConverter pdfConverter = null;
        public PdfProvider(PdfConverter converter)
        {
            pdfConverter = converter;
            pdfConverter.NavigationTimeout = 20 * 60;//David 11/14/2014
        }
        public PdfProvider()
            : this(PdfType.General)
        {
        }

        private PdfType pdfType;
        public PdfProvider(PdfType type, string userName = "", string startPdfPath = "", string endPdfPath = "", string avoidPageBreakSelctor = "", int pdfPageWidth = 0)
        {
           // _logger = ObjectFactory.GetInstance<ISunnetLog>();
           // _logger.Debug("test PdfProvider with type");
            pdfType = type;
            switch (type)
            {
                case PdfType.Assessment_Portrait:
                case PdfType.Assessment_Landscape:
                case PdfType.Observable:
                    pdfConverter = GetPdfConverterForAssessment(type, startPdfPath, endPdfPath, avoidPageBreakSelctor, pdfPageWidth);
                    break;
                case PdfType.COT_Portrait:
                case PdfType.COT_Landscape:
                case PdfType.COT_Landscape_No_Pager:
                    pdfConverter = GetPdfConverterForCot(type, userName);
                    break;
                case PdfType.ParentPinPage:
                    pdfConverter = GetPdfConverterForParentPinPage(type, startPdfPath, endPdfPath, avoidPageBreakSelctor, pdfPageWidth);
                    break;
                default:
                    pdfConverter = GetPdfConverter();
                    break;
            }
            pdfConverter.NavigationTimeout = 20 * 60;


        }

        public bool IsPortrait
        {
            get
            {
                return pdfConverter.PdfDocumentOptions.PdfPageOrientation == PdfPageOrientation.Portrait;
            }
            set
            {
                pdfConverter.PdfDocumentOptions.PdfPageOrientation = value ? PdfPageOrientation.Portrait : PdfPageOrientation.Landscape;
            }
        }

        private PdfConverter GetPdfConverter()
        {
            PdfConverter pdfConverter = new PdfConverter();
            pdfConverter.PdfDocumentOptions.ShowHeader = false;
            pdfConverter.PdfDocumentOptions.ShowFooter = true;
            pdfConverter.PdfDocumentOptions.LeftMargin = 20;
            pdfConverter.PdfDocumentOptions.RightMargin = 10;
            pdfConverter.PdfDocumentOptions.TopMargin = 20;
            pdfConverter.PdfDocumentOptions.BottomMargin = 10;
            pdfConverter.PdfDocumentOptions.AutoSizePdfPage = true;

            pdfConverter.PdfDocumentOptions.FitWidth = true;

            pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;

            pdfConverter.HtmlViewerWidth = 768; /// pdfConverter.PdfDocumentOptions.PdfPageOrientation == PdfPageOrientation.Portrait ? 768 : 1024;

            pdfConverter.PdfFooterOptions.FooterHeight = 15;

            var date = DateTime.Now;
            var font = new System.Drawing.Font(new System.Drawing.FontFamily("Calibri"), 8, System.Drawing.GraphicsUnit.Point);
            TextElement footerText = new TextElement(0, 4, string.Format("© {0} The University of Texas Health Science Center at Houston", DateTime.Now.Year), font);
            footerText.EmbedSysFont = true;
            footerText.TextAlign = HorizontalTextAlign.Left;
            pdfConverter.PdfFooterOptions.AddElement(footerText);
            footerText = new TextElement(0, 4, " Created on " + date.ToString("MMM dd yyyy")
                                               + " at " + date.ToString(" hh:mm tt") + "".PadLeft(40, ' '), font);

            footerText.EmbedSysFont = true;
            footerText.TextAlign = HorizontalTextAlign.Right;
            pdfConverter.PdfFooterOptions.AddElement(footerText);

            return pdfConverter;
        }

        private PdfConverter GetPdfConverterForCot(PdfType type, string userName)
        {
            PdfConverter pdfConverter = new PdfConverter();
            pdfConverter.PdfDocumentOptions.ShowHeader = false;
            pdfConverter.PdfDocumentOptions.ShowFooter = true;
            pdfConverter.PdfDocumentOptions.LeftMargin = 20;
            pdfConverter.PdfDocumentOptions.RightMargin = 10;
            pdfConverter.PdfDocumentOptions.TopMargin = 20;
            pdfConverter.PdfDocumentOptions.BottomMargin = 10;
            pdfConverter.PdfDocumentOptions.AutoSizePdfPage = true;

            pdfConverter.PdfDocumentOptions.FitWidth = true;

            pdfConverter.PdfDocumentOptions.PdfPageOrientation = type == PdfType.COT_Portrait
               ? PdfPageOrientation.Portrait
               : PdfPageOrientation.Landscape;

            pdfConverter.HtmlViewerWidth = pdfConverter.PdfDocumentOptions.PdfPageOrientation == PdfPageOrientation.Portrait ? 768 : 1024;

            pdfConverter.PdfFooterOptions.FooterHeight = 15;
            var date = DateTime.Now;
            var font = new System.Drawing.Font(new System.Drawing.FontFamily("Calibri"), 8, System.Drawing.GraphicsUnit.Point);
            TextElement footerText = new TextElement(0, 4, string.Format("© {0} The University of Texas Health Science Center at Houston", DateTime.Now.Year), font);
            footerText.EmbedSysFont = true;
            footerText.TextAlign = HorizontalTextAlign.Left;
            pdfConverter.PdfFooterOptions.AddElement(footerText);
            footerText = new TextElement(0, 4, " Reported on " + date.ToString("MMM dd yyyy")
                                               + " at " + date.ToString(" hh:mm tt") + (userName == "" ? "" : " by: " + userName) + "".PadLeft(20, ' ')
                                               + (type == PdfType.COT_Landscape_No_Pager ? "" : "Page &p;/&P;") + "".PadLeft(15, ' '), font);

            footerText.EmbedSysFont = true;
            footerText.TextAlign = HorizontalTextAlign.Right;
            pdfConverter.PdfFooterOptions.AddElement(footerText);
            return pdfConverter;
        }

        private PdfConverter GetPdfConverterForAssessment(PdfType type, string startPdfPath = "", string endPdfPath = "", string avoidPageBreakSelctor = "", int pdfPageWidth = 0)
        {


            PdfConverter pdfConverter = new PdfConverter();
            pdfConverter.PdfDocumentOptions.ShowHeader = false;
            pdfConverter.PdfDocumentOptions.ShowFooter = true;
            pdfConverter.PdfDocumentOptions.LeftMargin = 20;
            pdfConverter.PdfDocumentOptions.RightMargin = 10;
            pdfConverter.PdfDocumentOptions.TopMargin = 20;
            pdfConverter.PdfDocumentOptions.BottomMargin = 10;
            pdfConverter.PdfDocumentOptions.AutoSizePdfPage = true;

            pdfConverter.PdfDocumentOptions.FitWidth = true;

            pdfConverter.PdfDocumentOptions.PdfPageOrientation = type == PdfType.Assessment_Landscape
                ? PdfPageOrientation.Landscape
                : PdfPageOrientation.Portrait;

            pdfConverter.HtmlViewerWidth = pdfConverter.PdfDocumentOptions.PdfPageOrientation == PdfPageOrientation.Portrait ? 768 : 1024;

            pdfConverter.PdfFooterOptions.FooterHeight = 15;
            var date = DateTime.Now;
            var font = new System.Drawing.Font(new System.Drawing.FontFamily("Calibri"), 8, System.Drawing.GraphicsUnit.Point);
            TextElement footerText = new TextElement(0, 4, string.Format("© {0} The University of Texas Health Science Center at Houston", DateTime.Now.Year), font);
            footerText.EmbedSysFont = true;
            footerText.TextAlign = HorizontalTextAlign.Left;
            pdfConverter.PdfFooterOptions.AddElement(footerText);
            footerText = new TextElement(0, 4, " Reported on " + date.ToString("MMM dd yyyy") + " at " + date.ToString(" hh:mm tt") + "".PadLeft(20, ' ') + "Page &p;/&P;            ", font);
            footerText.EmbedSysFont = true;
            footerText.TextAlign = HorizontalTextAlign.Right;
            if (avoidPageBreakSelctor != "")
            {
                pdfConverter.PdfDocumentOptions.AvoidHtmlElementsBreakSelectors = new string[] { avoidPageBreakSelctor };
            }
            pdfConverter.PdfFooterOptions.AddElement(footerText);

            if (startPdfPath != "")
            {
                pdfConverter.PdfDocumentOptions.PdfPageSize.Width = 612;
                pdfConverter.PdfDocumentOptions.AddStartDocument(startPdfPath);
            }
            if (endPdfPath != "")
            {
                pdfConverter.PdfDocumentOptions.PdfPageSize.Width = 612;
                pdfConverter.PdfDocumentOptions.AddEndDocument(endPdfPath);
            }
            if (pdfPageWidth != 0)
            {
                pdfConverter.PdfDocumentOptions.PdfPageSize.Width = pdfPageWidth;
            }
            //pdfConverter.PdfDocumentOptions.JpegCompressionLevel = 0;
            //pdfConverter.PdfDocumentOptions.ImagesScalingEnabled = true;
            //pdfConverter.ImagePartSize = 3200000;
            //pdfConverter.PdfDocumentOptions.ImagesScalingEnabled=false
            //pdfConverter.ImagePartSize=32000
            return pdfConverter;
        }

        private PdfConverter GetPdfConverterForParentPinPage(PdfType type, string startPdfPath = "", string endPdfPath = "", string avoidPageBreakSelctor = "", int pdfPageWidth = 0)
        {
            PdfConverter pdfConverter = new PdfConverter();
            pdfConverter.PdfDocumentOptions.ShowHeader = false;
            pdfConverter.PdfDocumentOptions.ShowFooter = true;
            pdfConverter.PdfDocumentOptions.LeftMargin = 10;
            pdfConverter.PdfDocumentOptions.RightMargin = 10;
            pdfConverter.PdfDocumentOptions.TopMargin = 25;
            pdfConverter.PdfDocumentOptions.BottomMargin = 10;
            pdfConverter.PdfDocumentOptions.AutoSizePdfPage = true;

            pdfConverter.PdfDocumentOptions.FitWidth = true;

            pdfConverter.PdfDocumentOptions.PdfPageOrientation = type == PdfType.Assessment_Landscape
                ? PdfPageOrientation.Landscape
                : PdfPageOrientation.Portrait;

            pdfConverter.HtmlViewerWidth = pdfConverter.PdfDocumentOptions.PdfPageOrientation == PdfPageOrientation.Portrait ? 768 : 1024;

            pdfConverter.PdfFooterOptions.FooterHeight = 15;
            var date = DateTime.Now;
            var font = new System.Drawing.Font(new System.Drawing.FontFamily("Calibri"), 8, System.Drawing.GraphicsUnit.Point);
            TextElement footerText = new TextElement(0, 4, string.Format("© {0} The University of Texas Health Science Center at Houston", DateTime.Now.Year), font);
            footerText.EmbedSysFont = true;
            footerText.TextAlign = HorizontalTextAlign.Left;
            pdfConverter.PdfFooterOptions.AddElement(footerText);
            footerText = new TextElement(0, 4, " Reported on " + date.ToString("MMM dd yyyy") + " at " + date.ToString(" hh:mm tt") + "".PadLeft(20, ' ') + "Page &p;/&P;            ", font);
            footerText.EmbedSysFont = true;
            footerText.TextAlign = HorizontalTextAlign.Right;
            if (avoidPageBreakSelctor != "")
            {
                pdfConverter.PdfDocumentOptions.AvoidHtmlElementsBreakSelectors = new string[] { avoidPageBreakSelctor };
            }
            pdfConverter.PdfFooterOptions.AddElement(footerText);

            if (startPdfPath != "")
            {
                pdfConverter.PdfDocumentOptions.PdfPageSize.Width = 612;
                pdfConverter.PdfDocumentOptions.AddStartDocument(startPdfPath);
            }
            if (endPdfPath != "")
            {
                pdfConverter.PdfDocumentOptions.PdfPageSize.Width = 612;
                pdfConverter.PdfDocumentOptions.AddEndDocument(endPdfPath);
            }
            if (pdfPageWidth != 0)
            {
                pdfConverter.PdfDocumentOptions.PdfPageSize.Width = pdfPageWidth;
            }
            return pdfConverter;
        }


        void pdfConverter_PrepareRenderPdfPageEvent(PrepareRenderPdfPageParams eventParams)
        {
            if (eventParams.PageNumber > 1)
            {
                PdfPage pdfPage = eventParams.Page;
                pdfPage.ShowHeader = false;
                pdfPage.Margins.Top = 10;
            }
        }

        /// <summary>
        /// Generate PDF file with the whole html content
        /// </summary>
        /// <param name="contentHtml">internal content</param>
        /// <param name="fileName">export file name</param>
        public void GeneratePDF(string contentHtml, string fileName)
        {
            pdfConverter.LicenseKey = SFConfig.EVOPDFKEY;
            pdfConverter.JavaScriptEnabled = false;
            byte[] pdfBytes = pdfConverter.GetPdfBytesFromHtmlString(contentHtml);

            ExportPdfFile(pdfBytes, fileName);
        }

        public void SavePdf(string contentHtml, string localFilename)
        {
            pdfConverter.LicenseKey = SFConfig.EVOPDFKEY;
            byte[] pdfBytes = pdfConverter.GetPdfBytesFromHtmlString(contentHtml);

            var folder = Path.GetDirectoryName(localFilename);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            using (var writer = System.IO.File.Create(localFilename))
            {
                writer.Write(pdfBytes, 0, pdfBytes.Count());
                writer.Flush();
                writer.Close();
            }
        }
        public void SavePdf(byte[] pdfBytes, string localFilename)
        {
            pdfConverter.LicenseKey = SFConfig.EVOPDFKEY;
            var folder = Path.GetDirectoryName(localFilename);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            using (var writer = System.IO.File.Create(localFilename))
            {
                writer.Write(pdfBytes, 0, pdfBytes.Count());
                writer.Flush();
                writer.Close();
            }
        }

        private void AddHeader(Document pdfDocument, string headerPath)
        {
            var header = pdfDocument.Templates.AddNewTemplate(0, 0);
            for (int i = 1; i < pdfDocument.Pages.Count; i++)
            {
                pdfDocument.Pages[i].Header = header;
            }
        }

        public byte[] GetPdfBytes(string contentHtml)
        {
            if (SFConfig.EVOPDFKEY != "")
                pdfConverter.LicenseKey = SFConfig.EVOPDFKEY;
            byte[] pdfBytes = pdfConverter.GetPdfBytesFromHtmlString(contentHtml);
            return pdfBytes;
        }

        public void GeneratePDFFromUrl(string url, string fileName)
        {
            var cookies = HttpContext.Current.Request.Cookies;
            for (int i = 0; i < cookies.Count; i++)
            {
                var httpCookie = cookies[i];
                if (httpCookie != null) pdfConverter.HttpRequestCookies.Add(httpCookie.Name, httpCookie.Value);
            }
            byte[] pdf = pdfConverter.GetPdfBytesFromUrl(url);
            ExportPdfFile(pdf, fileName);
        }

        private void ExportPdfFile(byte[] pdfBytes, string fileName)
        {
            if (!fileName.EndsWith(".pdf", StringComparison.CurrentCultureIgnoreCase))
            {
                fileName += ".pdf";
            }
            pdfConverter.LicenseKey = SFConfig.EVOPDFKEY;
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.AddHeader("Content-Type", "application/pdf");
            response.AddHeader("Content-Disposition", String.Format("attachment; filename=" + fileName + "; size={0}", pdfBytes.Length));
            response.BinaryWrite(pdfBytes);
            response.End();
        }

        public int SplitPdf(string source, int count)
        {
            var sourceDocument = new Document(source);
            var totalPages = sourceDocument.Pages.Count;
            int results = 0;
            if (count > 0)
            {
                var size = totalPages / count;
                while (count < totalPages)
                {
                    var document = new Document();
                    document.AppendDocument(sourceDocument);
                    results++;
                }
            }
            return results;
        }

    }
}
