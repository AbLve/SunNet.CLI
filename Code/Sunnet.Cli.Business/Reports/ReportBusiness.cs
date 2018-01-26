using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using LinqKit;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade.Models;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Reports.Entities;
using System.Linq.Expressions;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Cli.Core.Users;
using Sunnet.Framework;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Excel;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Vcw.Models;
using System.IO;
using Sunnet.Cli.Core.Cot;
using Sunnet.Cli.Core.Cec;
using Sunnet.Cli.Core.Ade;
using Sunnet.Framework.Encrypt;
using StructureMap;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Core.MasterData;
using Sunnet.Framework.Log;
using Text = DocumentFormat.OpenXml.Spreadsheet.Text;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Students;

namespace Sunnet.Cli.Business.Reports
{
    public partial class ReportBusiness
    {
        private readonly IReportContract _reportService;
        private readonly ICotContract _cotService;
        private readonly ICecContract _cecService;
        private readonly IAdeContract _adeService;
        private readonly IEncrypt _encrypter;
        private readonly ISunnetLog _logger;
        private readonly MasterDataBusiness _masterBusiness;
        private readonly IUserContract _userService;
        private readonly ICpallsContract _cpallsContract;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly ClassBusiness _classBusiness;
        private readonly StudentBusiness _studentBusiness;

        public ReportBusiness(EFUnitOfWorkContext unit = null)
        {
            _reportService = DomainFacade.CreateReportService(unit);
            _cotService = DomainFacade.CreateCotContract(null);
            _cecService = DomainFacade.CreateCecServer(null);
            _adeService = DomainFacade.CreateAdeService(null);
            _encrypter = ObjectFactory.GetInstance<IEncrypt>();
            _logger = ObjectFactory.GetInstance<ISunnetLog>();
            _masterBusiness = new MasterDataBusiness(unit);
            _userService = DomainFacade.CreateUserService(null);
            _cpallsContract = DomainFacade.CreateCpallsService();
            _schoolBusiness = new SchoolBusiness();
            _classBusiness = new ClassBusiness();
            _studentBusiness = new StudentBusiness();
        }

        /// <summary>
        /// 生成报表的文件名
        /// </summary>
        /// <param name="reportName"></param>
        /// <returns></returns>
        string CreateReportName(string reportName)
        {
            string directory = Path.Combine(SFConfig.ProtectedFiles, string.Format("Reports/{0}{1}/", DateTime.Now.Year, DateTime.Now.Month));
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            return System.IO.Path.Combine(directory, string.Format("{0}_{1}.xlsx", reportName, DateTime.Now.ToString("yyyyMMdd_HHmmssssss")));
        }
    }

    class MediaCustomStylesheet : CustomStylesheet
    {
        public override void AddCellFormats(CellFormats cellFormats)
        {   // index 11
            // Cell  黄色
            var cellFormat10 = new CellFormat
            {
                NumberFormatId = 4,
                FontId = 0,
                FillId = 5, //无填充色
                BorderId = 1, //有边框
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat10);

            // index 12
            // Cell  浅绿色
            var cellFormat11 = new CellFormat
            {
                NumberFormatId = 4,
                FontId = 0,
                FillId = 2, //无填充色
                BorderId = 1, //有边框
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat11);

            // index 13
            // Cell  黄色
            // 斜体粗体
            var cellFormat12 = new CellFormat
            {
                NumberFormatId = 4,
                FontId = 2,
                FillId = 5, //无填充色
                BorderId = 1, //有边框
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat12);

            // Index 14 Center Hol
            var cellFormat14 = new CellFormat
            {
                NumberFormatId = 4,
                FontId = 1,
                FillId = 0,
                BorderId = 1, // All border
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };
            cellFormats.Append(cellFormat14);

            // Index 15  数据居中显示
            var cellFormat15 = new CellFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(167),
                FontId = 0,
                FillId = 0,
                BorderId = 1, // All border
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };
            cellFormats.Append(cellFormat15);

            // Index 15  数据居中显示且显示小数
            var cellFormat16 = new CellFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(166),
                FontId = 0,
                FillId = 0,
                BorderId = 1, // All border
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };
            cellFormats.Append(cellFormat16);
        }
    }

    class MediaTextCell : Cell
    {
        public MediaTextCell(string header, string text, int index)
        {
            this.DataType = CellValues.InlineString;
            this.CellReference = header + index;
            //Add text to the text cell.
            this.InlineString = new InlineString { Text = new Text { Text = text } };
            this.StyleIndex = 10;
        }
    }

    internal class PDTextCell : Cell
    {
        public PDTextCell(string header, string text, int index)
        {
            this.DataType = CellValues.InlineString;
            this.CellReference = header + index;
            //Add text to the text cell.
            this.InlineString = new InlineString { Text = new Text { Text = text } };
            this.StyleIndex = 13;
        }
    }

    class MediaNumberCell : Cell
    {
        public MediaNumberCell(string header, string text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text);
            this.StyleIndex = 12;
        }
    }

    class MediaTestCell : Cell
    {
        public MediaTestCell(string header, string text, int index)
        {
            this.DataType = CellValues.InlineString;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text);
            this.StyleIndex = 12;
        }
    }

    class MediaBICell : TextCell
    {
        public MediaBICell(string header, string text, int index) :
            base(header, text, index)
        {
            this.StyleIndex = 9;
        }
    }

    class HeaderCenterCell : TextCell
    {
        public HeaderCenterCell(string header, string text, int index) :
            base(header, text, index)
        {
            this.StyleIndex = 14;
        }
    }

    internal class ReportNumberCenterCell : Cell
    {
        public ReportNumberCenterCell(string header, string text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text);
            this.StyleIndex = 15;
        }
    }

    internal class ReportNumberD2CenterCell : Cell
    {
        public ReportNumberD2CenterCell(string header, string text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text);
            this.StyleIndex = 16;
        }
    }

}
