using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

using VecLib;

namespace LibTest
{
    class VecTest : ILibTest
    {
        vec2d vecTest = new vec2d(50, 0);
        float fRotateSpeed = .01f;

        public String LibTest(out bool testResult)
        {
            testResult = true;
            return "Success";
        }

        public void PaintTest(Graphics g, Rectangle viewPort)
        {
            // Draw axes
            g.DrawLine(Pens.Black, viewPort.X, viewPort.Y + viewPort.Height / 2.0f, viewPort.X + viewPort.Width, viewPort.Y + viewPort.Height / 2.0f);
            g.DrawLine(Pens.Black, viewPort.X + viewPort.Width / 2.0f, viewPort.Y, viewPort.X + viewPort.Width / 2.0f, viewPort.Y + viewPort.Height);

            // Tranlaste to center
            g.TranslateTransform(viewPort.X + viewPort.Width / 2.0f, viewPort.Y + viewPort.Height / 2.0f);

            // Draw vector
            g.DrawString("<" + vecTest.x + ", " + vecTest.y + ">\n" + String.Format("{0:n}", vecTest.magnitude()) + "/" + String.Format("{0:n}", vecTest.angle()) + "rad", new Font("Segoe UI", 12), Brushes.Black, 50, -50);
            g.DrawLine(Pens.Orange, 0, 0, (float)vecTest.x, (float)vecTest.y);

            vecTest = vecTest.rotate(fRotateSpeed);
        }
    }
}
