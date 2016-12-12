using System;

namespace SldWorksLogic.ShellBuilding
{
    public class Line : IEquatable<Line>
    {
        public string Name { get; set; }
        public Point First { get; set; }
        public Point Second { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double D { get; set; }
        public double Length
        {
            get
            {
                return GetLength();
            }
        }

        public Line()
        {
            First = default(Point);
            Second = default(Point);
            Name = string.Empty;
        }

        public Line(Point first, Point second)
        {
            First = first;
            Second = second;
        }

        public bool LineIsSame(Line other)
        {
            return other.A.Equals(A) && other.B.Equals(B) && other.D.Equals(D);
        }

        public bool Equals(Line other)
        {
            if (other == null) return false;

            return (First.Equals(other.First) && Second.Equals(other.Second)) || (First.Equals(other.Second) && Second.Equals(other.First));
        }

        private double GetLength()
        {
            var deltaX = Pow(First.Coordinates[0], Second.Coordinates[0]);
            var deltaY = Pow(First.Coordinates[1], Second.Coordinates[1]);
            var deltaZ = Pow(First.Coordinates[2], Second.Coordinates[2]);

            return Math.Sqrt(deltaX + deltaY + deltaZ);
        }

        private double Pow(double first, double second)
        {
            return Math.Pow(second - first, 2);
        }

        public override bool Equals(object obj)
        {
            var line = obj as Line;
            return line != null && this.Equals(line);
        }

        public override int GetHashCode()
        {
            var i = int.MaxValue;

            i = i & First.GetHashCode() & Second.GetHashCode();

            return i;
        }
    }
}
