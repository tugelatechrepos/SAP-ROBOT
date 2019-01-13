using AutomationToolStrategy;
using Logger;
using MouseKeyboardHandler;
using Project.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using VisionStrategy;

namespace SAPAutomationJob
{

    public class SAPAutomationJobWithSikuli
    {
        #region Declarations

        private IAutomation _AutomationTool;
        private IMouseKeyboardHandler _MouseKeyboardHandler;
        private IVision _Vision;
        private ValidationResults _ValidationResults;
        private ICollection<string> _AccountList;
        private List<RawPayment> _RawPaymentDataList;

        [Import]
        public IAccountDataReader AccountDataReader { get; set; }
        [Import]
        public IVisionStrategyFactory VisionStrategyFactory { get; set; }
        [Import]
        public IAutomationStrategyFactory AutomationToolStrategyFactory { get; set; }
        [Import]
        public IMouseKeyboardHandlerFactory MouseKeyboardHandlerFactory { get; set; }
        [Import]
        public ILogger Logger { get; set; }
        [Import]
        public IFatalHandler FatalHandler { get; set; }

        #endregion Declarations

        public void Execute()
        {
            _ValidationResults = new ValidationResults();
            Logger.TRACE($"Started executing SAP Automation Job at {DateTime.Now}");

            initializeStrategies();
            openApplication();
            accessloginPage();
            acessSAPMenuPage();
            accessAccountsReceivable();
            assignRawPaymentDataListForAccountList();
        }

        private void initializeStrategies()
        {
            if (!_ValidationResults.IsValid) return;

            _Vision = VisionStrategyFactory.GetVisionStrategy();
            _AutomationTool = AutomationToolStrategyFactory.GetAutomationStrategy();
            _MouseKeyboardHandler = MouseKeyboardHandlerFactory.GetMouseKeyboardHandler();
        }

        private void openApplication()
        {
            if (!_ValidationResults.IsValid) return;

            _AutomationTool.InitializeApplication(ConfigurationManager.AppSettings["ApplicationPath"]);
            var IsExists = _Vision.Exists(ImagePathConstants.SAP_FIRST_SCREEN, 40);
            if (!IsExists)
            {
                handleFatalCondition(string.Empty, _ValidationResults);
                return;
            }
            _AutomationTool.Click("1068");
        }

        private void accessloginPage()
        {
            if (!_ValidationResults.IsValid) return;

            var IsExists = _Vision.Exists(ImagePathConstants.SAP_USERNAME_PASWORD, 40);
            if (!IsExists)
            {
                handleFatalCondition(string.Empty, _ValidationResults);
                return;
            }

            _MouseKeyboardHandler.InputString(ConfigurationManager.AppSettings["SAPUserId"]);
            _MouseKeyboardHandler.KeyTyping(Keys.Tab);
            Thread.Sleep(1000); // this 1 second wouldn't hurt
            _MouseKeyboardHandler.InputString(ConfigurationManager.AppSettings["SAPPassword"]);
            Thread.Sleep(1000); // Why this 1 second is required?
            _MouseKeyboardHandler.KeyTyping(Keys.Enter);
        }

        private void acessSAPMenuPage()
        {
            if (!_ValidationResults.IsValid) return;

            var IsExists = _Vision.Exists(ImagePathConstants.SAP_MENU_PAGE, 40);
            if (!IsExists)
            {
                handleFatalCondition(string.Empty, _ValidationResults);
                return;
            }       

        }
        
        private void accessAccountsReceivable()
        {
            if (!_ValidationResults.IsValid) return;

            _MouseKeyboardHandler.InputString("FPL9");
            _MouseKeyboardHandler.KeyTyping(Keys.Enter);
            var IsExists = _Vision.Exists(ImagePathConstants.SAP_ACCOUNTS_RECEIVABLE, 40);
            if (!IsExists)
            {
                handleFatalCondition(string.Empty, _ValidationResults);
                return;
            }

            _MouseKeyboardHandler.KeyTyping(Keys.Down);
        }

        private void assignRawPaymentDataListForAccountList()
        {
            if (!_ValidationResults.IsValid) return;

            _AccountList = AccountDataReader.getAccountsList();
            if (_AccountList == null || !_AccountList.Any()) return;
            _RawPaymentDataList = new List<RawPayment>();

            foreach (var account in _AccountList)
            {
                var rawpayment = new RawPayment { AccountNumber = account };

                ClipboardHelper.SetText(account);
                _MouseKeyboardHandler.KeyDown(Keys.LControlKey);
                _MouseKeyboardHandler.KeyTyping(Keys.V);
                _MouseKeyboardHandler.KeyUp(Keys.LControlKey);
                _MouseKeyboardHandler.KeyTyping(Keys.Enter);
                if (_Vision.Exists(ImagePathConstants.ACCOUNT_NOT_FOUND, 0)) { refreshAccountNumberField(); continue; }
                if (!_Vision.Exists(ImagePathConstants.PAYMENT_LIST, 5))
                {
                    handleFatalCondition("", _ValidationResults);
                    return;
                }
                _MouseKeyboardHandler.MouseClick(new Point()); //Click on Payment List Tab
                //Put focus on Command window
                _MouseKeyboardHandler.KeyDown(Keys.LControlKey);
                _MouseKeyboardHandler.KeyTyping(Keys.OemQuestion);// forward slash
                _MouseKeyboardHandler.KeyUp(Keys.LControlKey);
                
                _MouseKeyboardHandler.InputString("%pc");
                _MouseKeyboardHandler.KeyTyping(Keys.Enter);
                if (!_Vision.Exists(ImagePathConstants.SAVE_TO_CLIPBOARD_OPTION, 5))
                {
                    handleFatalCondition(string.Empty, _ValidationResults);
                    return;
                }

                _MouseKeyboardHandler.MouseClick(new Point()); //Click on ClipboardOption
                _MouseKeyboardHandler.KeyTyping(Keys.Enter);
                Thread.Sleep(200);//Check it can be further tuned
                var data = Clipboard.GetText();
                rawpayment.RawPaymentData = data;
                _RawPaymentDataList.Add(rawpayment);             
                _MouseKeyboardHandler.MouseClick(new Point()); //Click on Back button
                if(!_Vision.Exists(ImagePathConstants.SAP_ACCOUNTS_RECEIVABLE, 5))
                {
                    handleFatalCondition(string.Empty, _ValidationResults);
                    return;
                }
                _MouseKeyboardHandler.KeyTyping(Keys.Down);
                refreshAccountNumberField();
            }
        }

        private void refreshAccountNumberField()
        {
            _MouseKeyboardHandler.KeyDown(Keys.LControlKey);
            _MouseKeyboardHandler.KeyTyping(Keys.A);
            _MouseKeyboardHandler.KeyTyping(Keys.Delete);
            _MouseKeyboardHandler.KeyUp(Keys.LControlKey);
        }

        private ValidationResults handleFatalCondition(string message,ValidationResults validationResults)
        {
            if (!validationResults.IsValid) return validationResults;

            var response = FatalHandler.HandleFatalCondition(new FatalHandlerRequest { });
            return response.ValidationResults;
        }

    }
}
