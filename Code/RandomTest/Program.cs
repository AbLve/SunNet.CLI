using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Tool;
using System;
using System.Collections.Generic;
using System.Threading;
using Sunnet.Cli.Core.Ade.Enums;
using System.Linq;
using System.Linq.Expressions;
using Sunnet.Cli.Business.Cpalls.Models;

namespace RandomTest
{
    class Program
    {

        static void TestWithArray( int count, int times)
        {
            for (int j = 0; j < times; j++)
            {
                int[] arrayRandom = new int[count];
                for (int i = 0; i < count; i++)
                {
                    arrayRandom[i] = i;
                }
                //var startTick = DateTime.Now.Ticks;
                //Console.WriteLine("     start time : " + DateTime.Now.ToString("HH:mm:ss:fff"));

                RandomTool.GetRandomNum(arrayRandom);
                //var endTick = DateTime.Now.Ticks;
                //Console.WriteLine("     end time : " + DateTime.Now.ToString("HH:mm:ss:fff"));
                //long tick = endTick - startTick;
                //long seconds = tick / TimeSpan.TicksPerSecond;
                //long milliseconds = tick / TimeSpan.TicksPerMillisecond;
                //long microseconds = tick / 10;
                //Console.WriteLine("     Seconds Elapsed :" + seconds);
                //Console.WriteLine("     Millseconds Elapsed :" + milliseconds);
                //Console.WriteLine("     Microseconds Elapsed :" + microseconds);
                //Console.WriteLine("     random result : ");
                for (var i = 0; i < arrayRandom.Length; i++)
                {
                    Console.Write("{0, 3}", arrayRandom[i]);
                }
                Console.WriteLine("");
                //Console.WriteLine("     ------------------------");
            }
        }

        static void TestWithList(int count, int times)
        {
            for (int j = 0; j < times; j++)
            {
                
                List<int> randomList = new List<int>();
                for (int i = 0; i < count; i++)
                {
                    randomList.Add(i);
                }
                //Console.WriteLine("     source List : ");

                //for (var i = 0; i < randomList.Count; i++)
                //{
                //    Console.Write(" " + randomList[i].ToString() + " ");
                //}
                //Console.WriteLine("");
               //var startTick = DateTime.Now.Ticks;
                //Console.WriteLine("     start time : " + DateTime.Now.ToString("HH:mm:ss:fff"));

                RandomTool.GetRandomNum(randomList);
               
                //var endTick = DateTime.Now.Ticks;
                //Console.WriteLine("     end time : " + DateTime.Now.ToString("HH:mm:ss:fff"));

                //long tick = endTick - startTick;
                //long seconds = tick / TimeSpan.TicksPerSecond;
                //long milliseconds = tick / TimeSpan.TicksPerMillisecond;
                //long microseconds = tick / 10;
                //Console.WriteLine("     Seconds Elapsed :" + seconds);
                //Console.WriteLine("     Millseconds Elapsed :" + milliseconds);
                //Console.WriteLine("     Microseconds Elapsed :" + microseconds);
                //Console.WriteLine("     random result : ");
                for (var i = 0; i < randomList.Count; i++)
                {
                    Console.Write("{0, 3}" , randomList[i]);
                }
                Console.WriteLine("");
                //Console.WriteLine("     ------------------------");
            }
        }

        public static void TestWithStringList(int times)
        {
            for (int j = 0; j < times; j++)
            {

                List<string> randomList = new List<string>();
                randomList.Add("A");
                randomList.Add("B");
                randomList.Add("C");
                randomList.Add("D");
                randomList.Add("E");
                randomList.Add("J");
                RandomTool.GetRandomNum(randomList);
             
                for (var i = 0; i < randomList.Count; i++)
                {
                    Console.Write("{0, 3}", randomList[i]);
                }
                Console.WriteLine("");
                //Console.WriteLine("     ------------------------");
            }

        }

       private static List<AnswerEntity> GenerateAnswers(int count)
        {
            List<AnswerEntity> answers = new List<AnswerEntity>();
            if (count < 1)
                return answers;
            for (int i=1; i<=count; i++)
            {
                AnswerEntity a = new AnswerEntity();
                a.ImageType = ImageType.Selectable;//Defalut
                //please add the non-selectable imageType for testing at here
                if (i == 1 || i == count)
                    a.ImageType = ImageType.NonSelectable;
                a.Picture = "P"+i;
                a.ResponseAudio = "RA"+i;
                a.AudioTime = i * 10 + i;
                a.PictureTime = i * 100 + i;
                answers.Add(a);
            }
            return answers;
        }

        public static void TestTXKEAItems(int times)
        {
            for (int j = 0; j < times; j++)
            {
                Console.WriteLine("*****# of Time :" + (j+1));
                //Console.WriteLine("     start time : " + DateTime.Now.ToString("HH:mm:ss:fff"));
                ExecCpallsItemModel item = new ExecCpallsItemModel();
                List<AnswerEntity> answers = GenerateAnswers(10);
                item.Answers = answers;

                //========Same code from 
                List<AnswerEntity> RandomAnswers = new List<AnswerEntity>();//重新排列后的answer集合
                List<AnswerEntity> OrderedAnswers = item.Answers.Select(
                    r => new AnswerEntity() { PictureTime = r.PictureTime, AudioTime = r.AudioTime }).ToList();

                List<int> selectableIndex = item.Answers.Select((r, i) => new { r, i }).
                    Where(r => r.r.ImageType == ImageType.Selectable).Select(r => r.i).ToList();
                RandomTool.GetRandomNum(selectableIndex);//重新排列Selectable的Index

                for (int i = 0; i < item.Answers.Count; i++)
                {
                    AnswerEntity TargetAnswer = item.Answers[i];
                    //只有Selectable的才随机显示
                    if (TargetAnswer.ImageType == ImageType.NonSelectable)
                    {
                        RandomAnswers.Add(TargetAnswer);
                    }
                    else
                    {
                        if (selectableIndex.Count > 0)//保证还有得拿
                        {
                            int randomIndexValue = selectableIndex[0];//总拿第一个的值
                            selectableIndex.RemoveAt(0);//去掉，下次自动拿下一个
                            item.Answers[randomIndexValue].PictureTime = OrderedAnswers[i].PictureTime;
                            item.Answers[randomIndexValue].AudioTime = OrderedAnswers[i].AudioTime;
                            RandomAnswers.Add(item.Answers[randomIndexValue]);
                        }
                    }
                }
                //========
                //Console.WriteLine("     end time : " + DateTime.Now.ToString("HH:mm:ss:fff"));
                for (var i = 0; i < RandomAnswers.Count; i++)
                {
                    Console.WriteLine("{0, 25}", RandomAnswers[i].Picture + "," + RandomAnswers[i].ImageType.ToString() +
                        ",ResponseAudio:" + RandomAnswers[i].ResponseAudio +
                        ",AudioTime:" + RandomAnswers[i].AudioTime+ ",PictureTime:"+ RandomAnswers[i].PictureTime);
                }

            }

        }
        static void Main(string[] args)
        {
            //int count = 5;
            //int times = 20;
            //Console.WriteLine("******************Testing with Array.....******************");
            //TestWithArray(count, times);
            //Console.WriteLine("******************Testing with List.....******************");
            //TestWithList(count, times);
            //Console.WriteLine("******************Testing with String List.....******************");
            //TestWithStringList(times);
            TestTXKEAItems(10);
            Console.WriteLine("Please press any key to exit.....");
            Console.Beep();
            Console.ReadKey();



        }
    }
}
