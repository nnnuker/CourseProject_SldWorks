using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SldWorksLogic.Concrete;
using SldWorksLogic.Infrastructure;
using SldWorksLogic.Interfaces;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SldWorksLogic
{
    public class SldWorksWorker
    {
        private readonly SldWorks swApp;
        private static MathUtility mathUtility;
        private readonly AssemblyDoc assDoc;
        private List<Element> loadedElements;
        private Element element;
        private readonly ModelDoc2 swDoc;
        private int longstatus = 0;
        private int longwarnings = 0;

        private readonly MountingElement[] elements = PathConfig.GetPaths();
        private SelectionMgr mgr;

        public static MathUtility MathUtility{get { return mathUtility; } }

        public SldWorksWorker()
        {
            swApp = new SldWorks();
            assDoc = swApp.ActiveDoc;
            swDoc = swApp.ActiveDoc;
            mathUtility = (MathUtility)swApp.GetMathUtility();
        }

        public void AddUnits()
        {
            loadedElements = GetLoadedElements();

            foreach (var element in elements)
            {
                OpenDoc(element.Path);
                AddComponent(element, 0, 0, 0);
            }
        }

        public void CreateMates()
        {
            var faces = GetSelectedFaces();

            foreach (var entity in faces)
            {
                ((Entity)entity).DeSelect();
            }

            var supports = elements.Where(e => e.ElementType == ElementType.Support);

            SelectFaces(supports, element.GetSupportFace(), swMateType_e.swMateCOINCIDENT);

            var detents = elements.Where(e => e.ElementType == ElementType.Detent);

            SelectFaces(detents.Skip(2), element.GetGuideFace(), swMateType_e.swMateCOINCIDENT);

            SelectFaces(detents.Take(2), element.GetMountingFace(), swMateType_e.swMateCOINCIDENT);

            CreateMatesHorizont(elements, element.GetSupportFace().First(), swMateType_e.swMatePARALLEL, 0);

            //CreateMatesHorizont(supports, element.GetSupportFace().First(), swMateType_e.swMatePARALLEL, 0);

            //CreateMatesHorizont(detents, element.GetSupportFace().First(), swMateType_e.swMatePARALLEL, 0);
        }

        private void SelectFaces(IEnumerable<MountingElement> mountingElements, IEnumerable<Entity> elementFaces, swMateType_e type)
        {
            var mount = mountingElements.ToList();
            var faces = elementFaces.ToList();

            var mountCount = mount.Count();
            var facesCount = faces.Count();

            var ceiling = Math.Ceiling((double)mountCount / facesCount);

            for (int i = 0; i < facesCount; i++)
            {
                var list = new List<MountingElement>();
                for (int j = 0; j < ceiling; j++)
                {
                    if (mountCount > 0)
                    {
                        var m = mount.Take(1).First();
                        mount.Remove(m);

                        mountCount--;

                        list.Add(m);
                    }                    
                }

                CreateMates(list, faces[i], type);
            }    
        }

        private void CreateMates(IEnumerable<MountingElement> mountingElements, Entity elementFace, swMateType_e type)
        {
            elementFace.Select(true);
            foreach (var elem in mountingElements)
            {
                elem.GetMatingFace().Select(true);
                CreateMate((int)type, 1);
                elem.GetMatingFace().DeSelect();
            }
            elementFace.DeSelect();
        }

        private void CreateMatesHorizont(IEnumerable<MountingElement> mountingElements, Entity elementFace, swMateType_e type, int align)
        {
            elementFace.Select(true);
            foreach (var elem in mountingElements)
            {
                elem.GetHorizontFace().Select(true);
                CreateMate((int)type, align);
                elem.GetHorizontFace().DeSelect();
            }
            elementFace.DeSelect();
        }

        private void CreateMate(int type, int align)
        {
            assDoc.AddMate3(type, align, false, 0, 0, 0, 0, 0, 0, 0, 0, false, out longstatus);
            assDoc.EditRebuild();
        }

        private void OpenDoc(params string[] paths)
        {
            foreach (var path in paths)
            {
                swApp.OpenDocSilent(path, 1, longwarnings);
            }
        }

        private void AddComponent(MountingElement element, double x, double y, double z)
        {
            element.Component2 = assDoc.AddComponent4(element.Path, string.Empty, x, y, z);
            assDoc.EditRebuild();
            swApp.CloseDoc(element.Path);
        }

        private List<Element> GetLoadedElements()
        {
            object[] objects = assDoc.GetComponents(true);
            var components = objects.Cast<Component2>();

            if (components == null || !components.Any())
            {
                throw new ArgumentException("No components in document");
            }

            return components.Select(comp => (new Element(comp))).ToList();
        }

        private List<Face2> GetSelectedFaces()
        {
            mgr = swDoc.SelectionManager;

            var selectedObjectCount = mgr.GetSelectedObjectCount();

            var faces = new List<Face2>(selectedObjectCount);

            for (var i = 0; i < selectedObjectCount; i++)
            {
                faces.Add(mgr.GetSelectedObject6(i + 1, -1));
            }

            if (faces.Count == 0)
            {
                throw new ArgumentException("Not enough selected faces");
            }

            if (!CheckFacesAreFromOne(faces))
            {
                throw new ArgumentException("Faces are from different components");
            }

            return faces;
        }

        private bool CheckFacesAreFromOne(IEnumerable<Face2> faces)
        {
            bool flag = false;

            foreach (var loaded in loadedElements)
            {
                var except = faces.Except(loaded.GetFaces());

                if (!except.Any())
                {
                    flag = true;
                    element = new Element(loaded.Component2, faces);
                    break;
                }
            }

            return flag;
        }

        private Entity[] CreateEntitiesFromFaces(IEnumerable<Face2> faces)
        {
            var count = faces.Count();

            var entities = new Entity[count];
            for (var i = 0; i < count; i++)
            { 
                entities[i] = (Entity)faces.ElementAt(i);
                entities[i].DeSelect();
            }

            return entities;
        }
    }
}
