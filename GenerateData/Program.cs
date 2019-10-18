using System;
using System.Data;
using System.IO;
using System.Text;

namespace GenerateData
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("1：生成数据");
            Console.WriteLine("2：退出");
            string value = Console.ReadLine();
            switch (value)
            {
                case "1":
                    Console.WriteLine("数据需要生成的数量：");
                    string dataNumber = Console.ReadLine();
                    Console.WriteLine("输入开始日期(yyyy-MM-dd)：");
                    string startDate = Console.ReadLine();
                    Console.WriteLine("输入测点文件地址（csv文件）：");
                    string filePath = Console.ReadLine();
                    Console.WriteLine("输入输出文件路径：");
                    string outPutPath = Console.ReadLine();
                    StartGenerateData(Int32.Parse(dataNumber), filePath, outPutPath, DateTime.Parse(startDate));
                    Console.WriteLine("数据生成成功，按任意键退出");
                    Console.ReadKey();
                    break;
                case "2":

                    break;
            }
        }

        public static void StartGenerateData(int number, string filePath, string outPutPath, DateTime startDate)
        {
            var fileStream = File.OpenRead(filePath);
            StreamReader sr = new StreamReader(fileStream);
            string fileContent = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            string[] testPoint = fileContent.Replace("\r\n", "$").TrimEnd('$').Split('$');
            // fileContent.Replace("\r\n",",").TrimEnd(',').Split(',');
            DataTable dt = new DataTable();
            dt.Columns.Add("Col1");
            dt.Columns.Add("Col2");
            dt.Columns.Add("Col3");
            dt.Columns.Add("Col4");
            dt.AcceptChanges();
            Random rm = new Random();
            for (int i = 1; i <= number; i++)
            {
                for (int j = 0; j < testPoint.Length; j++)
                {
                    startDate = startDate.AddSeconds(10);
                    DataRow newRow = dt.NewRow();
                    newRow["Col1"] = testPoint[j].Split(',')[0];
                    newRow["Col2"] = startDate.ToString("MM/dd/yyyy HH:mm:ss");
                    newRow["Col3"] = rm.Next(1, 100);
                    newRow["Col4"] = testPoint[j].Split(',')[1];
                    dt.Rows.Add(newRow);
                }
            }

            ConvertDataTableToCSV(dt, outPutPath);

        }

        public static void ConvertDataTableToCSV(DataTable dt, string outPutPath)
        {
            StringBuilder result = new StringBuilder();
            StringBuilder head = new StringBuilder();
            StringBuilder body = new StringBuilder();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                head.Append(dt.Columns[i].ColumnName + ",");
            }
            result.Append(head.ToString().TrimEnd(',') + "\r\n");
            int index = 0;
            foreach (DataRow row in dt.Rows)
            {
                body.Clear();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    body.Append(row[i].ToString().Replace(",", "") + ",");
                }
                result.Append(body.ToString().TrimEnd(',') + "\r\n");
                if (index > 100000)
                {
                    WriteFile(outPutPath, result.ToString());
                    result.Clear();
                    index = 0;
                }
                index++;
            }
            WriteFile(outPutPath, result.ToString());
        }

        public static void WriteFile(string outPutPath, string result)
        {
            FileStream outPutStream;
            if (File.Exists(Path.Combine(outPutPath, "Data.csv")))
            {
                outPutStream = new FileStream(Path.Combine(outPutPath, "Data.csv"), FileMode.Append);
            }
            else
            {
                outPutStream = new FileStream(Path.Combine(outPutPath, "Data.csv"), FileMode.Create);
            }

            UTF8Encoding utf8 = new UTF8Encoding(false);
            StreamWriter sw = new StreamWriter(outPutStream, utf8);
            sw.Write(result);
            sw.Close();
            outPutStream.Close();
        }
    }
}