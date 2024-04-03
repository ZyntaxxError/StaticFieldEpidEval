using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using VMS.CA.Scripting;
using VMS.DV.PD.Scripting;

namespace StaticFieldEpidEval.Models
{

    /// <summary>
    /// Class that holds all portal dose results for a single field
    /// </summary>
    public class PortalDoseResult
    {
        // hard coded value for tolerance as these are reported to SSM
        // identical values as those found in PlanChecker in the configuration file
        internal const double DefaultIduVrtInVivo = 1700.0;
        internal const double ToleranceInVivoPercent = 15.0; 
        internal const double DefaultIduVrtInVitro = 1000.0;
        internal const double ToleranceInVitroPercent = 7.0;
        public double IduVrt { get; set; }
        public double IduLat { get; set; }
        public double IduLng { get; set; }

        public string FieldId { get; set; }

        public string ReadoutPositionInCollimatorAsString { get; set; }

        public double PortalDosePixelValueCU { get; set; }
        public double PredictedValueCU { get; set; }
        public double PixelValueDeviationPercent { get; set; }
        public string CalculationLog { get; set; }
        public bool IsResultWithinTolerance { get; set; }

        internal List<Check> Checks { get; set; }


        public PortalDoseResult(PDBeam pdBeam, PredictedFieldData predictedFieldData, bool inVivo)
        {
            StringBuilder calculationLog = new StringBuilder();
            Checks = new List<Check>();
            calculationLog.AppendLine($"field id {pdBeam.Id}");
            calculationLog.AppendLine($"images {pdBeam.PortalDoseImages.Count}");

            if (pdBeam.PortalDoseImages.Count > 0)
            {
                FieldId = pdBeam.Beam.Id;
                PortalDoseImage imageToEvaluate = GetImageToEvaluate(calculationLog, pdBeam);

                ReadoutPositionInCollimatorAsString = $"( {predictedFieldData.ReadOutPositionCollimatorAtIso.X / 10:F1}, {predictedFieldData.ReadOutPositionCollimatorAtIso.Y / 10:F1} )";

                // get the pixel value from the portal dose image and the predicted value from the plan checker
                if (imageToEvaluate != null)
                {
                    PortalDosePixelValueCU = GetPortalDosePixelValueCU(pdBeam, imageToEvaluate, predictedFieldData, ref calculationLog);
                    PredictedValueCU = GetPredictedValueCU(predictedFieldData.PredictedValue, inVivo, ref calculationLog);
                }
                else
                {
                    PortalDosePixelValueCU = double.NaN;
                    PredictedValueCU = double.NaN;
                }

                // check that the predicted value and the pixel value are defined doubles and not NaN, else set PixelValueDeviationPercent to NaN
                // ReWrite this as a standard if to increase readability
                if (!double.IsNaN(PredictedValueCU) && !double.IsNaN(PortalDosePixelValueCU))
                {
                    PixelValueDeviationPercent = (PortalDosePixelValueCU - PredictedValueCU) / PredictedValueCU * 100;
                    IsResultWithinTolerance = inVivo ? Math.Abs(PixelValueDeviationPercent) <= ToleranceInVivoPercent : Math.Abs(PixelValueDeviationPercent) <= ToleranceInVitroPercent;
                }
                else
                {
                    PixelValueDeviationPercent = double.NaN;
                    IsResultWithinTolerance = false;
                }

           
                CalculationLog = calculationLog.ToString();
            }
            else
            {
                Checks.Add(new Check(CheckResult.Error, $"No portal dose images available for field {FieldId}"));
            }
        }



        
        /// <summary>
        /// Select the image to evaluate for the field, if a composite image is available, select the last one, else select the last image for the last session
        /// </summary>
        /// <param name="calculationLog"></param>
        /// <param name="pdBeam"></param>
        /// <returns></returns>
        private PortalDoseImage GetImageToEvaluate(StringBuilder calculationLog, PDBeam pdBeam)
        {
            var planSessions = pdBeam.PDPlanSetup.Sessions;
            PortalDoseImage imageToEvaluate = null;
            var compositeImages = pdBeam.PDPlanSetup.CompositeImages.Where(p => p.PDBeam.Id == FieldId).ToList();
            if (compositeImages.Any())
            {
                var compositeImage = compositeImages.LastOrDefault();
                // create a warning if more than one composite image is found
                if (compositeImages.Count > 1)
                {
                    calculationLog.AppendLine($"WARNING: More than one composite image found for field {FieldId}, evaluation will be done for image with Id: {compositeImage.Id}");
                    Checks.Add(new Check(CheckResult.Warning, $"More than one composite image found for field {FieldId}"));
                }
                var imagesInComposite = compositeImage.ComposedImages;
                calculationLog.AppendLine($"Nr of images in composite: {imagesInComposite.Count()}");
                // Check that all images in the composite image are from the same field
                if (imagesInComposite.Any(i => i.PDBeam.Id != FieldId))
                {
                    calculationLog.AppendLine($"ERROR: Composite image contains images from different fields");
                    Checks.Add(new Check(CheckResult.Error, $"Composite image contains images from different fields for field {FieldId}"));
                }
                else
                {
                    // check that all images in the composite image have the same IduVrt, IduLat and IduLng within 1 mm
                    var firstImageInComposite = imagesInComposite.FirstOrDefault();
                    if (imagesInComposite.Any(i => Math.Abs(i.Image.IDULat - firstImageInComposite.Image.IDULat) > 1))
                    {
                        calculationLog.AppendLine($"ERROR: Composite image contains images with different IduLat");
                        Checks.Add(new Check(CheckResult.Error, $"Composite image contains images with different IduLat for field {FieldId}"));
                    }
                    else if (imagesInComposite.Any(i => Math.Abs(i.Image.IDULng - firstImageInComposite.Image.IDULng) > 1))
                    {
                        calculationLog.AppendLine($"ERROR: Composite image contains images with different IduLng");
                        Checks.Add(new Check(CheckResult.Error, $"Composite image contains images with different IduLng for field {FieldId}"));
                    }
                    else if (imagesInComposite.Any(i => Math.Abs(i.Image.SID - firstImageInComposite.Image.SID) > 1))
                    {
                        calculationLog.AppendLine($"ERROR: Composite image contains images with different IduVrt");
                        Checks.Add(new Check(CheckResult.Error, $"Composite image contains images with different IduVrt for field {FieldId}"));
                    }
                    else
                    {
                        imageToEvaluate = compositeImage;
                    }
                }
                
                calculationLog.AppendLine($"Composite image: {compositeImage.Id}");
                Checks.Add(new Check(CheckResult.Information, $"Composite image used for evaluation of field {FieldId}."));
            }
            else
            {
                calculationLog.AppendLine($"No composite image available for field");
                calculationLog.AppendLine($"Nr of sessions: {planSessions.Count}");
                var lastPortalDoseSessionImages = planSessions.LastOrDefault().PortalDoseImages;

                // select all images for the last session with beam id equal to FieldId
                var lastSessionImagesForField = lastPortalDoseSessionImages.Where(i => i.PDBeam.Id == FieldId);
                
                // add the number of images for the last session and field id to the calculation log
                calculationLog.AppendLine($"Nr of images for last session for field {FieldId}: {lastSessionImagesForField.Count()}");
                // Create a check with warning if more than one image is found for the last session, suggest using composite image
                if (lastSessionImagesForField.Count() > 1)
                {
                    calculationLog.AppendLine($"WARNING: More than one image found for last session for field {FieldId}.");
                    calculationLog.AppendLine($"Evaluation will be done for image with Id: {lastSessionImagesForField.LastOrDefault().Id}");
                    Checks.Add(new Check(CheckResult.Warning, $"More than one image found for last session for field {FieldId}. Evaluation will be done for image with Id: {lastSessionImagesForField.LastOrDefault().Id}. " +
                        $"If the beam was interrupted, create a composite image for evaluation."));
                }
                calculationLog.AppendLine($"Last session date: {lastPortalDoseSessionImages.LastOrDefault().Image.CreationDateTime}");
                imageToEvaluate = lastSessionImagesForField.LastOrDefault();
            }
            return imageToEvaluate;
        }


