using AutomationFramework.Helpers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework.Views
{
    public class AGDataCareersView
    {
        internal List<string> GetJobList()
        {
            List<string> jobList = new List<string>();
            try
            {
                FrameworkHelper.WebDriver.SwitchTo().Frame(FrameworkHelper.WebDriver.FindElement(By.XPath(ObjectNameMapper.NameMapper["JobsFrame"])));
                ReadOnlyCollection<IWebElement> jobListElements = FrameworkHelper.GetElementsByXPath(ObjectNameMapper.NameMapper["AllJobs"]);
                
                foreach(IWebElement jobElement in jobListElements)
                {
                    jobList.Add(jobElement.FindElement(By.TagName(ObjectNameMapper.NameMapper["TagNamea"])).Text);
                }
            }
            finally
            {
                FrameworkHelper.WebDriver.SwitchTo().ParentFrame();
            }
            return jobList;
        }
    }
}
