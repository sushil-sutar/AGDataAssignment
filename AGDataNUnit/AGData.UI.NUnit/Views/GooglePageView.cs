using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutomationFramework
{
    public class GooglePageView
    {
        private const string searchTextName = "q";
        private const string searchButtonName = "btnK";

        internal void SetSearchCriteria(string searchCriteria)
        {
            FrameworkHelper.SetTextByName(searchTextName, searchCriteria);
        }

        internal void Search()
        {
            FrameworkHelper.ClickElementByName(searchButtonName);
        }

        internal bool SearchResult(string searchResult)
        {

            bool isElementPresent = false;
            string xpath = "//*[text()='"+ searchResult + "']";

            IWebElement webElement= FrameworkHelper.GetElementByXPath(xpath);
            if (webElement != null)
                isElementPresent = true;

            return isElementPresent;
        }
    }
}
