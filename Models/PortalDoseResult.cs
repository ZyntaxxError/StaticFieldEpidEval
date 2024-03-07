using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMS.CA.Scripting;
using VMS.DV.PD.Scripting;

namespace StaticFieldEpidEval.Models
{

    /// <summary>
    /// Class that holds all portal dose results for a single field
    /// </summary>
    public class PortalDoseResult
    {
        public double IduVrt { get; set; }
        public double IduLat { get; set; }
        public double IduLng { get; set; }

        public string FieldId { get; set; }
        public double PortalDosePixelValueCU { get; set; }
        public double PredictedValueCU { get; set; }
        public double PixelValueDeviationPercent { get; set; }
        public string CalculationLog { get; set; }


        public PortalDoseResult(PDBeam pdBeam, PredictedFieldData predictedFieldData)
        {
            StringBuilder calculationLog = new StringBuilder();
            // TODO: check if more than one image, in that case why?
            
            calculationLog.AppendLine($"images {pdBeam.PortalDoseImages.Count}");

            if (pdBeam.PortalDoseImages.Count > 0)
            {
                FieldId = pdBeam.Beam.Id;
                IduVrt = pdBeam.PortalDoseImages.FirstOrDefault().Image.SID;
                IduLat = pdBeam.PortalDoseImages.FirstOrDefault().Image.IDULat;
                IduLng = pdBeam.PortalDoseImages.FirstOrDefault().Image.IDULng;
                PredictedValueCU = predictedFieldData.PredictedValue;
                PortalDosePixelValueCU = GetPortalDosePixelValueCU(pdBeam, predictedFieldData, ref calculationLog);
                PixelValueDeviationPercent = (PortalDosePixelValueCU - predictedFieldData.PredictedValue) / predictedFieldData.PredictedValue * 100;
                calculationLog.AppendLine($"pixel value deviation from predicted: {PixelValueDeviationPercent:F1}%");
            }
            else
            {

            }
        }

        private double GetPortalDosePixelValueCU(PDBeam pdBeam, PredictedFieldData predictedFieldData, ref StringBuilder calculationLog)
        {
            var doseImage = pdBeam.PortalDoseImages.FirstOrDefault();
            Frame portalDose = doseImage.Image.Frames[0];
            // size and resolution of portalDose and refDoseOnPortal are identical
            int sizeX = portalDose.XSize;
            int sizeY = portalDose.YSize;

            calculationLog.AppendLine($"sizeX {sizeX}, sizeY {sizeY}");

            IduLat = doseImage.Image.IDULat;
            IduLng = doseImage.Image.IDULng;
            IduVrt = doseImage.Image.SID;

            // shift of IDU projected to isocenter
            var iduLatOnIso = IduLat * 1000 / IduVrt;
            var iduLngOnIso = IduLng * 1000 / IduVrt;

            ushort[,] pixelsPort = new ushort[sizeX, sizeY];
            portalDose.GetVoxels(0, pixelsPort);

            // how is the index ordered in pixelsPort? assuming upper left corner in BEV [0,0]
            double pixelsPerMmAtIso = 1190 / (400 * 1000 / IduVrt);

            Beam beam = pdBeam.Beam;

            var collAngle = beam.ControlPoints[0].CollimatorAngle;
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
            calculationLog.AppendLine($"pixel value  {portalDose.VoxelToDisplayValue(pixelsPort[readoutPositionIndexX, readoutPositionIndexY]):F3}");
            calculationLog.AppendLine($"readoutPositionIndexX:{readoutPositionIndexX:F1}, readoutPositionIndexY:{readoutPositionIndexY:F1}");
            calculationLog.AppendLine($"Distance from image left:{checkX:F1} mm, distance from image top:{checkY:F1} mm");
            return portalDose.VoxelToDisplayValue(pixelsPort[readoutPositionIndexX, readoutPositionIndexY]);
        }
    }
}
