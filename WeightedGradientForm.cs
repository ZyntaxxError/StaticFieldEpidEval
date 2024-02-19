using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VMS.CA.Scripting;

namespace StaticFieldEpidEval
{
    /// <summary>
    /// Dialog allowing setting the parameters for gradient weighted difference and for performing the displaying the resulting analysis.
    /// </summary>
    public partial class WeightedGradientForm : Form
    {

        const int m_border = 20;
        const int m_borderLeft = 180;
        const int m_borderBottom = 50;

        private readonly CalculateGWDD m_gwdd;
        private Bitmap m_bitmap;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="analysis">Analysis containing a portal dose image and a reference dose image.
        /// The gradient weighted difference is calculated between these two images in the analysis.</param>
        public WeightedGradientForm(PDAnalysis analysis)
        {
            InitializeComponent();

            m_gwdd = new CalculateGWDD(analysis);

            textBoxDoseDiff.Text = (analysis == null) ? "3.0" : (analysis.GammaParamDoseDiff * 100).ToString("0.0");
            textBoxDTA.Text = (analysis == null) ? "3.0" : analysis.GammaParamDTA.ToString("0.0");

            PerformAnalysis();
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (m_bitmap != null)
            {
                m_bitmap.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Paints the dialog.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (m_bitmap != null)
            {
                int availWidth = ClientSize.Width - m_border - m_borderLeft;
                int availHeight = ClientSize.Height - m_border - m_borderBottom;
                float aspectRatioBitmap = (float)m_bitmap.Width / m_bitmap.Height;
                float aspectRatioWindow = (float)availWidth / availHeight;
                int width, height;
                if (aspectRatioWindow > aspectRatioBitmap)
                {
                    height = availHeight;
                    width = (int)(height * aspectRatioBitmap);
                }
                else
                {
                    width = availWidth;
                    height = (int)(width / aspectRatioBitmap);
                }
                e.Graphics.DrawImage(m_bitmap, m_borderLeft + (availWidth - width) / 2, m_border + (availHeight - height) / 2, width, height);
            }
        }

        private void OnPerformAnalysisClicked(object sender, EventArgs e)
        {
            PerformAnalysis();
        }

        private void PerformAnalysis()
        {
            double doseDiff;
            if (!double.TryParse(textBoxDoseDiff.Text, out doseDiff))
            {
                MessageBox.Show("Dose Difference parameter invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            doseDiff *= 0.01; // map to [0, 1] from [0, 100] percents
            if (doseDiff < 0.001 || doseDiff > 0.2)
            {
                MessageBox.Show("Dose Difference parameter invalid, must be in [0.1, 20]", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            double dta;
            if (!double.TryParse(textBoxDTA.Text, out dta))
            {
                MessageBox.Show("DTA parameter invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dta < 0.01 || dta > 20.0)
            {
                MessageBox.Show("DTA parameter invalid, must be in [0.01, 20]", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CalculateGWDD.GWDDData gwddData = m_gwdd.Calculate(doseDiff, dta);

            DisplayStatisticsData(gwddData);

            // Create the bitmap from the float array and update the screen
            if (m_bitmap != null)
            {
                m_bitmap.Dispose();
            }
            m_bitmap = GenerateBitmap(gwddData.GWDD);
            Invalidate();
        }

        // Displays the staticists data of the gradient weighted dose difference
        private void DisplayStatisticsData(CalculateGWDD.GWDDData gwddData)
        {
            resultGWDDL1.Text = (gwddData.AreaGWDDLessThanOne * 100).ToString("0.0") + " %";
            resultMaxGWDD.Text = gwddData.MaxGWDD.ToString("0.00");
            resultMeanGWDD.Text = gwddData.MeanGWDD.ToString("0.00");
        }

        // Creates a 32 bit per pixel bitmap from the given gradient weighted dose difference map.
        // Colors similar to the ones used in Gamma analysis are used in the bitmap.
        private static Bitmap GenerateBitmap(float[,] gwdd)
        {
            int sizeX = gwdd.GetLength(0);
            int sizeY = gwdd.GetLength(1);
            Bitmap bm = new Bitmap(sizeX, sizeY, PixelFormat.Format32bppRgb);
            unsafe
            {
                BitmapData bmData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                uint* pBitmapData = (uint*)bmData.Scan0;
                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        float value = gwdd[x, y];
                        Color color = GammaLikeColor(value);
                        *pBitmapData = (uint)color.ToArgb();
                        pBitmapData++;
                    }
                }
                bm.UnlockBits(bmData);
            }

            return bm;
        }

        private static Color GammaLikeColor(float value)
        {
            if (value <= 1)
            {
                return Color.FromArgb(0, RampAboveMinTrigger(value, 0, 1), 0);
            }
            return Color.FromArgb(RampAboveMaxTrigger(value, 60, 0, 1), RampAboveMinTrigger(value, 0, 1), 0);
        }

        private static byte RampAboveMinTrigger(double value, double minTrigger, double maxTrigger)
        {
            double interval = 2 * (maxTrigger - minTrigger);
            if (value < minTrigger)
            { // off side
                return 255;
            }
            else if (value < minTrigger + interval)
            { // ramp
                return (byte)(255.0 * (1.0 - ((value - minTrigger) / interval)));
            }
            else
            { // off side
                return 0;
            }
        }

        private static byte RampAboveMaxTrigger(double value, byte offset, double minTrigger, double maxTrigger)
        {
            double interval = 2 * (maxTrigger - minTrigger);
            if (value < maxTrigger)
            { // off side
                return 0;
            }
            else if (value < maxTrigger + interval)
            { // ramp
                return (byte)(offset + ((255 - offset) * (1.0 - ((value - maxTrigger) / interval))));
            }
            else
            { // off side
                return offset;
            }
        }

    }
}
