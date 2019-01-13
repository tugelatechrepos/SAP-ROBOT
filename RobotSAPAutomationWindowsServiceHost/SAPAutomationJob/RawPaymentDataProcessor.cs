using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPAutomationJob
{
    public class RawPaymentDataProcessor
    {
        #region Declarations

        private string _RawPaymentData;
        private ICollection<string[]> _PaymentArrayItemList;

        #endregion Declarations

        public ICollection<string[]> ProcessRawPaymentData(string RawPaymentData)
        {
            _RawPaymentData = RawPaymentData;
            _PaymentArrayItemList = new List<string[]>();

            processRawData();

            return _PaymentArrayItemList;
        }
        private void processRawData()
        {
            var paymentDataArray = _RawPaymentData.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            var paymentList = paymentDataArray.Where(x => x.Contains("ZAR") || x.Contains("General ledger items total")).ToList();

            foreach (var payment in paymentList)
            {
                if (payment.Contains("General ledger items total")) break;

                var updatedPaymnet = payment.Replace("|", string.Empty);
                updatedPaymnet = updatedPaymnet.Replace("-", string.Empty);
                var result = updatedPaymnet.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                _PaymentArrayItemList.Add(result);
            }
        }
    }
}
