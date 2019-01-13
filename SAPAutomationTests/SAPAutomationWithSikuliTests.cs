using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPAutomationTests
{
    [TestClass]
    public class SAPAutomationWithSikuliTests
    {
        [TestMethod]
        public void SAPAutomationTestWithSikuli_SanityCheck()
        {
            var job = new SAPAutomationJob.SAPAutomationJobWithSikuli();
            job.Execute();
        }
    }
}
