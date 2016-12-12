using System.Collections.Generic;
using SolidWorks.Interop.sldworks;

namespace SldWorksLogic.Interfaces
{
    public interface IElement
    {
        List<Face2> SelectedFaces { get; set; }
        List<Entity> GetSupportFace();
        List<Entity> GetGuideFace();
        List<Entity> GetMountingFace();
    }
}
