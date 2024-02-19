using System;
using System.Collections.Generic;
using System.Linq;
using VMS.CA.Scripting;

namespace StaticFieldEpidEval
{
    /// <summary>
    /// Calculates the gradient weighted dose difference.
    /// </summary>
    public class CalculateGWDD
    {

        /// <summary>
        /// Result of the <see cref="CalculateGWDD.Calculate"/> method.
        /// Contains the gradient weighted dose difference map and some statistics data.
        /// </summary>
        public class GWDDData
        {
            /// <summary>Gradient weighted dose difference map</summary>
            public readonly float[,] GWDD;
            /// <summary>Statistics data: Area where the GWDD is less than one (inside of the ROI, in percents)</summary>
            public readonly double AreaGWDDLessThanOne;
            /// <summary>Statistics data: Maximum GWDD (inside of the ROI)</summary>
            public readonly double MaxGWDD;
            /// <summary>Statistics data: Mean GWDD (inside of the ROI)</summary>
            public readonly double MeanGWDD;

            public GWDDData(float[,] gwdd, double areaGWDDLessThanOne, double maxGWDD, double meanGWDD)
            {
                GWDD = gwdd;
                AreaGWDDLessThanOne = areaGWDDLessThanOne;
                MaxGWDD = maxGWDD;
                MeanGWDD = meanGWDD;
            }
        }

        private readonly PDAnalysis m_analysis;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="analysis">Analysis containing a portal dose image and a reference dose image.
        /// The gradient weighted difference is calculated between these two images in the analysis.</param>
        public CalculateGWDD(PDAnalysis analysis)
        {
            m_analysis = analysis;
        }

        /// <summary>
        /// Calculates the gradient weighted dose difference.
        /// </summary>
        /// <param name="doseDiff">Dose difference in percents (0, 1) of the maximum reference dose</param>
        /// <param name="dta">Distance to agreement in mm</param>
        /// <returns>The returned GWDD data has dimension [XSize, YSize] (of portal dose)</returns>
        /// <remarks>The returned map is normalized similar to the normalization done in the Gamma analysis.
        /// 0 is the best value, 1 is acceptable, > 1 needs investigation</remarks>
        public GWDDData Calculate(double doseDiff, double dta)
        {
            double gradientNormalizer = Math.Sqrt(2) - 1;

            Frame refDoseOnPortal = m_analysis.ReferenceImageOnPortalDoseImageGrid.Frames[0];
            Frame portalDose = m_analysis.PortalDoseImage.Image.Frames[0];
            double normFactorDTA = 1.0 / dta;
            double normFactorDoseDiff = 1.0 / (MaxReferenceDoseValue(m_analysis) * doseDiff);

            // Conversion from pixel raw values to dose values taking normalization as defined by the analysis into account
            double slopeRef, interceptRef;
            m_analysis.GetTransferFunction(true, out slopeRef, out interceptRef);
            double slopePort, interceptPort;
            m_analysis.GetTransferFunction(false, out slopePort, out interceptPort);

            // size and resolution of portalDose and refDoseOnPortal are identical
            int sizeX = portalDose.XSize;
            int sizeY = portalDose.YSize;

            // the inverse is calculated to avoid a (slow) division in the inner pixel loops
            double resXNormInv = 1.0 / (portalDose.XRes * normFactorDTA);
            double resYNormInv = 1.0 / (portalDose.YRes * normFactorDTA);

            // Fetch the pixels
            ushort[,] pixelsRef = new ushort[sizeX, sizeY];
            refDoseOnPortal.GetVoxels(0, pixelsRef);

            ushort[,] pixelsPort = new ushort[sizeX, sizeY];
            portalDose.GetVoxels(0, pixelsPort);

            // Fetch the ROI map
            byte[,] roiMap = m_analysis.ROIMask;

            // Initialize the statistics data counters
            int numPixelsInROI = 0;
            int numPixelsLessThanOne = 0;
            double sumGWDD = 0;
            double maxGWDD = 0;

            // Output array
            float[,] gwdd = new float[sizeX, sizeY];

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    double doseRef = (pixelsRef[x, y] * slopeRef + interceptRef) * normFactorDoseDiff;
                    double dosePort = (pixelsPort[x, y] * slopePort + interceptPort) * normFactorDoseDiff;

                    // find the gradient in x direction
                    double doseRefLeft = (x > 0) ? (pixelsRef[x - 1, y] * slopeRef + interceptRef) * normFactorDoseDiff : double.NaN;
                    double doseRefRight = (x < sizeX - 1) ? (pixelsRef[x + 1, y] * slopeRef + interceptRef) * normFactorDoseDiff : double.NaN;
                    double gradientX;
                    if (double.IsNaN(doseRefLeft))
                    {
                        gradientX = (doseRefRight - doseRef) * resXNormInv;
                    }
                    else if (double.IsNaN(doseRefRight))
                    {
                        gradientX = (doseRef - doseRefLeft) * resXNormInv;
                    }
                    else
                    {
                        gradientX = (doseRefRight - doseRefLeft) * resXNormInv * 0.5;
                    }

                    // find the gradient in y direction
                    double doseRefTop = (y > 0) ? (pixelsRef[x, y - 1] * slopeRef + interceptRef) * normFactorDoseDiff : double.NaN;
                    double doseRefBottom = (y < sizeY - 1) ? (pixelsRef[x, y + 1] * slopeRef + interceptRef) * normFactorDoseDiff : double.NaN;
                    double gradientY;
                    if (double.IsNaN(doseRefTop))
                    {
                        gradientY = (doseRefBottom - doseRef) * resYNormInv;
                    }
                    else if (double.IsNaN(doseRefBottom))
                    {
                        gradientY = (doseRef - doseRefTop) * resYNormInv;
                    }
                    else
                    {
                        gradientY = (doseRefBottom - doseRefTop) * resYNormInv * 0.5;
                    }

                    // we take the larger absolute value of the x and y-gradients
                    double gradient = Math.Max(Math.Abs(gradientX), Math.Abs(gradientY)) * gradientNormalizer;

                    // weighting is done with the inverse of the gradient.
                    double gwddValue = Math.Abs(dosePort - doseRef) / (gradient + 1);

                    // calculate statistics data if pixel is inside of the ROI
                    if (roiMap[x, y] == 1)
                    {
                        numPixelsInROI++;
                        if (gwddValue < 1)
                        {
                            numPixelsLessThanOne++;
                        }
                        sumGWDD += gwddValue;
                        maxGWDD = Math.Max(gwddValue, maxGWDD);
                    }

                    gwdd[x, y] = (float)gwddValue;
                }
            }

            return new GWDDData(gwdd, (numPixelsInROI == 0) ? double.NaN : (double)numPixelsLessThanOne / numPixelsInROI,
              maxGWDD, (numPixelsInROI == 0) ? double.NaN : sumGWDD / numPixelsInROI);
        }

        // Finds the maximum dose in the reference image in display values (taking normalization into account)
        private static double MaxReferenceDoseValue(PDAnalysis analysis)
        {
            int minValue, maxValue;
            analysis.ReferenceImage.GetMinMax(out minValue, out maxValue, true);
            double slope, intercept;
            analysis.GetTransferFunction(true, out slope, out intercept);
            return maxValue * slope + intercept;
        }
    }
}
