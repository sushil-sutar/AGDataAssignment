using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationFramework.Helpers;

namespace AutomationFramework.Views
{
    public class AGDataHomePageView
    {
        internal void NavigateToCareers()
        {
            FrameworkHelper.ClickElement(ObjectNameMapper.NameMapper["MegaMenuCompany"], MouseClickType.LeftCLick);
            FrameworkHelper.ClickElement(ObjectNameMapper.NameMapper["MenuItemCareers"], MouseClickType.LeftCLick);
        }
    }
}
