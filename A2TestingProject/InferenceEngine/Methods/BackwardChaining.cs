using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment_2_Inference_Engine
{
    public class BackwardChaining : IMethod
    {
        private Sentence[] _setences; 
        private List<string> _discovered; //stores known terms from sentences (DB)

        public BackwardChaining(string[] aSetenceStrings)
        {
            _setences = new Sentence[aSetenceStrings.Length];

            //initialize sentences array
            for (int i = 0; i < _setences.Length; i++)
                if (aSetenceStrings[i].Trim() != "") _setences[i] = new Sentence(aSetenceStrings[i].Trim());

            _discovered = new List<string>();
        }

        public Sentence[] GetSentences
        {
            get { return _setences; }
        }

        public void OutputSentences()
        {
            Console.WriteLine($"[BC] Current Knowledge Base (Cosisting of {_setences.Length} rules)");
            for (int i = 0; i < _setences.Length; i++)
            {
                Console.WriteLine($"\t ({i + 1}) > {_setences[i].GetSentence}");
            }
        }

        public string Ask(string aQuery)
        {
            ///Backward Chaining
            /// - FROM GOAL STATE TO INIT STATE
            /// - e.g. (aQuery d)
            /// p2=>p3 ; p3=>p1 ; c=>e ; b&e=>f ; f&g=>h ; ((p1=>d)) ; p1&p3=>c ; a ; b ; p2; //pick where goal is implied
            /// p2=>p3 ; ((p3=>p1)) ; c=>e ; b&e=>f ; f&g=>h ; p1=>d ; p1&p3=>c ; a ; b ; p2; //pick sentence for p1's implication/stated
            /// ((p2=>p3)) ; p3=>p1 ; c=>e ; b&e=>f ; f&g=>h ; p1=>d ; p1&p3=>c ; a ; b ; p2; //pick sentence for p3's implication/stated
            /// p2=>p3 ; p3=>p1 ; c=>e ; b&e=>f ; f&g=>h ; p1=>d ; p1&p3=>c ; a ; b ; ((p2)); //pick sentence for p2's implication/stated
            /// end = p2, p3, p1, d with backtracing (backward chaining)
            ///-------------------------------------------------------------------------------------------------------------------------

            Console.WriteLine($"[>] ASK KB : {aQuery}");
            List<string> entailed = new List<string>();
            List<string> termFocus = new List<string>(); //consider this a queue

            //find single symbols for discovered
            foreach(Sentence s in _setences)
            {
                Terms t = s.GetSentenceTerms();
                if(t.TermValues.Length == 1)
                {
                    if(!_discovered.Contains(t.TermValues[0]))
                        _discovered.Add(t.TermValues[0]);
                }
            } 

            //add goal
            termFocus.Add(aQuery);

            int maxIterations = 10;

            while(maxIterations != 0)
            {
                for (int i = 0; i < _setences.Length; i++)
                {
                    //gets sentence components (terms) i.e p2=> p1 returns new String[] { "p2" , "p1" }
                    Terms sentenceComponants = _setences[i].GetSentenceTerms();

                    int count = termFocus.Count;
                    for (int j = 0; j < count; j++)
                    {
                        // FIND SUB - GOAL IMPLCATION (EG p1 => d (would be d))
                        if (sentenceComponants.TermValues[sentenceComponants.TermValues.Length - 1] == termFocus[j])
                        {

                            for (int k = 0; k < sentenceComponants.TermValues.Length - 1; k++)
                            {
                                bool flag = true;
                                // CHECK IF LHS ELEMENT OF SENTENCE COMPONANTS IS IN DISCOVERED
                                if (_discovered.Contains(sentenceComponants.TermValues[k]))
                                {
                                    //IF TRUE ADD LHS TO ENTAILED
                                    if (sentenceComponants.LogicalOperators.Count > 1)
                                    {
                                        bool temp;
                                        switch (sentenceComponants.LogicalOperators[0])
                                        {
                                            case LogicalOperator.Conjunction:
                                                temp = true;
                                                for (int x = 0; x < sentenceComponants.TermValues.Length - 1; x++)
                                                {
                                                    if (!_discovered.Contains(sentenceComponants.TermValues[x]))
                                                    {
                                                        temp = false;
                                                        termFocus.Insert(0, sentenceComponants.TermValues[x]);
                                                        break;
                                                    }else
                                                    {
                                                        temp = true;
                                                    }
                                                }

                                                flag = temp;

                                                break;
                                            case LogicalOperator.Disjunction:
                                                entailed.Add(sentenceComponants.TermValues[k]);
                                                if (sentenceComponants.TermValues[sentenceComponants.TermValues.Length - 1] == aQuery)
                                                {
                                                    entailed.Add(sentenceComponants.TermValues[sentenceComponants.TermValues.Length - 1]);
                                                    return toString(entailed);
                                                }
                                                break;
                                            /*case LogicalOperator.Biconditional:
                                                bool[] results = new bool[2];
                                                for (int x = 0; x < sentenceComponants.TermValues.Length - 1; x++)
                                                {
                                                    if (!_discovered.Contains(sentenceComponants.TermValues[x]))
                                                    {
                                                        results[x] = false;
                                                        //termFocus.Insert(0, sentenceComponants.TermValues[x]);
                                                    }
                                                    else
                                                    {
                                                        results[x] = true;
                                                    }
                                                }

                                                if(results[0] != results[1])
                                                {
                                                    flag = false;
                                                }
                                                break;*/
                                        }
                                    }

                                    // CHECK FLAG
                                    if (flag)
                                    {
                                        entailed.Add(sentenceComponants.TermValues[k]);

                                        //add last element if it is the goal state, given that we know its LHS
                                        if (sentenceComponants.TermValues[sentenceComponants.TermValues.Length - 1] == aQuery)
                                        {
                                            entailed.Add(sentenceComponants.TermValues[sentenceComponants.TermValues.Length - 1]);
                                        }
                                       
                                    } else
                                    {
                                        break;
                                    }
                                      
                                    // ADD TO DISCOVERED
                                    _discovered.Add(sentenceComponants.TermValues[sentenceComponants.TermValues.Length - 1]);
                                    // REMOVE FROM FOCUS
                                    termFocus.Remove(sentenceComponants.TermValues[sentenceComponants.TermValues.Length - 1]);

                                    if(entailed[entailed.Count - 1] == aQuery) 
                                        return toString(entailed);
                                }
                                else // IF NOT ADD TO TERM FOCUS LIST
                                {
                                    if(!termFocus.Contains(sentenceComponants.TermValues[k]))
                                        termFocus.Insert(0, sentenceComponants.TermValues[k]); 
                                }
                            }
                           if (count != termFocus.Count) break; //break out if term focus different size

                        }
                    }

                }
                maxIterations--;
            }

            return null;
        }

        public string toString(List<string> aEntailed)
        {
            List<string> temp = new List<string>();
            string result = "";
            foreach (string s in aEntailed)
                temp.Add(s);

            foreach(string s in temp)
                result += s + "; ";

            return result;
        }
    }
}
