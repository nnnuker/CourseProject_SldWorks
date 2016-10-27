using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SldWorksLogic.ShellBuilding
{
    public class Line
    {
        public MathPoint First { get; set; }
        public MathPoint Second { get; set; }

        public Line(MathPoint first, MathPoint second)
        {
            First = first;
            Second = second;
        }
    }
}
