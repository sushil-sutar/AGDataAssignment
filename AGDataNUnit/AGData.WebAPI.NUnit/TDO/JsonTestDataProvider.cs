using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGDataNUnit.WebAPI.NUnit
{
    public abstract class JsonTestDataProvider : IEnumerable
    {
        private readonly string _resourceName;
        protected JsonTestDataProvider(string resourceName)
        {
            _resourceName = resourceName;
        }
        public IEnumerator GetEnumerator()
        {
            var assembly = GetType().Assembly;
            var names = assembly.GetManifestResourceNames();
            var testDataName = names.Single(n => n.Contains("." + _resourceName));

            var manifestStream = assembly.GetManifestResourceStream(testDataName);
            if (manifestStream == null)
                throw new InvalidOperationException("Unable to load specific resource");

            using (var streamReader = new StreamReader(manifestStream))
            using (JsonReader reader = new JsonTextReader(streamReader))
            {
                dynamic jObj = JObject.Load(reader);

                return ProcessItems(jObj);
            }
        }

        protected abstract IEnumerator ProcessItems(dynamic rootObject);
    }
}
