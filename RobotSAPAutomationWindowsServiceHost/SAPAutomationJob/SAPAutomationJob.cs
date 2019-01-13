
using OpenQA.Selenium.Winium;
using Quartz;
using Quellatalo.Nin.TheHands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAPAutomationJob
{

    public class RawPayment
    {
        public string AccountNumber { get; set; }
        public string RawPaymentData { get; set; }
    }

    public class SAPAutomationJob : IJob
    {
        #region Declarations

        private AccountDataReader _AccountDataReader;
        private ICollection<string> _AccountList;
        private ICollection<RawPayment> _RawPaymentDataList;
        private KeyboardHandler _keyboadHandler;
        private MouseHandler _mouseHandler;
        private Point _PaymentListCordinates;
        private Point _ClipboardSaveCordinates;
        private Point _BackCordinates;
        private RawPaymentDataProcessor _RawPaymentDataProcessor;
        private PaymentListProcessor _PaymentListProcessor;
        private List<Payment> _PaymentList;
        private ExportPaymentListProcessor _ExportPaymentListProcessor;

        #endregion Declarations

        public void Execute(IJobExecutionContext context)
        {
            var options = new DesktopOptions();
            options.ApplicationPath = ConfigurationManager.AppSettings["ApplicationPath"];
            var driver = new WiniumDriver(@"C:\Users\user\source\repos\RobotSAPAutomationWindowsServiceHost\SAPAutomationJob\Winium\", options);
            

            Thread.Sleep(5000);
            driver.FindElementById("1068").Click();

            _PaymentListCordinates = new Point(404, 300);
            _ClipboardSaveCordinates = new Point(847, 704);
            _BackCordinates = new Point(266,52);

             _keyboadHandler = new KeyboardHandler();
             _mouseHandler = new MouseHandler();

            Thread.Sleep(5000);

            _keyboadHandler.StringInput(ConfigurationManager.AppSettings["SAPUserId"]);
            _keyboadHandler.KeyTyping(Keys.Tab);

            Thread.Sleep(1000);
            _keyboadHandler.StringInput(ConfigurationManager.AppSettings["SAPPassword"]);

            Thread.Sleep(1000);
            _keyboadHandler.KeyTyping(Keys.Enter);

            Thread.Sleep(2000);
            _keyboadHandler.StringInput("FPL9");
            _keyboadHandler.KeyTyping(Keys.Enter);

            Thread.Sleep(2000);
           _keyboadHandler.KeyTyping(Keys.Down);

            assignAccountList();
            assignRawPaymentDataListForAccountList();
            processRawPaymentDataList();
            exportPaymentList();
        }

        private void assignAccountList()
        {
            _AccountDataReader = new AccountDataReader();
            _AccountList =  _AccountDataReader.getAccountsList();
        }

        private void assignRawPaymentDataListForAccountList()
        {
            if (_AccountList == null || !_AccountList.Any()) return;
            _RawPaymentDataList = new List<RawPayment>();

            foreach (var account in _AccountList)
            {
                var rawpayment = new RawPayment { AccountNumber = account };

                ClipboardHelper.SetText(account);
                _keyboadHandler.KeyDown(Keys.LControlKey);
                _keyboadHandler.KeyTyping(Keys.V);
                _keyboadHandler.KeyUp(Keys.LControlKey);
                _keyboadHandler.KeyTyping(Keys.Enter);
                Thread.Sleep(2000);
                _mouseHandler.Click(_PaymentListCordinates);
                Thread.Sleep(2000);
                _keyboadHandler.KeyTyping(Keys.Tab);
                Thread.Sleep(1000);
                _keyboadHandler.KeyTyping(Keys.Tab);
                Thread.Sleep(1000);
                _keyboadHandler.StringInput("%pc");
                _keyboadHandler.KeyTyping(Keys.Enter);
                Thread.Sleep(1000);
                _mouseHandler.Click(_ClipboardSaveCordinates);

                _keyboadHandler.KeyTyping(Keys.Enter);
                Thread.Sleep(2000);
                var data = Clipboard.GetText();
                rawpayment.RawPaymentData = data;
                _RawPaymentDataList.Add(rawpayment);
                //Thread.Sleep(500);
                //_keyboadHandler.KeyTyping(Keys.F3);
                _mouseHandler.Click(_BackCordinates);
                Thread.Sleep(1000);
                _keyboadHandler.KeyTyping(Keys.Down);

                _keyboadHandler.KeyDown(Keys.LControlKey);
                _keyboadHandler.KeyTyping(Keys.A);
                _keyboadHandler.KeyTyping(Keys.Delete);
                _keyboadHandler.KeyUp(Keys.LControlKey);
            }
        }

        private void processRawPaymentDataList()
        {
            _RawPaymentDataProcessor = new RawPaymentDataProcessor();
            _PaymentListProcessor = new PaymentListProcessor();
            _PaymentList = new List<Payment>();

            foreach (var rawPayment in _RawPaymentDataList)
            {
                var paymentDataItemArray = _RawPaymentDataProcessor.ProcessRawPaymentData(rawPayment.RawPaymentData);
                var paymentList = _PaymentListProcessor.ProcessPaymentList(new PaymentListRequest { AccountNumber = rawPayment.AccountNumber, FormattedPaymentList = paymentDataItemArray }).ToList();

                _PaymentList.AddRange(paymentList);
            }
        }

        private void exportPaymentList()
        {
            _ExportPaymentListProcessor = new ExportPaymentListProcessor();
            _ExportPaymentListProcessor.Export(new ExportRequest { PaymentList = _PaymentList, ColumnNamesList = getColumnNamesList() });
        }

        private Dictionary<string, Type> getColumnNamesList()
        {
            var ColumnNamesList = new Dictionary<string, Type>();
            ColumnNamesList.Add("AccountNumber", typeof(string));
            ColumnNamesList.Add("ServiceId", typeof(string));
            ColumnNamesList.Add("PaymentDate", typeof(string));
            ColumnNamesList.Add("PaymentType", typeof(string));
            ColumnNamesList.Add("Amount", typeof(decimal));
            ColumnNamesList.Add("Currency", typeof(string));
            ColumnNamesList.Add("Description", typeof(string));

            return ColumnNamesList;
        }
    }
}
