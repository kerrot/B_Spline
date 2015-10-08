using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace B_Spline
{
    public partial class Form1 : Form
    {
        private List<B_SplineCurve> curves = new List<B_SplineCurve>();
        private B_SplineCurve current = new B_SplineCurve();

        private const int CONTROLPOINTSIZE = 10;
        private const int LINESIZE = 5;
        private const int STIPPLELINESIZE = 1;

        public Form1()
        {
            InitializeComponent();

            current.ControlPoints.Add(new Point());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            glControl1_Resize(this, EventArgs.Empty);
            GL.ClearColor(Color.Black);
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (glControl1.ClientSize.Height == 0)
                glControl1.ClientSize = new System.Drawing.Size(glControl1.ClientSize.Width, 1);

            GL.Viewport(0, 0, glControl1.ClientSize.Width, glControl1.ClientSize.Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.Ortho(0, glControl1.ClientSize.Width, 0, glControl1.ClientSize.Height, 1, -1);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            glControl1.MakeCurrent();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Clear(ClearBufferMask.ColorBufferBit);

            foreach (B_SplineCurve curve in curves)
            {
                DrawCurve(curve, false);
            }

            if (current.ControlPoints.Count() > 0)
            {
                DrawCurve(current, true);
            }

            glControl1.SwapBuffers();
        }

        private void DrawCurve(B_SplineCurve curve, bool editing)
        {
            if (curve.Degree > 1)
            {
                GL.Enable(EnableCap.LineStipple);
                GL.LineStipple(1, Convert.ToInt16("1111000011110000", 2));
                GL.Color3((editing) ? Color.Gray : Color.Brown);
                GL.LineWidth(STIPPLELINESIZE);
                GL.Begin(PrimitiveType.LineStrip);

                foreach (Point p in curve.ControlPoints)
                {
                    GL.Vertex2(p.X, p.Y);
                }

                GL.End();

                GL.Disable(EnableCap.LineStipple);
            }

            if (!editing || curve.Degree > 1)
            {
                GL.Color3((editing) ? Color.Gray : Color.Brown);
                GL.LineWidth(LINESIZE);
                GL.Begin(PrimitiveType.LineStrip);

                foreach (Point p in curve.Output)
                {
                    GL.Vertex2(p.X, p.Y);
                }

                GL.End();
            }

            if (editing)
            {
                GL.Color3(Color.White);
                GL.LineWidth(LINESIZE);
                GL.Begin(PrimitiveType.LineStrip);

                foreach (Point p in curve.CurrentOutput)
                {
                    GL.Vertex2(p.X, p.Y);
                }

                GL.End();
            }

            GL.Color3((editing) ? Color.Yellow : Color.Brown);
            GL.PointSize(CONTROLPOINTSIZE);
            GL.Begin(PrimitiveType.Points);
            foreach (Point p in curve.ControlPoints)
            {
                GL.Vertex2(p.X, p.Y);
            }
            GL.End();
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (current.ControlPoints.Count() == 1)
                {
                    current.Degree = (int)numericUpDown1.Value;
                }
                
                Point p = current.ControlPoints.Last();
                p.X = e.X;
                p.Y = glControl1.ClientSize.Height - e.Y;

                current.Output = current.CurrentOutput;
                current.CurrentOutput = new List<Point>();

                current.ControlPoints.Add(new Point());
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                current.ControlPoints.RemoveAt(current.ControlPoints.Count() - 1);
                current.Compute();
                curves.Add(current);

                current = new B_SplineCurve();
                current.ControlPoints.Add(new Point());
            }
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = current.ControlPoints.Last();
            p.X = e.X;
            p.Y = glControl1.ClientSize.Height - e.Y;

            current.Compute();

            glControl1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            curves.Clear();
            glControl1.Invalidate();
        }
    }
}
