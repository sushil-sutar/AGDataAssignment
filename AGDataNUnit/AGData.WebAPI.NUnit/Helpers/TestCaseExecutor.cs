using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Net;

namespace AGDataNUnit.WebAPI.NUnit.Helpers
{
    public abstract class TestCaseExecutor
    {
        private static Dictionary<string, JToken> CachedResponses { get; set; }

        protected TestCaseExecutor()
        {
            CachedResponses = new Dictionary<string, JToken>();
        }

        [TestFixtureSetUp]
        public void TestClassSetup()
        {

        }

        [TestFixtureSetUp]
        public void TestClassTearDown()
        {

        }

        public virtual void ExecuteTestCase(TestCaseDto requestDto)
        {
            if (requestDto.IsIgnored)
            {
                Assert.Inconclusive("Ignored");
            }

            bool isScuccessStatusCode;
            var errorMessage = string.Empty;

            var responseMessage = GetHttpResponseMessage(requestDto);

            bool entityCompareResult = CompareEntities(responseMessage, requestDto, out isScuccessStatusCode, ref errorMessage);

            if (isScuccessStatusCode)
            {
                string actualJson = string.Empty;
                if (string.IsNullOrEmpty(responseMessage.Content.ReadAsStringAsync().Result))
                    actualJson = "";
                else
                    actualJson = JToken.Parse(responseMessage.Content.ReadAsStringAsync().Result).ToString(Formatting.Indented);

                Assert.IsTrue(entityCompareResult, errorMessage
                    + Environment.NewLine +
                    "*********************************************"
                    + Environment.NewLine +
                    "Actual Response : " + actualJson);
            }
            else
            {
                Assert.Fail();
            }

        }

        private static bool CompareEntities(HttpResponseMessage httpResponseMessage, TestCaseDto requestDto, out bool isScuccessStatusCode, ref string errorMessage)
        {
            if(httpResponseMessage.IsSuccessStatusCode)
            {
                string response = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if(string.IsNullOrEmpty(response) || response.Equals("\"\""))
                {
                    isScuccessStatusCode = true;
                    return EmptyResponse("", requestDto.ExpectedOutput.ToString());
                }

                var actualOutputResponse = JToken.Parse(response);
                var actualOutput = new JArray();
                if(actualOutputResponse is JObject)
                {
                    actualOutput.Add(actualOutputResponse);
                }
                else if (actualOutputResponse is JValue)
                {
                    actualOutput.Add(actualOutputResponse);
                }
                else
                {
                    actualOutput = JsonConvert.DeserializeObject<JArray>(actualOutputResponse.ToString());
                }

                JArray expectedOutput = new JArray();
                if(requestDto.ExpectedOutput is JObject)
                {
                    expectedOutput.Add(requestDto.ExpectedOutput);
                }
                else if (requestDto.ExpectedOutput is JValue)
                {
                    expectedOutput.Add(requestDto.ExpectedOutput);
                }
                else
                {
                    expectedOutput = JArray.Parse(requestDto.ExpectedOutput.ToString());
                }

                bool result = actualOutput.Compare(expectedOutput, out errorMessage);
                isScuccessStatusCode = true;
                return result;
            }
            else
            {
                isScuccessStatusCode = false;
                Assert.Fail("Error while getting HTTP response" + httpResponseMessage);
            }

            return false;
        }

        private static void KeepDesiredProperties(JArray actualOutput, JArray expectedOutput, List<string> listDesiredProperties)
        {
            throw new NotImplementedException();
        }

        private static void RemoveIgnoreProperties(JArray actualOutput, JArray expectedOutput, object p)
        {
            throw new NotImplementedException();
        }

        private static bool EmptyResponse(string v, dynamic dynamic)
        {
            throw new NotImplementedException();
        }

        private static void AddToCachedResponse(string variableName, JToken response)
        {
            if(!string.IsNullOrEmpty(variableName))
            {
                if(!CachedResponses.ContainsKey(variableName))
                {
                    CachedResponses.Add(variableName, response.DeepClone());
                }
                else
                {
                    throw new InvalidOperationException("Referred variable already stored in cached responses. " + variableName);
                }
            }
        }

