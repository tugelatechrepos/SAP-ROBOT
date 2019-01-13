using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPAutomationJob
{
    public class PaymentListRequest
    {
        public string AccountNumber { get; set; }
        public ICollection<string[]> FormattedPaymentList { get; set; }

    }

    public class PaymentListProcessor
    {
        #region Declarations

        private string _AccountNumber;
        private ICollection<string[]> _PaymentArrayItemList;
        private ICollection<Payment> _PaymentList;

        #endregion Declarations

        public ICollection<Payment> ProcessPaymentList(PaymentListRequest Request)
        {
            _AccountNumber = Request.AccountNumber;
            _PaymentArrayItemList = Request.FormattedPaymentList;
            _PaymentList = new List<Payment>();

            process();

            return _PaymentList;
        }

        private void process()
        {
            foreach (var PaymentArrayItem in _PaymentArrayItemList)
            {
                var payment = new Payment();
                payment.AccountNumber = _AccountNumber;
                payment.ServiceId = PaymentArrayItem[0];
                payment.PaymentDate = GetFormattedDate(PaymentArrayItem[1]);

                var zarIndex = Array.IndexOf(PaymentArrayItem, "ZAR");
                var paymentAmountIndex = zarIndex - 1;
                var paymentAmount = PaymentArrayItem[paymentAmountIndex];
                payment.Amount = getFormattedPaymentAmount(paymentAmount);
                payment.Currency = PaymentArrayItem[zarIndex];
                payment.PaymentType = getFormattedPaymentType(PaymentArrayItem);
                payment.Description = getPaymentDescription(PaymentArrayItem);

                _PaymentList.Add(payment);
            }
        }

        private DateTime GetFormattedDate(string Date)
        {
            var dateArray = Date.Split('.');
            var day = Convert.ToInt32(dateArray[0]);
            var month = Convert.ToInt32(dateArray[1]);
            var year = Convert.ToInt32(dateArray[2]);
            var paymentDate = new DateTime(year, month, day);
            return paymentDate;
        }

        private decimal getFormattedPaymentAmount(string PaymentAmount)
        {
            if (PaymentAmount.Contains("."))
            {
                PaymentAmount = PaymentAmount.Replace(".", "");
            }

            if (PaymentAmount.Contains(","))
            {
                var CommaIndex = PaymentAmount.IndexOf(",");
                var paymentAmountStringBuilder = new StringBuilder(PaymentAmount);
                paymentAmountStringBuilder[CommaIndex] = '.';
                PaymentAmount = paymentAmountStringBuilder.ToString();
            }

            return Convert.ToDecimal(PaymentAmount);
        }

        private string getFormattedPaymentType(string[] PaymentArrayItem)
        {
            var startIndex = 2;
            var zarIndex = Array.IndexOf(PaymentArrayItem, "ZAR");
            var endIndex = zarIndex - 2;
            var paymentTypeBuilder = new StringBuilder();
            for (int i = startIndex; i <= endIndex; i++)
            {
                paymentTypeBuilder.Append(PaymentArrayItem[i]);
                paymentTypeBuilder.Append(" ");
            }
            return paymentTypeBuilder.ToString();
        }

        private string getPaymentDescription(string[] PaymentArrayItem)
        {
            var zarIndex = Array.IndexOf(PaymentArrayItem, "ZAR");
            var startIndex = zarIndex + 1;
            var endIndex = PaymentArrayItem.Length - 1;
            var paymentDescriptionBuilder = new StringBuilder();

            for (int i = startIndex; i <= endIndex; i++)
            {
                paymentDescriptionBuilder.Append(PaymentArrayItem[i]);
                paymentDescriptionBuilder.Append(" ");
            }
            return paymentDescriptionBuilder.ToString();
        }
    }
}

