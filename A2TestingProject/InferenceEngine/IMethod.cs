using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment_2_Inference_Engine
{
    interface IMethod
    {
        public string Ask(string aQuery);
        public void OutputSentences();
    }
}
