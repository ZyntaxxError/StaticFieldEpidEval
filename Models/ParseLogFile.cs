using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.CA.Scripting;
using System.Windows.Forms;

namespace StaticFieldEpidEval.Models
{
    internal class ParseLogFile
    {
        public bool InVivo { get; set; }
        public List<PredictedFieldData> PredictedFieldData { get; set; }


        public ParseLogFile(string uid)
        {
            var fieldLines = GetLinesAfterUID(uid);
            if (fieldLines != null && fieldLines.Count > 0)
            {
                MessageBox.Show("Found " + fieldLines.Count + " lines of data for UID " + uid);
                PredictedFieldData = new List<PredictedFieldData>();
                foreach (var fieldLine in fieldLines)
                {
                    string[] parts = fieldLine.Split(',');
                    if (parts.Length == 4)
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
                            // TODO: Parsing failed, handle error
                        }
                    }
                    else
                    {
                        // Invalid input, handle error
                    }
                }
            }
        }


        /// <summary>
        /// Retrieves the lines representing the field data from clipboard if UID found in the text
        /// and creates a list of PredictedFieldData objects from the data.
        /// Sets the InVivo property to true if the field is in vivo, false if in vitro.
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
                    if (line.Trim() == "END_INVIVO" || line.Trim() == "END_INVITRO")
                    {
                        InVivo = line.Trim() == "END_INVIVO";
                        break;
                    }
                    result.Add(line);
                }
                else if (line.Contains(uID))
                {
                    foundUID = true;
                }
            }
            return foundUID ? result : null;
        }
    }

    public class PredictedFieldData
    {
        public string FieldId { get; set; }
        public Vector2D ReadOutPositionCollimatorAtIso { get; set; }
        public double PredictedValue { get; set; }
    }
}
