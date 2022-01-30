using System;

namespace AGDataNUnit.WebAPI.NUnit
{
    public class JsonFileName : Attribute
    {
        public static string GetFileName { get; set; }
        public string FileName
        {
            get { return GetFileName; }
            set { GetFileName = value; }
        }
    }
}
