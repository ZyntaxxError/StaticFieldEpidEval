using StaticFieldEpidEval.Models;
using StaticFieldEpidEval.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.DV.PD.Scripting;

namespace StaticFieldEpidEval.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {

        private List<PortalDoseResult> _portalDoseResults;
        public List<PortalDoseResult> PortalDoseResults
        { 
          get { return _portalDoseResults; }
          set { _portalDoseResults = value; OnPropertyChanged(); }
        }

        public MainViewModel(ScriptContext context)
        {
            StringBuilder calculationLog = new StringBuilder();

            var pdBeam = context.DoseImage.PDBeam;
            var pdPlan = pdBeam.PDPlanSetup;
            var plan = pdBeam.PDPlanSetup.PlanSetup;
            var planUID = plan.UID;

            // Collect data from the log file, i.e. the calculation log from PlanCheck copied to clipboard 
            var parsedLogFile = new ParseLogFile(planUID);

            calculationLog.AppendLine($"Plan id {plan.Id}");
            calculationLog.AppendLine($"pdPlan id {pdPlan.Id}");
            calculationLog.AppendLine($"images {pdBeam.PortalDoseImages.Count}");

            List<PDBeam> pdBeams = pdPlan.Beams.Where(b => b.PortalDoseImages.Count >= 1).ToList();

            PortalDoseResults = new List<PortalDoseResult>();

            if (parsedLogFile.PredictedFieldData.Any())
            {
                foreach (var predictedFieldData in parsedLogFile.PredictedFieldData)
                {
                    pdBeam = pdBeams.Where(b => b.Beam.Id.Equals(predictedFieldData.FieldId)).FirstOrDefault();
                    if (pdBeam != null)
                    {
                        var pdResult = new PortalDoseResult(pdBeam, predictedFieldData, parsedLogFile.InVivoFlag);
                        PortalDoseResults.Add(pdResult);
                        calculationLog.AppendLine(pdResult.CalculationLog);
                        //MessageBox.Show($"Field id {predictedFieldData.FieldId}, predicted value {predictedFieldData.PredictedValue}, portal dose pixel value {pdResult.PortalDosePixelValueCU}, deviation {pdResult.PixelValueDeviationPercent:F1}%");
                    }
                    else
                    {
                        MessageBox.Show($"No beam found for field id {predictedFieldData.FieldId}");
                    }
                }
            }
            else
            {
                MessageBox.Show("No predicted field data found");
            }
            //MessageBox.Show(calculationLog.ToString());
        }


        private List<string> _messages;

        public List<string> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }
    }
}
