using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGDataNUnit.WebAPI.NUnit
{
    public class TestCaseBaseDto
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MethodType { get; set; }
        public string Url { get; set; }
        public dynamic Body { get; set; }
    }
}
