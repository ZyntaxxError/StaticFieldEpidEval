using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.CA.Scripting;

namespace StaticFieldEpidEval
{
    public class RandomTest
    {

        public string SomeText { get; set; }

        public RandomTest(PlanSetup plan, Beam beam)
        {

            SomeText = $"Plan Id: {plan.Id}, Beam Id: {beam.Id}";



        }


    }
}
