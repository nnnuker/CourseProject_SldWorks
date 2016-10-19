using System;
using System.Collections.Generic;
using System.Linq;
using SldWorksLogic.Infrastructure;
using SldWorksLogic.Interfaces;
using SolidWorks.Interop.sldworks;

namespace SldWorksLogic.Concrete
{
    public sealed class Element : BaseElement, IElement
    {
        private List<Face2> selectedFaces;
        private List<Entity> supportFace;
        private List<Entity> guideFace;
        private List<Entity> mountingFace;

        public IEnumerable<Face2> SelectedFaces { get { return selectedFaces; } set { selectedFaces = value.ToList(); } }

        #region Ctors
        public Element()
        {
        }
        public Element(Component2 component)
        {
            Initialize(component, new List<Face2>());
        }

        public Element(Component2 component, IEnumerable<Face2> selectedFaces)
        {
            Initialize(component, selectedFaces);
        }

        #endregion

        #region Public faces

        public IEnumerable<Entity> GetSupportFace()
        {
            return supportFace;
        }

        public IEnumerable<Entity> GetGuideFace()
        {
            return guideFace;
        }

        public IEnumerable<Entity> GetMountingFace()
        {
            return mountingFace;
        }

        #endregion

        #region Private methods

        private void Initialize(Component2 component, IEnumerable<Face2> selectedFaces)
        {
            if (component == null)
                throw new ArgumentNullException("Component is null");

            if (selectedFaces == null)
                throw new ArgumentNullException("Selected faces are null");

            this.selectedFaces = selectedFaces.ToList();

            Component2 = component;

            var list = SortByParallel(this.selectedFaces);

            SortFaces(list);

            if (list.Count >= 3)
            {
                supportFace = list[0].Cast<Entity>().ToList();
                guideFace = list[1].Cast<Entity>().ToList();
                mountingFace = list[2].Cast<Entity>().ToList();
            }
        }

        private List<List<Face2>> SortByParallel(List<Face2> faces)
        {
            var list = new List<List<Face2>>();
            var listFaces = faces.ToList();

            while (listFaces.Count > 0)
            {
                var temp = new List<Face2>();
                var face = listFaces.First();

                temp.Add(face);
                listFaces.Remove(face);

                var parallel = listFaces.FindAll(f => MathVectorHelper.IsParallel(GetVector(face), GetVector(f)));

                temp.AddRange(parallel);
                list.Add(temp);
            }

            return list;
        }

        private double[] GetVector(Face2 face)
        {
            return (double[])(object)face.Normal;
        }

        private double[] GetBox(Face2 face)
        {
            return (double[])(object)face.GetBox();
        }

        private void SortFaces(List<List<Face2>> groupFaces)
        {
            double[] array = new double[6];
            foreach (var group in groupFaces)
            {
                foreach (var face in group)
                {
                    var arr = GetBox(face);
                    for (int i = 0; i < 3; i++)
                    {
                        if (arr[i] < array[i])
                        {
                            array[i] = arr[i];
                        }
                    }
                }
            }

            //var count = faces.Count();
            //var faceUVBounds = new double[count][];
            //var size = new double[count][];

            //for (var i = 0; i < count; i++)
            //{
            //    faceUVBounds[i] = new double[4];
            //    faceUVBounds[i] = faces[i].GetUVBounds();

            //    size[i] = new double[2];
            //    size[i][0] = faceUVBounds[i][1] - faceUVBounds[i][0];
            //    size[i][1] = faceUVBounds[i][3] - faceUVBounds[i][2];
            //}

            //for (var i = 0; i < count; i++)
            //{
            //    for (var j = 0; j < count - 1; j++)
            //    {
            //        if (size[j][0] > size[j + 1][0])
            //        {
            //            var temp = size[j];
            //            var tem = faces[j];

            //            size[j] = size[j + 1];
            //            faces[j] = faces[j + 1];

            //            size[j + 1] = temp;
            //            faces[j + 1] = tem;
            //        }
            //    }
            //}
        }

        

        #endregion
    }
}
