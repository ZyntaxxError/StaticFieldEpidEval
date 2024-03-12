using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticFieldEpidEval.Models
{

    internal class Check
    {
        /// <summary>
        /// The exact result of the check
        /// </summary>
        public CheckResult Result { get; set; }

        /// <summary>
        /// The name of the check
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Constructor for the Check class
        /// </summary>
        /// <param name="result"></param>
        /// <param name="description"></param>
        public Check(CheckResult result, string description)
        {
            Result = result;
            Description = description;
        }
    }

}
