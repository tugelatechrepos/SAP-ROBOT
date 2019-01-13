using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationToolStrategy
{
    public interface IAutomationStrategyFactory
    {
        IAutomation GetAutomationStrategy();
    }

    public interface IAutomation
    {
        void InitializeApplication(string ApplicationPath);
        void Click(string AutomationId);
    }

    [Export(typeof(IAutomationStrategyFactory))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AutomationStrategyFactory  : IAutomationStrategyFactory
    {
        public IAutomation GetAutomationStrategy()
        {
            var IsWiniumStrategy = Convert.ToBoolean(ConfigurationManager.AppSettings["WiniumAutomationStrategy"]);
            if (IsWiniumStrategy) return new WiniumAutomation();
            return null;
        }
    }

    
}
