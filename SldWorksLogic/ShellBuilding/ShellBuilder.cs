using SldWorksLogic.Concrete;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SldWorksLogic.Infrastructure;

namespace SldWorksLogic.ShellBuilding
{
    public class ShellBuilder
    {
        private readonly MathUtility mathUtility = SldWorksWorker.MathUtility;
        private List<Line> lines;

        public ShellBuilder()
        {
            lines = new List<Line>();
        }

        private double[] GetBox(Component2 component)
        {
            var box = (double[])(object)component.GetBox(false, false);
            return MathHelper.GetGlobalCoords(box, component.Transform2, mathUtility);
        }

        private List<MathPoint> GetPoints(double[] box)
        {
            return new List<MathPoint>() 
            { 
                mathUtility.CreatePoint(new double[] { box[0], 0, box[2] }), 
                mathUtility.CreatePoint(new double[] { box[0], 0, box[5] }),
                mathUtility.CreatePoint(new double[] { box[3], 0, box[5] }),
                mathUtility.CreatePoint(new double[] { box[3], 0, box[2] })
            };
        }

        public void GetShell(IEnumerable<MountingElement> elements)
        {
            var points = new List<MathPoint>();

            foreach (var element in elements)
            {
                var array = GetBox(element.Component2);

                points.AddRange(GetPoints(array));
            }

            CreateLines(points);
        }

        private void CreateLines(List<MathPoint> points)
        {
            foreach (var point in points)
            {
                foreach (var item in points)
                {
                    if (item == point)
                        continue;

                    var A = 1 / (((double[])point.ArrayData)[0] - ((double[])item.ArrayData)[0]);
                    var B = 1 / (((double[])point.ArrayData)[1] - ((double[])item.ArrayData)[1]);

                    var L = Math.Sqrt(Math.Pow(A, 2) + Math.Pow(B, 2));
                    A = A / L;
                    B = B / L;

                    var D = -A * ((double[])point.ArrayData)[0] - B * ((double[])point.ArrayData)[1];

                    if (CheckD(D, A, B, points.Except(new []{ point, item })))
                    {
                        lines.Add(new Line(point, item));
                        break;
                    }
                }
            }
        }

        private bool CheckD(double D, double A, double B, IEnumerable<MathPoint> points)
        {
            var max = true;
            var min = true;

            foreach (var item in points)
            {
                var Dj = -A * ((double[])item.ArrayData)[0] - B * ((double[])item.ArrayData)[1];

                if (Dj > D)
                {
                    max = false;
                }

                if (Dj < D)
                {
                    min = false;
                }
                
                if (!max && !min)
                {
                    break;
                }
            }

            return max || min;
        }

        //public void BuildingSurface()
        //{
        //    Component2 newComp;
        //    double X1 = 0, Y1 = 0, X2 = 0, Y2 = 0;

        //    assDoc.InsertNewVirtualPart(faces1[0], out newComp);
        //    newComp.Select(true);
        //    assDoc.EditPart2(true, false, longstatus);

        //    Feature sk = newComp.FeatureByName("Эскиз1");

        //    sk.Select(true);
        //    swDoc.SketchManager.InsertSketch(true);

        //    //Feature sur = newComp.FeatureByName("Исходная точка");
        //    //Feature sur1 = comp[0].FeatureByName("Исходная точка");
        //    //Feature sur2 = comp[1].FeatureByName("Исходная точка");
        //    //Feature sur3 = comp[2].FeatureByName("Исходная точка");

        //    //object p1, p2, p3, p4, p5, p6;

        //    //double r1 = swDoc.ClosestDistance(sur, sur1, out p1, out p2);
        //    //double r2 = swDoc.ClosestDistance(sur, sur2, out p3, out p4);
        //    //double r3 = swDoc.ClosestDistance(sur, sur3, out p5, out p6);            

        //    //double[] P1 = (double[])p1;
        //    //double[] P2 = (double[])p2;
        //    //double[] P3 = (double[])p3;
        //    //double[] P4 = (double[])p4;
        //    //double[] P5 = (double[])p5;
        //    //double[] P6 = (double[])p6;

