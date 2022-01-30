using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Configuration;

namespace AutomationFramework
{
    public enum BrowserType
    {
        InternetExplore,
        Chrome
    }

    public class UITestFixtureBase
    {
        private BrowserType _browserType;
        
        protected UITestFixtureBase(BrowserType browserType)
        {
            _browserType = browserType;
        }

        //[SetUp]
        public virtual void TestSetup()
        {
            switch (_browserType)
            {
                case BrowserType.Chrome:
                    ChromeOptions chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArguments("--disable-infobars");
                    chromeOptions.AddArgument("--start-maximized");
                    chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
                    FrameworkHelper.WebDriver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), chromeOptions, TimeSpan.FromSeconds(120));
                    break;
                case BrowserType.InternetExplore:
                    InternetExplorerOptions internetExplorerOptions = new InternetExplorerOptions();
                    FrameworkHelper.WebDriver = new InternetExplorerDriver(InternetExplorerDriverService.CreateDefaultService(), internetExplorerOptions, TimeSpan.FromSeconds(120));
                    FrameworkHelper.WebDriver.Manage().Window.Maximize();
                    break;
            }
            //FrameworkHelper.Wait = new WebDriverWait(FrameworkHelper.WebDriver, TimeSpan.FromSeconds(30));
        }

        //[TearDown]
        public virtual void TestTearDown()
        {
            string errorFileName = TestContext.CurrentContext.Test.Name + "_error.png";
            if(TestContext.CurrentContext.Result.Equals("Failed"))
            {
                Screenshot screenshot = ((ITakesScreenshot)FrameworkHelper.WebDriver).GetScreenshot();
                screenshot.SaveAsFile(errorFileName, ScreenshotImageFormat.Png);
                Thread.Sleep(1000);
            }
            FrameworkHelper.WebDriver.Quit();
        }

        internal void OpenBrowser()
        {
            string homeUrl = System.Configuration.ConfigurationManager.AppSettings["ApplicationUrl"];
            FrameworkHelper.WebDriver.Navigate().GoToUrl(homeUrl);
            //FrameworkHelper.WaitUntilPageLoadReady();
        }
    }
}
