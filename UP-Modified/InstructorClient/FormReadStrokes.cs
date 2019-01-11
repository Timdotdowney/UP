using Microsoft.Ink;
using SlideModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstructorClient
{
    public partial class FormReadStrokes : Form
    {
        public FormReadStrokes()
        {
            InitializeComponent();
        }

        public ArrayList Slides { get;  set; }


        private Slide CurrentSlide
        {
            get
            {
                if (currentIndex < Slides.Count)
                {
                    toolStripStatusLabelIndex.Text = String.Format("Slide {0} of {1}", currentIndex + 1, Slides.Count);
                    return (Slide)Slides[currentIndex];
                }
                return null;
            }
        }

        int currentIndex = 0;
        private int CurrentIndex
        {
            get {
                return currentIndex;
            }
            set
            {
                if (currentIndex != value)
                {
                    currentIndex = value;
                    if (CurrentSlide?.InstructorImage != null)
                    {
                        panelDraw.Size = CurrentSlide.InstructorImage.Size;
                        panelDraw.BackgroundImage = CurrentSlide.InstructorImage;
                    } else
                    {
                        panelDraw.BackgroundImage = null;
                    }
                }
            }
        }

        Microsoft.Ink.InkCollector theInkCollector;

        private void FormReadStrokes_Load(object sender, EventArgs e)
        {
            // Create the InkCollector.
            theInkCollector = new InkCollector(this.Handle);

            // Attach an event handler for the Stroke event.
            //theInkCollector.Stroke += new InkCollectorStrokeEventHandler(theInkCollector_Stroke);

            // Enable the InkCollector.
            // theInkCollector.Enabled = true;
            textBoxScaleX.Text = 0.7f.ToString();
            textBoxScaleY.Text = 0.69f.ToString();
            
        }

        // Event handler for the InkCollector's Stroke event.
        //private void theInkCollector_Stroke(object sender, InkCollectorStrokeEventArgs e)
        //{
        //    // Force the form to repaint.
        //    Refresh();
        //}

        private void FormReadStrokes_Paint(object sender, PaintEventArgs e)
        {

        }

        #region Button Navigation

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            if (CurrentIndex > 0)
            {
                CurrentIndex--;
            }
            panelDraw.Invalidate();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (CurrentIndex < Slides.Count - 1)
            {
                CurrentIndex++;
            }
            panelDraw.Invalidate();
        }

        #endregion

        private void panelDraw_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (CurrentSlide != null)
            {
                //if (CurrentSlide.InstructorImage != null)
                //{
                //   using (Graphics imageGraphics = Graphics.FromImage(CurrentSlide.InstructorImage))
                //   {
                //        PaintWithImageGraphics(imageGraphics);
                //   }
                //} else
                //{
                    PaintWithImageGraphics(g);
                //}
            }
        }

       

        private void PaintWithImageGraphics(Graphics g)
        {
            GraphicsState state = g.Save();
            g.ScaleTransform(ScaleX, ScaleY);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            foreach (Stroke theStroke in CurrentSlide.Ink.Strokes)
            {
                //theStroke.Scale(ScaleX, ScaleY);
                
                // Convert the stroke's points and Bezier points from ink space to pixel coordinates.
                Point[] ptStrokePoints = theStroke.GetPoints();
                theInkCollector.Renderer.InkSpaceToPixel(g, ref ptStrokePoints);
                DrawingAttributes att = theStroke.DrawingAttributes;
                g.DrawLines(new Pen(att.Color, att.Width/17), ptStrokePoints);
                //foreach (Point pt in ptStrokePoints)
                //{
                //    // draw a little ellipse line from each point
                //    g.DrawEllipse(Pens.Magenta, pt.X - 2, pt.Y - 2, 4, 4);
                //}
            }
            g.Restore(state);
        }

        private void panelDraw_MouseDown(object sender, MouseEventArgs e)
        {
            toolStripStatusLabelLocation.Text = e.Location.ToString();
        }

        float ScaleX
        {
            get
            {
                float scale = 0.68f;
                float.TryParse(textBoxScaleX.Text, out scale);
                return scale;
            }
            set
            {
                textBoxScaleX.Text = value.ToString();
            }
        }
        float ScaleY
        {
            get
            {
                float scale = 0.68f;
                float.TryParse(textBoxScaleY.Text, out scale);
                return scale;
            }
            set
            {
                textBoxScaleY.Text = value.ToString();
            }
        }

        private void Rescale(object sender, EventArgs e)
        {
            panelDraw.Invalidate();
        }
    }
}
