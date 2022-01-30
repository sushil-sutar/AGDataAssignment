using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGDataNUnit.WebAPI.NUnit
{
    public class TestDataProvider : JsonTestDataProvider
    {
        public TestDataProvider() : base(JsonFileName.GetFileName)
        {

        }
        protected override IEnumerator ProcessItems(dynamic rootObject)
        {
            foreach (dynamic obj in rootObject.testCases)
            {
                TestCaseDto requestDto;
                try
                {
                    bool isIgnored = false;
                    if(obj.IsIgnored != null)
                    {
                        if(!bool.TryParse(obj.IsIgnored.ToString(), out isIgnored))
                        {
                            isIgnored = false;
                        }
                    }


                    requestDto = new TestCaseDto
                    {
                        Id = (decimal)obj.Id,
                        Name = (string)obj.Name,
                        Description = (string)obj.Description,
                        Body = obj.Body,
                        MethodType = (string)obj.MethodType,
                        Url = (string)obj.Url,
                        ExpectedOutput=obj.ExpectedOutput,
                        IsIgnored = isIgnored,
                    };

                }
                catch(Exception ex)
                {
                    throw;
                }
                yield return new TestCaseData(requestDto).SetName(string.Format("{0} : {1}", obj.Id, obj.Name));
            }
        }

        private static List<T> CreateTestCaseDtos<T>(dynamic Object) where T : TestCaseBaseDto
        {
            var dtos = new List<T>();
            if(Object !=null)
            {
                foreach(dynamic obj in Object)
                {
                    try
                    {
                        var dto = (T)Activator.CreateInstance(typeof(T), obj);
                        dtos.Add(dto);
                    }
                    catch(Exception ex)
                    {
                        throw new Exception("Failed to create Test case dto:" +ex.Message);
                    }
                }
            }
            return dtos;
        }
    }
}
