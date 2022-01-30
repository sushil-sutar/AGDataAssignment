using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;

namespace AutomationFramework
{

    [SetUpFixture]
    public class UITestSuiteFixture
    {
        

        [SetUp]
        public virtual void TestFixctureSetup()
        {
            Logger.InitializeLog();
            //Deploy application
            //setup test data
            //Load XPaths
            AutomationFramework.Helpers.ObjectNameMapper.InitializeObjectNameMapper();
        }

        internal void OpenBrowser()
        {
            string homeUrl = System.Configuration.ConfigurationManager.AppSettings["ApplicationUrl"];
            FrameworkHelper.WebDriver.Navigate().GoToUrl(homeUrl);
            FrameworkHelper.WaitUntilPageLoadReady();
        }

        [TearDown]
        public virtual void TestFixctureTearDown()
        {
            //delete DB
            //remove website
        }

    }
}
