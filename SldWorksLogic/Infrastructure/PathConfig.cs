using System.IO;
using SldWorksLogic.Concrete;
using SldWorksLogic.Interfaces;

namespace SldWorksLogic.Infrastructure
{
    public static class PathConfig
    {
        public static MountingElement[] GetPaths()
        {
            //var supports = @"d:\Univer\S05\ATP\CourseProject_SldWorks\SldWorksLogic\Units\supports\Locator7.SLDPRT";
            //var detents = @"d:\Univer\S05\ATP\CourseProject_SldWorks\SldWorksLogic\Units\detents\Locator10.SLDPRT";

            var supports = Directory.GetCurrentDirectory() + @"\Units\supports\Locator7.SLDPRT";
            var detents = Directory.GetCurrentDirectory() + @"\Units\detents\Locator10.SLDPRT";

            return new []
            {
                new MountingElement(supports, ElementType.Support),
                new MountingElement(supports, ElementType.Support),
                new MountingElement(supports, ElementType.Support),
                new MountingElement(detents, ElementType.Detent),
                new MountingElement(detents, ElementType.Detent),
                new MountingElement(detents, ElementType.Detent)
            };
        }
    }
}
