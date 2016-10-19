using SolidWorks.Interop.sldworks;

namespace SldWorksLogic.Interfaces
{
    public interface ILoadingElement
    {
        string Path { get; set; }
        ElementType ElementType { get; set; }

        Feature GetMatingFace();
        Feature GetHorizontFace();
    }
}
