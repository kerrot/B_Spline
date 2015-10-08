using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Spline
{
    public class Point
    {
        public double X;
        public double Y;
    }

    public class B_SplineCurve
    {
        private const int RESOLUTION = 10;

        public List<Point> CurrentOutput = new List<Point>();
        public List<Point> Output = new List<Point>();
        public List<Point> ControlPoints = new List<Point>();
        public int Degree = 1;


        public void Compute()
        {
            CurrentOutput.Clear();

            if (ControlPoints.Count() > 1)
            {
                double ustep = 1.0 / (RESOLUTION * (ControlPoints.Count() - 1));

                int n = ControlPoints.Count() - 1;
                int k = (ControlPoints.Count() > Degree) ? Degree + 1 : ControlPoints.Count();
                int m = k + n;

                double[] t = new double[m + 1];
                for (int i = 0; i < k; ++i)
                {
                    t[i] = 0;
                    t[t.Count() - 1 - i] = 1;
                }
                if (m + 1 > 2 * k)
                {
                    double tstep = 1.0 / (m - 2 * (k - 1));
                    for (int i = k; i < m - k + 1; ++i)
                    {
                        t[i] = t[i - 1] + tstep;
                    }
                }

                for (double u = 0; u < 1; u += ustep)
                {
                    OutputPoint(t, k, u);
                }

                CurrentOutput.Add(ControlPoints.Last());
            }
        }

        private void OutputPoint(double[] t, int k, double u)
        {
            int i, j, r;
            double d1, d2;

            int l = 0;

            for (l = 0; l < t.Count() && t[l] <= u; ++l) ;
            l -= 1;

            Point[] A = new Point[k];

            for (j = 0; j < k; ++j)
            {
                int index = l - k + 1 + j;
                if (index < 0 || index > ControlPoints.Count() - 1)
                {
                    return;
                }
                A[j] = new Point() { X = ControlPoints[index].X, Y = ControlPoints[index].Y };
            }

            for (r = 1; r < k; ++r)
            {
                for (j = k - 1; j >= r; --j)
                {
                    i = l - k + 1 + j;
                    d1 = u - t[i];
                    d2 = t[i + k - r] - u;
                    A[j].X = (d1 * A[j].X + d2 * A[j - 1].X) / (d1 + d2);
                    A[j].Y = (d1 * A[j].Y + d2 * A[j - 1].Y) / (d1 + d2);
                }
            }

            CurrentOutput.Add(A[k - 1]);
        }
    }
}
