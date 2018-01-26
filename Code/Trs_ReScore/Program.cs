using System;
using System.Collections.Generic;
using System.Linq;
using Sunnet.Cli.Business.Trs;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Framework.Log;
using StructureMap;
using Sunnet.Framework.Csv;
using Sunnet.Framework;
using System.Configuration;
using System.IO;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.TRSClasses;
using Sunnet.Cli.Core.TRSClasses.Entites;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Trs;

namespace Trs_ReScore
{
    class Program
    {
        static void Main(string[] args)
        {
            IoC.Init();

            TrsBusiness trsBusiness = new TrsBusiness();
            UserBusiness userBusiness = new UserBusiness();
            SchoolBusiness schoolBusiness = new SchoolBusiness();
            TRSClassBusiness trsClassBusiness = new TRSClassBusiness();
            ISunnetLog logHelper = ObjectFactory.GetInstance<ISunnetLog>();

            DateTime createOn = DateTime.Parse("2016-09-01");
            int Super_Admin_Id = int.Parse(ConfigurationManager.AppSettings["SuperAdminUserId"]);
            try
            {
                logHelper.Info("Start processing -->");
                Console.WriteLine("Start processing -->");

                List<TRSAssessmentEntity> trsAssessments = trsBusiness.GetAssessmentByCreateDate(createOn);

                logHelper.Info("There are " + trsAssessments.Count + " TrsAssessment need check.-->");
                Console.WriteLine("There are " + trsAssessments.Count + " TrsAssessment need check.-->");

                List<TRSAssessmentEntity> changedTrsAssessments = new List<TRSAssessmentEntity>();

                Dictionary<int, int> dicOldStar = new Dictionary<int, int>();//Dictionary<TrsAssessmentId,OldStar>
                Dictionary<int, int> dicOldStarCat2 = new Dictionary<int, int>();//Dictionary<TrsAssessmentId,Category2 OldStar>
                List<TRSAssessmentItemEntity> changedTrsItems = new List<TRSAssessmentItemEntity>();
                Dictionary<int, int> dicOldItemAnswer = new Dictionary<int, int>();//Dictionary<TrsAssessmentItemId,OldAnswer>
                Dictionary<int, SchoolEntity> dicSchool = new Dictionary<int, SchoolEntity>();
                Dictionary<int, TRSClassEntity> dicTrsClass = new Dictionary<int, TRSClassEntity>();

                foreach (TRSAssessmentEntity assessment in trsAssessments)
                {
                    Console.WriteLine("***************************************************************************");
                    logHelper.Info("Begin check TrsAssessment ID:" + assessment.ID);
                    Console.WriteLine("Begin check TrsAssessment ID:" + assessment.ID);

                    List<TRSAssessmentItemEntity> items = assessment.AssessmentItems
                        .Where(e => e.AgeGroup != 0 && e.GroupSize != 0 && e.CaregiversNo != 0 && e.AnswerId != 77).ToList();
                    if (items.Count == 0)
                    {
                        logHelper.Info("TrsAssessment ID:" + assessment.ID + " no item need change.");
                        Console.WriteLine("TrsAssessment ID:" + assessment.ID + " no item need change.");
                        continue;
                    }

                    bool hasItemChanged = false;
                    foreach (TRSAssessmentItemEntity item in items)
                    {
                        Console.WriteLine();
                        logHelper.Info("Begin check item, TrsAssessment ID:" + assessment.ID + ",TrsAssessmentItem ID:" + item.ID);
                        Console.WriteLine("Begin check item, TrsAssessment ID:" + assessment.ID + ",TrsAssessmentItem ID:" + item.ID);

                        int oldAnswer = item.AnswerId == null ? 0 : item.AnswerId.Value;
                        int newAnswer = CalcItemScore(item);
                        if (oldAnswer != newAnswer)
                        {
                            hasItemChanged = true;
                            logHelper.Info("Begin update item, TrsAssessment ID:" + assessment.ID + ",TrsAssessmentItem ID:" + item.ID);
                            Console.WriteLine("Begin update item, TrsAssessment ID:" + assessment.ID + ",TrsAssessmentItem ID:" + item.ID);

                            dicOldItemAnswer.Add(item.ID, oldAnswer);
                            int count = trsBusiness.UpdateAssessmentItemAnswer(item.ID, newAnswer);
                            if (count > 0)
                            {
                                logHelper.Info("Update item success, TrsAssessment ID:" + assessment.ID + ",TrsAssessmentItem ID:" + item.ID);
                                Console.WriteLine("Update item success, TrsAssessment ID:" + assessment.ID + ",TrsAssessmentItem ID:" + item.ID);
                            }
                            else
                            {
                                logHelper.Info("Update item faild, TrsAssessment ID:" + assessment.ID + ",TrsAssessmentItem ID:" + item.ID);
                                Console.WriteLine("Update item faild, TrsAssessment ID:" + assessment.ID + ",TrsAssessmentItem ID:" + item.ID);
                            }

                            item.AnswerId = newAnswer;
                            changedTrsItems.Add(item);

                            if (!dicSchool.ContainsKey(item.Assessment.SchoolId))
                            {
                                SchoolEntity school = schoolBusiness.GetSchool(item.Assessment.SchoolId);
                                if (school != null)
                                    dicSchool.Add(school.ID, school);
                            }
                            if (!dicTrsClass.ContainsKey(item.ClassId))
                            {
                                TRSClassEntity trsClassEntity = trsClassBusiness.GetTRSClass(item.ClassId);
                                if (trsClassEntity != null)
                                    dicTrsClass.Add(trsClassEntity.ID, trsClassEntity);
                            }
                        }
                        else
                        {
                            logHelper.Info("Don't need to change TrsAssessmentItem,TrsAssessment ID:" + assessment.ID + ",TrsAssessmentItem ID:" + item.ID);
                            Console.WriteLine("Don't need to change TrsAssessmentItem, TrsAssessment ID:" + assessment.ID + ",TrsAssessmentItem ID:" + item.ID);
                        }
                    }
                    Console.WriteLine();
                    if (!hasItemChanged)
                        continue;

                    UserBaseEntity user = userBusiness.GetUser(Super_Admin_Id);
                    var model = trsBusiness.GetAssessmentModel(assessment.ID, user);
                    model.UpdateStar();
                    TRSStarEnum modelStar = model.Star;
                    if (assessment.ID == 4850)
                    {
                        int i = 1;
                    }
                    logHelper.Info("Begin check star of category 2, TrsAssessment ID:" + assessment.ID);
                    Console.WriteLine("Begin check star of category 2, TrsAssessment ID:" + assessment.ID);
                    List<TrsStarEntity> stars = assessment.Stars.ToList();
                    if (stars.Any())
                    {
                        foreach (TrsStarEntity trsStar in stars)
                        {
                            if (trsStar.Retain || !model.StarOfCategory.ContainsKey(trsStar.Category))
                            {
                                model.StarOfCategory[trsStar.Category] = trsStar.Star;
                            }
                        }
                        TrsStarEntity starOfCatergory2 = assessment.Stars.Where(s => s.Category == TRSCategoryEnum.Category2).FirstOrDefault();
                        if (starOfCatergory2 != null && starOfCatergory2.Retain == false)
                        {
                            KeyValuePair<TRSCategoryEnum, TRSStarEnum> modelStarCategory2 = model.StarOfCategory.Where(s => s.Key == TRSCategoryEnum.Category2).FirstOrDefault();
                            if (modelStarCategory2.Value != starOfCatergory2.Star)
                            {
                                dicOldStarCat2.Add(assessment.ID, (int)starOfCatergory2.Star);
                                logHelper.Info("Begin update TrsStar of category 2, TrsAssessment ID:" + assessment.ID + ",TrsStar ID:" + starOfCatergory2.ID);
                                Console.WriteLine("Begin update TrsStar of category 2, TrsAssessment ID:" + assessment.ID + ",TrsStar ID:" + starOfCatergory2.ID);

                                trsBusiness.UpdateTrsStar(starOfCatergory2.ID, (byte)modelStarCategory2.Value);

                                logHelper.Info("Update TrsStar of category 2 success, TrsAssessment ID:" + assessment.ID + ",TrsStar ID:" + starOfCatergory2.ID);
                                Console.WriteLine("Update TrsStar of category 2 success, TrsAssessment ID:" + assessment.ID + ",TrsStar ID:" + starOfCatergory2.ID);
                            }
                        }

                        int numOfFour = model.StarOfCategory.Count(x => x.Value == TRSStarEnum.Four);
                        int numOfTwo = model.StarOfCategory.Count(x => x.Value == TRSStarEnum.Two);
                        if (numOfFour == 4 && numOfTwo == 1)
                        {
                            modelStar = TRSStarEnum.Three;
                        }
                        else
                        {
                            modelStar = model.StarOfCategory.Where(x => x.Value != 0).Min(x => x.Value);
                        }

                    }
                    logHelper.Info("Check star of category 2 end, TrsAssessment ID:" + assessment.ID);
                    Console.WriteLine("Check star of category 2 end, TrsAssessment ID:" + assessment.ID);


                    if (modelStar != assessment.Star)
                    {
                        dicOldStar.Add(assessment.ID, (int)assessment.Star);
                        logHelper.Info("Begin update TrsAssessment ID:" + assessment.ID);
                        Console.WriteLine("Begin update TrsAssessment ID:" + assessment.ID);

                        assessment.UpdatedOn = DateTime.Now;
                        int count = trsBusiness.UpdateTrsAssessmentStar(assessment.ID, (byte)modelStar);
                        if (count > 0)
                        {
                            logHelper.Info("Update TrsAssessment success,TrsAssessment ID:" + assessment.ID);
                            Console.WriteLine("Update TrsAssessment success,TrsAssessment ID:" + assessment.ID);
                        }
                        else
                        {
                            logHelper.Info("Update TrsAssessment faild,TrsAssessment ID:" + assessment.ID);
                            Console.WriteLine("Update TrsAssessment faild,TrsAssessment ID:" + assessment.ID);
                        }
                        assessment.Star = modelStar;
                        changedTrsAssessments.Add(assessment);
                    }
                    else
                    {
                        logHelper.Info("Don't need to change TrsAssessment ID:" + assessment.ID);
                        Console.WriteLine("Don't need to change TrsAssessment ID:" + assessment.ID);
                    }
                    Console.WriteLine("***************************************************************************");
                }

                logHelper.Info("Process end, total of " + changedTrsAssessments.Count + " TrsAssessment changed -->");
                Console.WriteLine("Process end, total of " + changedTrsAssessments.Count + " TrsAssessment changed -->");

                Console.WriteLine("***************************************************************************");
                logHelper.Info("Begin create csv file-->");
                Console.WriteLine("Begin create csv file-->");
                CreateCsv(dicOldStar, dicOldItemAnswer, changedTrsItems, dicSchool, dicTrsClass, dicOldStarCat2);
                logHelper.Info("Create csv file end-->");
                Console.WriteLine("Create csv file end-->");

                Console.WriteLine("***************************************************************************");
                logHelper.Info("Begin move and delete PDF result-->");
                Console.WriteLine("Begin move and delete PDF result-->");
                DeletePDF(changedTrsAssessments);
                logHelper.Info("Move and delete PDF result end-->");
                Console.WriteLine("Move and delete PDF result end-->");
            }
            catch (Exception ex)
            {
                Console.WriteLine("****************************************Exception***********************************");
                logHelper.Debug("Exception:" + ex.Message);
                Console.WriteLine("Exception:" + ex.Message);
                Console.WriteLine("Please click Enter to close this console window.");
                Console.ReadLine();
            }
            finally
            {
                Console.WriteLine("****************************************End***********************************");
                Console.WriteLine("Please click Enter to close this console window.");
                Console.ReadLine();
            }
        }

