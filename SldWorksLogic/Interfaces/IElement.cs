using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;

namespace SldWorksLogic.Interfaces
{
    public interface IElement
    {
        IEnumerable<Face2> SelectedFaces { get; set; }
        IEnumerable<Entity> GetSupportFace();
        IEnumerable<Entity> GetGuideFace();
        IEnumerable<Entity> GetMountingFace();
    }
}
