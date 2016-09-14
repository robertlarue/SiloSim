using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiloSim
{
    public class DisabledRichTextBox : System.Windows.Forms.RichTextBox
    {
        // See: http://wiki.winehq.org/List_Of_Windows_Messages

        private const int WM_SETFOCUS = 0x7;
        private const int WM_ENABLE = 0xa;

        private const int WM_SETCURSOR = 0x20;
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (!(m.Msg == WM_SETFOCUS || m.Msg == WM_ENABLE || m.Msg == WM_SETCURSOR))
            {
                base.WndProc(ref m);
            }
        }
    }

}
