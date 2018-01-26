using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using LinqKit;
using Microsoft.CSharp;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Reports.Model;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Models;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Core.Common.Enums;
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
using Sunnet.Framework.Csv;
using Sunnet.Framework.Excel;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Vcw.Models;
using System.IO;
using Sunnet.Cli.Core.Cot;
using Sunnet.Cli.Core.Cec;
using Sunnet.Cli.Core.Ade;
using Sunnet.Framework.Encrypt;
using StructureMap;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Core.MasterData;
using Sunnet.Framework.Log;
using Sunnet.Framework.StringZipper;
using Text = DocumentFormat.OpenXml.Spreadsheet.Text;
using Sunnet.Cli.Core.Export.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Business.Ade.Models;

namespace Sunnet.Cli.Business.Reports
{

    static class ExtensionMeasure
    {
        /// <summary>
        /// 父级Measure 标题.
        /// </summary>
        /// <param name="measure">The measure.</param>
        /// <returns></returns>
        internal static string GetResultTitle(this BaseMeasureModel measure)
        {
            return measure.Label + " Results";
        }

        internal static string GetName(this BaseMeasureModel measure)
        {
            return measure.Label;
        }

        internal static string GetScoreTitle(this BaseMeasureModel measure)
        {
            return "CP_" + measure.Label;
        }

        internal static string GetName(this BaseMeasureModel measure, BaseMeasureModel parent)
        {
            if (parent != null && parent.ID > 1)
                return parent.Label + "_" + measure.Label;
            return measure.GetName();
        }

        internal static string GetAssessmentDate(this BaseMeasureModel measure, BaseMeasureModel parent)
        {
            if (parent != null && parent.ID > 1)
                return parent.Label + "_" + measure.Label + "_Date";
            return measure.GetName() + "_Date";
        }

        internal static string GetScoreTitle(this BaseMeasureModel measure, BaseMeasureModel parent)
        {
            if (parent != null && parent.ID > 1)
                return "CP_" + parent.Label + "_" + measure.Label;
            return measure.GetScoreTitle();
        }



        internal static string GetTitle(this BaseItemModel item, BaseMeasureModel measure)
        {
            return measure.Label + "_" + item.Label;
        }

        internal static bool BiggerThan4(this CircleDataExportStudentMeasureModel studentMeasure)
        {
            var startDate = CommonAgent.GetStartDateForAge(studentMeasure.SchoolYear);
            int month; int day;
            CommonAgent.CalculatingAge(startDate.Year, studentMeasure.BirthDay, out month, out day);
            var year = month / 12;
            return year >= 4;
        }

        internal static string GetStudentMeasureResult(this CircleDataExportStudentMeasureModel studentMeasure)
        {
            if (studentMeasure.Benchmark < 0)
            {
                return "";
            }
            else if (studentMeasure.Goal >= studentMeasure.Benchmark)
            {
                return "Proficient";
            }
            else if (studentMeasure.BiggerThan4())
            {
                return "Emerging";
            }
            else
            {
                return "Developing";
            }
        }

        internal static string GetStudentItemResult(this CircleDataExportStudentItemModel studentItem)
        {
            switch (studentItem.Type)
            {
                case ItemType.Quadrant:
                case ItemType.RapidExpressive:
                case ItemType.Receptive:
                case ItemType.ReceptivePrompt:
                    //return studentItem.IsCorrect ? "1" : "0";
                    return studentItem.Goal != null ? studentItem.Goal.Value.ToString("N2") : "";
                case ItemType.MultipleChoices:
                case ItemType.Pa:
                case ItemType.Checklist:
                case ItemType.TypedResponse:
                case ItemType.TxkeaExpressive:
                case ItemType.TxkeaReceptive:
                    //return studentItem.Goal > 0 ? "1" : "0";
                    return studentItem.Goal != null ? studentItem.Goal.Value.ToString("N2") : "";
                default:
                    return "";
            }
        }

        #region Reference Table

        internal static string GetToalScoreTitle(this BaseMeasureModel measure, BaseMeasureModel parent)
        {
            return "Total Score " + measure.Name;
        }
        internal static string GetCutPointTitle(this BaseMeasureModel measure, BaseMeasureModel parent)
        {
            return "Cut Point " + measure.Name;
        }

        internal static string GetDescription(this BaseItemModel item)
        {
            return item.Description;
        }

        #endregion
    }

