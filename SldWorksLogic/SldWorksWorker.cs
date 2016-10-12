using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SldWorksLogic.Infrastructure;
using SolidWorks.Interop.sldworks;

namespace SldWorksLogic
{
    public class SldWorksWorker
    {
        private readonly SldWorks swApp;
        private readonly AssemblyDoc assDoc;
        private ModelDoc2 swDoc;
        private int longstatus = 0;
        private int longwarnings = 0;

        private readonly Element[] paths = PathConfig.GetPaths();
        private SelectionMgr mgr;

        public SldWorksWorker()
        {
            swApp = new SldWorks();
            assDoc = swApp.ActiveDoc;
            swDoc = swApp.ActiveDoc;
        }

        public void AddUnits()
        {
            foreach (var path in paths)
            {
                for (var i = 0; i < path.Count; i++)
                {
                    OpenDoc2(path.Path);
                }
            }
        }

        private void OpenDoc2(params string[] paths)
        {
            foreach (var path in paths)
            {
                swApp.OpenDocSilent(path, 1, longwarnings);
                AddComponent(path, 0, 0, 0);
            }
        }

        private void AddComponent(string path, double x, double y, double z)
        {
            var comp = assDoc.AddComponent4(path, string.Empty, x, y, z);
            assDoc.EditRebuild();
            swApp.CloseDoc(path);
        }

        public void CreateMates()
        {
            var faces = GetSelectedFaces();

            if (faces.Length < 3)
            {
                throw new ArgumentException("Not enough selected faces");
            }

            SortFaces(faces);

            var entities = CreateEntitiesFromFaces(faces);

            var comp = (Component2[])assDoc.GetComponents(true);
            var bodies = new Body2[comp.Length];
            var bodyFaces = new object[comp.Length][];

            for (var i = 0; i < comp.Length; i++)
            {
                bodies[i] = (Body2)comp[i].GetBody();
                bodyFaces[i] = bodies[i].GetFaces();
            }

            for (var i = 0; i < bodyFaces.Length; i++)
            {
                for (var j = 0; j < bodyFaces.Length - 1; j++)
                {
                    if (bodyFaces[j].Length > bodyFaces[j + 1].Length)
                    {
                        var temp = bodyFaces[j];
                        var tcomp = comp[j];

                        comp[j] = comp[j + 1];
                        bodyFaces[j] = bodyFaces[j + 1];

                        comp[j + 1] = tcomp;
                        bodyFaces[j + 1] = temp;
                    }
                }
            }
            
            var circle1 = (Face2)bodyFaces[0][2];
            var circle2 = (Face2)bodyFaces[1][2];
            var circle3 = (Face2)bodyFaces[2][2];

            var en1 = (Entity)circle1;
            var en2 = (Entity)circle2;
            var en3 = (Entity)circle3;

            var sur1 = comp[5].FeatureByName("Плоскость1");
            var sur2 = comp[5].FeatureByName("Плоскость2");
            var axis = comp[4].FeatureByName("AxisForMating");

            entities[1].Select(true);
            sur1.Select(true);
            assDoc.AddMate3(4, 1, false, 0, 0, 0, 0, 0, 0, 0, 0, false, out longstatus);
            assDoc.EditRebuild();
            sur1.DeSelect();

            sur2.Select(true);
            assDoc.AddMate3(4, 1, false, 0, 0, 0, 0, 0, 0, 0, 0, false, out longstatus);
            assDoc.EditRebuild();
            sur2.DeSelect();
            entities[1].DeSelect();

            entities[2].Select(true);
            axis.Select(true);
            assDoc.AddMate3(1, 0, false, 0, 0, 0, 0, 0, 0, 0, 0, false, out longstatus);
            assDoc.EditRebuild();
            entities[2].DeSelect();
            axis.DeSelect();

            entities[0].Select(true);
            en1.Select(true);
            assDoc.AddMate3(0, 1, false, 0, 0, 0, 0, 0, 0, 0, 0, false, out longstatus);
            assDoc.EditRebuild();
            en1.DeSelect();

            en2.Select(true);
            assDoc.AddMate3(0, 1, false, 0, 0, 0, 0, 0, 0, 0, 0, false, out longstatus);
            assDoc.EditRebuild();
            en2.DeSelect();

            en3.Select(true);
            assDoc.AddMate3(0, 1, false, 0, 0, 0, 0, 0, 0, 0, 0, false, out longstatus);
            assDoc.EditRebuild();
            en3.DeSelect();
        }

        private Face2[] GetSelectedFaces()
        {
            swDoc = (ModelDoc2)swApp.ActiveDoc;
            mgr = swDoc.SelectionManager;

            var selectedObjectCount = mgr.GetSelectedObjectCount();

            var faces = new Face2[selectedObjectCount];

            for (var i = 0; i < selectedObjectCount; i++)
            {
                faces[i] = mgr.GetSelectedObject6(i + 1, -1);
            }

            return faces;
        }

        private Entity[] CreateEntitiesFromFaces(Face2[] faces)
        {
            var count = faces.Length;

            var entities = new Entity[count];
            for (var i = 0; i < count; i++)
            { 
                entities[i] = (Entity)faces[i];
                entities[i].DeSelect();
            }

            return entities;
        }

        private void SortFaces(Face2[] faces)
        {
            var count = faces.Length;
            var faceUVBounds = new double[count][];
            var size = new double[count][];

            for (var i = 0; i < count; i++)
            {
                faceUVBounds[i] = new double[4];
                faceUVBounds[i] = faces[i].GetUVBounds();

                size[i] = new double[2];
                size[i][0] = faceUVBounds[i][1] - faceUVBounds[i][0];
                size[i][1] = faceUVBounds[i][3] - faceUVBounds[i][2];
            }

            for (var i = 0; i < count; i++)
            {
                for (var j = 0; j < count - 1; j++)
                {
                    if (size[j][0] > size[j + 1][0])
                    {
                        var temp = size[j];
                        var tem = faces[j];

                        size[j] = size[j + 1];
                        faces[j] = faces[j + 1];

                        size[j + 1] = temp;
                        faces[j + 1] = tem;
                    }
                }
            }
        }
    }
}
