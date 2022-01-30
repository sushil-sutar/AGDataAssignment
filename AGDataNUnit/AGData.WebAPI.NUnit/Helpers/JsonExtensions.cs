using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AGDataNUnit.WebAPI.NUnit.Helpers
{
    public static class JsonExtensions
    {

        private static StringBuilder CompareObjects(JObject actualObject, JObject expectedObject)
        {
            StringBuilder differenceBuilder = new StringBuilder();
            if (JToken.DeepEquals(actualObject, expectedObject))
            {
                return differenceBuilder;
            }
            if(actualObject.Count != expectedObject.Count)
            {
                differenceBuilder = differenceBuilder.Append("Expected message count doesn't match with actual message count");
                return differenceBuilder;
            }

            foreach (KeyValuePair<string, JToken> actualKeyValuePair in actualObject)
            {
                differenceBuilder.Append("Comparing " + actualKeyValuePair.Key + Environment.NewLine);
                if (actualKeyValuePair.Value.Type == JTokenType.Object)
                {
                    if (expectedObject.GetValue(actualKeyValuePair.Key) == null)
                    {
                        differenceBuilder.Append("Key " + actualKeyValuePair.Key
                            + " not found" + Environment.NewLine);
                    }
                    else if (expectedObject.GetValue(actualKeyValuePair.Key).Type != JTokenType.Object)
                    {
                        differenceBuilder.Append("Key " + actualKeyValuePair.Key
                            + " is absent in actual result" + Environment.NewLine);
                    }
                    else
                    {
                        differenceBuilder.Append(CompareObjects(actualKeyValuePair.Value.ToObject<JObject>(),
                            expectedObject.GetValue(actualKeyValuePair.Key).ToObject<JObject>()));
                    }
                }

                if (actualKeyValuePair.Value.Type == JTokenType.Array)
                {
                    if (expectedObject.GetValue(actualKeyValuePair.Key) == null)
                    {
                        differenceBuilder.Append("Key " + actualKeyValuePair.Key
                            + " not found" + Environment.NewLine);
                    }
                    else if (expectedObject.GetValue(actualKeyValuePair.Key).Type != JTokenType.Object)
                    {
                        differenceBuilder.Append("Key " + actualKeyValuePair.Key
                            + " is not an Array in target" + Environment.NewLine);
                    }
                    else
                    {
                        differenceBuilder.Append(CompareArrays(actualKeyValuePair.Value.ToObject<JArray>(),
                            expectedObject.GetValue(actualKeyValuePair.Key).ToObject<JArray>()));
                    }
                }
                else
                {
                    JToken actual = actualKeyValuePair.Value;
                    var expected = expectedObject.SelectToken(actualKeyValuePair.Key);
                    if(expected == null)
                    {
                        differenceBuilder.Append("Key " + actualKeyValuePair.Key +
                            " not found " + Environment.NewLine);
                    }
                    else
                    {
                        bool areUnEqual = true;
                        if(actualKeyValuePair.Value is JValue)
                        {
                            if(CompareValues(actualKeyValuePair.Value as JValue, expectedObject.SelectToken(actualKeyValuePair.Key) as JValue))
                            {
                                areUnEqual = false;
                            }
                        }
                        else if(!JToken.DeepEquals(actual, expected))
                        {
                            areUnEqual = false;
                        }

                        if(areUnEqual)
                        {
                            differenceBuilder.Append("======Difference========" + Environment.NewLine);
                            differenceBuilder.Append("Name: ExpectedValue, ActualValue " + Environment.NewLine +
                                "[" +actualKeyValuePair.Key+ ":,"+ "'"+expectedObject.Property(actualKeyValuePair.Key).Value + "', '"+
                                actualKeyValuePair.Value+"']");
                        }
                        else  if(differenceBuilder == null)
                        { differenceBuilder.Clear(); }
                    }
                }
                if(differenceBuilder.ToString().Equals("comparing "+actualKeyValuePair.Key +Environment.NewLine))
                {
                    differenceBuilder.Clear();
                }
            }
            return differenceBuilder;
        }

        private static bool CompareValues(JValue actualObject, JValue expectedObject)
        {
            var expected = (expectedObject == null) ? "" : expectedObject.ToString();
            var actual = (actualObject == null) ? "" : actualObject.ToString();

            return actual.Equals(expected);
        }

        private static StringBuilder CompareArrays(JArray source, JArray target, string arrayName = "")
        {
            StringBuilder returnString = new StringBuilder();
            if (JToken.DeepEquals(source, target))
            {
                return returnString;
            }

            for(int index = 0; index < source.Count; index ++)
            {
                var expected = source[index];
                if(expected.Type == JTokenType.Object)
                {
                    var actual = (index >= target.Count) ? new JObject(): target[index];
                    returnString.Append(CompareObjects(expected.ToObject<JObject>(), actual.ToObject<JObject>()));
                }
                else
                {
                    var actual = (index >= target.Count) ? "" : target[index];
                    if(!JToken.DeepEquals(expected,actual))
                    {
                        if(String.IsNullOrEmpty(arrayName))
                        {
                            returnString.Append("Index " + index + ": " + expected
                                + " !=" + actual + Environment.NewLine);
                        }
                        else
                        {
                            returnString.Append("Key " + arrayName + "[" + index + "]" + expected +
                                "!= " + actual + Environment.NewLine);
                        }
                    }
                }
            }
            return returnString;
        }

        public static bool Compare<T>(this T source, T target, out string differentPath) where T: JToken
        {
            StringBuilder diffString = new StringBuilder();
            if(source.Type == JTokenType.Array)
            {
                diffString.Append(CompareArrays(source.ToObject<JArray>(), target.ToObject<JArray>()));
            }
            else if(source.Type == JTokenType.Object)
            {
                diffString.Append(CompareObjects(source.ToObject<JObject>(), target.ToObject<JObject>()));
            }

            if(diffString.Length > 0 )
            {
                differentPath = diffString.ToString();
                return false;
            }
            else
            {
                differentPath = "";
                return true;
            }
        }
    }
}
