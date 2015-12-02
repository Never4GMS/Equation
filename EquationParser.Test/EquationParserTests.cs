using Equation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Equation.Test
{
    [TestClass]
    public class EquationParserTests
    {
        [TestMethod]
        public void GetSentenceList()
        {
            var statement = "x^2 + 3.5xy + y = y^2 - xy + y";
            var expected = new List<string>(statement.Split(' '));
            var result = EquationParser.GetSentenceList(statement);

            Assert.AreEqual(expected.Count, result.Count);
            Assert.AreEqual(string.Concat(expected), string.Concat(result));
        }

        [TestMethod]
        public void MoveSentencesFromTheRightToTheLeftForStatement()
        {
            var statement = "x^2 + 3.5xy + y = y^2 - xy + y";
            var expected = new List<string>("x^2 + 3.5xy + y - y^2 + xy - y = 0".Split(' '));
            var result = EquationParser.MoveRight(statement);

            Assert.AreEqual(expected.Count, result.Count);
            Assert.AreEqual(string.Concat(expected), string.Concat(result));
        }

        [TestMethod]
        public void MoveSentencesFromTheRightToTheLeftForSentances()
        {
            var sentances = new List<string>("x^2 + 3.5xy + y = y^2 - xy + y".Split(' '));
            var expected = new List<string>("x^2 + 3.5xy + y - y^2 + xy - y = 0".Split(' '));
            var result = EquationParser.MoveRight(sentances);

            Assert.AreEqual(expected.Count, result.Count);
            Assert.AreEqual(string.Concat(expected), string.Concat(result));
        }

        [TestMethod]
        public void GroupByVariablesAndDigits()
        {
            var sentances = new List<string>("x^2 + 2xy + 14y - 14 + 5xy - 10y + 12 = 0".Split(' '));
            var expected = new List<string>("x^2 + 7xy + 4y - 2 = 0".Split(' '));
            var result = EquationParser.GroupByRight(sentances);

            Assert.AreEqual(expected.Count, result.Count);
            Assert.AreEqual(string.Concat(expected), string.Concat(result));
        }

        [TestMethod]
        public void GroupByVariables()
        {
            var sentances = new List<string>("x^2 + 3.5xy + y - y^2 + xy - y = 0".Split(' '));
            var expected = new List<string>("x^2 - y^2 + 4.5xy = 0".Split(' '));
            var result = EquationParser.GroupByRight(sentances);

            Assert.AreEqual(expected.Count, result.Count);
            Assert.AreEqual(string.Concat(expected), string.Concat(result));
        }

        [TestMethod]
        public void OpenParentancesWithoutParentancesReturnSameStatement()
        {
            var statement = "x^2 + 7xy + 4y - 2 = 0";
            var result = EquationParser.OpenParenteces(statement);
            Assert.AreEqual(statement, result);
        }

        [TestMethod]
        public void OpenSingleParenteces()
        {
            var sentances = "2 * (2x + 5) * 3 + (y - 3x) * 0.5 = 0";
            var expected = "12x + 30 + 0.5y - 1.5x = 0";
            var result = EquationParser.OpenParenteces(sentances);
                        
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ToSimpleForm()
        {
            var sentance = "x^2 + 3.5xy + 2 * (2x + 5) * 3 + y = y^2 - xy + y - (y - 3x) * 0.5";
            var expected = "x^2 - y^2 + 4.5xy + 10.5x + 0.5y + 30 = 0";
            var result = EquationParser.ToSimpleForm(sentance);

            Assert.AreEqual(expected, result);
        }
    }
}