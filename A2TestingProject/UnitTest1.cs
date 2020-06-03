using NUnit.Framework;
using System.Collections.Generic;

namespace Assignment_2_Inference_Engine
{
    public class Tests
    {
        IMethod Method;

        [SetUp]
        public void Setup()
        {
            //Setup class object
            Method = null;
        }

        ///
        ///Forward Chaining Tests
        ///

        [TestCase(new string[] { "p2 => p3", "p3 => p1", "c => e",
            "b & e => f", "f & g => h", "p1 => d",
            "p1 & p3 => c",  "a" , "b", "p2"  }, "d")]
        public void Test1ForwardChaining(string[] aSentences, string aQuery)
        {
            Method = new ForwardChaining(aSentences);
            string result = Method.Ask(aQuery);

            Assert.AreEqual("a; b; p2; p3; p1; d; " , result);
        }

        [TestCase(new string[] { "p2 => p3", "p3 => p1", "c => e",
            "b & e => f", "f & g => h", "p1 => d",
            "p1 & p3 => c",  "a" , "b", "p2"  }, "z")]
        public void Test2ForwardChaining(string[] aSentences, string aQuery)
        {
            Method = new ForwardChaining(aSentences);

            string result = Method.Ask(aQuery);

            Assert.AreEqual(null, result);
        }

        [TestCase(new string[] { "p2 => p3", "p3 => p1", "c => e",
            "b & e => f", "f & g => h", "p1 => d",
            "p1 & p3 => c",  "a" , "b", "p2"  }, "p3")]
        public void Test3ForwardChaining(string[] aSentences, string aQuery)
        {
            Method = new ForwardChaining(aSentences);

            string result = Method.Ask(aQuery);

            Assert.AreEqual("a; b; p2; p3; ", result);
        }

        [TestCase(new string[] { "z =>c", "a=> z", "z&c =>f",  "a", "x"  }, "f")]
        public void Test4ForwardChaining(string[] aSentences, string aQuery)
        {
            Method = new ForwardChaining(aSentences);

            string result = Method.Ask(aQuery);

            Assert.AreEqual("a; x; z; c; f; ", result);
        }


        ///
        ///Backward Chaining Tests
        ///

        [TestCase(new string[] { "p2 => p3", "p3 => p1", "c => e",
            "b & e => f", "f & g => h", "p1 => d",
            "p1 & p3 => c",  "a" , "b", "p2"  }, "d")]
        public void Test1BackwardChaining(string[] aSentences, string aQuery)
        {
            Method = new BackwardChaining(aSentences);

            string result = Method.Ask(aQuery);

            Assert.AreEqual("p2; p3; p1; d; ", result);
        }

        [TestCase(new string[] { "p2 => p3", "p3 => p1", "c => e",
            "b & e => f", "f & g => h", "p1 => d",
            "p1 & p3 => c",  "a" , "b", "p2"  }, "c")]
        public void Test2BackwardChaining(string[] aSentences, string aQuery)
        {
            Method = new BackwardChaining(aSentences);

            string result = Method.Ask(aQuery);

            Assert.AreEqual("p2; p3; p1; c; ", result);
        }

        [TestCase(new string[] { "p2 => p3", "p3 => p1", "c => e",
            "b & e => f", "f & g => h", "p1 => d",
            "p1 & p3 => c",  "a" , "b", "p2"  }, "v")]
        public void Test3BackwardChaining(string[] aSentences, string aQuery)
        {
            Method = new BackwardChaining(aSentences);

            string result = Method.Ask(aQuery);

            Assert.AreEqual(null, result);
        }

        [TestCase(new string[] { "p2 => p3", "p3 => p1", "c => e",
            "b & e => f", "f & g => h", "p1 => d",
            "z || p1 => c",  "a" , "b", "p2"  }, "c")]
        public void Test4BackwardChaining(string[] aSentences, string aQuery)
        {
            Method = new BackwardChaining(aSentences);

            string result = Method.Ask(aQuery);

            Assert.AreEqual("p2; p3; p1; c; ", result);
        }

        [TestCase(new string[] { "p2 => p3", "p3 => p1", "c => e",
            "b & e => f", "f & g => h", "p1 => d",
            "p1 &z => c",  "a" , "b", "p2",  }, "c")]
        public void Test5BackwardChaining(string[] aSentences, string aQuery)
        {
            Method = new BackwardChaining(aSentences);

            string result = Method.Ask(aQuery);

            Assert.AreEqual(null, result);
        }

        ///
        ///TTChecking Tests
        ///

        [TestCase(new string[] { "p =>r", "q&r => p", "q", "r" }, "p")]
        public void Test1TTChecking(string[] aSentences, string aQuery)
        {
            Method = new TTChecking(aSentences);

            string result = Method.Ask(aQuery);

            Assert.AreEqual("1", result);
        }

        [TestCase(new string[] { "p2 => p3", "p3 => p1", "c => e",
            "b & e => f", "f & g => h", "p1 => d",
            "p1 & p3 => c",  "a" , "b", "p2"  }, "z")]
        public void Test2TTChecking(string[] aSentences, string aQuery)
        {
            Method = new TTChecking(aSentences);

            string result = Method.Ask(aQuery);

            Assert.AreEqual(null, result);
        }


    }


}