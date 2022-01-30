using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutomationFramework.TestDataObject;
using NUnit.Framework;
using AutomationFramework.Helpers;
using AutomationFramework.Views;

namespace AutomationFramework.Tests
{
    [TestFixture(BrowserType.Chrome, Category = "Chrome")]
    //[TestFixture(BrowserType.InternetExplore, Category = "IE")]
    public class AGDataCareersTest : UITestFixtureBase
    {

        public AGDataCareersTest(BrowserType browserType) : base(browserType)
        {
            
        }

        [SetUp]
        public override void TestSetup()
        {
            base.TestSetup();
            OpenBrowser();
        }

        [TearDown]
        public override void TestTearDown()
        {
            base.TestTearDown();
        }



        [TestCase]
        public void ListAGDataJobs()
        {
            int index = 0;
            List<CareersDto> expectedJobList = ExcelHelper.LoadObjects<CareersDto>("TestData\\Careers.xlsx", "JobList");
            //Navigate to careers
            AGDataHomePageView agDataHomePageView = new AGDataHomePageView();
            agDataHomePageView.NavigateToCareers();

            //Validate job list
            AGDataCareersView agDataCareersView = new AGDataCareersView();
            List<string> jobs = agDataCareersView.GetJobList();

            foreach(CareersDto expectedJob in expectedJobList)
            {
                if(expectedJob.JobName != jobs[index])
                {
                    Assert.Fail("Unable to find job " + expectedJob.JobName);
                }
                index++;
            }
        }
    }
}
