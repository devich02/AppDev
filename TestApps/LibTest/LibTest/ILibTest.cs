using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace LibTest
{
    public interface ILibTest
    {
        void PaintTest(Graphics g, Rectangle viewPort);
        String LibTest(out bool testResult);
    }
}
