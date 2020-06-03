using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment_2_Inference_Engine
{
    public class TTChecking : IMethod
    {
        private List<string> _symbols;
        private Sentence[] _sentences;

        public TTChecking(string[] aSentenceStrings)
        {
            _symbols = new List<string>();
            _sentences = new Sentence[aSentenceStrings.Length];

            for (int i = 0; i < _sentences.Length; i++)
                if (aSentenceStrings[i].Trim() != "") _sentences[i] = new Sentence(aSentenceStrings[i].Trim());


        }

        public string Ask(string aQuery)
        {
            Console.WriteLine($"[>] ASK KB : {aQuery}");
            string result = null;

            //store terms (variables and logic from each sentence)
            List<Terms> lTerms = new List<Terms>();
            foreach (Sentence s in _sentences)
                lTerms.Add(s.GetSentenceTerms());


            //if _symbols is empty
            if (_symbols.Count == 0)
            {
                foreach (Terms t in lTerms)
                {
                    foreach (string s in t.TermValues)
                        if (!_symbols.Contains(s))
                            _symbols.Add(s); //add each individual UNIQUE term (ie. a, b, c, d, e, f, g, h, p1, p2, p3)
                }

            }

            _symbols.Sort(); //sort in assending to format nicely (alphabetical order) (A-Z)

            //calculate TT size
            int rowCount = (int)Math.Pow(2, _symbols.Count);
            int colCount = _symbols.Count + lTerms.Count + 2; //+2 for KB and Query
            ///Columns : a b c d e f g h p1 p2 p3 [ + each sentence + KB + Query] = 20 Col
            ///Rows : 2^11 = 2048
            
            ///Console.WriteLine($"rows = {rowCount} -- cols = {colCount}"); DEBUG

            //initialize TT data-structure
            int[,] tt = new int[colCount, rowCount]; //where col = x, and row = y

            //set base values of tt
            tt = GenerateTTValues(tt, _symbols.Count);

            //solve tt
            result = SolveTT(tt, lTerms, aQuery);

            return result;
            

        }

        //returns 0 = false 1 = true
        public int LogicalOperation(int aT1, int aT2, LogicalOperator aOperator)
        {
            int result = 1;
            switch (aOperator)
            {
                case LogicalOperator.Biconditional:
                    if (aT1 != aT2 && aT2 != aT1) result = 0;
                    break;
                case LogicalOperator.Implication:
                    if (aT1 != 0 && aT2 != 1) result = 0;
                    break;
                case LogicalOperator.Conjunction:
                    if (aT1 != 1 || aT2 != 1) result = 0;
                    break;
                case LogicalOperator.Disjunction:
                    if (aT1 != 1 && aT2 != 1) result = 0;
                    break;
            }
            return result;
        }

        public int FindSymbolPosition(string aSymbol)
        {
            int result = -1;
            for(int i = 0; i < _symbols.Count; i++)
            {
                if(_symbols[i] == aSymbol)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        public int Entails(int aKBValue, int aQueryValue)
        {
            int s1 = LogicalOperation(aKBValue, aQueryValue, LogicalOperator.Implication);

            return LogicalOperation(s1, aKBValue, LogicalOperator.Biconditional);

        }

        public string SolveTT(int[,] aTT, List<Terms> aKB, string aQuery)
        {
            int numberOfEntailments = 0; //number of models where (KB entails aQuery)

            if(aTT != null)
            {
                //find last
                int limit = 0;
                for(int i = 0;  i < aTT.GetLength(0); i++) //assuming that the last row will be continuous 1s til limit is hit (0)
                {
                    if (aTT[i, aTT.GetLength(1)-1] == 0)
                    {
                        limit = i;
                        break;
                    }
                        
                }


                for (int row = 0; row < aTT.GetLength(1); row++)
                {
                    for (int col = limit; col < aTT.GetLength(0)-2; col++)
                    {
                        //if (aKB[col - limit].TermValues.Length == 1) continue;

                        if (aKB[col - limit].TermValues.Length > 2 && aKB[col - limit].LogicalOperators.Count > 1)
                        {
                            int result = -1;
                            int index = 0;
                            int temp = 0;

                            while (result == -1)
                            {
                                int s1 = aTT[FindSymbolPosition(aKB[col - limit].TermValues[index]), row];
                                int s2 = aTT[FindSymbolPosition(aKB[col - limit].TermValues[index + 1]), row];
                                temp = LogicalOperation(s1, s2, aKB[col - limit].LogicalOperators[index]);

                                index += 2; ;
                                if (index >= aKB[col - limit].TermValues.Length - 1)
                                {
                                    result = LogicalOperation(temp, aTT[FindSymbolPosition(aKB[col - limit].TermValues[index]), row], aKB[col - limit].LogicalOperators[aKB[col - limit].LogicalOperators.Count - 1]);
                                    aTT[col, row] = result;
                                }
                            }

                        }
                        else if(aKB[col - limit].TermValues.Length == 2)
                        {

                            int s1 = aTT[FindSymbolPosition(aKB[col - limit].TermValues[0]), row];
                            int s2 = aTT[FindSymbolPosition(aKB[col - limit].TermValues[1]), row];
                            //Console.WriteLine($"{aKB[col - limit].TermValues[0]} = {s1} -- {aKB[col - limit].TermValues[1]} = {s2}");
                            aTT[col, row] = LogicalOperation(s1, s2, aKB[col - limit].LogicalOperators[0]);
                        } else
                        {
                            aTT[col, row] = aTT[FindSymbolPosition(aKB[col - limit].TermValues[0]), row];
                        }


                    }

                    int kbCol = aTT.GetLength(0) - 2;
                    int queryCol = aTT.GetLength(0) - 1;
                    int tValue = 1; //used to assign truth value
                    //find result for kbCol
                    for (int col = limit; col < aTT.GetLength(0)-2; col++)
                    {
                        if (aTT[col, row] == 0)
                        {
                            tValue = 0;
                            break;
                        }
                    }
                    aTT[kbCol, row] = tValue; //assign kb truth value

                    int queryValue = 0;
                    if (FindSymbolPosition(aQuery) != -1) //check whether the ask(ed) is a sentence
                    {
                        queryValue = aTT[FindSymbolPosition(aQuery), row];
                    } else
                    {
                        return null;
                    }
                      
                    

                    //Satisfiabiluty is conntect to inference via the following:
                    // KB entails query IFF (if and only if) KB ^ ~a is unsatisfiable
                    // A sentence is unsatisfiable if its true in no nodels (A ^ ~A)
                    
                    aTT[queryCol, row] = Entails(aTT[kbCol, row], queryValue);

                    if (aTT[queryCol, row] == 1)
                        numberOfEntailments++;


                }
            }
            return numberOfEntailments.ToString();
        }

        //Generates base values for TT
        //aTT is the given 2D array
        //aLimit is the column limit
        public int[,] GenerateTTValues(int[,] aTT, int aLimit)
        {
            int[] state = new int[aLimit]; //hold state of values


            for (int row = 1; row < aTT.GetLength(1); row++)
            {
                for (int col = aLimit-1; col >= 0; col--)
                {
                   
                    if (state[col] == 0)
                    {
                        state[col] = 1;
                        break;
                        
                    } else
                    {
                        state[col] = 0;
                    }

                }
                
                for(int i = 0; i < aLimit; i++)
                {
                    aTT[i, row] = state[i];
                }

            }

            return aTT;
        }

        public void OutputSentences()
        {
            Console.WriteLine($"[TT] Current Knowledge Base (Cosisting of {_sentences.Length} rules)");
            for (int i = 0; i < _sentences.Length; i++)
            {
                Console.WriteLine($"\t ({i + 1}) > {_sentences[i].GetSentence}");
            }
        }
    }
}
