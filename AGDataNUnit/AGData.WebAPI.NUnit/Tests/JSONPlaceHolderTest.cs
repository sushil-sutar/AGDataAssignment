using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using AGDataNUnit.WebAPI.NUnit.Helpers;

namespace AGDataNUnit.WebAPI.NUnit.Tests.JSONPlaceHolder
{
    [TestFixture(Category = "AGDataNUnit.WebAPI.NUnit")]
    public class JSONPlaceHolderTest : TestCaseExecutor
    {
        [JsonFileName(FileName = "JSONPlaceHolderTest.json")]
        [TestCaseSource(typeof(TestDataProvider))]
        public void JSONPlaceHolderGetTest(TestCaseDto requestTdo)
        {
            ExecuteTestCase(requestTdo);
        }
    }
}
