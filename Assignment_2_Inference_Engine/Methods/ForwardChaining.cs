using System;
using System.Collections.Generic;

namespace Assignment_2_Inference_Engine
{
    public class ForwardChaining : IMethod
    {
        private Sentence[] _setences;
        private List<string> _discovered; //stores known terms from sentences (DB)

        public ForwardChaining(string[] aSentenceStrings) 
        {
            _setences = new Sentence[aSentenceStrings.Length];

            //initialize sentences array
            for (int i = 0; i < _setences.Length; i++)
                if(aSentenceStrings[i].Trim() != "") _setences[i] = new Sentence(aSentenceStrings[i].Trim());

            _discovered = new List<string>();
        }

        public Sentence[] GetSentences
        {
            get { return _setences; }
        }

        public void OutputSentences()
        {
            Console.WriteLine($"[FC] Current Knowledge Base (Cosisting of {_setences.Length} rules)");
            for(int i = 0; i < _setences.Length; i++)
            {
                Console.WriteLine($"\t ({i + 1}) > {_setences[i].GetSentence}");
            }
        }

        public string Ask(string aQuery) // Where query is a sentence which we desire proof for
        {
            Console.WriteLine($"[>] ASK KB : {aQuery}");
            List<string> entailed = new List<string>();
            
            int maxIterations = 10;

            while (maxIterations != 0)
            {
                for (int i = 0; i < _setences.Length; i++)
                {
                    //gets sentence components (terms) i.e p2=> p1 returns new String[] { "p2" , "p1" }
                    Terms sentenceComponants = _setences[i].GetSentenceTerms();

                    string lastTermElement = sentenceComponants.TermValues[sentenceComponants.TermValues.Length - 1];
                    bool flag = true;

                    if(sentenceComponants.LogicalOperators.Count > 1)
                    {
                        
                        switch(sentenceComponants.LogicalOperators[0])
                        {
                            case LogicalOperator.Conjunction: // AND operator
                                for (int j = 0; j < sentenceComponants.TermValues.Length - 1; j++)
                                {
                                    if (!_discovered.Contains(sentenceComponants.TermValues[j]))
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                break;
                            case LogicalOperator.Disjunction: //OR operator
                                for (int j = 0; j < sentenceComponants.TermValues.Length - 1; j++)
                                { 
                                    if (_discovered.Contains(sentenceComponants.TermValues[j]))
                                    {
                                        flag = true;
                                        break;
                                    } else
                                    {
                                        flag = false;
                                    }
                                }
                                break;
                        }

                    } else
                    {
                        for (int j = 0; j < sentenceComponants.TermValues.Length - 1; j++)
                        {
                            if (!_discovered.Contains(sentenceComponants.TermValues[j]))
                            {
                                flag = false;
                                break;
                            }
                        }
                    }



                    if (flag)
                    {
                        if (!_discovered.Contains(lastTermElement))
                        { //ensure lastTermElement doesnt exist in disco

                            entailed.Add(lastTermElement);
                            _discovered.Add(lastTermElement);
                        }


                        //check if we have reached the desired state (eg d)
                        //if so, end the while loop and break current for loop
                        if (entailed[entailed.Count - 1] == aQuery)
                        {
                            return toString(entailed);
                        }
                    }
                }
                maxIterations--;
            }


            return null;

        }

        public string toString(List<string> aEntailed)
        {
            string result = "";
            foreach (string s in aEntailed)
                result += (s + "; ");

            return result;
        }
    }
}