    internal static class ExtensionCsvFileWriter
    {
        internal static void WriteRow(this CsvFileWriter writer, ExportFileType fileType, params object[] args)
        {
            var row = new CsvRow();
            args.ForEach(cell => row.Add(cell.ToString()));
            switch (fileType)
            {
                case ExportFileType.Comma:
                    writer.WriteRow(row);
                    break;
                case ExportFileType.Pipe:
                    writer.WriteRowSeparateByPipe(row);
                    break;
                case ExportFileType.Tab:
                    writer.WriteRowSeparateByTab(row);
                    break;
            }
        }

    }

    public partial class ReportBusiness
    {
        private readonly List<string> _properties = new List<string>() { "Wave", "School_Year", "Community_Name", "Community_Engage_ID", "District_Number",
            "School_Name", "School_Engage_ID", "School_Number", "School_Type", "School_Status", "Homeroom_Teacher",
            "Teacher_Engage_ID", "Teacher_Number", "Teacher_State_ID", "Teacher_PrimaryEmail",
            "Class_Name", "Class_Engage_ID", "Class_Internal_ID", "Class_Status", "Day_Type", "Class_Level",
            "Student_First_Name", "Student_Middle_Name", "Student_Last_Name", "Student_DOB", "Student_Status", "Student_Engage_ID",
            "Local_Student ID", "Student_State_ID", "Assessment_Language", "Student_Gender", "Student_Ethnicity", "Student_Grade_Level" };

        #region CIRCLE Data Export Excel

        public List<CircleDataExportStudentModel> GetCircleDataExport(int communityId, int year, int schoolId,
            List<Wave> waves, List<int> measures, List<int> measuresIncludeItems, DateTime startDate, DateTime endDate, 
            DateTime dobStartDate, DateTime dobEndDate, UserBaseEntity user)
        {
            _logger.Info("1. Start query data from Sql Server");
            var ws = waves.Select(x => (int)x).ToList();
            var types = new List<ItemType>()
            {
                ItemType.Checklist,
                ItemType.MultipleChoices,
                ItemType.Pa,
                ItemType.Quadrant,
                ItemType.RapidExpressive,
                ItemType.Receptive,
                ItemType.ReceptivePrompt,
                ItemType.TypedResponse,
                ItemType.TxkeaExpressive,
                ItemType.TxkeaReceptive
            };

            var measureModels = _cpallsContract.GetCircleDataExportStudentMeasureModelsWithItems(communityId,
                year.ToSchoolYearString(), schoolId, ws, measures.Union(measuresIncludeItems).Distinct().ToList(),
                measuresIncludeItems, types, startDate, endDate);
            _logger.Info("2. Measures Data Loaded");
            var assesssmentId = 0;
            if (measures.Count > 0)
                assesssmentId = _adeService.GetMeasure(measures[0]).AssessmentId;

            IList<ClassEntity> classList = _classBusiness.GetClassesForCircleReport(communityId, schoolId, user, assesssmentId);

            var classIds = classList.Select(c => c.ID).ToList();
            List<int> studentIds = _studentBusiness.GetStudentsByClassIds(classIds);
            var students = _reportService.GetCircleDataExportStudentModels(communityId, schoolId);
            students = students.Where(s => studentIds.Contains(s.ID) && s.BirthDate >= dobStartDate && s.BirthDate <= dobEndDate).ToList();

            foreach (var item in students)
            {
                var newClassStr = "";
                var classes = item.Classes.Split(';');
                foreach (var classItem in classes)
                {
                    var classEntity = ParseClass(classItem);
                    if (classList.Any(c => c.Name == classEntity.Name && c.ClassId == classEntity.ClassId))
                    {
                        newClassStr += classItem + ";";
                    }
                }
                item.Classes = newClassStr;
            }


            _logger.Info("4. Students Data Loaded, Preparing...");

            if (students != null && measureModels != null)
            {
                students.ForEach(stu =>
                {
                    stu.Measures = measureModels.FindAll(x => x.StudentId == stu.ID).ToList();
                });
            }
            _logger.Info("5. Prepared");
            return students;
        }
        private ClassEntity ParseClass(string classStr)
        {
            var classModel = new ClassEntity();
            var props = classStr.Split('|');
            classModel.Name = props.Length >= 1 ? props[0] : "";
            classModel.ClassId = props.Length >= 2 ? props[1] : "";
            classModel.Status = (EntityStatus)int.Parse(props.Length >= 3 ? props[2] : "0");
            classModel.DayType = (DayType)(int.Parse(props.Length >= 4 ? props[3] : "0"));

            classModel.Classlevel = int.Parse(props.Length >= 5 ? props[4] : "0");
            return classModel;
        }
        private List<ParentMeasureModel> GetBaseMeasures(List<int> measureIds, List<int> measuresIncludeItems, List<Wave> waves)
        {
            var types = new List<ItemType>()
            {
               ItemType.Checklist,
               ItemType.MultipleChoices,
               ItemType.Pa,
               ItemType.Quadrant,
               ItemType.RapidExpressive,
               ItemType.Receptive,
               ItemType.ReceptivePrompt,
               ItemType.TypedResponse,
               ItemType.TxkeaExpressive,
               ItemType.TxkeaReceptive
            };
            var measures =
                _adeService.Measures.Where(x => measureIds.Contains(x.ID) || measureIds.Contains(x.ParentId)).Where(x => x.IsDeleted == false)
                    .Select(x => new ParentMeasureModel()
                    {
                        AssessmentLanguage = x.Assessment.Language,
                        ID = x.ID,
                        Name = x.Name,
                        Label = x.Label,
                        ShortName = x.ShortName,
                        ParentId = x.ParentId,
                        ParentSort = x.ParentId > 1 ? x.Parent.Sort : x.Sort,
                        Sort = x.ParentId > 1 ? x.Parent.Sort + x.Sort : x.Sort,
                        Status = x.Status,
                        Items = x.Items
                        .Where(i => measuresIncludeItems.Contains(i.MeasureId) || measuresIncludeItems.Contains(i.Measure.ParentId))
                        .Where(i => i.IsDeleted == false && types.Contains(i.Type) && i.Scored) //不计分的 Item 不显示 在CIRCLE Data Export中
                        .OrderBy(i => i.Sort).Select(i => new BaseItemModel()
                        {
                            Description = i.Description,
                            Label = i.Label,
                            ID = i.ID,
                            Status = i.Status
                        })
                    }).OrderBy(x => x.AssessmentLanguage).ThenBy(x => x.ParentSort).ThenBy(x => x.Sort).ToList();
            var hostType = typeof(MeasureEntity).ToString();
            var allMeasureId = measures.Select(x => x.ID).ToList();
            var hasCutoffScores = _adeService.CutOffScores.Where(x => waves.Contains(x.Wave) && x.HostType == hostType
                && (allMeasureId.Contains(x.HostId))
                && x.FromYear + x.FromMonth + x.ToYear + x.ToMonth + x.CutOffScore > 0)
                    .Select(x => x.HostId).Distinct().ToList();
            measures.ForEach(mea => mea.HasCutPoint = hasCutoffScores.Any(x => x == mea.ID));
            return measures;
        }

