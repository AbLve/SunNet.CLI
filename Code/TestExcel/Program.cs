using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace TestExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start....");

            var  dt = new DataTable();
            string strCNN = string.Empty;

            strCNN = "Provider=Microsoft.ACE.OleDb.12.0;Data Source =" 
                + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Community_Template.xlsx") 
                + ";Extended Properties = 'Excel 12.0;HDR=Yes;IMEX=1;'";

            OleDbConnection cnn = new OleDbConnection(strCNN);
            try
            {
                cnn.Open();
                Console.WriteLine("open excel succeed");
            }
            catch (Exception ex)
            {
                cnn.Dispose();
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("End ....");
        }
    }
}
