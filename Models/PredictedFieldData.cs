using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticFieldEpidEval.Models
{
    /// <summary>
    /// Class to hold the predicted field data parsed from the logfile by the ParseLogFile class
    /// </summary>
    public class PredictedFieldData
    {
        public string FieldId { get; set; }
        public Vector2D ReadOutPositionCollimatorAtIso { get; set; }
        public double PredictedValue { get; set; }
    }
}
