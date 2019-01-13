using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPAutomationJob
{
    public class AccountDataReader
    {
        public ICollection<string> getAccountsList()
        {
            string filePath = ConfigurationManager.AppSettings["AccountSourcePath"];
            var accountList = File.ReadAllLines(filePath).ToList();
            return accountList;
        }
    }
}
