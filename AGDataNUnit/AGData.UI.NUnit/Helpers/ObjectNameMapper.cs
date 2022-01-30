using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework.Helpers
{
    public static class ObjectNameMapper
    {
        public static Dictionary<string, string> NameMapper;

        public static void InitializeObjectNameMapper()
        {
            NameMapper = new Dictionary<string, string>();
            string objectNameMapperFile = Directory.GetCurrentDirectory() + System.Configuration.ConfigurationManager.AppSettings["ObjectNameMapperFile"];
            List<string> headers;
            List<string[]> objectMappers = ExcelHelper.ReadData(objectNameMapperFile, System.Configuration.ConfigurationManager.AppSettings["ObjectNameMapperSheetName"], out headers);

            foreach(string[] objectMapper in objectMappers)
            {
                NameMapper.Add(objectMapper[1], objectMapper[2]);
            }
            
        }

    }
}
