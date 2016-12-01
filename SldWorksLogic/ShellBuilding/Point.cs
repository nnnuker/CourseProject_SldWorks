using System;
using System.Linq;

namespace SldWorksLogic.ShellBuilding
{
    public class Point:IEquatable<Point>
    {
        public double[] Coordinates { get; set; }

        public Point(double[] coordinates)
        {
            Coordinates = coordinates;
        }

        public bool Equals(Point other)
        {
            if (other == null || Coordinates.Length != other.Coordinates.Length)
            {
                return false;
            }

            for (int i = 0; i < Coordinates.Length; i++)
            {
                if (!Coordinates[i].Equals(other.Coordinates[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            var val = obj as Point;
            
            return val != null && this.Equals(val);
        }

        public override int GetHashCode()
        {
            return Coordinates.Aggregate(int.MaxValue, (current, coord) => current & coord.GetHashCode());
        }
    }
}