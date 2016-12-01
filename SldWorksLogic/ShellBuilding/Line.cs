using System;

namespace SldWorksLogic.ShellBuilding
{
    public class Line:IEquatable<Line>
    {
        public string Name { get; set; }
        public Point First { get; set; }
        public Point Second { get; set; }

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

        public bool Equals(Line other)
        {
            if (other == null) return false;

            return (First.Equals(other.First) && Second.Equals(other.Second)) || (First.Equals(other.Second) && Second.Equals(other.First));
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