        public HttpResponseMessage GetHttpResponseMessage(TestCaseBaseDto requestDto)
        {
            HttpResponseMessage responseMessage = null;
            ReplaceVariables(requestDto);
            switch(requestDto.MethodType)
            {
                case "GET":
                    responseMessage = GetRequestCompareEntitiesResult(requestDto);
                    break;
                case "POST":
                    responseMessage = PostRequestCompareEntitiesResult(requestDto);
                    break;
                case "PUT":
                    responseMessage = PutRequestCompareEntitiesResult(requestDto);
                    break;
                case "DELETE":
                    responseMessage = DeleteRequestCompareEntitiesResult(requestDto);
                    break;
                default:
                    Assert.Fail("Invalid Method type");
                    break;
            }

            return responseMessage;
        }

        private HttpResponseMessage DeleteRequestCompareEntitiesResult(TestCaseBaseDto requestDto)
        {
            using (var client = new HttpClient(GetAuthenticationHeader()))
            {
                HttpResponseMessage deleteMessage = client.DeleteAsync(requestDto.Url).Result;
                return deleteMessage;
            }
        }

        private HttpResponseMessage PutRequestCompareEntitiesResult(TestCaseBaseDto requestDto)
        {
            using (var client = new HttpClient(GetAuthenticationHeader()))
            {
                StringContent putContent = null;
                if(requestDto.Body !=null)
                {
                    JObject putRequestBody = JObject.Parse(requestDto.Body.ToString());
                    putContent = new StringContent(putRequestBody.ToString(), Encoding.UTF8, "application/json");
                }
                var putResponseMessage = client.PutAsync(requestDto.Url, putContent).Result;
                return putResponseMessage;
            }
        }

        private HttpResponseMessage PostRequestCompareEntitiesResult(TestCaseBaseDto requestDto)
        {
            var authenticatorHeader = GetAuthenticationHeader();

            using (var client = new HttpClient(authenticatorHeader))
            {
                var body = requestDto.Body.ToString();
                JObject postRequestBody = JObject.Parse(body);
                var postRequestBodyString = postRequestBody.ToString();
                var postContent = new StringContent(postRequestBodyString, Encoding.UTF8, "application/json");

                var task = client.PostAsync(requestDto.Url, postContent);

                var postResponseMessage = task.Result;
                return postResponseMessage;

            }
        }

        public HttpResponseMessage GetRequestCompareEntitiesResult(TestCaseBaseDto requestDto)
        {
            using (var client = new HttpClient(GetAuthenticationHeader()))
            {
                HttpResponseMessage getResponseMessage = client.GetAsync(requestDto.Url).Result;
                return getResponseMessage;
            }
        }

        

        private static HttpClientHandler GetAuthenticationHeader()
        {
            return new HttpClientHandler
            {
                Credentials = CredentialCache.DefaultNetworkCredentials
            };
        }

        private static void ReplaceVariables(TestCaseBaseDto testCaseBaseDto)
        {
            if(CachedResponses.Count == 0)
            {
                return;
            }

            testCaseBaseDto.Url = ReplaceVariables(testCaseBaseDto.Url);
            if(testCaseBaseDto.Body != null && !string.IsNullOrEmpty(testCaseBaseDto.Body.ToString()))
            {
                testCaseBaseDto.Body = JObject.Parse(ReplaceVariables(testCaseBaseDto.Body.ToString()));
            }
        }

        private static string ReplaceVariables(string data)
        {
            if(CachedResponses.Count == 0)
            {
                return data;
            }
            foreach(Match match in Regex.Matches(data, @"(?<={)(.*?)(?=})"))
            {
                string value = match.Value;
                if (string.IsNullOrEmpty(value))
                    continue;
                string[] arr =value.Split('$');
                var cachedResponseVariableName = arr[0];
                var searchQueryStringParam = arr[1];
                if(!CachedResponses.ContainsKey(cachedResponseVariableName))
                {
                    throw new InvalidOperationException("Refererred variable not stored in cached responses " + cachedResponseVariableName);
                }

                var variableValue = CachedResponses[cachedResponseVariableName].SelectTokens(searchQueryStringParam).FirstOrDefault();
                if (variableValue==null)
                {
                    throw new InvalidOperationException(searchQueryStringParam + " is not found in "+cachedResponseVariableName);
                }

                value = "{" + value + "}";
                if(variableValue is JValue)
                {
                    data = data.Replace(value, variableValue.ToString().Replace(@"""", @"\"""));
                }
                else
                {
                    value = "\"" + value + "\"";
                    data = data.Replace(value, JsonConvert.SerializeObject(variableValue));
                }
            }
            return data;
        }
    }
}
