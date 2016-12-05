using System;
using System.Collections.Generic;
using System.Linq;
using SldWorksLogic.Concrete;
using SldWorksLogic.Infrastructure;
using SldWorksLogic.Interfaces;
using SldWorksLogic.ShellBuilding;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SldWorksLogic
{
    public class SldWorksWorker
    {
        #region Fields

        private readonly SldWorks swApp;
        private readonly AssemblyDoc assDoc;
        private List<Element> allLoadedElements;
        private Element selectedElement;
        private readonly ModelDoc2 swDoc;
        private int longstatus = 0;
        private int longwarnings = 0;

        private readonly MountingElement[] elements = PathConfig.GetPaths();
        private SelectionMgr mgr;
        private ShellBuilder shellBuilder;
        private MathUtility mathUtility;

        #endregion

        #region Ctors

        public SldWorksWorker()
        {
            swApp = new SldWorks();
            assDoc = swApp.ActiveDoc;
            swDoc = swApp.ActiveDoc;
            mathUtility = (MathUtility)swApp.GetMathUtility();
            shellBuilder = new ShellBuilder(mathUtility);
        }

        #endregion

        #region Public methods

        public void AddUnits()
        {
            allLoadedElements = GetLoadedElements();

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

            SelectFaces(supports, selectedElement.GetSupportFace(), swMateType_e.swMateCOINCIDENT);

            var detents = elements.Where(e => e.ElementType == ElementType.Detent);

            SelectFaces(detents.Skip(2), selectedElement.GetGuideFace(), swMateType_e.swMateCOINCIDENT);

            SelectFaces(detents.Take(2), selectedElement.GetMountingFace(), swMateType_e.swMateCOINCIDENT);

            CreateMatesHorizont(elements, selectedElement.GetSupportFace().First(), swMateType_e.swMatePARALLEL, 0);
        }

        public void CreateShell()
        {
            //new[] { elements.First(m => m.ElementType == ElementType.Detent) }

            var lines = shellBuilder.GetShell(elements.Where(m => m.ElementType == ElementType.Support));

            BuildingSurface(lines, selectedElement.SelectedFaces.First());
        }

        #endregion

        #region Private methods

        #region Create mates

        private void SelectFaces(IEnumerable<MountingElement> mountingElements, IEnumerable<Entity> elementFaces, swMateType_e type)
        {
            var mount = mountingElements.ToList();
            var faces = elementFaces.ToList();

            var mountCount = mount.Count;
            var facesCount = faces.Count;

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

        #endregion

        #region Initializing

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

        #endregion

        #region Get selected faces

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

            foreach (var loaded in allLoadedElements)
            {
                var except = faces.Except(loaded.GetFaces());

                if (!except.Any())
                {
                    flag = true;
                    selectedElement = new Element(loaded.Component2, mathUtility, faces);
                    break;
                }
            }

            return flag;
        }

        #endregion

        #region Shell building

        private void BuildingSurface(IEnumerable<Line> lines, Face2 face)
        {
            Component2 newComp;
            //var mateFace = GetLowestFace(elements);
            //var f = (Face2[])(object)mateFace.GetFaces();

            assDoc.InsertNewVirtualPart(face, out newComp);
            newComp.Select(true);
            assDoc.EditPart2(true, false, longstatus);

            Feature sk = newComp.FeatureByName("Эскиз1");

            sk.Select(true);
            swDoc.SketchManager.InsertSketch(true);
            var transform = swDoc.SketchManager.ActiveSketch.ModelToSketchTransform;
            transform = (MathTransform)(object)transform.Inverse();

            foreach (var line in lines)
            {
                line.First.Coordinates = MathHelper.Transform(line.First.Coordinates, transform, mathUtility);
                line.Second.Coordinates = MathHelper.Transform(line.Second.Coordinates, transform, mathUtility);

                var l = swDoc.SketchManager.CreateLine(-line.First.Coordinates[0], -line.First.Coordinates[2], -line.First.Coordinates[1],
                    -line.Second.Coordinates[0], -line.Second.Coordinates[2], -line.Second.Coordinates[1]);
            }

            swDoc.FeatureManager.FeatureExtrusion2(true, false, false, 0, 0, 0.01, 0.01, false, false,
                false, false, 0, 0, false, false, false, false, true, true, true, 0, 0, false);
            mgr.EnableContourSelection = false;
            assDoc.AssemblyPartToggle();
            assDoc.EditAssembly();
            swDoc.ClearSelection2(true);

            //swDoc.Extension.SelectByID2("На месте1", "MATE", 0, 0, 0, false, 0, null, 0);
            //swDoc.EditDelete();

            //assDoc.AddMate3(0, 1, false, 0, 0, 0, 0, 0, 0, 0, 0, false, out longstatus);
        }

        #endregion

        private Feature GetLowestFace(IEnumerable<MountingElement> mountingElements)
        {
            double[] min = null;
            int index = 0;

            for (int i = 0; i < mountingElements.Count(); i++)
            {
                var elem = mountingElements.ElementAt(i);

                object objBox = new double[6];
                var flag = elem.GetHorizontFace().GetBox(ref objBox);

                var box = MathHelper.GetGlobalCoords((double[])objBox, elem.Component2.Transform2, mathUtility);

                if (min == null || CheckMin(min, box))
                {
                    min = box;
                    index = i;
                }
            }

            return mountingElements.ElementAt(index).GetHorizontFace();
        }

        private bool CheckMin(double[] min, double[] pretender)
        {
            return min[1] < pretender[1];
        }

        #endregion
    }
}