        public bool GenerateCircleDataExport(string fileFullPath, List<CircleDataExportStudentModel> students,
            List<int> measureIds, List<int> measuresIncludeItems, string schoolYear, List<Wave> waves)
        {
            _logger.Info("6. Generating Excel");
            SpreadsheetDocument excelDoc = null;
            try
            {
                excelDoc = SpreadsheetDocument.Create(fileFullPath, SpreadsheetDocumentType.Workbook);
                var workbookPart = excelDoc.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                var style = excelDoc.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                var styles = new CircleDataExportExcelStylesheet();
                styles.Save(style);

                var sheets = excelDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                uint sheetId = 1;

                XAxis xAxis = new XAxis();
                YAxis yAxis = new YAxis();

                var studentPropsCount = _properties.Count;
                var measures = GetBaseMeasures(measureIds, measuresIncludeItems, waves);

                var widthAdjust = 10;

                // sheet1
                var studentsPart1 = students.Where(x => x.SchoolType.StartsWith("Demo") == false).ToList();
                WriteStudents("Reports", waves, studentsPart1, schoolYear, workbookPart, ref sheetId, sheets, yAxis, xAxis, studentPropsCount,
                    _properties, widthAdjust, measures);
                // sheet 2: Demo Schools
                yAxis.Reset();
                xAxis.Reset();

                var studentsPart2 = students.Where(x => x.SchoolType.StartsWith("Demo")).ToList();
                WriteStudents("Demo Reports", waves, studentsPart2, schoolYear, workbookPart, ref sheetId, sheets, yAxis, xAxis, studentPropsCount,
                    _properties, widthAdjust, measures);

                // save excel file
                workbookPart.Workbook.Save();
                _logger.Info("7. Excel Generated");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Info("7. Exception occured");
                _logger.Debug(ex);
                return false;
            }
            finally
            {
                _logger.Info("8. Over");
                if (excelDoc != null)
                {
                    excelDoc.Close();
                    excelDoc.Dispose();
                }
            }
        }

