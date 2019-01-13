using Sikuli4Net.sikuli_REST;
using Sikuli4Net.sikuli_UTIL;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionStrategy
{
    [Export(typeof(IVision))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SikuliVision : IVision
    {
        #region Declarations

        private APILauncher _Launch;
        private Screen _Screen;

        #endregion Declarations

        public SikuliVision()
        {
            _Launch = new APILauncher(true);
            _Launch.Start();
            _Screen = new Screen();
        }

        public bool Exists(string path, int timeout)
        {
            var pattern = new Pattern(path);
            return _Screen.Exists(pattern, timeout);
        }
    }
}