        /// <summary>
        /// Get the predicted value corrected for the actual IDU Vrt if it doesn't equal the default IDU Vrt within a certain tolerance
        /// </summary>
        /// <param name="predictedValue"></param>
        /// <param name="inVivo"></param>
        /// <param name="calculationLog"></param>
        /// <returns></returns>
        private double GetPredictedValueCU(double predictedValue, bool inVivo, ref StringBuilder calculationLog)
        {
            double expectedIduVrt = inVivo ? DefaultIduVrtInVivo : DefaultIduVrtInVitro;
            double vrtTolerance = inVivo ? 10 : 5; // mm tolerance for IDU Vrt, smaller tolerance for in vitro as the value is more critical

            if (Math.Abs(expectedIduVrt - IduVrt) > vrtTolerance)
            {
                calculationLog.AppendLine($"Warning: IDU Vrt is not at the expected value");
                calculationLog.AppendLine($"Expected IDU Vrt: {expectedIduVrt:F1}, actual IDU Vrt: {IduVrt:F1}");
                calculationLog.AppendLine("A correction will be applied to the predicted value");
                calculationLog.AppendLine($"Predicted value before correction: {predictedValue:F3}");
                double correction = Math.Pow(expectedIduVrt / IduVrt, 2);
                calculationLog.AppendLine($"Correction factor: {correction:F3}");
                predictedValue *= correction;
                Checks.Add(new Check(CheckResult.Warning, $"IDU Vrt is not at the expected value, check the calculation log for details."));
            }
            else
            {
                calculationLog.AppendLine($"Predicted value: {predictedValue:F3} CU");
            }
            return predictedValue;
        }