        private static void WriteStudents(string title, List<Wave> waves, List<CircleDataExportStudentModel> studentsPart1, string schoolYear,
            WorkbookPart workbookPart, ref uint sheetId,
            Sheets sheets, YAxis yAxis, XAxis xAxis, int studentPropsCount,
            List<string> properties, int widthAdjust, List<ParentMeasureModel> measures)
        {
            var cellContent = "";
            var cellsNeedMerge = new List<string>();
            Cell cell;

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var relId = workbookPart.GetIdOfPart(worksheetPart);
            var currentSheetData = new SheetData();
            var sheet1 = new Sheet()
            {
                Name = title,
                SheetId = UInt32Value.FromUInt32(sheetId++),
                Id = relId
            };
            sheets.Append(sheet1);

            // Headers
            var columns = new Columns();
            var firstRow = new Row(); // first row
            var secondRow = new Row(); // second row : empty;
            var thirdRow = new Row(); // third row: item title
            var rowIndex = yAxis.Next;
            var columnIndex = xAxis.Next;

            firstRow.Append(new HorizontalCenterTextCell(columnIndex, "ID Data Columns", rowIndex));
            xAxis.Step(studentPropsCount - 1);
            cellsNeedMerge.Add(columnIndex + rowIndex + ":" + xAxis.GetByStep(0) + rowIndex);
            secondRow.Append(new LeftBorderTextCell(columnIndex, "", rowIndex + 1));
            secondRow.Append(new RightBorderTextCell(xAxis.GetByStep(0), "", rowIndex + 1));

            rowIndex = yAxis.Step(2);
            xAxis.Reset();

            var isWriteFirst = true;
            properties.ForEach(prop =>
            {
                if (!isWriteFirst) firstRow.Append(new EmptyCell());

                thirdRow.Append(new TextCell(xAxis.Next, prop, rowIndex));
                columns.Append(new CustomColumn((UInt32)xAxis.Raw, (UInt32)xAxis.Raw, prop.Length + widthAdjust));

                if (isWriteFirst) isWriteFirst = false;
            });

            yAxis.Reset();
            foreach (var measure in measures)
            {
                rowIndex = yAxis.Next;

                columnIndex = xAxis.Next;
                cellContent = measure.GetResultTitle();
                firstRow.Append(new HorizontalCenterTextCell(columnIndex, cellContent, rowIndex));
                var stepOfMeasure = (measure.HasCutPoint ? 2 : 1) + measure.Items.Count();
                cellsNeedMerge.Add(columnIndex + rowIndex + ":" + xAxis.GetByStep(stepOfMeasure - 1) + rowIndex);

                var emptyCells = stepOfMeasure - 1;
                for (int i = 0; i < emptyCells; i++)
                    firstRow.Append(new EmptyCell());

                secondRow.Append(new RightBorderTextCell(xAxis.GetByStep(emptyCells), "", rowIndex + 1));

                rowIndex = yAxis.Step(2);
                var parentMeasure = measures.Find(x => x.ID == measure.ParentId);
                cellContent = measure.GetName(parentMeasure);
                thirdRow.Append(new TextCell(columnIndex, cellContent, rowIndex));
                columns.Append(new CustomColumn((UInt32)xAxis.Raw, (UInt32)xAxis.Raw, cellContent.Length + widthAdjust));

                if (measure.HasCutPoint)
                {
                    cellContent = measure.GetScoreTitle(measures.Find(x => x.ID == measure.ParentId));
                    thirdRow.Append(new TextCell(xAxis.Next, cellContent, rowIndex));
                    columns.Append(new CustomColumn((UInt32)xAxis.Raw, (UInt32)xAxis.Raw, cellContent.Length + widthAdjust));
                }

                if (measure.Items != null && measure.Items.Any())
                {
                    foreach (var item in measure.Items)
                    {
                        cellContent = item.GetTitle(parentMeasure ?? measure);
                        thirdRow.Append(new TextCell(xAxis.Next, cellContent, rowIndex));
                        columns.Append(new CustomColumn((UInt32)xAxis.Raw, (UInt32)xAxis.Raw, cellContent.Length + widthAdjust));
                    }
                }

                yAxis.Reset();
            }

            currentSheetData.Append(firstRow);
            currentSheetData.Append(secondRow);
            currentSheetData.Append(thirdRow);

            // 学生数据
            yAxis.Step(3);
            foreach (var wave in waves)
            {
                foreach (var student in studentsPart1)
                {
                    var row = new Row();
                    rowIndex = yAxis.Next;
                    xAxis.Reset();
                    row.Append(new LeftBorderTextCell(xAxis.Next, wave.ToDescription(), rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, schoolYear, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.CommunityName, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.CommunityIdentity, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.DistrictNumber, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.SchoolName, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.SchoolIdentity, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.SchoolNumber, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.SchoolType, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.SchoolStatus.ToDescription(), rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.HomeroomTeacher, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.TeacherId, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.TeacherNumber, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.TeacherPrimaryEmailAddress, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.ClassName, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.ClassId, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.ClassStatus, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.DayType, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.Classlevel, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.FirstName, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.MiddleName, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.LastName, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.BirthDate.ToString("MM/dd/yyyy"), rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.StudentStatus.ToDescription(), rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.StudentIdentity, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.LocalStudentID, rowIndex));
                    row.Append(new NoBorderTextCell(xAxis.Next, student.TSDSStudentID, rowIndex));
                    row.Append(new RightBorderTextCell(xAxis.Next, student.GradeLevel.ToDescription(), rowIndex));

                    var measuresOfWave = student.Measures != null ? student.Measures.Where(x => x.Wave == wave).ToList() :
                        new List<CircleDataExportStudentMeasureModel>();
                    if (!measuresOfWave.Any())
                    {
                        foreach (var measure in measures)
                        {
                            var stepOfMeasure = (measure.HasCutPoint ? 2 : 1) + measure.Items.Count();
                            row.Append(new RightBorderTextCell(xAxis.Step(stepOfMeasure), "", rowIndex));
                        }
                        currentSheetData.Append(row);
                        continue;
                    }

                    foreach (var measure in measures)
                    {
                        var studentMeasure = measuresOfWave.Find(x => x.MeasureId == measure.ID);
                        if (studentMeasure == null)
                        {
                            row.Append(new LeftBorderTextCell(xAxis.Next, "", rowIndex));

                            if (measure.Items != null && measure.Items.Any())
                            {
                                if (measure.HasCutPoint)
                                {
                                    row.Append(new NoBorderTextCell(xAxis.Next, "", rowIndex));
                                }
                                for (int i = 0; i < measure.Items.Count() - 1; i++)
                                {
                                    row.Append(new NoBorderTextCell(xAxis.Next, "", rowIndex));
                                }
                                row.Append(new RightBorderTextCell(xAxis.Next, "", rowIndex));
                            }
                            else
                            {
                                if (measure.HasCutPoint)
                                {
                                    row.Append(new RightBorderTextCell(xAxis.Next, "", rowIndex));
                                }
                            }

                            continue;
                        }

                        ///四舍六入5成双
                        cellContent = Math.Round(studentMeasure.Goal).ToString();  //studentMeasure.Goal.ToPrecisionString(2);
                        row.Append(new LeftBorderGoalCenterTextCell(xAxis.Next, cellContent, rowIndex));

                        if (measure.HasCutPoint)
                        {
                            cellContent = studentMeasure.LabelText;
                            if (measure.Items != null && measure.Items.Any())
                                cell = new NoBorderTextCell(xAxis.Next, cellContent, rowIndex);
                            else
                                cell = new RightBorderTextCell(xAxis.Next, cellContent, rowIndex);
                            row.Append(cell);
                        }

                        if (measure.Items != null && measure.Items.Any() && studentMeasure.Items != null)
                        {
                            for (int i = 0; i < measure.Items.Count(); i++)
                            {
                                var item = measure.Items.ElementAt(i);
                                var studentItem = studentMeasure.Items.Find(it => it.ItemId == item.ID);
                                if (studentItem == null)
                                    cellContent = "";
                                else
                                {
                                    if (studentItem.Scored && (studentItem.PauseTime > 0 || studentItem.Type == ItemType.Checklist))
                                        cellContent = studentItem.GetStudentItemResult();
                                    else
                                        cellContent = string.Empty;
                                }


                                if (i == measure.Items.Count() - 1)
                                    cell = new RightBorderItemResultTextCell(xAxis.Next, cellContent, rowIndex);
                                else
                                    cell = new NoBorderItemResultTextCell(xAxis.Next, cellContent, rowIndex);
                                row.Append(cell);
                            }
                        }
                    }
                    currentSheetData.Append(row);
                }
            }

            // 最后一行的下边框
            var bottomRow = new Row();
            xAxis.Reset();
            rowIndex = yAxis.Next;

            properties.ForEach(prop =>
            {
                bottomRow.Append(new TopBorderTextCell(xAxis.Next, "", rowIndex));
            });
            foreach (var measure in measures)
            {
                bottomRow.Append(new TopBorderTextCell(xAxis.Next, "", rowIndex));
                if (measure.HasCutPoint)
                    bottomRow.Append(new TopBorderTextCell(xAxis.Next, "", rowIndex));
                if (measure.Items != null && measure.Items.Any())
                {
                    for (int i = 0; i < measure.Items.Count(); i++)
                    {
                        bottomRow.Append(new TopBorderTextCell(xAxis.Next, "", rowIndex));
                    }
                }
            }
            currentSheetData.Append(bottomRow);

            worksheetPart.Worksheet = new Worksheet(); // sheet 1 end
            worksheetPart.Worksheet.Append(columns);
            worksheetPart.Worksheet.Append(currentSheetData);
            worksheetPart.Worksheet.MergeCells(cellsNeedMerge);
        }
        #endregion