        private static int CalcItemScore(TRSAssessmentItemEntity item)
        {
            int ageGroup = (int)item.AgeGroup;
            int groupSize = item.GroupSize;
            int caregiversNo = item.CaregiversNo;
            int ratio = 0;
            if (groupSize != 0 && caregiversNo != 0)
            {
                //Round down r, if age group is not 0-11 mo
                if (ageGroup == 1)
                    ratio = groupSize / caregiversNo;
                else
                    ratio = (int)Math.Floor((double)groupSize / caregiversNo);
            }
            int r_score = -1;
            //Calculate r_score
            switch (ageGroup)
            {
                case 1://0-11m
                    if (ratio > 4.5)
                        r_score = 0;
                    else if (ratio > 4)
                        r_score = 2;
                    else
                        r_score = 3;
                    break;
                case 2://12-17m
                    if (ratio > 6)
                        r_score = 0;
                    else if (ratio > 4)
                        r_score = 2;
                    else
                        r_score = 3;
                    break;
                case 3://18-23m
                    if (ratio > 8)
                        r_score = 0;
                    else if (ratio > 6)
                        r_score = 1;
                    else if (ratio > 5)
                        r_score = 2;
                    else
                        r_score = 3;
                    break;
                case 4://2y
                    if (ratio > 10)
                        r_score = 0;
                    else if (ratio > 7)
                        r_score = 1;
                    else if (ratio > 6)
                        r_score = 2;
                    else
                        r_score = 3;
                    break;
                case 5://3y
                    if (ratio > 12)
                        r_score = 0;
                    else if (ratio > 9)
                        r_score = 1;
                    else if (ratio > 8)
                        r_score = 2;
                    else
                        r_score = 3;
                    break;
                case 6://4y
                    if (ratio > 16)
                        r_score = 0;
                    else if (ratio > 13)
                        r_score = 1;
                    else if (ratio > 9)
                        r_score = 2;
                    else
                        r_score = 3;
                    break;
                case 7://5y
                    if (ratio > 16)
                        r_score = 0;
                    else if (ratio > 11)
                        r_score = 1;
                    else if (ratio > 10)
                        r_score = 2;
                    else
                        r_score = 3;
                    break;
                case 8://6-8y
                case 9://9-13y
                    if (ratio > 17)
                        r_score = 0;
                    else if (ratio > 16)
                        r_score = 1;
                    else if (ratio > 11)
                        r_score = 2;
                    else
                        r_score = 3;
                    break;
            }

            int g = groupSize;
            int g_score = -1;
            //Calculate g_score
            switch (ageGroup)
            {
                case 1://0-11m
                    if (g > 9)
                        g_score = 0;
                    else if (g > 8)
                        g_score = 2;
                    else
                        g_score = 3;
                    break;
                case 2://12-17m
                    if (g > 12)
                        g_score = 0;
                    else
                        g_score = 3;
                    break;
                case 3://18-23m
                    if (g > 18)
                        g_score = 0;
                    else if (g > 15)
                        g_score = 2;
                    else
                        g_score = 3;
                    break;
                case 4://2y
                    if (g > 21)
                        g_score = 0;
                    else if (g > 18)
                        g_score = 2;
                    else
                        g_score = 3;
                    break;
                case 5://3y
                    if (g > 27)
                        g_score = 0;
                    else if (g > 24)
                        g_score = 2;
                    else
                        g_score = 3;
                    break;
                case 6://4y
                    if (g > 32)
                        g_score = 0;
                    else if (g > 27)
                        g_score = 1;
                    else
                        g_score = 3;
                    break;
                case 7://5y
                    if (g > 33)
                        g_score = 0;
                    else if (g > 30)
                        g_score = 2;
                    else
                        g_score = 3;
                    break;
                case 8://6-8y
                case 9://9-13y
                    if (g > 34)
                        g_score = 0;
                    else if (g > 33)
                        g_score = 1;
                    else
                        g_score = 3;
                    break;
            }
            //Final Score = min of r_score and g_score
            var score = Math.Min(r_score, g_score);
            var newAnswer = 77;
            switch (score)
            {
                case 0:
                    newAnswer = 71;
                    break;
                case 1:
                    newAnswer = 72;
                    break;
                case 2:
                    newAnswer = 73;
                    break;
                case 3:
                    newAnswer = 74;
                    break;
            }
            return newAnswer;
        }

