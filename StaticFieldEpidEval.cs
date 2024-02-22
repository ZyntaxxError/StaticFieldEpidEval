using StaticFieldEpidEval;
using StaticFieldEpidEval.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using VMS.CA.Scripting;

namespace VMS.DV.PD.Scripting
{
    // This sample script implements an alternative to Gamma evaluation that is much faster:
    // Gradient weighted dose difference.
    // The dose difference at a point P(x, y) is weighted by the inverse of the local gradient at P(x, y).
    // Before weighting, the x any y axis are normalized the same way as when doing the Gamma analysis, therefore
    // this methods has the same two parameters: DTA (in mm) and DoseDifference (in %).
    // This sample script also demonstrates the usage of the Windows Forms GUI library as an alternative to WPF.
    public class Script
    {
        public Script()
        {
        }

        public void Execute(ScriptContext context)
        {
            if (context.DoseImage == null)
            {
                MessageBox.Show("The current context does not allow to perform an analysis because it does not contain a dose image.");
                return;
            }
            //else
            //{
            //    var mainWindow = new MainView();
            //    SetupWindow(mainWindow, "Plan Checker");
            //}
            // TODO: convert this to wpf and move everything to the viewmodel
            StringBuilder exploreThing = new StringBuilder();

            var pdBeam = context.DoseImage.PDBeam;
            var pdPlan = pdBeam.PDPlanSetup;
            var plan = pdBeam.PDPlanSetup.PlanSetup;
            var planUID = plan.UID;

            var parsedLogFile = new ParseLogFile(planUID);

            // List<string> mainFieldId = new List<string>();
            // start by checking if invivo or invitro to know which section of the log file to read
            // for some reason there is no IDUVrt, only IDULat and IDULng. Go by UID of the plan instead
            // var sdf = pdBeam.PortalDoseImages[0].Image;

            exploreThing.AppendLine($"Plan id {plan.Id}");
            exploreThing.AppendLine($"pdPlan id {pdPlan.Id}");
            exploreThing.AppendLine($"images {pdBeam.PortalDoseImages.Count}");
            MessageBox.Show(exploreThing.ToString());

            List<PDBeam> pdBeams = pdPlan.Beams.Where(b => b.PortalDoseImages.Count >= 1).ToList();

            List<PortalDoseResult> result = new List<PortalDoseResult>();
            if (parsedLogFile.PredictedFieldData.Any())
            {
                foreach (var predictedFieldData in parsedLogFile.PredictedFieldData)
                {
                    pdBeam = pdBeams.Where(b => b.Beam.Id.Equals(predictedFieldData.FieldId)).FirstOrDefault();
                    if (pdBeam != null)
                    {
                        result.Add(new PortalDoseResult(pdBeam, predictedFieldData));
                    }
                    else
                    {
                        MessageBox.Show($"No beam found for field id {predictedFieldData.FieldId}");
                    }
                }
            }


            //Beam beam = pdBeam.Beam;
            //var doseImage = context.DoseImage;
            //var doseImageType = doseImage.DoseImageType;
            //doseImage.GetMinMax(out int minValue, out int maxValue, false);
            //Frame portalDose = doseImage.Image.Frames[0];
            //// size and resolution of portalDose and refDoseOnPortal are identical
            //int sizeX = portalDose.XSize;
            //int sizeY = portalDose.YSize;

            //var iduLat = doseImage.Image.IDULat;
            //var iduLng = doseImage.Image.IDULng;
            //var iduVrt = doseImage.Image.SID;

            //// shift of IDU projected to isocenter
            //var iduLatOnIso = iduLat * 1000 / iduVrt;
            //var iduLngOnIso = iduLng * 1000 / iduVrt;
            ////VVector pdOrigin = portalDose.Origin; // moves if EPID moves
            //ushort[,] pixelsPort = new ushort[sizeX, sizeY];
            //portalDose.GetVoxels(0, pixelsPort);
            //// how is the index ordered in pixelsPort? assuming upper left corner in BEV [0,0]
            //double pixelsPerMmAtIso = 1190 / (400 * 1000 / iduVrt);
            //Vector2D ReadOutPositionCollimatorAtIso = new Vector2D(0, 0);
            //var collAngle = beam.ControlPoints[0].CollimatorAngle;
            //// rotate so adapt to coordinate system for idu
            //var ReadOutPositionIDUAtIso = Vector2D.RotateVector(ReadOutPositionCollimatorAtIso, collAngle);
            //Vector2D centralAxisPDindex = new Vector2D(1190 / 2, 1190 / 2); // Default position of field central axis if IDU centered
            //centralAxisPDindex.X -= iduLatOnIso * pixelsPerMmAtIso; // correct for UDU Lat
            //centralAxisPDindex.Y += iduLngOnIso * pixelsPerMmAtIso; // correct for IDU Lng
            //int readoutPositionIndexX = (int)Math.Round(centralAxisPDindex.X + ReadOutPositionIDUAtIso.X * pixelsPerMmAtIso);
            //int readoutPositionIndexY = (int)Math.Round(centralAxisPDindex.Y - ReadOutPositionIDUAtIso.Y * pixelsPerMmAtIso); //the index position direction is opposite the coordinated for IDULng
            //// TODO: some safety checks here to avoid index out of bounds
            //if (readoutPositionIndexX < 0)
            //{
            //    exploreThing.AppendLine($"ERROR:  readoutPositionIndexX < 0 {readoutPositionIndexX}");
            //    readoutPositionIndexX = 0;
            //}
            //if (readoutPositionIndexX > 1190)
            //{
            //    exploreThing.AppendLine($"ERROR:  readoutPositionIndexX >= 1190 {readoutPositionIndexX}");
            //    readoutPositionIndexX = 0;
            //}
            //if (readoutPositionIndexY < 0)
            //{
            //    exploreThing.AppendLine($"ERROR:  readoutPositionIndexY < 0 {readoutPositionIndexY}");
            //    readoutPositionIndexY = 0;
            //}
            //if (readoutPositionIndexY > 1190)
            //{
            //    exploreThing.AppendLine($"ERROR:  readoutPositionIndexY >= 1190 {readoutPositionIndexY}");
            //    readoutPositionIndexY = 0;
            //}
            //// to check, calculate distance from index 0, i.e. edge of plate
            //double checkX = readoutPositionIndexX / pixelsPerMmAtIso;
            //double checkY = readoutPositionIndexY / pixelsPerMmAtIso;
            // if no lat or lng move the central_pixel_index =  [1190 / 2, 1190 / 2]
            //var pdPlan = pdBeam.PDPlanSetup;
            //var platta = pdBeam.PortalDoseImages.Count;
            //exploreThing.AppendLine($"minValue {minValue}, maxValue {maxValue}");
            //exploreThing.AppendLine($"sizeX {sizeX}, sizeY {sizeY}");
            //exploreThing.AppendLine($"pdOrigin, x:{pdOrigin.x:F1}, y:{pdOrigin.y:F1}, z:{pdOrigin.z:F1}");
            // size of the EPID is 1190 pixels in lat and long
            //// Create the dialog window of the script and show it
            //using (WeightedGradientForm wgForm = new WeightedGradientForm(context.Analysis)) {
            //  wgForm.ShowDialog();}
        }

        private void SetupWindow(Window window, string title)
        {
            window.Title = title;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;
            window.ShowDialog();
        }



    }
}