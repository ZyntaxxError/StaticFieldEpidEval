using System;
using System.Linq;
using System.Text;
using VMS.CA.Scripting;
using VMS.DV.PD.Scripting;

namespace StaticFieldEpidEval.Models
{

    /// <summary>
    /// Class that holds all portal dose results for a single field
    /// </summary>
    public class PortalDoseResult
    {
        internal const double DefaultIduVrtInVivo = 1700.0;
        internal const double DefaultIduVrtInVitro = 1000.0;
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


        public PortalDoseResult(PDBeam pdBeam, PredictedFieldData predictedFieldData, bool inVivo)
        {
            StringBuilder calculationLog = new StringBuilder();

            calculationLog.AppendLine($"field id {pdBeam.Id}");
            calculationLog.AppendLine($"images {pdBeam.PortalDoseImages.Count}");

            if (pdBeam.PortalDoseImages.Count > 0)
            {
                FieldId = pdBeam.Beam.Id;

                var planSessions = pdBeam.PDPlanSetup.Sessions;

                calculationLog.AppendLine($"Nr of sessions: {planSessions.Count}");
                var firstSession = planSessions.FirstOrDefault();
                var lastSessionImages = planSessions.LastOrDefault().PortalDoseImages;
                // add number of images for last session and the date for the last session to the calculation log
                calculationLog.AppendLine($"Nr of images for last session: {lastSessionImages.Count}");
                calculationLog.AppendLine($"Last session date: {lastSessionImages.FirstOrDefault().Image.CreationDateTime}");

                // select all images for the last session with beam id equal to FieldId
                var lastSessionImagesForField = lastSessionImages.Where(i => i.PDBeam.Id == FieldId);
                // add the number of images for the last session and field id to the calculation log
                calculationLog.AppendLine($"Nr of images for last session and field id {FieldId}: {lastSessionImagesForField.Count()}");
                
                var plannedMUs = pdBeam.PlannedMUs;
              



                IduVrt = pdBeam.PortalDoseImages.FirstOrDefault().Image.SID;
                IduLat = pdBeam.PortalDoseImages.FirstOrDefault().Image.IDULat;
                IduLng = pdBeam.PortalDoseImages.FirstOrDefault().Image.IDULng;

                ReadoutPositionInCollimatorAsString = $"( {predictedFieldData.ReadOutPositionCollimatorAtIso.X / 10:F1}, {predictedFieldData.ReadOutPositionCollimatorAtIso.Y / 10:F1} )";
                
                // if IduVrt not the default for InVivo or InVitro, add a correction to the calculation log
                PredictedValueCU = GetPredictedValueCU(predictedFieldData.PredictedValue, inVivo, ref calculationLog);

                PortalDosePixelValueCU = GetPortalDosePixelValueCU(pdBeam, predictedFieldData, ref calculationLog);
                PixelValueDeviationPercent = (PortalDosePixelValueCU - predictedFieldData.PredictedValue) / predictedFieldData.PredictedValue * 100;
                calculationLog.AppendLine($"pixel value deviation from predicted: {PixelValueDeviationPercent:F1}%");
                CalculationLog = calculationLog.ToString();
            }
            else
            {
               // TODO: handle error
            }
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
                calculationLog.AppendLine($"Warning: IDU Vrt is not the default value");
                calculationLog.AppendLine($"Expected IDU Vrt: {expectedIduVrt:F1}, actual IDU Vrt: {IduVrt:F1}");
                calculationLog.AppendLine("A correction will be applied to the predicted value");
                return predictedValue * Math.Pow(expectedIduVrt / IduVrt, 2);
            }
            else
            {
                return predictedValue;
            }
        }