        //    double[] obj1 = comp[0].GetBox(false, false);
        //    double[] obj2 = comp[1].GetBox(false, false);
        //    double[] obj3 = comp[2].GetBox(false, false);
        //    double[] obj4 = comp[5].GetBox(false, false);

        //    double[][] points = new double[][] 
        //    { new double[] { obj4[0] * (-1), obj4[2] * (-1) },
        //      new double[] { obj4[0] * (-1), obj4[5] * (-1) },
        //      new double[] { obj4[3] * (-1), obj4[5] * (-1) },
        //      new double[] { obj4[3] * (-1), obj4[2] * (-1) },
        //      new double[] { obj2[3] * (-1), obj2[5] * (-1) },
        //      new double[] { obj2[3] * (-1), obj2[2] * (-1) },
        //      new double[] { obj3[3] * (-1), obj3[5] * (-1) },
        //      new double[] { obj3[3] * (-1), obj3[2] * (-1) },
        //      new double[] { obj3[0] * (-1), obj3[2] * (-1) },
        //      new double[] { obj3[0] * (-1), obj3[5] * (-1) },
        //      new double[] { obj1[0] * (-1), obj1[2] * (-1) },
        //      new double[] { obj1[0] * (-1), obj1[5] * (-1) } };

        //    //double[][] points = new double[][] 
        //    //{ new double[] { obj1[0] * (-1), obj1[2] * (-1) },
        //    //  new double[] { obj1[0] * (-1), obj1[5] * (-1) },
        //    //  new double[] { obj1[3] * (-1), obj1[5] * (-1) },
        //    //  new double[] { obj1[3] * (-1), obj1[2] * (-1) },
        //    //  new double[] { obj2[0] * (-1), obj2[2] * (-1) },
        //    //  new double[] { obj2[0] * (-1), obj2[5] * (-1) },
        //    //  new double[] { obj2[3] * (-1), obj2[5] * (-1) },
        //    //  new double[] { obj2[3] * (-1), obj2[2] * (-1) },
        //    //  new double[] { obj3[3] * (-1), obj3[5] * (-1) },
        //    //  new double[] { obj3[3] * (-1), obj3[2] * (-1) },
        //    //  new double[] { obj3[0] * (-1), obj3[2] * (-1) },
        //    //  new double[] { obj3[0] * (-1), obj3[5] * (-1) } }; 

        //    int count = points.Length;

        //    for (int i = 0; i < count - 1; i++)
        //    {
        //        X1 = points[i][0];
        //        Y1 = points[i][1];
        //        X2 = points[i + 1][0];
        //        Y2 = points[i + 1][1];
        //        swDoc.SketchManager.CreateLine(X1, Y1, 0, X2, Y2, 0);
        //    }

        //    swDoc.SketchManager.CreateLine(points[0][0], points[0][1], 0, points[count - 1][0], points[count - 1][1], 0);

        //    swDoc.FeatureManager.FeatureExtrusion2(true, false, false, 0, 0, 0.01, 0.01, false, false,
        //        false, false, 0, 0, false, false, false, false, true, true, true, 0, 0, false);
        //    mgr.EnableContourSelection = false;
        //    assDoc.AssemblyPartToggle();
        //    assDoc.EditAssembly();
        //    swDoc.ClearSelection2(true);

        //    swDoc.Extension.SelectByID2("На месте1", "MATE", 0, 0, 0, false, 0, null, 0);
        //    swDoc.EditDelete();

        //    Body2 bodies = newComp.GetBody();
        //    object[] tempFaces = bodies.GetFaces();

        //    Face2 circle1 = (Face2)faces[0][2];

        //    Entity en1 = (Entity)circle1;

        //    en1.Select(true);
        //    Entity en = (Entity)tempFaces[tempFaces.Length - 1];
        //    en.Select(true);
        //    assDoc.AddMate3(0, 1, false, 0, 0, 0, 0, 0, 0, 0, 0, false, out longstatus);
        //}
    }
}
