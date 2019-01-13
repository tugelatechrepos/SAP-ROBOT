using Quellatalo.Nin.TheHands;
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
    [Export(typeof(IMouseKeyboardHandler))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TheHandsHandler : IMouseKeyboardHandler
    {
        #region Declarations

        private KeyboardHandler _KeyboardHandler;
        private MouseHandler _MouseHandler;

        #endregion Declarations

        public TheHandsHandler()
        {
            _KeyboardHandler = new KeyboardHandler();
            _MouseHandler = new MouseHandler();
        }

        public void InputString(string Input)
        {
            _KeyboardHandler.StringInput(Input);
        }

        public void KeyTyping(Keys keys)
        {
            _KeyboardHandler.KeyTyping(keys);
        }

        public void KeyDown(Keys keys)
        {
            _KeyboardHandler.KeyDown(keys);
        }

        public void KeyUp(Keys keys)
        {
            _KeyboardHandler.KeyUp(keys);
        }

        public void MouseClick(Point Cordinates)
        {
            _MouseHandler.Click(Cordinates);
        }
    }
}