        private double GetPortalDosePixelValueCU(PDBeam pdBeam, PredictedFieldData predictedFieldData, ref StringBuilder calculationLog)
        {
            var doseImage = pdBeam.PortalDoseImages.FirstOrDefault();
            Frame portalDoseFrame = doseImage.Image.Frames[0];

            // TODO: get all frames and calculate the sum pixel value(?) or maybe check if composite image is available


            calculationLog.AppendLine($"Nr of frames for the first image: {doseImage.Image.Frames.Count}");

            IduLat = doseImage.Image.IDULat;
            IduLng = doseImage.Image.IDULng;
            IduVrt = doseImage.Image.SID;
            // Add actual position of IDU to calculation log
            calculationLog.AppendLine($"IDU Lat: {IduLat:F1}, IDU Lng: {IduLng:F1}, IDU Vrt: {IduVrt:F1}");

            // shift of IDU projected to isocenter, the shift is in mm
            var iduLatOnIso = IduLat * 1000 / IduVrt;
            var iduLngOnIso = IduLng * 1000 / IduVrt;

            int sizeX = portalDoseFrame.XSize;
            int sizeY = portalDoseFrame.YSize;
            calculationLog.AppendLine($"sizeX {sizeX}, sizeY {sizeY}");

            // get the pixels from the portal dose image
            ushort[,] pixelsPort = new ushort[sizeX, sizeY];
            portalDoseFrame.GetVoxels(0, pixelsPort);

            // how is the index ordered in pixelsPort? assuming upper left corner in BEV [0,0]
            double pixelsPerMmAtIso = 1190 / (400 * 1000 / IduVrt); // TODO: 1190 is the number of pixels in the portal dose image?, 400 is size in mm in each direction,replace with a constant

            var collAngle = pdBeam.Beam.ControlPoints[0].CollimatorAngle;
            // The readout position from planChecker is defined in the collimator coordinate system
            // rotate so that the readout position adapt to coordinate system for the idu
            var ReadOutPositionIDUAtIso = Vector2D.RotateVector(predictedFieldData.ReadOutPositionCollimatorAtIso, collAngle);

            Vector2D centralAxisPDindex = new Vector2D(1190 / 2, 1190 / 2); // Default position of field central axis if IDU centered

            centralAxisPDindex.X -= iduLatOnIso * pixelsPerMmAtIso; // correct for UDU Lat
            centralAxisPDindex.Y += iduLngOnIso * pixelsPerMmAtIso; // correct for IDU Lng

            int readoutPositionIndexX = (int)Math.Round(centralAxisPDindex.X + ReadOutPositionIDUAtIso.X * pixelsPerMmAtIso);
            int readoutPositionIndexY = (int)Math.Round(centralAxisPDindex.Y - ReadOutPositionIDUAtIso.Y * pixelsPerMmAtIso); //the index position direction is opposite the coordinated for IDULng

            // some safety checks here to avoid index out of bounds
            if (readoutPositionIndexX < 0)
            {
                calculationLog.AppendLine($"ERROR:  readoutPositionIndexX < 0 {readoutPositionIndexX}");
                readoutPositionIndexX = 0;
            }
            if (readoutPositionIndexX > 1190)
            {
                calculationLog.AppendLine($"ERROR:  readoutPositionIndexX >= 1190 {readoutPositionIndexX}");
                readoutPositionIndexX = 0;
            }
            if (readoutPositionIndexY < 0)
            {
                calculationLog.AppendLine($"ERROR:  readoutPositionIndexY < 0 {readoutPositionIndexY}");
                readoutPositionIndexY = 0;
            }
            if (readoutPositionIndexY > 1190)
            {
                calculationLog.AppendLine($"ERROR:  readoutPositionIndexY >= 1190 {readoutPositionIndexY}");
                readoutPositionIndexY = 0;
            }

            // to check, calculate distance from index 0, i.e. edge of plate, add to calculation log
            double checkX = readoutPositionIndexX / pixelsPerMmAtIso;
            double checkY = readoutPositionIndexY / pixelsPerMmAtIso;

            calculationLog.AppendLine($"iduLat:{IduLat:F1}, iduLng:{IduLng:F1}, iduVrt:{IduVrt:F1}");
            calculationLog.AppendLine($"Predicted value: {predictedFieldData.PredictedValue:F1}");
            calculationLog.AppendLine($"pixel value  {portalDoseFrame.VoxelToDisplayValue(pixelsPort[readoutPositionIndexX, readoutPositionIndexY]):F3}");
            calculationLog.AppendLine($"readoutPositionIndexX:{readoutPositionIndexX:F1}, readoutPositionIndexY:{readoutPositionIndexY:F1}");
            calculationLog.AppendLine($"Distance from image left:{checkX:F1} mm, distance from image top:{checkY:F1} mm");
            return portalDoseFrame.VoxelToDisplayValue(pixelsPort[readoutPositionIndexX, readoutPositionIndexY]);
        }
    }
}
