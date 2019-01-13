using OpenQA.Selenium.Winium;
using Sikuli4Net.sikuli_REST;
using Sikuli4Net.sikuli_UTIL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SAPAutomationJob
{
    public class SAPAutomationJobWithSikuli
    {
        private APILauncher _Launch;
        private Screen _Screen;

        public void Execute()
        {
            var options = new DesktopOptions();
            options.ApplicationPath = ConfigurationManager.AppSettings["ApplicationPath"];
            var driver = new WiniumDriver(@"C:\Users\user\source\repos\RobotSAPAutomationWindowsServiceHost\SAPAutomationJob\Winium\", options);

            launchSikuli();           
            var SAPFirstScreenPath = @"C:\Users\user\source\repos\RobotSAPAutomationWindowsServiceHost\SAPAutomationTests\bin\Debug\Images\SAPFirstScreen.PNG";
            var pattern = new Pattern(SAPFirstScreenPath);


            // var MinimizedWindow = @"C:\Users\user\source\repos\RobotSAPAutomationWindowsServiceHost\SAPAutomationTests\bin\Debug\MinimizedWindow.PNG";
            // var MaximizedWindow = @"C:\Users\user\source\repos\RobotSAPAutomationWindowsServiceHost\SAPAutomationTests\bin\Debug\MaximizedWindow.PNG";
            //var logonImagePath = @"..\..\..\Images\LogOn.PNG";
            //var logonPattern = new Pattern(logonImagePath);
            // var minimizedWindowPattern = new Pattern(MinimizedWindow);
            // var MaximizedWindowPattern = 
            // int i = 0;

            var time1 = DateTime.Now;
            var IsExists = _Screen.Exists(pattern, 40);
            var time2 = DateTime.Now;
            var diffv = (time2 - time1).TotalSeconds;
        }

        private void launchSikuli()
        {
            _Launch = new APILauncher(true);
            _Launch.Start();
            _Screen = new Screen();
            
        }
    }
}
