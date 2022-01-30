using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGDataNUnit.WebAPI.NUnit
{
    [SetUpFixture]
    public class UITestSuiteFixture
    {
        [SetUp]
        public virtual void TestFixtureSetup()
        {
            //Test suite setup part here
        }

        [TearDown]
        public virtual void TestFixtureTearDown()
        {
            //Test suite clean up part here
        }
    }
}
