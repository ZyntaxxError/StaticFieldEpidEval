using StaticFieldEpidEval.Models;
using StaticFieldEpidEval.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

        private bool _showCalculationLog;
        public bool ShowCalculationLog
        {
            get { return _showCalculationLog; }
            set { _showCalculationLog = value; OnPropertyChanged(); }
        }

        private string _planId;
        public string PlanId
        {
            get { return _planId; }
            set { _planId = value; OnPropertyChanged(); }
        }


        public MainViewModel(ScriptContext context)
        {
            Checks = new ObservableCollection<Check>();

            var pdBeam = context.DoseImage.PDBeam;
            var pdPlan = pdBeam.PDPlanSetup;
            var plan = pdBeam.PDPlanSetup.PlanSetup;
            PlanId = plan.Id;

            // Collect data from the log file, i.e. the calculation log from PlanCheck copied to clipboard 
            var parsedLogFile = new ParseLogFile(plan.UID);

            // If any errors or warnings from parsing the log, add to Checks
            if (parsedLogFile.Checks.Any())
            {
                foreach (var check in parsedLogFile.Checks)
                {
                    Checks.Add(check);
                }
            }

            List<PDBeam> pdBeams = pdPlan.Beams.Where(b => b.PortalDoseImages.Count >= 1).ToList();

            PortalDoseResults = new List<PortalDoseResult>();

            if (parsedLogFile.PredictedFieldData != null && parsedLogFile.PredictedFieldData.Any())
            {
                foreach (var predictedFieldData in parsedLogFile.PredictedFieldData)
                {
                    pdBeam = pdBeams.Where(b => b.Beam.Id.Equals(predictedFieldData.FieldId)).FirstOrDefault();
                    if (pdBeam != null)
                    {
                        var pdResult = new PortalDoseResult(pdBeam, predictedFieldData, parsedLogFile.InVivoFlag);
                        PortalDoseResults.Add(pdResult);
                        // Add the Checks to the Checks collection, only if there's not already an identical check in the collection
                        if (pdResult.Checks.Any())
                        {
                            foreach (var check in pdResult.Checks)
                            {
                                if (!Checks.Any(c => c.Description == check.Description && c.Result == check.Result))
                                {
                                    Checks.Add(check);
                                }
                            }
                        }
                    }
                    else
                    {
                        Checks.Add(new Check(CheckResult.Error, $"No portal dose beam found for field id {predictedFieldData.FieldId}"));
                    }
                }
            }
            else
            {
                Checks.Add(new Check(CheckResult.Error, "No predicted field data found, copy the log file to clipboard in PlanChecker"));
            }
        }
    }
}
