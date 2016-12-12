using SldWorksLogic.Concrete;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SldWorksLogic.ShellBuilding
{
    public class ShellBuilder
    {
        private readonly MathUtility mathUtility;

        public ShellBuilder(MathUtility mathUtility)
        {
            this.mathUtility = mathUtility;
        }

        private double[] GetBox(Component2 component)
        {
            var box = (double[])(object)component.GetBox(true, true);
            //return MathHelper.GetGlobalCoords(box, component.Transform2, mathUtility);
            return box;
        }

        private List<Point> GetPoints(double[] box)
        {
            return new List<Point>()
            {
                new Point(new double[] { box[0], box[1], box[2] }),
                new Point(new double[] { box[0], box[1], box[5] }),
                new Point(new double[] { box[3], box[1], box[5] }),
                new Point(new double[] { box[3], box[1], box[2] })
            };
        }

        public List<Line> GetShell(IEnumerable<MountingElement> elements)
        {
            var points = new List<Point>();

            foreach (var element in elements)
            {
                var array = GetBox(element.Component2);

                points.AddRange(GetPoints(array));
            }

            return CreateLines(points);
        }

        private List<Line> CreateLines(List<Point> points)
        {
            var lines = new List<Line>();

            var i = 0;
            foreach (var point in points)
            {
                foreach (var item in points)
                {
                    if (item.Equals(point))
                        continue;

                    double A = 0;
                    double B = 0;
                    double D = 0;

                    A = item.Coordinates[2] - point.Coordinates[2];
                    B = point.Coordinates[0] - item.Coordinates[0];

                    var L = Math.Sqrt(Math.Pow(A, 2) + Math.Pow(B, 2));
                    A = A / L;
                    B = B / L;

                    D = -A * point.Coordinates[0] - B * point.Coordinates[2];

                    if (CheckD(D, A, B, points.Except(new[] { point, item })))
                    {
                        i++;

                        var line = new Line() { First = point, Second = item, Name = i.ToString(), A = A, B = B, D = D };

                        var same = lines.Find(l => l.LineIsSame(line));

                        if (same == null)
                        {
                            lines.Add(line);
                        }
                        else
                        {
                            if (line.Length >= same.Length)
                            {
                                lines.Remove(same);
                                lines.Add(line);
                            }
                        }
                    }
                }
            }

            return lines.Distinct().ToList();
        }

        private bool CheckD(double D, double A, double B, IEnumerable<Point> points)
        {
            var max = true;
            var min = true;

            foreach (var point in points)
            {
                var Dj = -A * point.Coordinates[0] - B * point.Coordinates[2];

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
    }
}
