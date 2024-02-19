using StaticFieldEpidEval;
using StaticFieldEpidEval.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VMS.CA.Scripting;
using VMS.DV.PD.Scripting;

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

  

            StringBuilder exploreThing = new StringBuilder();

            var pdBeam = context.DoseImage.PDBeam;
            var pdPlan = pdBeam.PDPlanSetup;
            var plan = pdBeam.PDPlanSetup.PlanSetup;
            var planUID = plan.UID;

            var parsedLogFile = new ParseLogFile(planUID);

            // start by checking if invivo or invitro to know which section of the log file to read
            // for some reason there is no IDUVrt, only IDULat and IDULng. Go by UID of the plan instead
            var sdf = pdBeam.PortalDoseImages[0].Image;


            exploreThing.AppendLine($"Plan id {plan.Id}");
            exploreThing.AppendLine($"pdPlan id {pdPlan.Id}");
            List<PDBeam> pdBeams = pdPlan.Beams.Where(b => b.PortalDoseImages.Count >= 1).ToList();
            // might want to reduce to only main fields before creating the list of result

            List<PortalDoseResult> result = new List<PortalDoseResult>();
            foreach (var pdb in pdBeams)
            {
                result.Add(new PortalDoseResult(pdb));
            }



            Beam beam = pdBeam.Beam;

            var doseImage = context.DoseImage;
            var doseImageType = doseImage.DoseImageType;

            doseImage.GetMinMax(out int minValue, out int maxValue, false);



            //TODO: check plan UID from copied json file



            Frame portalDose = doseImage.Image.Frames[0];
            // size and resolution of portalDose and refDoseOnPortal are identical
            int sizeX = portalDose.XSize;
            int sizeY = portalDose.YSize;


            var iduLat = doseImage.Image.IDULat;
            var iduLng = doseImage.Image.IDULng;
            var iduVrt = doseImage.Image.SID;

            // shift of IDU projected to isocenter
            var iduLatOnIso = iduLat * 1000 / iduVrt;
            var iduLngOnIso = iduLng * 1000 / iduVrt;

            //VVector pdOrigin = portalDose.Origin; // moves if EPID moves

            ushort[,] pixelsPort = new ushort[sizeX, sizeY];
            portalDose.GetVoxels(0, pixelsPort);

            // how is the index ordered in pixelsPort? assuming upper left corner in BEV [0,0]

            double pixelsPerMmAtIso = 1190 / (400 * 1000 / iduVrt);
            Vector2D ReadOutPositionCollimatorAtIso = new Vector2D(0, 0);


            // check if there is a comment with measured position
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                string clipboardText = Clipboard.GetText(TextDataFormat.Text);
                MessageBox.Show("clipboardtext:" + clipboardText);
            }
            else
            {
                double[] commentPosition = GetMeasuredPositionFromComment(beam);
                if (commentPosition != null && double.IsNaN(commentPosition[0]) == false && double.IsNaN(commentPosition[1]))
                {
                    ReadOutPositionCollimatorAtIso.X = commentPosition[0] * 10; // assuming cm in comment
                    ReadOutPositionCollimatorAtIso.Y = commentPosition[1] * 10;
                }
            }


            var collAngle = beam.ControlPoints[0].CollimatorAngle;

            // rotate so adapt to coordinate system for idu
            var ReadOutPositionIDUAtIso = Vector2D.RotateVector(ReadOutPositionCollimatorAtIso, collAngle);

            Vector2D centralAxisPDindex = new Vector2D(1190 / 2, 1190 / 2); // Default position of field central axis if IDU centered

            centralAxisPDindex.X -= iduLatOnIso * pixelsPerMmAtIso; // correct for UDU Lat
            centralAxisPDindex.Y += iduLngOnIso * pixelsPerMmAtIso; // correct for IDU Lng

            int readoutPositionIndexX = (int)Math.Round(centralAxisPDindex.X + ReadOutPositionIDUAtIso.X * pixelsPerMmAtIso);
            int readoutPositionIndexY = (int)Math.Round(centralAxisPDindex.Y - ReadOutPositionIDUAtIso.Y * pixelsPerMmAtIso); //the index position direction is opposite the coordinated for IDULng

            // TODO: some safety checks here to avoid index out of bounds

            if (readoutPositionIndexX < 0)
            {
                exploreThing.AppendLine($"ERROR:  readoutPositionIndexX < 0 {readoutPositionIndexX}");
                readoutPositionIndexX = 0;
            }
            if (readoutPositionIndexX > 1190)
            {
                exploreThing.AppendLine($"ERROR:  readoutPositionIndexX >= 1190 {readoutPositionIndexX}");
                readoutPositionIndexX = 0;
            }
            if (readoutPositionIndexY < 0)
            {
                exploreThing.AppendLine($"ERROR:  readoutPositionIndexY < 0 {readoutPositionIndexY}");
                readoutPositionIndexY = 0;
            }
            if (readoutPositionIndexY > 1190)
            {
                exploreThing.AppendLine($"ERROR:  readoutPositionIndexY >= 1190 {readoutPositionIndexY}");
                readoutPositionIndexY = 0;
            }


            // to check, calculate distance from index 0, i.e. edge of plate

            double checkX = readoutPositionIndexX / pixelsPerMmAtIso;
            double checkY = readoutPositionIndexY / pixelsPerMmAtIso;

            // if no lat or lng move the central_pixel_index =  [1190 / 2, 1190 / 2]

            //var pdPlan = pdBeam.PDPlanSetup;

            //var platta = pdBeam.PortalDoseImages.Count;

            //exploreThing.AppendLine($"minValue {minValue}, maxValue {maxValue}");
            //exploreThing.AppendLine($"sizeX {sizeX}, sizeY {sizeY}");
            //exploreThing.AppendLine($"pdOrigin, x:{pdOrigin.x:F1}, y:{pdOrigin.y:F1}, z:{pdOrigin.z:F1}");
            // size of the EPID is 1190 pixels in lat and long



            MessageBox.Show(exploreThing.ToString());


            //// Create the dialog window of the script and show it
            //using (WeightedGradientForm wgForm = new WeightedGradientForm(context.Analysis)) {
            //  wgForm.ShowDialog();}

        }

        internal double[] GetMeasuredPositionFromComment(Beam beam)
        {
            string secondLine = string.Empty;
            var commentLines = beam.Comment.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            if (commentLines.Length > 1)
            {
                secondLine = commentLines[1];
            }

            if (!string.IsNullOrEmpty(secondLine))
            {
                string comment = secondLine.Replace(',', '.');
                string[] positionsInComment = comment.Split(';');
                if (double.TryParse(positionsInComment[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double positionX) &&
                    double.TryParse(positionsInComment[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double positionY))
                {
                    return new double[] { positionX, positionY };
                }
                else
                {
                    return new double[] { double.NaN, double.NaN };
                }
            }
            else
            {
                return new double[] { double.NaN, double.NaN };
            }
        }
    }




}