        public static void CreateCsv(Dictionary<int, int> dicOldStar, Dictionary<int, int> dicOldItemAnswer,
            List<TRSAssessmentItemEntity> changedTrsItems, Dictionary<int, SchoolEntity> dicSchool,
            Dictionary<int, TRSClassEntity> dicTrsClass, Dictionary<int, int> dicOldStarCat2)
        {
            TrsBusiness trsBusiness = new TrsBusiness();
            string fileDir = ConfigurationManager.AppSettings["ExcelAddress"];
            string fileFullPath = ConfigurationManager.AppSettings["ExcelAddress"] + "Changed_TRSAssessment.csv";
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
            CsvFileWriter csvWriter = new CsvFileWriter(fileFullPath);
            CsvRow csvTitle = new CsvRow();
            csvTitle.Add("TRSAssessment ID");
            csvTitle.Add("Assessment Type");
            csvTitle.Add("Assessment Old Star");
            csvTitle.Add("Assessment New Star");
            csvTitle.Add("Assessment Status");
            csvTitle.Add("Create On");

            csvTitle.Add("Cat2 Old Star");
            csvTitle.Add("Cat2 New Star");

            csvTitle.Add("School ID");
            csvTitle.Add("School Engage ID");
            csvTitle.Add("School Name");

            csvTitle.Add("TRS Class ID");
            csvTitle.Add("TRS Class Engage ID");
            csvTitle.Add("TRS Class Name");

            csvTitle.Add("Assessment Item ID");
            csvTitle.Add("Item Old Answer ID");
            csvTitle.Add("Item Old Score");
            csvTitle.Add("Item New Answer ID");
            csvTitle.Add("Item New Score");
            csvTitle.Add("Age Group");
            csvTitle.Add("Group Size");
            csvTitle.Add("Caregiver No");

            csvWriter.WriteRow(csvTitle);

            foreach (TRSAssessmentItemEntity item in changedTrsItems)
            {
                CsvRow csvRow = new CsvRow();
                int assessmentId = item.Assessment.ID;
                csvRow.Add(assessmentId.ToString());
                csvRow.Add(item.Assessment.Type.ToDescription());
                csvRow.Add(dicOldStar.ContainsKey(assessmentId) ? dicOldStar[assessmentId].ToString() : ((int)item.Assessment.Star).ToString());
                csvRow.Add(((int)item.Assessment.Star).ToString());
                csvRow.Add(item.Assessment.Status.ToString());
                csvRow.Add(item.Assessment.CreatedOn.ToShortDateString());

                TrsStarEntity starOfCatergory2 = trsBusiness.GetTrsStarByCategory(assessmentId, TRSCategoryEnum.Category2);
                string newStarCat2 = "NULL";
                if (starOfCatergory2 != null)
                {
                    newStarCat2 = ((int)starOfCatergory2.Star).ToString();
                }
                csvRow.Add(dicOldStarCat2.ContainsKey(assessmentId) ? ((int)dicOldStarCat2[assessmentId]).ToString() : newStarCat2);
                csvRow.Add(newStarCat2);

                if (dicSchool.ContainsKey(item.Assessment.SchoolId))
                {
                    SchoolEntity school = dicSchool[item.Assessment.SchoolId];
                    csvRow.Add(school.ID.ToString());
                    csvRow.Add(school.SchoolId);
                    csvRow.Add(school.Name);
                }
                else
                {
                    csvRow.Add("NULL");
                    csvRow.Add("NULL");
                    csvRow.Add("NULL");
                }

                if (dicTrsClass.ContainsKey(item.ClassId))
                {
                    TRSClassEntity classEntity = dicTrsClass[item.ClassId];
                    csvRow.Add(classEntity.ID.ToString());
                    csvRow.Add(classEntity.TRSClassId);
                    csvRow.Add(classEntity.Name);
                }
                else
                {
                    csvRow.Add("NULL");
                    csvRow.Add("NULL");
                    csvRow.Add("NULL");
                }

                int oldAnswerId = dicOldItemAnswer.ContainsKey(item.ID) ? dicOldItemAnswer[item.ID] : item.AnswerId.Value;
                string oldScore = GetScore(oldAnswerId);
                string newScore = GetScore(item.AnswerId.Value);

                csvRow.Add(item.ID.ToString());
                csvRow.Add(oldAnswerId.ToString());
                csvRow.Add(oldScore);
                csvRow.Add(item.AnswerId.ToString());
                csvRow.Add(newScore);
                csvRow.Add(item.AgeGroup.ToDescription());
                csvRow.Add(item.GroupSize.ToString());
                csvRow.Add(item.CaregiversNo.ToString());

                csvWriter.WriteRow(csvRow);
            }
            csvWriter.Close();
        }

