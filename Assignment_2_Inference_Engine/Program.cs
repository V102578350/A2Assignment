using System;
using System.Collections.Generic;
using System.IO;

namespace Assignment_2_Inference_Engine
{
    class Program    
    {
        static void Main(string[] args)
        {
            //string used to store filename from command line argument
            string filename = null;
            
            //string used to store method from command line argument
            string methodValue = null;

            //Check command-line arguments
            if (args.Length < 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine("Please enter the following command line arguments:");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("> iengine.exe { method } { filename }");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("> method = TT, FC, BC (Truth Table, Forward Chaining, Backward Chaining)");
                Console.WriteLine("> filename = a horn form KB *.txt file with ASK and TELL");

                Environment.Exit(1);
            } else
            {
                //check arguments
                methodValue = args[0].ToUpper(); //convert to uppercase so that it is in the correct format (ie tt = TT)
                filename = args[1];
            }

            //Open reader for Horn Form KB File
            StreamReader rdr = new StreamReader(filename);

            //Method interface initialized as null
            IMethod Method = null;

            //string variable used to store ASK in Horn Form KB
            string ask = null;


            //Check Whether end of file has been reached
            while (!rdr.EndOfStream)
            {
                //store current line in string
                string line = rdr.ReadLine();

                //check if current line is TELL identifier
                if(line == "TELL")
                {
                    //Read next line and store 
                    line = rdr.ReadLine();
                    //split line into array of strings using ; delimiter
                    string[] sentences = line.Split(';');

                    //temp list used to add individual sentences
                    List<string> temp = new List<string>();

                    //Loop iterates though array and
                    //check whether there is no empty 
                    //empty sentences, then adds to temp
                    //list.
                    foreach (string s in sentences)
                        if (s.Trim() != "") temp.Add(s.Trim());

                    //Initialize method
                    Method = GenerateMethod(methodValue, temp.ToArray());
                }

                //Reads next line and store
                line = rdr.ReadLine();

                //check if line is ASK identifier
                //if so, store value in ask string
                if (line == "ASK")
                    ask = rdr.ReadLine();
            }
            
            //Call ASK function from 
            string result = Method.Ask(ask);

            if(result != null)
            {
                Console.WriteLine("YES: " + result);
            } else
            {
                Console.WriteLine($"NO : The sentence '{ask}' is not entailed from the knowledge base!");
            }
        }


        //GenerateMethod function takes string aMethod and array of strings
        //aSentenceString and returns an IMethod object
        //Enforces a switch statement using aMethod as the subject to determine
        //which kind of method should be initialized in the interface object.
        //
        // If aMethod enter is invalid, the program terminates with informative
        // exit output.
        public static IMethod GenerateMethod(string aMethod, string[] aSentenceStrings)
        {
            IMethod result = null;
            switch (aMethod)
            {
                case "TT":
                    result = new TTChecking(aSentenceStrings);
                    break;
                case "FC":
                    result = new ForwardChaining(aSentenceStrings);
                    break;
                case "BC":
                    result = new BackwardChaining(aSentenceStrings);
                    break;
                default:
                    Console.WriteLine("Invalid method! Please Choose from TT, FC or BC.");
                    Environment.Exit(1);
                    break;
            }

            return result;
        }
    }
}
