using System;
using SldWorksLogic.Interfaces;
using SolidWorks.Interop.sldworks;

namespace SldWorksLogic.Concrete
{
    public class MountingElement: BaseElement, ILoadingElement
    {
        public string Path { get; set; }
        public ElementType ElementType { get; set; }

        public MountingElement()
        {
            Path = string.Empty;
            ElementType = ElementType.Support;
        }

        public MountingElement(string pathDir, ElementType elementType)
        {
            if (pathDir == null) throw new ArgumentNullException("pathDir");

            Path = pathDir;
            ElementType = elementType;
        }

        public Feature GetMatingFace()
        {
            if (Component2 == null)
            {
                return null;
            }

            return Component2.FeatureByName("face_for_mating");
        }

        public Feature GetHorizontFace()
        {
            if (Component2 == null)
            {
                return null;
            }

            return Component2.FeatureByName("face_for_horizont");
        }
    }
}