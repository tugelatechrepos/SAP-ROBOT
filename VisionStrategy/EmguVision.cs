using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace VisionStrategy
{
    [Export(typeof(IVision))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EmguVision : IVision
    {
        #region Declarations

        private CurrentScreen _CurrentScreen;

        #endregion Declarations

        public EmguVision()
        {
            _CurrentScreen = new CurrentScreen();
        }

        public bool Exists(string path,int timeout)
        {
            var IsExists = false;
            var templateImage = (Bitmap)System.Drawing.Image.FromFile(path);
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            while (!(IsExists = _CurrentScreen.Exists(new GetImageExistsRequest { TemplateImage = templateImage }).ImageFound))
            {
                if (TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds).TotalSeconds > timeout) break;
            }
            stopwatch.Stop();
            return IsExists;
        }
    }

    public class GetImageExistsRequest
    {
        public Bitmap TemplateImage { get; set; }
    }

    public class GetImageExistsResponse
    {
        public bool ImageFound { get; set; }
    }

    public class CurrentScreen
    {
        #region Declarations

        private Bitmap _TemplateImage;
        private Bitmap _DesktopImage;        
        private GetImageExistsRequest _Request;
        private GetImageExistsResponse _Response;

        #endregion Declarations

        public GetImageExistsResponse Exists(GetImageExistsRequest Request)
        {
            _Request = Request;
            _Response = new GetImageExistsResponse();            
            _TemplateImage = Request.TemplateImage;

            assignCurrentScreen();
            findMatch(_DesktopImage, _TemplateImage);
            dispose();

            return _Response;
        }

        private void assignCurrentScreen()
        {
            _DesktopImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var gfx = Graphics.FromImage(_DesktopImage);
            gfx.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
        }

        private void findMatch(Bitmap sourceImage, Bitmap TemplateImage)
        {
            var source = new Image<Bgr, byte>(sourceImage);
            var template = new Image<Bgr, byte>(TemplateImage);

            using (var result = source.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
            {
                double[] minValues, maxValues;
                Point[] minLocations, maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                if (maxValues[0] > 0.9) _Response.ImageFound = true;

            }
        }

        private void dispose()
        {
            if (_TemplateImage != null) _TemplateImage.Dispose();
            if (_DesktopImage != null) _DesktopImage.Dispose();
        }
    }
}