        public static string GetScore(int answerId)
        {
            string score = "NULL";
            switch (answerId)
            {
                case 71:
                    score = "0";
                    break;
                case 72:
                    score = "1";
                    break;
                case 73:
                    score = "2";
                    break;
                case 74:
                    score = "3";
                    break;
            }
            return score;
        }

        public static void DeletePDF(List<TRSAssessmentEntity> changedTrsAssessments)
        {
            ISunnetLog logHelper = ObjectFactory.GetInstance<ISunnetLog>();
            string protectFileUrl = SFConfig.ProtectedFiles;
            string PDFBbakUrl = ConfigurationManager.AppSettings["PDFBbakAddress"];
            try
            {
                if (PDFBbakUrl[PDFBbakUrl.Length - 1] != Path.DirectorySeparatorChar)
                    PDFBbakUrl += Path.DirectorySeparatorChar;

                if (!Directory.Exists(PDFBbakUrl))
                    Directory.CreateDirectory(PDFBbakUrl);

                foreach (TRSAssessmentEntity assessment in changedTrsAssessments)
                {
                    logHelper.Info("Begin move and delete PDF result,TrsAssessment ID:" + assessment.ID + "-->");
                    Console.WriteLine("Begin move and delete PDF result,TrsAssessment ID:" + assessment.ID + "-->");
                    string sourcePath = protectFileUrl + "Trs/" + assessment.ID;
                    if (!Directory.Exists(sourcePath))
                        continue;

                    string aimPath = PDFBbakUrl + assessment.ID.ToString();
                    if (!Directory.Exists(aimPath))
                        Directory.CreateDirectory(aimPath);
                    string[] fileList = Directory.GetFileSystemEntries(sourcePath);
                    //遍历所有的文件和目录
                    foreach (string file in fileList)
                    {
                        //先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                        if (Directory.Exists(file))
                            CopyDir(file, aimPath + "/" + Path.GetFileName(file));
                        //否则直接Copy文件
                        else
                            File.Copy(file, aimPath + "/" + Path.GetFileName(file), true);
                    }
                    DeleteFolder(sourcePath);
                    logHelper.Info("Move and delete PDF result,TrsAssessment ID:" + assessment.ID + " end-->");
                    Console.WriteLine("Move and delete PDF result,TrsAssessment ID:" + assessment.ID + " end-->");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CopyDir(string srcPath, string aimPath)
        {
            try
            {
                // 检查目标目录是否以目录分割字符结束如果不是则添加之
                if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;
                // 判断目标目录是否存在如果不存在则新建之
                if (!Directory.Exists(aimPath))
                    Directory.CreateDirectory(aimPath);
                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                string[] fileList = Directory.GetFileSystemEntries(srcPath);
                //遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    //先当作目录处理如果存在这个目录就递归Copy该目录下面的文件

                    if (Directory.Exists(file))
                        CopyDir(file, aimPath + Path.GetFileName(file));
                    //否则直接Copy文件
                    else
                        File.Copy(file, aimPath + Path.GetFileName(file), true);
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.ToString());
            }
        }

        /// <summary>
        /// 递归删除文件夹目录及文件
        /// </summary>
        /// <param name="dir"></param>  
        /// <returns></returns>
        public static void DeleteFolder(string dir)
        {
            if (Directory.Exists(dir)) //如果存在这个文件夹删除之 
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                        File.Delete(d); //直接删除其中的文件                        
                    else
                        DeleteFolder(d); //递归删除子文件夹 
                }
                Directory.Delete(dir, true); //删除已空文件夹                
            }
        }
    }
}
