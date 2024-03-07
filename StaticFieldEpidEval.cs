using StaticFieldEpidEval;
using StaticFieldEpidEval.Models;
using StaticFieldEpidEval.ViewModels;
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

        public void Execute(ScriptContext context, Window window)
        {
            if (context.DoseImage == null)
            {
                MessageBox.Show("The current context does not contain a dose image.");
                return;
            }
            else
            {
                window.Content = new MainView()
                {
                    DataContext = new MainViewModel(context)



                };
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.Title = "StaticFieldEpidEval: Portal dosimetry evaluation of static fields";
            }

            //StringBuilder exploreThing = new StringBuilder();

            //var pdBeam = context.DoseImage.PDBeam;
            //var pdPlan = pdBeam.PDPlanSetup;
            //var plan = pdBeam.PDPlanSetup.PlanSetup;
            //var planUID = plan.UID;



            //// Collect data from the log file, i.e. the calculation log from PlanCheck copied to clipboard 
            //var parsedLogFile = new ParseLogFile(planUID);

  

            //exploreThing.AppendLine($"Plan id {plan.Id}");
            //exploreThing.AppendLine($"pdPlan id {pdPlan.Id}");
            //exploreThing.AppendLine($"images {pdBeam.PortalDoseImages.Count}");
            //MessageBox.Show(exploreThing.ToString());

            //List<PDBeam> pdBeams = pdPlan.Beams.Where(b => b.PortalDoseImages.Count >= 1).ToList();

            //MessageBox.Show($"Found {pdBeams.Count} beams with portal dose images");


            //List<PortalDoseResult> result = new List<PortalDoseResult>();


            //if (parsedLogFile.PredictedFieldData.Any())
            //{
            //    foreach (var predictedFieldData in parsedLogFile.PredictedFieldData)
            //    {
            //        pdBeam = pdBeams.Where(b => b.Beam.Id.Equals(predictedFieldData.FieldId)).FirstOrDefault();
            //        if (pdBeam != null)
            //        {
            //            result.Add(new PortalDoseResult(pdBeam, predictedFieldData));
            //        }
            //        else
            //        {
            //            MessageBox.Show($"No beam found for field id {predictedFieldData.FieldId}");
            //        }
            //    }
            //} else
            //{
            //    MessageBox.Show("No predicted field data found");
            //}

        }





    }
}