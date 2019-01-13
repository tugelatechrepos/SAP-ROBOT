using OpenQA.Selenium.Winium;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationToolStrategy
{
    [Export(typeof(IAutomation))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class WiniumAutomation : IAutomation
    {
        #region Declarations

        private DesktopOptions _DesktopOptions;
        private WiniumDriver _WiniumDriver;

        #endregion Declarations

        public WiniumAutomation()
        {
            _DesktopOptions = new DesktopOptions();            
        }

        public void InitializeApplication(string ApplicationPath)
        {
            _DesktopOptions.ApplicationPath = ApplicationPath;
            _WiniumDriver = new WiniumDriver(ConfigurationManager.AppSettings["WiniumDriverPath"], _DesktopOptions);
        }

        public void Click(string AutomationId)
        {
            _WiniumDriver.FindElementById(AutomationId).Click();
        }
    }
}