        #region CIRCLE Data Export CSV

        public bool GenerateCircleDataExportCsv(string fileFullPath, List<CircleDataExportStudentModel> students,
            List<int> measureIds, List<int> measuresIncludeItems, string schoolYear, List<Wave> waves, ExportFileType fileType,
            List<int> scoreIds, List<ScoreReportModel> scoreReportModels)
        {
            _logger.Info("6. Generating CSV");

            var cellContent = "";
            var noContent = "";
            var measures = GetBaseMeasures(measureIds, measuresIncludeItems, waves);
            try
            {
                var destFolder = Path.GetDirectoryName(fileFullPath);
                var zipFilename = Path.GetFileName(fileFullPath);
                var folderToCompress = fileFullPath.Replace(".zip", "");
                if (!Directory.Exists(folderToCompress))
                    Directory.CreateDirectory(folderToCompress);

                var sheet1Name = Path.Combine(folderToCompress, "Report.csv");
                var sheet2Name = Path.Combine(folderToCompress, "DemoReport.csv");
                var sheet3Name = Path.Combine(folderToCompress, "Reference.csv");

                var studentsPart1 =
                       students.Where(
                           x => x.SchoolType.StartsWith("Demo", StringComparison.CurrentCultureIgnoreCase) == false)
                           .ToList();
                var studentsPart2 =
                        students.Where(x => x.SchoolType.StartsWith("Demo", StringComparison.CurrentCultureIgnoreCase))
                            .ToList();
                GenerateDataSheetFile(sheet1Name, schoolYear, waves, noContent, measures, studentsPart1, fileType, scoreIds, scoreReportModels);
                GenerateDataSheetFile(sheet2Name, schoolYear, waves, noContent, measures, studentsPart2, fileType, scoreIds, scoreReportModels);
                GenerateReferenceTable(sheet3Name, measures, fileType);
                _logger.Info("7. Report Generated");

                CSharpCodeStringZipper.CreateZip(destFolder, zipFilename, folderToCompress);
                Directory.Delete(folderToCompress, true);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Info("7. Exception occured");
                _logger.Debug(ex);
                return false;
            }
            finally
            {
                _logger.Info("8. Over");
            }
        }

