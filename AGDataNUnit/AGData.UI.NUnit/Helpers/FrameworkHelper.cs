using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework
{
    public enum MouseClickType
    {
        LeftCLick,
        RighClick,
        LeftDoubleClick
    }
    public static class FrameworkHelper
    {
        public static BrowserType DriverBrowserType;
        private static IWebDriver _webDriver;
        public static WebDriverWait Wait;

        public static IWebDriver WebDriver
        {
            get { return _webDriver; }
            set
            {
                _webDriver = value;
                if(_webDriver !=null)
                {
                    if(_webDriver is InternetExplorerDriver)
                    {
                        DriverBrowserType = BrowserType.InternetExplore;
                    }
                    else
                    {
                        DriverBrowserType = BrowserType.Chrome;
                    }
                }
            }
        }

        internal static IWebElement GetElementByXPath(string xpath)
        {
            return WebDriver.FindElement(By.XPath(xpath));
        }

        internal static ReadOnlyCollection<IWebElement> GetElementsByXPath(string xpath)
        {
            ReadOnlyCollection <IWebElement> elements= WebDriver.FindElements(By.XPath(xpath));
            
            return elements;
        }

        public static void WaitForElementExistenceById(string id)
        {
            try
            {
                Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(id)));
            }
            catch(Exception ex)
            {
                Logger.Log(LogType.Error, "Exception while waiting for element by id " + id + ". Error " + ex.Message);
            }
        }

        public static void WaitForElementExistenceByXPath(string xpath)
        {
            try
            {
                Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
            }
            catch (Exception ex)
            {
                Logger.Log(LogType.Error, "Exception while waiting for element by xpath " + xpath+ ". Error " + ex.Message);
            }
        }

        public static void WaitForElementExistenceByName(string name)
        {
            try
            {
                Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Name(name)));
            }
            catch (Exception ex)
            {
                Logger.Log(LogType.Error, "Exception while waiting for element by name " + name + ". Error " + ex.Message);
            }
        }

        public static IWebElement GetElementById(string id)
        {
            WaitForElementExistenceById(id);
            Logger.Log(LogType.Info, "Get Element by id " + id);
            return WebDriver.FindElement(By.Id(id));
        }

        public static IWebElement GetElement(string xPath)
        {
            WaitForElementExistenceById(xPath);
            Logger.Log(LogType.Info, "Get Element by xpath " + xPath);
            return WebDriver.FindElement(By.XPath(xPath));
        }

        public static void ClickElement(string id, MouseClickType mouseClickType)
        {
            IWebElement webElement = GetElement(id);
            MouseClickAction(webElement, mouseClickType);
        }

        public static void ClickElementByName(string name, MouseClickType mouseClickType = MouseClickType.LeftCLick)
        {
            IWebElement webElement = GetElementByName(name);
            MouseClickAction(webElement, mouseClickType);
        }

        public static void MouseClickAction(IWebElement webElement, MouseClickType mouseClickType)
        {
            switch(mouseClickType)
            {
                case MouseClickType.LeftDoubleClick:
                    Actions leftDoubleClick = new Actions(FrameworkHelper.WebDriver);
                    leftDoubleClick.DoubleClick(webElement);
                    leftDoubleClick.Perform();
                    Logger.Log(LogType.Info, "Click " );
                    break;
                case MouseClickType.RighClick:
                    Actions rightClick = new Actions(FrameworkHelper.WebDriver);
                    rightClick.ContextClick(webElement).SendKeys(Keys.ArrowDown).SendKeys(Keys.ArrowDown).SendKeys(Keys.Return).Build().Perform();
                    Logger.Log(LogType.Info, "Right Click " );
                    break;
                default:
                    Actions leftClick = new Actions(FrameworkHelper.WebDriver);
                    leftClick.Click(webElement);
                    leftClick.Perform();
                    Logger.Log(LogType.Info, "Left Double Click " );
                    break;
            }
        }

        internal static bool WaitUntilPageLoadReady(int maxWaitSeconds=300)
        {
            bool isSuccessful = true;
            var waitReady = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(maxWaitSeconds));
            waitReady.Until(WebDriver =>
            {
                return ((IJavaScriptExecutor)WebDriver).ExecuteScript("return document.readystate").Equals("complete");
            });

            if(!((IJavaScriptExecutor)WebDriver).ExecuteScript("return document.readystate").Equals("complete"))
                isSuccessful = false;
            return isSuccessful;
        }

        public static void SetText(string id, string value)
        {
            IWebElement webElement = GetElement(id);
            webElement.SendKeys(value);
            Logger.Log(LogType.Info, "Set text " + webElement.TagName + " " + value);
        }

        internal static void SetTextByName(string name, string value)
        {
            IWebElement webElement = GetElementByName(name);
            webElement.SendKeys(value);
            Logger.Log(LogType.Info, "Set text " + webElement.TagName + " " + value);
        }

        private static IWebElement GetElementByName(string name)
        {
            WaitForElementExistenceByName(name);
            Logger.Log(LogType.Info, "Get Element by Name " + name);
            return WebDriver.FindElement(By.Name(name));
        }
    }
}
