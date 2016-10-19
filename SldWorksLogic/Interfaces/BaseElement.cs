using System.Linq;
using SolidWorks.Interop.sldworks;
using System.Collections.Generic;

namespace SldWorksLogic.Interfaces
{
    public abstract class BaseElement
    {
        private Body2 body2;
        private List<Face2> faces;

        public virtual Component2 Component2 { get; set; }

        public virtual Body2 GetBody()
        {
            if (body2 != null)
            {
                return body2;
            }

            if (Component2 != null)
            {
                body2 = Component2.GetBody();
                return body2;
            }

            return null;
        }

        public virtual List<Face2> GetFaces()
        {
            if (faces != null)
            {
                return faces;
            }

            body2 = GetBody();

            if (body2 != null)
            {
                object[] objects = body2.GetFaces();
                faces = objects.Cast<Face2>().ToList();
                return faces;
            }

            return null;
        }
    }
}
