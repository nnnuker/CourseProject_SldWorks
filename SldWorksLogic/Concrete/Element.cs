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
        private readonly MathUtility mathUtility;
        private List<Face2> selectedFaces;
        private List<Entity> supportFace;
        private List<Entity> guideFace;
        private List<Entity> mountingFace;

        public List<Face2> SelectedFaces { get { return selectedFaces; } set { selectedFaces = value; } }

        #region Ctors
        public Element()
        {
        }
        public Element(Component2 component)
        {
            Initialize(component, new List<Face2>());
        }

        public Element(Component2 component, MathUtility mathUtility, IEnumerable<Face2> selectedFaces)
        {
            this.mathUtility = mathUtility;
            Initialize(component, selectedFaces);
        }

        #endregion

        #region Public methods

        public List<Entity> GetSupportFace()
        {
            return supportFace;
        }

        public List<Entity> GetGuideFace()
        {
            return guideFace;
        }

        public List<Entity> GetMountingFace()
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

            if (list.Count >= 3)
            {
                SortFaces(list);
                //supportFace = list[0].Cast<Entity>().ToList();
                //guideFace = list[1].Cast<Entity>().ToList();
                //mountingFace = list[2].Cast<Entity>().ToList();
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

                var parallel = listFaces.FindAll(f => MathHelper.IsParallel(GetVector(face), GetVector(f)));

                temp.AddRange(parallel);
                parallel.ForEach(p => listFaces.Remove(p));
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
            var groupBoxes = new List<Help>();
            foreach (var group in groupFaces)
            {
                var array = GetBox(group.First());
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

                    for (int i = 3; i < 6; i++)
                    {
                        if (arr[i] > array[i])
                        {
                            array[i] = arr[i];
                        }
                    }
                }
                groupBoxes.Add(new Help { Group = group, GroupArray = array, Max = GetMax(array)});
            }

            groupBoxes = groupBoxes.OrderByDescending(gr => gr.Max).ToList();

            supportFace = groupBoxes[0].Group.Cast<Entity>().ToList();
            guideFace = groupBoxes[1].Group.Cast<Entity>().ToList();
            mountingFace = groupBoxes[2].Group.Cast<Entity>().ToList();
        }

        private double GetMax(double[] array)
        {
            double xy = Math.Abs(array[3] - array[0]) * Math.Abs(array[4] - array[1]);
            double xz = Math.Abs(array[3] - array[0]) * Math.Abs(array[5] - array[2]);
            double yz = Math.Abs(array[4] - array[1]) * Math.Abs(array[5] - array[2]);

            return new[] { xy, xz, yz }.Max();
        }

        private class Help
        {
            public List<Face2> Group { get; set; }
            public double[] GroupArray { get; set; }
            public double Max { get; set; }
        }

        #endregion
    }
}
