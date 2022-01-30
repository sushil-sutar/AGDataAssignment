using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGDataNUnit.WebAPI.NUnit
{
    public class TestCaseDto : TestCaseBaseDto
    {
        public dynamic ExpectedOutput { get; set; }
        public bool IsIgnored { get; set; }
    }
}
