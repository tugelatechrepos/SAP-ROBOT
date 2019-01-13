using Logger;
using Project.Core;
using System.ComponentModel.Composition;

namespace SAPAutomationJob
{
    public interface IFatalHandler
    {
        FatalHandlerResponse HandleFatalCondition(FatalHandlerRequest Request);
    }

    public class FatalHandlerRequest
    {

    }

    public class FatalHandlerResponse
    {
        public ValidationResults ValidationResults { get; set; }
    }

    [Export(typeof(IFatalHandler))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FatalHandler : IFatalHandler
    {
        #region Declarations

        private FatalHandlerRequest _Request;
        private FatalHandlerResponse _Response;

        [Import]
        public ILogger Logger { get; set; }

        #endregion Declarations

        public FatalHandlerResponse HandleFatalCondition(FatalHandlerRequest Request)
        {
            _Request = Request;
            _Response = new FatalHandlerResponse { ValidationResults = new ValidationResults { IsValid = false } };

            logFatal();
            emailDistributionList();

            return _Response;
        }

        private void logFatal()
        {
            Logger.FATAL($"");
        }

        private void emailDistributionList()
        {

        }
    }
}