        private void GenerateDataSheetFile(string fileFullPath, string schoolYear, List<Wave> waves, string noContent,
            List<ParentMeasureModel> measures, List<CircleDataExportStudentModel> studentsPart, ExportFileType fileType,
            List<int> scoreIds, List<ScoreReportModel> scoreReportModels)
        {
            string cellContent;
            using (var writer = new CsvFileWriter(fileFullPath))
            {
                var firstRow = new CsvRow();
                var secondRow = new CsvRow();
                var thirdRow = new CsvRow();
                _properties.ForEach(prop =>
                    {
                        firstRow.Add(noContent);
                        secondRow.Add(noContent);
                        thirdRow.Add(prop);
                    });
                AdeBusiness _adeBusiness = new AdeBusiness();
                var scoreModels = _adeBusiness.GetScores(scoreIds);
                foreach (var scoreId in scoreIds)
                {
                    var scoreModel = scoreModels.FirstOrDefault(e => e.ID == scoreId);
                    firstRow.Add("");
                    secondRow.Add("");
                    thirdRow.Add(scoreModel.ScoreDomain);
                    var scoreReportModel = scoreReportModels.FirstOrDefault(
                            e => e.ScoreId == scoreId);
                    if (scoreReportModel != null)
                    {
                        var scoreAllMeasures = scoreReportModel.ScoreMeasures.DistinctBy(e => e.MeasureName).ToList();
                        foreach (var measureModel in scoreAllMeasures)
                        {
                            firstRow.Add("");
                            secondRow.Add("");
                            thirdRow.Add(measureModel.MeasureName);
                        }
                    }
                }
                foreach (var measure in measures)
                {
                    cellContent = measure.GetResultTitle();
                    firstRow.Add(cellContent);

                    var stepOfMeasure = (measure.HasCutPoint ? 3 : 2) + measure.Items.Count();
                    var emptyCells = stepOfMeasure - 1;
                    for (int i = 0; i < emptyCells; i++)
                        firstRow.Add(noContent);

                    secondRow.Add(noContent);
                    var parentMeasure = measures.Find(x => x.ID == measure.ParentId);
                    cellContent = measure.GetName(parentMeasure);
                    thirdRow.Add(cellContent);

                    cellContent = measure.GetAssessmentDate(parentMeasure);
                    thirdRow.Add(cellContent);

                    if (measure.HasCutPoint)
                    {
                        cellContent = measure.GetScoreTitle(measures.Find(x => x.ID == measure.ParentId));
                        thirdRow.Add(cellContent);
                    }

                    if (measure.Items != null && measure.Items.Any())
                    {
                        foreach (var item in measure.Items)
                        {
                            cellContent = item.GetTitle(parentMeasure ?? measure);
                            thirdRow.Add(cellContent);
                        }
                    }
                }

                switch (fileType)
                {
                    case ExportFileType.Comma:
                        writer.WriteRow(firstRow);
                        writer.WriteRow(secondRow);
                        writer.WriteRow(thirdRow);
                        break;
                    case ExportFileType.Pipe:
                        writer.WriteRowSeparateByPipe(firstRow);
                        writer.WriteRowSeparateByPipe(secondRow);
                        writer.WriteRowSeparateByPipe(thirdRow);
                        break;
                    case ExportFileType.Tab:
                        writer.WriteRowSeparateByTab(firstRow);
                        writer.WriteRowSeparateByTab(secondRow);
                        writer.WriteRowSeparateByTab(thirdRow);
                        break;
                }

                if (studentsPart == null)
                    return; // return true;

                GenerateCsvPart(schoolYear, waves, studentsPart, writer, measures, noContent, fileType, scoreIds, scoreReportModels);
            }
        }

