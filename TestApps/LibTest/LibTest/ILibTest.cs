using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

namespace LibTest
{
    public interface ILibTest
    {
        void Initialize();
        void PaintTest(Graphics g, Rectangle viewPort);
        String LibTest(out bool testResult);
        void KeyUp(KeyEventArgs e);
        void KeyDown(KeyEventArgs e);
    }
}
