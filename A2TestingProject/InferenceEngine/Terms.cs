using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment_2_Inference_Engine
{
    public class Terms
    {
        //how it works:
        // termValue logicalOperator boundValue
        // ie. p2 => p1

        private string[] _termValues;
        private List<LogicalOperator> _logicalOperators;

        public Terms(string[] aTermValues, List<LogicalOperator> aLogicalOperators)
        {
            _termValues = aTermValues;
            _logicalOperators = aLogicalOperators;
        }

        public string[] TermValues { get => _termValues; }
        public List<LogicalOperator> LogicalOperators { get => _logicalOperators; }
    }
}