        private void GenerateCsvPart(string schoolYear, List<Wave> waves, List<CircleDataExportStudentModel> studentsPart1, CsvFileWriter writer, List<ParentMeasureModel> measures,
            string noContent, ExportFileType fileType, List<int> scoreIds, List<ScoreReportModel> scoreReportModels)
        {
            string cellContent;
            foreach (var wave in waves)
            {
                foreach (var student in studentsPart1)
                {
                    var row = new CsvRow();
                    row.Add(wave.ToDescription());
                    row.Add(schoolYear);
                    row.Add(student.CommunityName);
                    row.Add(student.CommunityIdentity);
                    row.Add(student.DistrictNumber);
                    row.Add(student.SchoolName);
                    row.Add(student.SchoolIdentity);
                    row.Add(student.SchoolNumber);
                    row.Add(student.SchoolType);
                    row.Add(student.SchoolStatus.ToDescription());
                    row.Add(student.HomeroomTeacher);
                    row.Add(student.TeacherId);
                    row.Add(student.TeacherNumber);
                    row.Add(student.TeacherTSDSID);
                    row.Add(student.TeacherPrimaryEmailAddress);
                    row.Add(student.ClassName);
                    row.Add(student.ClassId);
                    row.Add(student.ClassInternalID);
                    row.Add(student.ClassStatus);
                    row.Add(student.DayType);
                    row.Add(student.Classlevel);
                    row.Add(student.FirstName);
                    row.Add(student.MiddleName);
                    row.Add(student.LastName);
                    row.Add(student.BirthDate.ToString("MM/dd/yyyy"));
                    row.Add(student.StudentStatus.ToDescription());
                    row.Add(student.StudentIdentity);
                    row.Add(student.LocalStudentID);
                    row.Add(student.TSDSStudentID);
                    row.Add((int) student.AssessmengLanguage == 0 ? "" : student.AssessmengLanguage.ToDescription());
                    row.Add((int)student.StudentGender == 0 ? "" : student.StudentGender.ToDescription());
                    if ((int)student.StudentEthnicity == 0)
                        row.Add("");
                    else
                        row.Add(student.StudentEthnicity == Ethnicity.Other
                            ? student.StudentEthnicityOther
                            : student.StudentEthnicity.ToDescription());
                    row.Add(student.GradeLevel.ToDescription());

                    AdeBusiness _adeBusiness = new AdeBusiness();
                    foreach (var scoreId in scoreIds)
                    {
                        //var saId = _adeBusiness.GetOneSaid(student.ID, 5092, (int)wave);
                        //decimal finalResult = _adeBusiness.GetFinalResult(saId, scoreId);
                        //row.Add(finalResult.ToString("N2"));
                        var scoreReportModel = scoreReportModels.FirstOrDefault(
                            e => e.ScoreId == scoreId && e.Wave == wave && e.StudentId == student.ID);
                        if (scoreReportModel != null && scoreReportModel.FinalScore != null)
                        {
                            row.Add(scoreReportModel.FinalScore.Value.ToString("f") + scoreReportModel.TargetRound);
                        }
                        else
                        {
                            row.Add("");
                        }

                        if (scoreReportModel != null)
                        {
                            //将当前Score下的所有Wave下的Measure查出来，并且过滤掉重复的
                            var scoreAllMeasures = scoreReportModel.ScoreMeasures.DistinctBy(e => e.MeasureName).ToList();
                            foreach (var measureModel in scoreAllMeasures)
                            {
                                //确认当前Measure是在哪个wave下，并且将当前wave下的goal值查询出来，最终在报表中显示
                                var currentMeasureModel = scoreReportModel.ScoreMeasures.FirstOrDefault(e => e.Wave == wave && e.MeasureId == measureModel.MeasureId);
                                row.Add(currentMeasureModel.Goal == null ? "" : currentMeasureModel.Goal.Value.ToString());
                            }
                        }
                    }

                    // Student Measures

                    var measuresOfWave = student.Measures != null
                        ? student.Measures.Where(x => x.Wave == wave).ToList()
                        : new List<CircleDataExportStudentMeasureModel>();
                    if (!measuresOfWave.Any())
                    {
                        // 学生没有记录,直接结束
                        switch (fileType)
                        {
                            case ExportFileType.Comma:
                                writer.WriteRow(row);
                                break;
                            case ExportFileType.Pipe:
                                writer.WriteRowSeparateByPipe(row);
                                break;
                            case ExportFileType.Tab:
                                writer.WriteRowSeparateByTab(row);
                                break;
                        }
                        continue;
                    }

                    foreach (var measure in measures)
                    {
                        var studentMeasure = measuresOfWave.Find(x => x.MeasureId == measure.ID);
                        if (studentMeasure == null)
                        {
                            row.Add(noContent);
                            row.Add(noContent);
                            if (measure.Items != null && measure.Items.Any())
                            {
                                if (measure.HasCutPoint)
                                {
                                    row.Add(noContent);
                                }
                                for (int i = 0; i < measure.Items.Count() - 1; i++)
                                {
                                    row.Add(noContent);
                                }
                                row.Add(noContent);
                            }
                            else
                            {
                                if (measure.HasCutPoint)
                                {
                                    row.Add(noContent);
                                }
                            }
                            continue;
                        }

                        cellContent = studentMeasure.Goal > -1 ? studentMeasure.Goal.ToPrecisionString(2) : "";
                        row.Add(cellContent);

                        cellContent = studentMeasure.UpdatedOn.ToString("MM/dd/yyyy HH:mm:ss");
                        row.Add(cellContent);

                        if (measure.HasCutPoint)
                        {
                            cellContent = studentMeasure.LabelText;
                            row.Add(cellContent);
                        }


                        if (measure.Items != null && measure.Items.Any())
                        {
                            for (int i = 0; i < measure.Items.Count(); i++)
                            {
                                var item = measure.Items.ElementAt(i);
                                var studentItem = studentMeasure.Items.Find(it => it.ItemId == item.ID);
                                if (studentItem == null)
                                    cellContent = noContent;
                                else
                                {
                                    if (studentItem.Scored && (studentItem.PauseTime > 0 || studentItem.Type == ItemType.Checklist))
                                        cellContent = studentItem.GetStudentItemResult();
                                    else
                                        cellContent = string.Empty;
                                }
                                row.Add(cellContent);
                            }
                        }
                    }
                    switch (fileType)
                    {
                        case ExportFileType.Comma:
                            writer.WriteRow(row);
                            break;
                        case ExportFileType.Pipe:
                            writer.WriteRowSeparateByPipe(row);
                            break;
                        case ExportFileType.Tab:
                            writer.WriteRowSeparateByTab(row);
                            break;
                    }
                }
                ;
            }
        }

        private void GenerateReferenceTable(string fileFullPath, List<ParentMeasureModel> measures, ExportFileType fileType)
        {
            using (var writer = new CsvFileWriter(fileFullPath))
            {
                writer.WriteRow(fileType, "Column Name", "Item Description");
                foreach (var measure in measures)
                {
                    var parentMeasure = measures.Find(x => x.ID == measure.ParentId);
                    writer.WriteRow(fileType, measure.GetName(parentMeasure), measure.GetToalScoreTitle(parentMeasure));
                    if (measure.HasCutPoint)
                    {
                        writer.WriteRow(fileType, measure.GetScoreTitle(parentMeasure), measure.GetCutPointTitle(parentMeasure));
                    }

                    if (measure.Items != null && measure.Items.Any())
                    {
                        foreach (var item in measure.Items)
                        {
                            writer.WriteRow(fileType, item.GetTitle(parentMeasure ?? measure), item.GetDescription());
                        }
                    }
                }
            }
        }

        #endregion
    }
}
