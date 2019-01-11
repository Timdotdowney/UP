using Microsoft.Ink;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace InstructorClient
{
    public partial class FormMSDNExample : Form
    {
        public FormMSDNExample()
        {
            InitializeComponent();
        }

        Microsoft.Ink.InkCollector theInkCollector;

        // Event handler for the form's load event.
        private void Form_Load(object sender, System.EventArgs e)
        {
            // Create the InkCollector.
            theInkCollector = new InkCollector(this.Handle);

            // Attach an event handler for the Stroke event.
            theInkCollector.Stroke += new InkCollectorStrokeEventHandler(theInkCollector_Stroke);

            // Enable the InkCollector.
            theInkCollector.Enabled = true;
        }

        // Event handler for the InkCollector's Stroke event.
        private void theInkCollector_Stroke(object sender, InkCollectorStrokeEventArgs e)
        {
            // Force the form to repaint.
            Refresh();
        }

        // Event handler for the Clear menu's Click event.
        private void menuClear_Click(object sender, System.EventArgs e)
        {
            // Delete the strokes in the InkCollector.
            theInkCollector.Ink.DeleteStrokes();

            // Force the form to repaint.
            Refresh();
        }

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        // Event handler for the Click event for subitems of the Display menu.
        private void menuDisplayItem_Click(object sender, System.EventArgs e)
        {
            // Check each subitem to see if it was clicked.
            foreach (System.Windows.Forms.ToolStripMenuItem menu in menuDisplay.DropDownItems)
            {
                if (sender == menu)
                {
                    // Toggle the menu item's checked property.
                    menu.Checked = !menu.Checked;
                }
            }

            // Force the form to repaint.
            this.Refresh();
        }

        // Event handler for the form's Paint event.
        private void Form_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Highlight specific points or cusps of each stroke.
            Strokes theStrokes = theInkCollector.Ink.Strokes;
            foreach (Stroke theStroke in theStrokes)
            {
                // Convert the stroke's points and Bezier points from ink space to pixel coordinates.
                Point[] ptBezierPoints = theStroke.BezierPoints;
                Point[] ptStrokePoints = theStroke.GetPoints();
                theInkCollector.Renderer.InkSpaceToPixel(g, ref ptBezierPoints);
                theInkCollector.Renderer.InkSpaceToPixel(g, ref ptStrokePoints);

                // If all points is checked, highlight the points of the stroke.
                if (menuAllPoints.Checked)
                {
                    foreach (Point pt in ptStrokePoints)
                    {
                        // draw a little diagonal line from each point
                        g.DrawEllipse(Pens.Magenta, pt.X - 2, pt.Y - 2, 4, 4);
                    }
                }

                // If polyline cusps is checked, highlight the cusps of the stroke.
                if (menuPolylineCusps.Checked)
                {
                    int[] theCusps = theStroke.PolylineCusps;
                    foreach (int i in theCusps)
                    {
                        // Draw a little rectangle around each polyline cusp.
                        g.DrawEllipse(Pens.BlueViolet, ptStrokePoints[i].X - 3, ptStrokePoints[i].Y - 3, 6, 6);
                    }
                }

                // If Bezier points is checked, highlight the Bezier points of the stroke.
                if (menuBezierPoints.Checked)
                {
                    foreach (Point pt in ptBezierPoints)
                    {
                        // Draw a little diagonal line from each Bezier point.
                        g.DrawEllipse(Pens.Goldenrod, pt.X - 4, pt.Y - 4, 8, 8);
                    }
                }

                // If Bezier cusps is checked, highlight the Bezier cusps of the stroke.
                if (menuBezierCusps.Checked)
                {
                    int[] theCusps = theStroke.BezierCusps;
                    foreach (int i in theCusps)
                    {
                        // Draw a little rectangle around each Bezier cusp.
                        g.DrawEllipse(Pens.Blue,
                            ptBezierPoints[i].X - 5, ptBezierPoints[i].Y - 5, 10, 10);
                    }
                }

                // If self intersections is checked, highlight the self intersections of the stroke.
                if (menuSelfIntersections.Checked)
                {
                    float[] theSelfIntersectionLocationsArray = theStroke.SelfIntersections;
                    foreach (float f in theSelfIntersectionLocationsArray)
                    {
                        Point pt = LocatePoint(theStroke, f);
                        theInkCollector.Renderer.InkSpaceToPixel(g, ref pt);
                        // Draw a little circle around each intersection.
                        g.DrawEllipse(Pens.Red, pt.X - 7, pt.Y - 7, 14, 14);
                    }
                }
            }
        }

        // This function returns the approximate point along
        // a stroke represented by a float, as a Point.
        private Point LocatePoint(Stroke theStroke, float theFIndex)
        {
            // Get the two nearest points to the point of interest.
            Point[] ptStrokePoints = theStroke.GetPoints((int)Math.Floor(theFIndex), 2);

            // Get fractional part to interpolate the distance between the points.
            float theFraction = theFIndex - (float)Math.Floor(theFIndex);
            int deltaX = (int)((ptStrokePoints[1].X - ptStrokePoints[0].X) * theFraction);
            int deltaY = (int)((ptStrokePoints[1].Y - ptStrokePoints[0].Y) * theFraction);

            // Return the interpolated point.
            return new Point(ptStrokePoints[0].X + deltaX, ptStrokePoints[0].Y + deltaY);
        }
    }
}
