using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/12/16
 * Description:		
 * Version History:	Created,2015/12/16
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Extensions;
using System.Web;
using System.Data;
using Sunnet.Cli.Core.Ade.Enums;
using System.Data.OleDb;
using Sunnet.Cli.Business.BUP;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Ade;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Core.Tool;
using System.IO;
using Sunnet.Framework;
using Sunnet.Framework.Encrypt;
using StructureMap;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.Ade
{
    public partial class AdeBusiness
    {
        //此变量只需在最后增加（若中间增加或者去除元素，引用该变量的索引需要修改）
        private string[] str_receptiveHeaders = new string[] { "Item_Name", "Practice_Item", "Description", "Background_Color", "Background_Image_Name", //5
                "Instruction_Audio_Name","Instruction_Text" ,"End_Instruction_Audio_Name","End_Instruction_Text","Number_Images", //10
            "Selection_Type","Image_Sequence","Overall_Timeout","Timeout_Value","BreakConditon","Stop_Incorrect","Stop_OutOf", //17
            "Scoring","Layout_Name","Image_Type","Target_Image_Name","Image_Time_Delay","Target_Audio_Name", //23
            "Audio_Time_Delay","Correct_Response","Correct_Score","Correct_Grouping","Response_Audio_Name","Audio_Time_Delay_Same","Image_order"}; //30

        private string[] str_expressiveHeaders = new string[] { "Item_Name", "Practice_Item", "Description", "Background_Color", "Background_Image_Name", //5
                "Instruction_Text" ,"Instruction_Audio_Name","Audio_Delay","Number_Images",//9
                "Layout_Name","Target_Image_Name","Image_Time_Delay","Target_Audio_Name","Audio_Time_Delay",//14
                "Response_Background_Color","Response_Background_Image_Name","Response_Text","Mandatory","Scored_Response_Type",//19
        "Number_of_Radio/Check_Buttons","Option1","Add_Text_Option1",//22
        "Option2","Add_Text_Option2","Option3","Add_Text_Option3",//26
        "Option4","Add_Text_Option4","Option5","Add_Text_Option5",//30
        "Option6","Add_Text_Option6","Overall_Timeout","Timeout_Value","Response_Type","Image_Delay_Same",//36
        "Score_Option1","Score_Option2","Score_Option3","Score_Option4","Score_Option5","Score_Option6","Image_order",//43
        //Steven 07/06/2016 ticket 2275 
        "Second_Response_Text","Second_Mandatory","Second_Response_Type","S_Number_of_Radio/Check_Buttons",//47
        "Second_Option1","Second_Text_Option1","Second_Option2","Second_Text_Option2",//51
        "Second_Option3","Second_Text_Option3","Second_Option4","Second_Text_Option4",//55
        "Second_Option5","Second_Text_Option5","Second_Option6","Second_Text_Option6"};//59

        private string[] imgExtensions = new string[] { ".gif", ".jpg", ".jpeg", ".bmp", ".png" };
        private string[] audioExtensions = new string[] { ".mp3" };

        private string yesValue = "yes"; //Yes选项文本
        private string noValue = "no"; //No选项文本
        private int outValue = 0; //通用int类型转换值
        private decimal outDecimalValue = 0;
        private IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();

        public string ExecuteQuerySql(string sql)
        {
            return _adeContract.ExecuteQuerySql(sql);
        }

        public List<TxkeaBupTaskModel> SearchTxkeaBupTasks(Expression<Func<TxkeaBupTaskEntity, bool>> condition, out int total,
            string sort = "ID", string order = "DESC", int first = 0, int count = 10)
        {
            var query = _adeContract.TxkeaBupTasks.AsExpandable()
                .Where(r => r.IsDeleted == false)
                .Where(condition).OrderBy(sort, order)
                .Select(SelectorEntityToBupTaskModel);
            total = query.Count();
            var result = query.Skip(first).Take(count).ToList();
            return result;
        }

        private static Expression<Func<TxkeaBupTaskEntity, TxkeaBupTaskModel>> SelectorEntityToBupTaskModel
        {
            get
            {
                return x => new TxkeaBupTaskModel()
                {
                    ID = x.ID,
                    AssessmentId = x.AssessmentId,
                    MeasureId = x.MeasureId,
                    Type = x.Type,
                    Status = x.Status,
                    OriginFileName = x.OriginFileName,
                    FilePath = x.FilePath,
                    IsDeleted = x.IsDeleted,
                    ResourcePath = x.ResourcePath,
                    CreatedBy = x.CreatedBy,
                    CreatedOn = x.CreatedOn,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedOn = x.UpdatedOn,
                };
            }
        }

        public string InvalidateFile(HttpPostedFileBase postFileBase)
        {
            if (postFileBase == null || postFileBase.ContentLength == 0)
                return "Please select a valid file.";

            string fileType = string.Empty;

            if (postFileBase.ContentLength == 0)
                return "Please select a valid file.";

            string[] name = postFileBase.FileName.Split('.');
            fileType = name[name.Length - 1];
            if (string.IsNullOrEmpty(fileType) || (fileType.ToLower() != "xls" && fileType.ToLower() != "xlsx"))
                return "Please select a valid excel.";

            return "";
        }

        public string InvalidateFile(string uploadPath, int count, TxkeaBupType type, out DataTable dt)
        {
            dt = new DataTable();
            string strCNN = string.Empty;

            strCNN = "Provider=Microsoft.ACE.OleDb.12.0;Data Source = " + uploadPath + ";Extended Properties = 'Excel 12.0;HDR=Yes;IMEX=1;'";

            OleDbConnection cnn = new OleDbConnection(strCNN);
            try
            {
                cnn.Open();
            }
            catch (Exception ex)
            {
                cnn.Dispose();
                logger.Debug(ex);

                return ex.Message;
            }

            DataTable schemaTable = cnn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });

            string tableName = string.Empty;
            foreach (DataRow dr in schemaTable.Rows)
            {
                tableName = dr[2].ToString().Trim();
                if (tableName.IndexOf("_FilterDatabase") < 0)
                    break;
            }
            if (tableName.StartsWith("'"))
            {
                tableName = tableName.Replace("'", "");
                tableName = tableName.EndsWith("$") == false ? tableName + "$" : tableName;
                tableName = "'" + tableName + "'";
            }
            else
            {
                tableName = tableName.EndsWith("$") == false ? tableName + "$" : tableName;
            }

            string strSQL = " SELECT * FROM [" + tableName + "]";

            OleDbDataAdapter cmd = new OleDbDataAdapter(strSQL, cnn);
            cmd.Fill(dt);
            cnn.Close();
            if (dt == null || dt.Rows.Count < 1 || dt.Columns.Count < count)
            {
                return "The template is incorrect.";
            }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.Trim();
            }


            try
            {
                switch (type)
                {
                    case TxkeaBupType.TxkeaReceptive:
                        if (!ValidReceptiveExcel(dt))
                            return "The template is incorrect.";
                        break;
                    case TxkeaBupType.TxkeaExpressive:
                        if (!ValidExpressiveExcel(dt))
                            return "The template is incorrect.";
                        break;
                    default:
                        return "Can not find action type.";
                }

            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                return ex.Message;
            }
            return string.Empty;
        }

        private bool ValidReceptiveExcel(DataTable dt)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string columnName = dt.Columns[i].ColumnName.Trim();
                if (!str_receptiveHeaders.Contains(columnName))
                {
                    return false;
                }
            }
            return true;
        }

        private bool ValidExpressiveExcel(DataTable dt)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string columnName = dt.Columns[i].ColumnName.Trim();
                if (!str_expressiveHeaders.Contains(columnName))
                {
                    return false;
                }
            }
            return true;
        }

        public string ProcessReceptiveItem(DataTable dt, int userId, string originFileName, string filePath,
            int AssessmentId, int MeasureId, string ResourcePath)
        {
            string physicPath = SFConfig.TxkeaResource + ResourcePath + "/".Replace("\\", "/");
            TxkeaBupTaskEntity taskEntity = new TxkeaBupTaskEntity();
            taskEntity.Logs = new List<TxkeaBupLogEntity>();
            taskEntity.AssessmentId = AssessmentId;
            taskEntity.MeasureId = MeasureId;
            taskEntity.Type = TxkeaBupType.TxkeaReceptive;
            taskEntity.Status = TxkeaBupStatus.Inprogress;
            taskEntity.OriginFileName = originFileName;
            taskEntity.FilePath = filePath;
            taskEntity.IsDeleted = false;
            taskEntity.ResourcePath = ResourcePath;
            taskEntity.CreatedBy = userId;
            taskEntity.UpdatedBy = userId;
            taskEntity.Remark = string.Empty;


            List<TxkeaReceptiveItemModel> itemEntities = new List<TxkeaReceptiveItemModel>();
            List<TxkeaLayoutModel> layouts = _adeContract.TxkeaLayouts.Where(r => r.IsDeleted == false)
                .Select(x => new TxkeaLayoutModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    Layout = x.Layout,
                    NumberOfImages = x.NumberOfImages,
                    ScreenWidth = x.ScreenWidth
                }).ToList();
            List<SelectItemModel> existedItems = _adeContract.Items.Where(r => r.IsDeleted == false)
                .Select(x => new SelectItemModel
                {
                    ID = x.ID,
                    Name = x.Label
                }).ToList();

            int itemStartIndex = 0;  //记录新的Item开始索引
            int recordCount = dt.Rows.Count;
            while (recordCount > 0 && itemStartIndex < recordCount)
            {
                TxkeaReceptiveItemModel model = new TxkeaReceptiveItemModel();
                model.Answers = new List<AnswerEntity>();
                model.MeasureId = MeasureId;
                model.ItemLayout = string.Empty;
                model.CreatedBy = userId;
                model.UpdatedBy = userId;
                model.Status = EntityStatus.Inactive;

                //新的Item图片数量
                int.TryParse(GetData(dt.Rows[itemStartIndex][str_receptiveHeaders[9]]), out outValue);
                int imageCount = outValue == 0 ? 1 : outValue;
                //新的Item名称
                string itemLabel = GetData(dt.Rows[itemStartIndex][str_receptiveHeaders[0]]);
                int selectableStartIndex = -1; //item 第一个ImageType 为 selectable 的索引

                for (int i = 0; i < imageCount; i++)  //将图片数量作为分组依据进行转换
                {
                    if (itemStartIndex + i >= dt.Rows.Count)
                    {
                        AddLog(taskEntity, itemStartIndex + 2, model.Label, "Number_Images is incorrect");
                        break;
                    }
                    else
                    {
                        AnswerEntity answer = new AnswerEntity();
                        DataRow dr = dt.Rows[itemStartIndex + i];
                        model.Label = itemLabel;
                        if (i == 0)  //同一个Item只保留一条基本信息
                        {
                            if (string.IsNullOrEmpty(model.Label))
                                AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Item_Name is incorrect");
                            else
                            {
                                if (itemEntities.Find(r => r.Label == itemLabel) != null || existedItems.Find(r => r.Name == itemLabel) != null)
                                    AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Item_Name already exists");
                            }

                            model.IsPractice = GetLowerData(dr[str_receptiveHeaders[1]]) == yesValue ? true : false;

                            model.Description = string.Empty;

                            string bgColor = GetData(dr[str_receptiveHeaders[3]]);
                            string bgImage = GetData(dr[str_receptiveHeaders[4]]);
                            model.BackgroundFill = bgColor;
                            model.BackgroundFillType = BackgroundFillType.Color;
                            if (!string.IsNullOrEmpty(bgImage))
                            {
                                if (File.Exists(physicPath + bgImage))
                                {
                                    if (ValidateFile(physicPath + bgImage, true, taskEntity,
                                        itemStartIndex + i + 2, model.Label, "Background_Image_Name"))
                                    {
                                        string bgImagePath = "assessment_" + AssessmentId + "/" + GetServerFileName(userId, physicPath + bgImage);
                                        if (!Directory.Exists(SFConfig.UploadFile + "assessment_" + AssessmentId))
                                            Directory.CreateDirectory(SFConfig.UploadFile + "assessment_" + AssessmentId);
                                        File.Copy(physicPath + bgImage, SFConfig.UploadFile + bgImagePath);

                                        model.BackgroundFill = bgImagePath;
                                        model.BackgroundFillType = BackgroundFillType.Image;
                                    }
                                }
                                else
                                    AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Incorrect path to Background_Image_Name");
                            }

                            model.InstructionAudio = GetData(dr[str_receptiveHeaders[5]]);
                            if (!string.IsNullOrEmpty(model.InstructionAudio))
                            {
                                if (File.Exists(physicPath + model.InstructionAudio))
                                {
                                    if (ValidateFile(physicPath + model.InstructionAudio, false, taskEntity,
                                        itemStartIndex + i + 2, model.Label, "Instruction_Audio_Name"))
                                    {
                                        string instructionPath = "assessment_" + AssessmentId + "/" + GetServerFileName(userId, physicPath + model.InstructionAudio);
                                        if (!Directory.Exists(SFConfig.UploadFile + "assessment_" + AssessmentId))
                                            Directory.CreateDirectory(SFConfig.UploadFile + "assessment_" + AssessmentId);
                                        File.Copy(physicPath + model.InstructionAudio, SFConfig.UploadFile + instructionPath);
                                        model.InstructionAudio = instructionPath;
                                    }
                                }
                                else
                                    AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Incorrect path to Instruction_Audio_Name");
                            }

                            model.InstructionText = GetData(dr[str_receptiveHeaders[6]]);

                            model.NumberOfImages = imageCount;
                            if (model.NumberOfImages < 0 || model.NumberOfImages > 9)
                                AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Number_Images is incorrect");

                            model.SelectionType = GetData(dr[str_receptiveHeaders[10]]) == SelectionType.MultiSelect.ToDescription()
                                ? SelectionType.MultiSelect : SelectionType.SingleSelect;

                            model.ImageSequence = GetData(dr[str_receptiveHeaders[11]]) == OrderType.Random.ToDescription() ?
                                OrderType.Random : OrderType.Sequenced;

                            model.OverallTimeOut = GetLowerData(dr[str_receptiveHeaders[12]]) == yesValue ? true : false;

                            if (model.OverallTimeOut)
                            {
                                if (!IfIntCorrect(GetData(dr[str_receptiveHeaders[13]])))
                                    AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Timeout_Value is incorrect");
                                else
                                {
                                    if (0 <= outValue && outValue < 100)
                                        outValue = 100;
                                    if (outValue > 30000)
                                        outValue = 30000;
                                    model.TimeoutValue = (outValue / 100) * 100;
                                }
                            }
                            else
                                model.TimeoutValue = 0;



                            string breakCondition = GetData(dr[str_receptiveHeaders[14]]);
                            model.BreakCondition = BreakCondition.IncorrectResponse;
                            if (breakCondition == BreakCondition.StopCondition.ToDescription())
                                model.BreakCondition = BreakCondition.StopCondition;
                            if (breakCondition == BreakCondition.None.ToDescription())
                                model.BreakCondition = BreakCondition.None;

                            int.TryParse(GetData(dr[str_receptiveHeaders[15]]), out outValue);
                            model.StopConditionX = outValue;

                            int.TryParse(GetData(dr[str_receptiveHeaders[16]]), out outValue);
                            model.StopConditionY = outValue;

                            if (model.BreakCondition == BreakCondition.StopCondition)
                            {
                                if (model.StopConditionX < 1 || model.StopConditionY < 1 || model.StopConditionX > model.StopConditionY)
                                    AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Please input correct Stop Condition values");
                            }
                            else
                            {
                                model.StopConditionX = 0;
                                model.StopConditionY = 0;
                            }

                            model.Scoring = GetData(dr[str_receptiveHeaders[17]]) == ScoringType.Partial.ToDescription() ?
                                ScoringType.Partial : ScoringType.AllorNone;

                            string layoutName = GetData(dr[str_receptiveHeaders[18]]);
                            var inputLayout = layouts.Find(r => r.Name == layoutName && r.NumberOfImages == model.NumberOfImages);
                            model.LayoutId = inputLayout == null ? 0 : inputLayout.ID;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(GetData(dr[str_receptiveHeaders[0]])) && GetData(dr[str_receptiveHeaders[0]]) != itemLabel)
                                AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Item_Name is different from the previous one");
                        }

                        answer.ImageType = GetData(dr[str_receptiveHeaders[19]]) == ImageType.NonSelectable.ToDescription() ?
                            ImageType.NonSelectable : ImageType.Selectable;

                        answer.Picture = GetData(dr[str_receptiveHeaders[20]]);
                        if (!string.IsNullOrEmpty(answer.Picture))
                        {
                            if (File.Exists(physicPath + answer.Picture))
                            {
                                if (ValidateFile(physicPath + answer.Picture, true, taskEntity,
                                        itemStartIndex + i + 2, model.Label, "Target_Image_Name"))
                                {
                                    string answerPicturePath = "assessment_" + AssessmentId + "/" + GetServerFileName(userId, physicPath + answer.Picture);
                                    if (!Directory.Exists(SFConfig.UploadFile + "assessment_" + AssessmentId))
                                        Directory.CreateDirectory(SFConfig.UploadFile + "assessment_" + AssessmentId);
                                    File.Copy(physicPath + answer.Picture, SFConfig.UploadFile + answerPicturePath);
                                    answer.Picture = answerPicturePath;
                                    ConvertTimeout(GetData(dr[str_receptiveHeaders[21]]), taskEntity, itemStartIndex + i + 2, model.Label, "Image_Time_Delay is incorrect");
                                    answer.PictureTime = outValue;
                                }
                            }
                            else
                                AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Incorrect path to Target_Image_Name");
                        }


                        answer.Audio = GetData(dr[str_receptiveHeaders[22]]);
                        if (!string.IsNullOrEmpty(answer.Audio))
                        {
                            if (File.Exists(physicPath + answer.Audio))
                            {
                                if (ValidateFile(physicPath + answer.Audio, false, taskEntity,
                                        itemStartIndex + i + 2, model.Label, "Target_Audio_Name"))
                                {
                                    string answerAudioPath = "assessment_" + AssessmentId + "/" + GetServerFileName(userId, physicPath + answer.Audio);
                                    if (!Directory.Exists(SFConfig.UploadFile + "assessment_" + AssessmentId))
                                        Directory.CreateDirectory(SFConfig.UploadFile + "assessment_" + AssessmentId);
                                    File.Copy(physicPath + answer.Audio, SFConfig.UploadFile + answerAudioPath);
                                    answer.Audio = answerAudioPath;
                                }
                            }
                            else
                                AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Incorrect path to Target_Audio_Name");
                        }

                        if (string.IsNullOrEmpty(answer.Picture) && string.IsNullOrEmpty(answer.Audio))
                            AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Please upload at least one image or audio");

                        if (string.IsNullOrEmpty(answer.Audio))
                            answer.AudioTime = 0;
                        else
                        {
                            //Audio_Time_Delay_Same
                            if (GetLowerData(dr[str_receptiveHeaders[28]]) == yesValue)
                                answer.AudioTime = answer.PictureTime;
                            else
                            {
                                ConvertTimeout(GetData(dr[str_receptiveHeaders[23]]), taskEntity, itemStartIndex + i + 2, model.Label, "Audio_Time_Delay is incorrect");
                                answer.AudioTime = outValue;
                            }
                        }

                        answer.IsCorrect = GetLowerData(dr[str_receptiveHeaders[24]]) == yesValue ? true : false;

                        if (!IfDecimalCorrect(GetData(dr[str_receptiveHeaders[25]])))
                            AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Score is incorrect ");
                        else
                            answer.Score = outDecimalValue;

                        if (!IfIntCorrect(GetData(dr[str_receptiveHeaders[26]])))
                            AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Sequence_Number is incorrect ");
                        answer.SequenceNumber = outValue;

                        answer.ResponseAudio = GetData(dr[str_receptiveHeaders[27]]);
                        if (!string.IsNullOrEmpty(answer.ResponseAudio))
                        {
                            if (File.Exists(physicPath + answer.ResponseAudio))
                            {
                                if (ValidateFile(physicPath + answer.ResponseAudio, false, taskEntity,
                                        itemStartIndex + i + 2, model.Label, "Response_Audio_Name"))
                                {
                                    string answerResponseAudioPath = "assessment_" + AssessmentId + "/" + GetServerFileName(userId, physicPath + answer.ResponseAudio);
                                    if (!Directory.Exists(SFConfig.UploadFile + "assessment_" + AssessmentId))
                                        Directory.CreateDirectory(SFConfig.UploadFile + "assessment_" + AssessmentId);
                                    File.Copy(physicPath + answer.ResponseAudio, SFConfig.UploadFile + answerResponseAudioPath);
                                    answer.ResponseAudio = answerResponseAudioPath;
                                }
                            }
                            else
                                AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Incorrect path to Response_Audio_Name");
                        }

                        if (answer.ImageType == ImageType.NonSelectable)
                        {
                            answer.IsCorrect = false;
                            answer.Score = 0;
                            answer.SequenceNumber = 0;
                        }
                        else
                        {
                            if (selectableStartIndex < 0)
                                selectableStartIndex = i;
                        }

                        model.Answers.Add(answer);
                    }
                }

                int correctAnswersCount = model.Answers.Where(r => r.IsCorrect == true).Count();
                //验证正确答案数量
                if (model.SelectionType == SelectionType.SingleSelect)
                {
                    if (correctAnswersCount == 0)
                        AddLog(taskEntity, itemStartIndex + 2, model.Label, "Please specify the correct answer(s)");

                    if (correctAnswersCount > 1)
                        AddLog(taskEntity, itemStartIndex + 2, model.Label, "Please have only one correct answer");
                }
                if (model.SelectionType == SelectionType.MultiSelect)
                {
                    if (correctAnswersCount < 2)
                        AddLog(taskEntity, itemStartIndex + 2, model.Label, "Please have multiple correct answers");
                }

                //验证可选图片之间不能穿插不可选图片
                int selectableAnswersLength = model.Answers.Where(r => r.ImageType == ImageType.Selectable).Count();
                if (selectableAnswersLength > 1)
                {
                    List<AnswerEntity> answers = model.Answers.ToList();
                    for (int i = selectableStartIndex; i < selectableAnswersLength + selectableStartIndex; i++)
                    {
                        if (answers[i].ImageType != ImageType.Selectable)
                        {
                            AddLog(taskEntity, itemStartIndex + 2, model.Label, "All selectable answers should be grouped together in blocks");
                            break;
                        }
                    }
                }

                itemEntities.Add(model);
                itemStartIndex += imageCount;
            }

            OperationResult result = new OperationResult(OperationResultType.Success);

            //验证不通过时，状态设置为错误
            if (taskEntity.Logs.Count > 0)
                taskEntity.Status = TxkeaBupStatus.Error;
            taskEntity.NumberOfItems = itemEntities.Count;
            result = _adeContract.InsertTask(taskEntity);
            if (result.ResultType == OperationResultType.Success)
            {
                if (taskEntity.Logs.Count == 0 && itemEntities.Count > 0)  // 验证通过添加item
                {
                    //获取当前measure下的最大排序值（可能会重复）
                    int currentSort = _adeContract.Items.Where(x => x.MeasureId == MeasureId).Select(x => x.Sort)
                    .OrderByDescending(x => x).FirstOrDefault() + 1;
                    List<ItemBaseEntity> itemBaseEntities = new List<ItemBaseEntity>();
                    foreach (TxkeaReceptiveItemModel item in itemEntities)
                    {
                        ItemBaseEntity entity = _adeContract.NewItemBaseEntity(ItemType.TxkeaReceptive);
                        item.UpdateEntity(entity);
                        item.Answers.ForEach(x => entity.Answers.Add(x));
                        entity.Sort = currentSort;
                        itemBaseEntities.Add(entity);
                        currentSort += 1;
                    }
                    result = _adeContract.InsertItem(itemBaseEntities, true);
                    if (result.ResultType != OperationResultType.Success)
                    {
                        taskEntity.Status = TxkeaBupStatus.Error;
                        taskEntity.Remark = result.Message;
                        _adeContract.UpdateTask(taskEntity);
                        return result.Message;
                    }
                    else
                    {
                        //若填写了Layout，则更改ItemLayout
                        itemBaseEntities = itemBaseEntities.Where(r => ((TxkeaReceptiveItemEntity)r).LayoutId > 0).ToList();
                        if (itemBaseEntities.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("BEGIN TRY ;");
                            sb.Append("BEGIN TRANSACTION;");
                            foreach (ItemBaseEntity item in itemBaseEntities)
                            {
                                string layoutContent = string.Empty;
                                var inputLayout = layouts.Find(r => r.ID == ((TxkeaReceptiveItemEntity)item).LayoutId);
                                layoutContent = inputLayout == null ? "" : inputLayout.Layout;
                                if (!string.IsNullOrEmpty(layoutContent))
                                {
                                    List<AnswerEntity> answers = item.Answers.ToList();
                                    for (int i = 0; i < answers.Count; i++)
                                    {
                                        string preSrc = SFConfig.AssessmentDomain + "Content/images/layout_" + (i + 1) + ".jpg?id=" + (i + 1);
                                        string curSrc = string.IsNullOrEmpty(answers[i].Picture) ?
                                            //若没有图片，则换成占位符
                                            SFConfig.StaticDomain + "content/images/layoutPlaceholder.png?id=" + answers[i].ID + "&sort=" + i :
                                            SFConfig.StaticDomain + "Upload/" + answers[i].Picture + "?id=" + answers[i].ID + "&sort=" + i;

                                        layoutContent = layoutContent.Replace(preSrc, curSrc)
                                            .Replace("\"id\":\"" + (i + 1) + "\"", "\"id\":\"" + answers[i].ID + "\"")
                                            .Replace("\"sort\":\"0\"", "\"sort\":\"" + i + "\"");
                                    }
                                    sb.AppendFormat(";UPDATE TxkeaReceptiveItems SET ItemLayout='{0}',ScreenWidth={1} WHERE ID={2}",
                                        layoutContent, inputLayout.ScreenWidth, item.ID);
                                }
                            }

                            sb.Append(";SELECT '1' ;COMMIT TRANSACTION;   ")
                           .Append(" END TRY ")
                           .Append(" BEGIN CATCH ;  ")
                           .Append("IF @@TRANCOUNT > 0  BEGIN ROLLBACK TRANSACTION; END")
                           .Append(" SELECT ERROR_MESSAGE() ;")
                           .Append(" END CATCH;");

                            _adeContract.ExecuteQuerySql(sb.ToString());
                        }

                        taskEntity.Status = TxkeaBupStatus.Completed;
                        _adeContract.UpdateTask(taskEntity);
                    }
                }
            }
            else
                return result.Message;

            return string.Empty;
        }

        public string ProcessExpressiveItem(DataTable dt, int userId, string originFileName, string filePath,
            int AssessmentId, int MeasureId, string ResourcePath)
        {
            string physicPath = SFConfig.TxkeaResource + ResourcePath + "/".Replace("\\", "/");
            TxkeaBupTaskEntity taskEntity = new TxkeaBupTaskEntity();
            taskEntity.Logs = new List<TxkeaBupLogEntity>();
            taskEntity.AssessmentId = AssessmentId;
            taskEntity.MeasureId = MeasureId;
            taskEntity.Type = TxkeaBupType.TxkeaExpressive;
            taskEntity.Status = TxkeaBupStatus.Inprogress;
            taskEntity.OriginFileName = originFileName;
            taskEntity.FilePath = filePath;
            taskEntity.IsDeleted = false;
            taskEntity.ResourcePath = ResourcePath;
            taskEntity.CreatedBy = userId;
            taskEntity.UpdatedBy = userId;
            taskEntity.Remark = string.Empty;


            List<TxkeaExpressiveItemEntity> itemEntities = new List<TxkeaExpressiveItemEntity>();
            List<TxkeaLayoutModel> layouts = _adeContract.TxkeaLayouts.Where(r => r.IsDeleted == false)
                .Select(x => new TxkeaLayoutModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    Layout = x.Layout,
                    NumberOfImages = x.NumberOfImages,
                    ScreenWidth = x.ScreenWidth
                }).ToList();
            List<SelectItemModel> existedItems = _adeContract.Items.Where(r => r.IsDeleted == false)
                .Select(x => new SelectItemModel
                {
                    ID = x.ID,
                    Name = x.Label
                }).ToList();

            int itemStartIndex = 0;  //记录新的Item开始索引
            int recordCount = dt.Rows.Count;
            while (recordCount > 0 && itemStartIndex < recordCount)
            {
                TxkeaExpressiveItemEntity model = new TxkeaExpressiveItemEntity();
                model.ImageList = new List<TxkeaExpressiveImageEntity>();
                model.Responses = new List<TxkeaExpressiveResponseEntity>();
                model.MeasureId = MeasureId;
                model.ItemLayout = string.Empty;
                model.CreatedBy = userId;
                model.UpdatedBy = userId;
                model.Status = EntityStatus.Inactive;
                model.Type = ItemType.TxkeaExpressive;
                model.CpallsItemLayout = string.Empty;

                //新的Item图片数量
                int.TryParse(GetData(dt.Rows[itemStartIndex][str_expressiveHeaders[8]]), out outValue);
                int imageCount = outValue == 0 ? 1 : outValue;
                //新的Item名称
                string itemLabel = GetData(dt.Rows[itemStartIndex][str_expressiveHeaders[0]]);

                for (int i = 0; i < imageCount; i++)  //将图片数量作为分组依据进行转换
                {
                    TxkeaExpressiveImageEntity image = new TxkeaExpressiveImageEntity();
                    TxkeaExpressiveResponseEntity response = new TxkeaExpressiveResponseEntity();

                    if (itemStartIndex + i >= dt.Rows.Count)
                    {
                        AddLog(taskEntity, itemStartIndex + 2, model.Label, "Number_Images is incorrect");
                        break;
                    }
                    else
                    {
                        DataRow dr = dt.Rows[itemStartIndex + i];
                        model.Label = itemLabel;
                        if (i == 0)  //同一个Item只保留一条基本信息
                        {
                            if (string.IsNullOrEmpty(model.Label))
                                AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Item_Name is incorrect");
                            else
                            {
                                if (itemEntities.Find(r => r.Label == itemLabel) != null || existedItems.Find(r => r.Name == itemLabel) != null)
                                    AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Item_Name already exists");
                            }
                            model.IsPractice = GetLowerData(dr[str_expressiveHeaders[1]]) == yesValue ? true : false;
                            model.Description = string.Empty;

                            string bgColor = GetData(dr[str_expressiveHeaders[3]]);
                            string bgImage = GetData(dr[str_expressiveHeaders[4]]);
                            model.BackgroundFill = bgColor;
                            model.BackgroundFillType = BackgroundFillType.Color;
                            if (!string.IsNullOrEmpty(bgImage))
                            {
                                if (File.Exists(physicPath + bgImage))
                                {
                                    if (ValidateFile(physicPath + bgImage, true, taskEntity,
                                        itemStartIndex + i + 2, model.Label, "Background_Image_Name"))
                                    {
                                        string bgImagePath = "assessment_" + AssessmentId + "/" + GetServerFileName(userId, physicPath + bgImage);
                                        if (!Directory.Exists(SFConfig.UploadFile + "assessment_" + AssessmentId))
                                            Directory.CreateDirectory(SFConfig.UploadFile + "assessment_" + AssessmentId);
                                        File.Copy(physicPath + bgImage, SFConfig.UploadFile + bgImagePath);

                                        model.BackgroundFill = bgImagePath;
                                        model.BackgroundFillType = BackgroundFillType.Image;
                                    }
                                }
                                else
                                    AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Incorrect path to Background_Image_Name");
                            }

                            model.InstructionText = GetData(dr[str_expressiveHeaders[5]]);

                            model.InstructionAudio = GetData(dr[str_expressiveHeaders[6]]);
                            if (!string.IsNullOrEmpty(model.InstructionAudio))
                            {
                                if (File.Exists(physicPath + model.InstructionAudio))
                                {
                                    if (ValidateFile(physicPath + model.InstructionAudio, false, taskEntity,
                                        itemStartIndex + i + 2, model.Label, "Instruction_Audio_Name"))
                                    {
                                        string instructionPath = "assessment_" + AssessmentId + "/" + GetServerFileName(userId, physicPath + model.InstructionAudio);
                                        if (!Directory.Exists(SFConfig.UploadFile + "assessment_" + AssessmentId))
                                            Directory.CreateDirectory(SFConfig.UploadFile + "assessment_" + AssessmentId);
                                        File.Copy(physicPath + model.InstructionAudio, SFConfig.UploadFile + instructionPath);
                                        model.InstructionAudio = instructionPath;
                                    }
                                }
                                else
                                    AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Incorrect path to Instruction_Audio_Name");
                            }

                            ConvertTimeout(GetData(dr[str_expressiveHeaders[7]]), taskEntity, itemStartIndex + i + 2, model.Label, "Audio_Delay is incorrect");
                            model.InstructionAudioTimeDelay = outValue;

                            model.Images = imageCount;
                            if (model.Images < 0 || model.Images > 9)
                                AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Number_Images is incorrect");

                            model.Timed = GetLowerData(dr[str_expressiveHeaders[32]]) == yesValue ? true : false;

                            if (model.Timed)
                            {
                                if (!IfIntCorrect(GetData(dr[str_expressiveHeaders[33]])))
                                    AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Timeout_Value is incorrect");
                                else
                                {
                                    if (0 <= outValue && outValue < 100)
                                        outValue = 100;
                                    if (outValue > 30000)
                                        outValue = 30000;
                                    model.Timeoutvalue = (outValue / 100) * 100;
                                }
                            }
                            else
                                model.Timeoutvalue = 0;

                            if (GetData(dr[str_expressiveHeaders[34]]).ToLower() == "simple")
                                model.ResponseType = TxkeaExpressiveResponoseType.Simple;
                            else
                                model.ResponseType = TxkeaExpressiveResponoseType.Detailed;

                            string layoutName = GetData(dr[str_expressiveHeaders[9]]);
                            var inputLayout = layouts.Find(r => r.Name == layoutName && r.NumberOfImages == model.Images);
                            model.LayoutId = inputLayout == null ? 0 : inputLayout.ID;

                            if (model.ResponseType == TxkeaExpressiveResponoseType.Detailed)
                            {
                                //两个Response
                                string bgResponseColor = GetData(dr[str_expressiveHeaders[14]]);
                                string bgResponseImage = GetData(dr[str_expressiveHeaders[15]]);
                                model.ResponseBackgroundFill = bgResponseColor;
                                model.ResponseBackgroundFillType = BackgroundFillType.Color;
                                if (!string.IsNullOrEmpty(bgResponseImage))
                                {
                                    if (File.Exists(physicPath + bgResponseImage))
                                    {
                                        if (ValidateFile(physicPath + bgResponseImage, true, taskEntity,
                                            itemStartIndex + i + 2, model.Label, "Response_Background_Image_Name"))
                                        {
                                            string bgResponseImagePath = "assessment_" + AssessmentId + "/" + GetServerFileName(userId, physicPath + bgResponseImage);
                                            if (!Directory.Exists(SFConfig.UploadFile + "assessment_" + AssessmentId))
                                                Directory.CreateDirectory(SFConfig.UploadFile + "assessment_" + AssessmentId);
                                            File.Copy(physicPath + bgResponseImage, SFConfig.UploadFile + bgResponseImagePath);

                                            model.ResponseBackgroundFill = bgResponseImagePath;
                                            model.ResponseBackgroundFillType = BackgroundFillType.Image;
                                        }
                                    }
                                    else
                                        AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Incorrect path to Response_Background_Image_Name");
                                }
                                //Response1
                                response.Text = GetData(dr[str_expressiveHeaders[16]]);
                                if (string.IsNullOrEmpty(response.Text))
                                    AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Response_Text is incorrect");

                                response.Mandatory = GetLowerData(dr[str_expressiveHeaders[17]]) == noValue ? false : true;

                                string responseType = GetData(dr[str_expressiveHeaders[18]]).Trim().ToLower();
                                response.Type = TypedResponseType.Radionbox;
                                if (responseType == "checkbox")
                                    response.Type = TypedResponseType.Checkbox;
                                if (responseType == "textbox")
                                    response.Type = TypedResponseType.Text;

                                int.TryParse(GetData(dr[str_expressiveHeaders[19]]), out outValue);
                                response.Buttons = outValue;
                                if (response.Buttons < 2 || response.Buttons > 6)
                                    AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Number_of_Radio/Check_Buttons is incorrect");
                                else
                                {
                                    response.Options = new List<TxkeaExpressiveResponseOptionEntity>();
                                    for (int y = 1; y < response.Buttons + 1; y++)
                                    {
                                        string label = GetData(dr["Option" + y]);
                                        if (string.IsNullOrEmpty(label))
                                        {
                                            AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Option" + y + " is incorrect");
                                            break;
                                        }
                                        else
                                        {
                                            if (response.Type == TypedResponseType.Checkbox || response.Type == TypedResponseType.Radionbox)
                                            {
                                                if (!IfDecimalCorrect(GetData(dr["Score_Option" + y])))
                                                {
                                                    AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Score_Option" + y + " is incorrect");
                                                    break;
                                                }
                                                else
                                                    response.Options.Add(new TxkeaExpressiveResponseOptionEntity()
                                                    {
                                                        Lable = label,
                                                        AddTextbox = GetLowerData(dr["Add_Text_Option" + y]) == yesValue ? true : false,
                                                        Score = outDecimalValue
                                                    });
                                            }
                                            else
                                            {
                                                response.Options.Add(new TxkeaExpressiveResponseOptionEntity()
                                                {
                                                    Lable = label,
                                                    AddTextbox = GetLowerData(dr["Add_Text_Option" + y]) == yesValue ? true : false
                                                });
                                            }
                                        }
                                    }
                                }
                                model.Responses.Add(response);

                                //Response2
                                TxkeaExpressiveResponseEntity response2 = new TxkeaExpressiveResponseEntity();
                                string responseText2 = GetData(dr[str_expressiveHeaders[43]]);
                                if (!string.IsNullOrEmpty(responseText2))
                                {
                                    response2.Text = responseText2;
                                    response2.Mandatory = GetLowerData(dr[str_expressiveHeaders[44]]) == noValue ? false : true;

                                    string responseType2 = GetData(dr[str_expressiveHeaders[45]]).Trim().ToLower();
                                    response2.Type = TypedResponseType.Radionbox;
                                    if (responseType2 == "checkbox")
                                        response2.Type = TypedResponseType.Checkbox;
                                    if (responseType2 == "textbox")
                                        response2.Type = TypedResponseType.Text;

                                    int.TryParse(GetData(dr[str_expressiveHeaders[46]]), out outValue);
                                    response2.Buttons = outValue;
                                    if (response2.Buttons < 2 || response2.Buttons > 6)
                                        AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "S_Number_of_Radio/Check_Buttons is incorrect");
                                    else
                                    {
                                        response2.Options = new List<TxkeaExpressiveResponseOptionEntity>();
                                        for (int y = 1; y < response2.Buttons + 1; y++)
                                        {
                                            string label = GetData(dr["Second_Option" + y]);
                                            if (string.IsNullOrEmpty(label))
                                            {
                                                AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Second_Option" + y + " is incorrect");
                                                break;
                                            }
                                            else
                                            {
                                                response2.Options.Add(new TxkeaExpressiveResponseOptionEntity()
                                                {
                                                    Lable = label,
                                                    AddTextbox = GetLowerData(dr["Second_Text_Option" + y]) == yesValue ? true : false
                                                });
                                            }
                                        }
                                    }
                                    model.Responses.Add(response2);
                                }
                                //Response2 End
                            }
                            else
                            {
                                model.ResponseBackgroundFill = string.Empty;
                                model.ResponseBackgroundFillType = BackgroundFillType.Color;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(GetData(dr[str_expressiveHeaders[0]])) && GetData(dr[str_expressiveHeaders[0]]) != itemLabel)
                                AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Item_Name is different from the previous one");
                        }

                        image.TargetImage = GetData(dr[str_expressiveHeaders[10]]);
                        if (!string.IsNullOrEmpty(image.TargetImage))
                        {
                            if (File.Exists(physicPath + image.TargetImage))
                            {
                                if (ValidateFile(physicPath + image.TargetImage, true, taskEntity,
                                        itemStartIndex + i + 2, model.Label, "Target_Image_Name"))
                                {
                                    string answerPicturePath = "assessment_" + AssessmentId + "/" + GetServerFileName(userId, physicPath + image.TargetImage);
                                    if (!Directory.Exists(SFConfig.UploadFile + "assessment_" + AssessmentId))
                                        Directory.CreateDirectory(SFConfig.UploadFile + "assessment_" + AssessmentId);
                                    File.Copy(physicPath + image.TargetImage, SFConfig.UploadFile + answerPicturePath);
                                    image.TargetImage = answerPicturePath;
                                }
                            }
                            else
                                AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Incorrect path to Target_Image_Name");
                        }

                        if (string.IsNullOrEmpty(image.TargetImage))
                            image.ImageTimeDelay = 0;
                        else
                        {
                            ConvertTimeout(GetData(dr[str_expressiveHeaders[11]]), taskEntity, itemStartIndex + i + 2, model.Label, "Image_Time_Delay is incorrect");
                            image.ImageTimeDelay = outValue;
                        }

                        image.TargetAudio = GetData(dr[str_expressiveHeaders[12]]);
                        if (!string.IsNullOrEmpty(image.TargetAudio))
                        {
                            if (File.Exists(physicPath + image.TargetAudio))
                            {
                                if (ValidateFile(physicPath + image.TargetAudio, false, taskEntity,
                                        itemStartIndex + i + 2, model.Label, "Target_Audio_Name"))
                                {
                                    string answerAudioPath = "assessment_" + AssessmentId + "/" + GetServerFileName(userId, physicPath + image.TargetAudio);
                                    if (!Directory.Exists(SFConfig.UploadFile + "assessment_" + AssessmentId))
                                        Directory.CreateDirectory(SFConfig.UploadFile + "assessment_" + AssessmentId);
                                    File.Copy(physicPath + image.TargetAudio, SFConfig.UploadFile + answerAudioPath);
                                    image.TargetAudio = answerAudioPath;
                                }
                            }
                            else
                                AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Incorrect path to Target_Audio_Name");
                        }

                        if (string.IsNullOrEmpty(image.TargetImage) && string.IsNullOrEmpty(image.TargetAudio))
                            AddLog(taskEntity, itemStartIndex + i + 2, model.Label, "Please upload at least one image or audio");

                        image.SameasImageDelay = GetLowerData(dr[str_expressiveHeaders[35]]) == yesValue;

                        if (string.IsNullOrEmpty(image.TargetAudio))
                            image.AudioTimeDelay = 0;
                        else
                        {
                            //Image_Delay_Same
                            if (image.SameasImageDelay)
                                image.AudioTimeDelay = image.ImageTimeDelay;
                            else
                            {
                                ConvertTimeout(GetData(dr[str_expressiveHeaders[13]]), taskEntity, itemStartIndex + i + 2, model.Label, "Audio_Time_Delay is incorrect");
                                image.AudioTimeDelay = outValue;
                            }
                        }

                        model.ImageList.Add(image);
                    }
                }
                itemEntities.Add(model);
                itemStartIndex += imageCount;
            }

            OperationResult result = new OperationResult(OperationResultType.Success);

            //验证不通过时，状态设置为错误
            if (taskEntity.Logs.Count > 0)
                taskEntity.Status = TxkeaBupStatus.Error;
            taskEntity.NumberOfItems = itemEntities.Count;
            result = _adeContract.InsertTask(taskEntity);
            if (result.ResultType == OperationResultType.Success)
            {
                if (taskEntity.Logs.Count == 0 && itemEntities.Count > 0)  // 验证通过添加item
                {
                    List<ItemBaseEntity> itemBaseEntities = new List<ItemBaseEntity>();
                    //获取当前measure下的最大排序值（可能会重复）
                    int currentSort = _adeContract.Items.Where(x => x.MeasureId == MeasureId).Select(x => x.Sort)
                    .OrderByDescending(x => x).FirstOrDefault() + 1;
                    foreach (ItemBaseEntity item in itemEntities)
                    {
                        item.Sort = currentSort;
                        itemBaseEntities.Add(item);
                        currentSort += 1;
                    }
                    result = _adeContract.InsertItem(itemBaseEntities, true);
                    if (result.ResultType != OperationResultType.Success)
                    {
                        taskEntity.Status = TxkeaBupStatus.Error;
                        taskEntity.Remark = result.Message;
                        _adeContract.UpdateTask(taskEntity);
                        return result.Message;
                    }
                    else
                    {
                        //若填写了Layout，则更改ItemLayout
                        itemBaseEntities = itemBaseEntities.Where(r => ((TxkeaExpressiveItemEntity)r).LayoutId > 0).ToList();
                        if (itemBaseEntities.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("BEGIN TRY ;");
                            sb.Append("BEGIN TRANSACTION;");
                            foreach (ItemBaseEntity item in itemBaseEntities)
                            {
                                string layoutContent = string.Empty;
                                var inputLayout = layouts.Find(r => r.ID == ((TxkeaExpressiveItemEntity)item).LayoutId);
                                layoutContent = inputLayout == null ? "" : inputLayout.Layout;
                                if (!string.IsNullOrEmpty(layoutContent))
                                {
                                    List<TxkeaExpressiveImageEntity> images = ((TxkeaExpressiveItemEntity)item).ImageList.ToList();
                                    for (int i = 0; i < images.Count; i++)
                                    {
                                        string preSrc = SFConfig.AssessmentDomain + "Content/images/layout_" + (i + 1) + ".jpg?id=" + (i + 1);
                                        string curSrc = string.IsNullOrEmpty(images[i].TargetImage) ?
                                            //若没有图片，则换成占位符
                                            SFConfig.StaticDomain + "content/images/layoutPlaceholder.png?id=" + images[i].ID + "&sort=" + i :
                                            SFConfig.StaticDomain + "Upload/" + images[i].TargetImage + "?id=" + images[i].ID + "&sort=" + i;

                                        layoutContent = layoutContent.Replace(preSrc, curSrc)
                                            .Replace("\"id\":\"" + (i + 1) + "\"", "\"id\":\"" + images[i].ID + "\"")
                                            .Replace("\"sort\":\"0\"", "\"sort\":\"" + i + "\"");
                                    }
                                    sb.AppendFormat(";UPDATE TxkeaExpressiveItems SET ItemLayout='{0}',ScreenWidth={1} WHERE ID={2}",
                                        layoutContent, inputLayout.ScreenWidth, item.ID);
                                }
                            }

                            sb.Append(";SELECT '1' ;COMMIT TRANSACTION;   ")
                           .Append(" END TRY ")
                           .Append(" BEGIN CATCH ;  ")
                           .Append("IF @@TRANCOUNT > 0  BEGIN ROLLBACK TRANSACTION; END")
                           .Append(" SELECT ERROR_MESSAGE() ;")
                           .Append(" END CATCH;");

                            _adeContract.ExecuteQuerySql(sb.ToString());
                        }

                        taskEntity.Status = TxkeaBupStatus.Completed;
                        _adeContract.UpdateTask(taskEntity);
                    }
                }
            }
            else
                return result.Message;

            return string.Empty;
        }

        private static void AddLog(TxkeaBupTaskEntity taskEntity, int rowNumber, string name, string remark)
        {
            TxkeaBupLogEntity logModel = new TxkeaBupLogEntity();
            logModel.RowNumber = rowNumber;
            logModel.ItemName = name;
            logModel.Remark = remark;
            taskEntity.Logs.Add(logModel);
        }

        private string GetData(object o)
        {
            if (o is DBNull) return string.Empty;
            return o.ToString().Trim();
        }

        private string GetLowerData(object o)
        {
            if (o is DBNull) return string.Empty;
            return o.ToString().Trim().ToLower();
        }

        private string GetServerFileName(int userId, string filePath)
        {
            return encrypt.Encrypt(userId.GetHashCode().ToString()) + "_" + Guid.NewGuid().ToString().Replace("-", "")
                + Path.GetExtension(filePath);
        }

        public TxkeaBupTaskEntity GetTxkeaBupTask(int id)
        {
            return _adeContract.GetTask(id);
        }

        public TxkeaBupTaskModel GetTxkeaBupTaskModel(int id)
        {
            return _adeContract.TxkeaBupTasks.Where(r => r.ID == id)
                .Select(r => new TxkeaBupTaskModel
                {
                    ID = r.ID,
                    AssessmentId = r.AssessmentId,
                    MeasureId = r.MeasureId,
                    Type = r.Type,
                    Status = r.Status,
                    OriginFileName = r.OriginFileName,
                    FilePath = r.FilePath,
                    IsDeleted = r.IsDeleted,
                    ResourcePath = r.ResourcePath,
                    Remark = r.Remark,
                    NumberOfItems = r.NumberOfItems,
                    CreatedOn = r.CreatedOn,
                    CreatedBy = r.CreatedBy,
                    UpdatedOn = r.UpdatedOn,
                    UpdatedBy = r.UpdatedBy,
                    Logs = r.Logs
                }).FirstOrDefault();
        }

        public OperationResult UpdateBupTask(TxkeaBupTaskEntity entity)
        {
            return _adeContract.UpdateTask(entity);
        }

        private bool ValidateFile(string filePath, bool isImage, TxkeaBupTaskEntity taskEntity, int index,
            string itemName, string columnName)
        {
            FileInfo file = new FileInfo(filePath);
            if (!(isImage ? imgExtensions.Contains(file.Extension) : audioExtensions.Contains(file.Extension)))
            {
                AddLog(taskEntity, index, itemName, "Incorrect extension to " + columnName);
                return false;
            }
            else
            {
                if (file.Length >= 2097152)  //不能超过2M
                {
                    AddLog(taskEntity, index, itemName, "Incorrect size to " + columnName + "(Maximum files size: 2 MB)");
                    return false;
                }
            }
            return true;
        }

        private void ConvertTimeout(string originValue, TxkeaBupTaskEntity taskEntity, int index, string label, string msg)
        {
            if (!string.IsNullOrEmpty(originValue))
            {
                if (!int.TryParse(originValue, out outValue))
                    AddLog(taskEntity, index, label, msg);
                else
                {
                    if (outValue > 30000)
                        outValue = 30000;
                }
            }
            else
            {
                outValue = 0;
            }
        }

        private bool IfDecimalCorrect(string originValue)
        {
            if (string.IsNullOrEmpty(originValue))
            {
                outDecimalValue = 0;
                return true;
            }
            else
                return decimal.TryParse(originValue, out outDecimalValue);
        }

        private bool IfIntCorrect(string originValue)
        {
            if (string.IsNullOrEmpty(originValue))
            {
                outValue = 0;
                return true;
            }
            else
                return int.TryParse(originValue, out outValue);
        }
    }
}
