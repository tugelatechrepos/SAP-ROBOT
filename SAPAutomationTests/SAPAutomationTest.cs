using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAPAutomationJob;

namespace SAPAutomationTests
{
    [TestClass]
    public class SAPAutomationTest
    {
        [TestMethod]
        public void SAPAutomationTest_SanityCheck()
        {
            var job = new SAPAutomationJob.SAPAutomationJob();
            job.Execute(null);
        }
    }
}
