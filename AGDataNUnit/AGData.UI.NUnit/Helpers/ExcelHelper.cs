using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace AutomationFramework.Helpers
{
    public static class ExcelHelper
    {
        public static List<T> LoadObjects<T>(string testDataFilePath, string sheetName = null)
        {
            var objects = new List<T>();
            if (sheetName == null)
                sheetName = typeof(T).Name;

            List<string> headers;
            var data = ReadData(testDataFilePath, sheetName, out headers);

            foreach (var value in data)
            {
                objects.Add(GenerateObject<T>(headers,value));
            }

            return objects;
        }

        private static T GenerateObject<T>(List<string> properties, string[] values)
        {
            var instance = (T) Activator.CreateInstance(typeof(T));
            int i = -1;
            foreach (var property in properties)
            {
                i++;
                if(values[i]==null)
                    continue;
                Type type = null;
                try
                {
                    var prop = (typeof(T).GetProperty(property));
                    type = prop.PropertyType;
                    object typedProperty = Convert.ChangeType(values[i], type);
                    instance.GetType().GetProperty(property).SetValue(instance, typedProperty);
                }
                catch (Exception ex)
                {
                    Logger.Log(LogType.Error, "Error Loading "+property+ " "+ values[i]+ " "+ type );
                    throw;
                }
            }

            return instance;
        }

        public static List<string[]> ReadData(string testDataFilePath, string sheetName, out List<string> headers)
        {
            List<string[]> data = new List<string[]>();
            //var excelPackage = new ExcelPackage(new FileInfo(Directory.GetCurrentDirectory() +"\\TestData\\" + testDataFilePath));
            var excelPackage = new ExcelPackage(new FileInfo(testDataFilePath));
            var worksheet = excelPackage.Workbook.Worksheets[sheetName];
            int row = 2;
            headers = GetHeaders(worksheet);
            string[] dataRows;
            do
            {
                dataRows = GetRows(worksheet, row, headers.Count);
                if(dataRows!=null)
                    data.Add(dataRows);
                row++;

            } while (dataRows != null);

            return data;
        }

        private static string[] GetRows(ExcelWorksheet worksheet, int rowNumber, int columnCount)
        {
            string[] rowData = new string[columnCount];

            for (int column = 1; column < columnCount + 1; column++)
            {
                rowData[column - 1] = GetValue(worksheet.GetValue(rowNumber, column));
            }

            bool isEmptyRow = true;
            foreach (var value in rowData)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    isEmptyRow = false;
                    break;
                }
            }

            if (isEmptyRow)
                rowData = null;

            return rowData;
        }

        private static List<string> GetHeaders(ExcelWorksheet worksheet)
        {
            List<string> headers = new List<string>();
            bool dataExist = true;
            int column = 1;
            while (dataExist)
            {
                string cell = GetValue(worksheet.GetValue(1, column));
                if (String.IsNullOrEmpty(cell))
                    dataExist = false;
                else
                {
                    headers.Add(cell);
                    column++;
                }

            }

            return headers;
        }

        private static string GetValue(object obj)
        {
            if (obj == null)
                return null;
            else
                return obj.ToString();
        }
    }
}
