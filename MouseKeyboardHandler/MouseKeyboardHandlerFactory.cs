using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseKeyboardHandler
{
   
    public interface IMouseKeyboardHandlerFactory
    {
        IMouseKeyboardHandler GetMouseKeyboardHandler();
    }


    public interface IMouseKeyboardHandler
    {
        void InputString(string Input);
        void KeyTyping(Keys keys);
        void KeyDown(Keys keys);
        void KeyUp(Keys keys);
        void MouseClick(Point Cordinates);
    }

    [Export(typeof(IMouseKeyboardHandlerFactory))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MouseKeyboardHandlerFactory
    {
        public IMouseKeyboardHandler GetMouseKeyboardHandler()
        {
            return new TheHandsHandler();
        }
    }
}
