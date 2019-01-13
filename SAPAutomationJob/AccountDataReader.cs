using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPAutomationJob
{
    public interface IAccountDataReader
    {
        ICollection<string> getAccountsList();
    }

    [Export(typeof(IAccountDataReader))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AccountDataReader : IAccountDataReader
    {
        public ICollection<string> getAccountsList()
        {
            string filePath = ConfigurationManager.AppSettings["AccountSourcePath"];
            var accountList = File.ReadAllLines(filePath).ToList();
            return accountList;
        }
    }
}
