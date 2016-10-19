using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SldWorksLogic.Infrastructure
{
    public static class MathVectorHelper
    {
        public static bool IsParallel(double[] first, double[] second)
        {
            bool flag = false;

            if (CheckHasZero(first) || CheckHasZero(second))
            {
                flag = FirstCollinear(first, second);
            }
            else
            {
                flag = SecondCollinear(first, second);
            }

            return flag;
        }

        private static bool CheckHasZero(double[] array)
        {
            return array.Any(d => d == 0.0);
        }

        private static bool FirstCollinear(double[] first, double[] second)
        {
            double temp = 0;
            for (var i = 0; i < first.Length; i++)
            {
                if (first[i] != 0)
                {
                    temp = second[i] / first[i];
                    break;
                }
            }

            var firstTemp = first.Select(d => d * temp);

            return firstTemp.SequenceEqual(second);
        }

        private static bool SecondCollinear(double[] first, double[] second)
        {
            bool flag = true;
            double temp = first[0] / second[0];

            for (int i = 1; i < first.Length; i++)
            {
                if (Math.Abs(temp - first[i] / second[i]) > 0)
                {
                    flag = false;
                    break;
                }
            }

            return flag;
        }
    }
}
