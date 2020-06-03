using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Assignment_2_Inference_Engine
{
    public class Sentence
    {
        private string _senctence;

        public Sentence(string aSentence)
        {
            this._senctence = aSentence;
        }

        public string GetSentence
        {
            get { return _senctence; }
        }

        public Terms GetSentenceTerms()
        {
            Terms result = null;
            List<LogicalOperator> loList = new List<LogicalOperator>(); // default

            string temp = _senctence.Replace(" ", String.Empty); //remove all spaces

            List<string> terms = new List<string>();
            string buildTerm = "";

            for (int i = 0; i < temp.Length; i++)
            {
                LogicalOperator checkLO = CheckLogicOperator(temp.ToCharArray()[i]);
                switch (checkLO)
                {
                    case LogicalOperator.Null:
                        buildTerm += temp.ToCharArray()[i];
                        break;
                    case LogicalOperator.Biconditional:
                        terms.Add(buildTerm);
                        loList.Add(LogicalOperator.Biconditional);
                        buildTerm = "";
                        i+= 2;
                        break;
                    case LogicalOperator.Implication:
                        terms.Add(buildTerm);
                        loList.Add(LogicalOperator.Implication);
                        buildTerm = "";
                        i++;
                        break;
                    case LogicalOperator.Conjunction:
                        loList.Add(LogicalOperator.Conjunction);
                        terms.Add(buildTerm);
                        buildTerm = "";
                        break; //no addition to i index since conjunction only uses 1 char (natural ++ from forloop accomidates for this)
                    case LogicalOperator.Disjunction:
                        loList.Add(LogicalOperator.Disjunction);
                        terms.Add(buildTerm);
                        buildTerm = "";
                        break; //no addition to i index since disjunction only uses 1 char (natural ++ from forloop accomidates for this)

                }
                
            }
            terms.Add(buildTerm); //add leading term (variables)

            //create term array
            //foreach (string s in terms)
            //   Console.WriteLine(">> " + s);

            result = new Terms(terms.ToArray(), loList);
            

            return result;
        }

        //determing which logic operator is used on operarands in sentence
        public LogicalOperator CheckLogicOperator(char aChar) 
        {
            switch(aChar)
            {
                case '=':
                    //the '=' sign should always be the begining of implication
                    return LogicalOperator.Implication;
                case '<':
                    return LogicalOperator.Biconditional;
                case '&':
                    return LogicalOperator.Conjunction;
                case '|':
                    return LogicalOperator.Disjunction;
                default:
                    return LogicalOperator.Null;
            }
        }

        
    }
}
