using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StaticFieldEpidEval.Models
{
    /// <summary>
    /// Parses the csv section of the logfile from PlanChecker.esapi.dll that has previously been copied to the clipboard.
    /// Determines if the field is in vivo or in vitro and creates a list of PredictedFieldData objects from the data.
    /// </summary>
    internal class ParseLogFile
    {
        public bool InVivoFlag { get; set; }
        public List<PredictedFieldData> PredictedFieldData { get; set; }
        public List<Check> Checks { get; set; }

        /// <summary>
        /// Constructor for the ParseLogFile class. Retrieves the lines representing the field data from clipboard if UID found in the text
        /// </summary>
        /// <param name="uid"></param>
        public ParseLogFile(string uid)
        {
            Checks = new List<Check>();
            PredictedFieldData = new List<PredictedFieldData>();

            // Get the lines representing the field data from clipboard if UID for the plan found in the text
            var fieldLines = GetLinesAfterUID(uid);

            if (fieldLines != null && fieldLines.Count > 0)
            {
                foreach (var fieldLine in fieldLines)
                {
                    string[] parts = fieldLine.Split(',');
                    if (parts.Length == 4)
                    {
                        ParseLineAndCreatePredictedFieldData(fieldLine, parts);
                    }
                    else
                    {
                        Checks.Add(new Check(CheckResult.Error, $"Error parsing predicted field data, unexpected number of parts ({parts.Length})."));
                    }
                }
            }
            else
            {
                Checks.Add(new Check(CheckResult.Error, "No predicted field data for the imaged plan found in clipboard"));
            }
        }


        /// <summary>
        /// Retrieves the lines representing the field data from clipboard if UID found in the text
        /// Sets the InVivoFlag to true if the fields are in vivo, false if in vitro.
        /// </summary>
        /// <param name="uID"></param>
        /// <returns></returns>
        private List<string> GetLinesAfterUID(string uID)
        {
            string[] lines = Clipboard.GetText().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            List<string> result = new List<string>();
            bool foundUID = false;

            foreach (string line in lines)
            {
                if (foundUID)
                {
                    if (line.Contains("END_INVIVO") || line.Contains("END_INVITRO"))
                    {
                        InVivoFlag = line.Contains("END_INVIVO");
                        break;
                    }
                    else
                    {
                        result.Add(line);
                    }
                }
                else if (line.Contains(uID))
                {
                    foundUID = true;
                }
            }
            return foundUID ? result : null;
        }


        /// <summary>
        /// Parses the line and creates a PredictedFieldData object from the data.
        /// </summary>
        /// <param name="fieldLine"></param>
        /// <param name="parts"></param>
        private void ParseLineAndCreatePredictedFieldData(string fieldLine, string[] parts)
        {
            if (int.TryParse(parts[0], out int intValue) &&
                double.TryParse(parts[1], out double doubleValue1) &&
                double.TryParse(parts[2], out double doubleValue2) &&
                double.TryParse(parts[3], out double doubleValue3))
            {
                PredictedFieldData.Add(new PredictedFieldData
                {
                    FieldId = intValue.ToString(),
                    ReadOutPositionCollimatorAtIso = new Vector2D(doubleValue1, doubleValue2),
                    PredictedValue = doubleValue3
                });
            }
            else
            {
                Checks.Add(new Check(CheckResult.Error, $"Error parsing predicted field data, could not parse the following line: \"{fieldLine}\"."));
            }
        }
    }


}
