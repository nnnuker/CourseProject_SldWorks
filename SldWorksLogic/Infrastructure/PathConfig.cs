using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SldWorksLogic.Infrastructure
{
    public static class PathConfig
    {
        public static Element[] GetPaths()
        {
            return new []
            {
                new Element(@"d:\Univer\S05\ATP\CourseProject_SldWorks\SldWorksLogic\Units\detents\Locator10.SLDPRT", 3, ElementType.Detent), 
                new Element(@"d:\Univer\S05\ATP\CourseProject_SldWorks\SldWorksLogic\Units\supports\Locator7.SLDPRT", 3, ElementType.Support) 
            };
        }
    }
}