        /// <summary>
        /// Retrieve the pixel value from the portal dose image for the readout position from the plan checker
        /// </summary>
        /// <param name="pdBeam"></param>
        /// <param name="doseImage"></param>
        /// <param name="predictedFieldData"></param>
        /// <param name="calculationLog"></param>
        /// <returns></returns>
        private double GetPortalDosePixelValueCU(PDBeam pdBeam, PortalDoseImage doseImage, PredictedFieldData predictedFieldData, ref StringBuilder calculationLog)
        {
            Frame portalDoseFrame = doseImage.Image.Frames[0];

            IduLat = doseImage.Image.IDULat;
            IduLng = doseImage.Image.IDULng;
            IduVrt = doseImage.Image.SID;
            // Add information for the image and actual position of IDU for the image to calculation log
            // Have only ever seen a single frame in the portal dose image, must be default for static fields
            calculationLog.AppendLine($"Dose image: {doseImage.Id}");
            calculationLog.AppendLine($"Nr of frames in image: {doseImage.Image.Frames.Count}");
            calculationLog.AppendLine($"IDU Lat: {IduLat:F1}, IDU Lng: {IduLng:F1}, IDU Vrt: {IduVrt:F1}");

            // shift of IDU projected to isocenter, the shift is in mm
            double iduLatOnIso = IduLat * (1000 / IduVrt);
            double iduLngOnIso = IduLng * (1000 / IduVrt);

            int sizeX = portalDoseFrame.XSize;
            int sizeY = portalDoseFrame.YSize;
            calculationLog.AppendLine($"Portal dose frame: sizeX {sizeX}, sizeY {sizeY}");

            // get the pixels from the portal dose image
            ushort[,] pixelsPort = new ushort[sizeX, sizeY];
            portalDoseFrame.GetVoxels(0, pixelsPort);

            // The index in pixelsPort is ordered with upper left corner in BEV at [0,0]
            double pixelsPerMmAtIso = 1190 / (400 * 1000 / IduVrt); // TODO: 1190 is the number of pixels in the portal dose image?, 400 is size in mm in each direction,replace with a constant

            // The readout position from planChecker is defined in the collimator coordinate system
            // rotate so that the readout position adapt to the coordinate system for the idu
            double collAngle = pdBeam.Beam.ControlPoints[0].CollimatorAngle;
            Vector2D readOutPositionIDUAtIso = Vector2D.RotateVector(predictedFieldData.ReadOutPositionCollimatorAtIso, collAngle);

            Vector2D centralAxisPDindex = new Vector2D(1190 / 2, 1190 / 2); // Default position of field central axis if IDU centered

            centralAxisPDindex.X -= iduLatOnIso * pixelsPerMmAtIso; // correct for UDU Lat
            centralAxisPDindex.Y += iduLngOnIso * pixelsPerMmAtIso; // correct for IDU Lng

            int readoutPositionIndexX = (int)Math.Round(centralAxisPDindex.X + readOutPositionIDUAtIso.X * pixelsPerMmAtIso);
            int readoutPositionIndexY = (int)Math.Round(centralAxisPDindex.Y - readOutPositionIDUAtIso.Y * pixelsPerMmAtIso); //the index position direction is opposite the coordinated for IDULng

            // some safety checks to avoid index out of bounds, e.g. when manually given readout position is outside the image
            if (readoutPositionIndexX < 0 || readoutPositionIndexX > 1190 || readoutPositionIndexY < 0 || readoutPositionIndexY > 1190)
            {
                calculationLog.AppendLine($"ERROR: pixel readout point is out of bounds (X = {readoutPositionIndexX}, Y = {readoutPositionIndexY})");
                Checks.Add(new Check(CheckResult.Error, $"Pixel readout point is out of bounds for field {pdBeam.Beam.Id}"));
            }

            // to check, calculate distance from index 0, i.e. edge of plate, add to calculation log
            double checkX = readoutPositionIndexX / pixelsPerMmAtIso;
            double checkY = readoutPositionIndexY / pixelsPerMmAtIso;

            // check that there are no Checks with CheckResult ERROR, if there are, set PixelValueDeviationPercent to NaN
            if (Checks.Any(c => c.Result == CheckResult.Error))
            {
                return double.NaN;
            }
            else
            {
                calculationLog.AppendLine($"pixel value  {portalDoseFrame.VoxelToDisplayValue(pixelsPort[readoutPositionIndexX, readoutPositionIndexY]):F3}");
                calculationLog.AppendLine($"readoutPositionIndexX:{readoutPositionIndexX:F1}");
                calculationLog.AppendLine($"readoutPositionIndexY:{readoutPositionIndexY:F1}");
                calculationLog.AppendLine($"Distance from image left:{checkX:F1} mm");
                calculationLog.AppendLine($"Distance from image top:{checkY:F1} mm");
                return portalDoseFrame.VoxelToDisplayValue(pixelsPort[readoutPositionIndexX, readoutPositionIndexY]);
            }
        }
    }
}
