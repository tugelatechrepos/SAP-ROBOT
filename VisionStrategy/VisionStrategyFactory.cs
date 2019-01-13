using System;
using System.ComponentModel.Composition;
using System.Configuration;

namespace VisionStrategy
{
    public interface IVisionStrategyFactory
    {
        IVision GetVisionStrategy();
    }

    public interface IVision
    {
        bool Exists(string path, int timeout);
    }

    [Export(typeof(IVisionStrategyFactory))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class VisionStrategyFactory  : IVisionStrategyFactory
    {
        #region Declarations
        #endregion Declarations
        public IVision GetVisionStrategy()
        {
            var UseSikuliVision = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSikuli"]);
            if (UseSikuliVision)
                return new SikuliVision();
            return new EmguVision();
        }
    }   
}
