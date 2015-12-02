using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Equation
{
    using CultureInfo = System.Globalization.CultureInfo;

    public static class EquationParser
    {
        public static List<string> GetSentenceList(string statement)
        {
            return statement.Split(' ').ToList();
        }

        public static List<string> GroupByRight(this List<string> sentances)
        {
            var groupedVariables = new List<string>();
            var rightSideOfEquation = string.Join(" ", sentances.TakeWhile(s => Resources.Equality != s));
            return Resources.Variable
                .Matches(rightSideOfEquation)
                .AsEnumerable<Match>()
                .Select(Variable.Create)
                .GroupBy(GroupedVariable.Create, GroupedVariable.EqualityComparer)
                .OrderByDescending(g => g.Key.Pow)
                .ThenByDescending(g => g.Key.Var.Length)
                .ThenBy(g => string.IsNullOrWhiteSpace(g.Key.Var) ? Resources.LastAlpha : g.Key.Var)
                .SelectMany(AggregateVariables)
                .Concat(sentances.SkipWhile(s => Resources.Equality != s))
                .Skip(1)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        public static List<string> MoveRight(string statement)
        {
            return MoveRight(GetSentenceList(statement));
        }

        public static List<string> MoveRight(this List<string> sentences)
        {
            var insertIndex = sentences.IndexOf(Resources.Equality);
            sentences[insertIndex] = ChangeSign(sentences[insertIndex + 1]) ?? Resources.Minus;

            for (var i = insertIndex + 1; i < sentences.Count; i++)
            {
                if (Resources.Sign.Contains(sentences[i]))
                {
                    sentences[i] = ChangeSign(sentences[i]);
                }
            }

            sentences.Add(Resources.Equality);
            sentences.Add(Resources.Zero);

            return sentences;
        }

        public static string OpenParenteces(string sentances)
        {
            return Resources.Parenteces.IsMatch(sentances) ?
                Resources.Parenteces.Replace(sentances, ParentecesEvaluator) :
                sentances;
        }

        private static IEnumerable<string> AggregateVariables(IGrouping<GroupedVariable, Variable> group)
        {
            var coef = group.Sum(s => ParseDouble(
                string.Concat(s.Sign, string.IsNullOrEmpty(s.Coef) ? "1" : s.Coef)));

            return coef == 0 ?
                new string[] { } :
                new Variable
                {
                    Sign = coef < 0 ? Resources.Minus : Resources.Plus,
                    Coef = Math.Abs(coef) == 1 ? string.Empty : Math.Abs(coef).ToString(CultureInfo.InvariantCulture),
                    Var = group.Key.Var,
                    Pow = group.Key.Pow
                }.AsEnum();
        }

        private static string ChangeSign(string sentence)
        {
            if (Resources.Sign.Contains(sentence))
            {
                return Resources.Plus == sentence ? Resources.Minus : Resources.Plus;
            }

            return null;
        }

        private static string ParentecesEvaluator(Match match)
        {
            var factor = match.Groups["factor"].Captures
                .AsEnumerable<Capture>().Aggregate(1d, (acc, cur) => acc * ParseDouble(cur.Value));
            var content = match.Groups["content"].Value;
            var sign = match.Groups["sign"].Value;
            var changeSign = sign == Resources.Minus;

            if (factor == 1d && !changeSign)
            {
                return match.Value;
            }

            var modifiedContent = new System.Text.StringBuilder();
            if (sign == Resources.Plus)
            {
                modifiedContent.Append(sign);
            }
                 
            modifiedContent.Append(string.Join(
                " ",
                Resources.Variable
                    .Matches(content)
                    .AsEnumerable<Match>()
                    .Select(Variable.Create)
                    .Select(v => v.Multiply(factor, changeSign).ToString())));

            return modifiedContent.ToString().Trim();
        }

        private static double ParseDouble(string value)
        {
            double digit;
            return double.TryParse(
                value,
                System.Globalization.NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out digit) ? digit : 0;
        }

        internal class GroupedVariable
        {
            private static IEqualityComparer<GroupedVariable> equality;

            public static IEqualityComparer<GroupedVariable> EqualityComparer
            {
                get
                {
                    if (equality == null)
                    {
                        equality = new GroupedVariableEqualityComparer();
                    }
                    return equality;
                }
            }

            public string Pow { get; set; }

            public string Var { get; set; }

            public static GroupedVariable Create(Variable v)
            {
                return new GroupedVariable { Var = v.Var, Pow = v.Pow };
            }

            internal class GroupedVariableEqualityComparer : IEqualityComparer<GroupedVariable>
            {
                public bool Equals(GroupedVariable x, GroupedVariable y)
                {
                    return x.Pow == y.Pow && x.Var == y.Var;
                }

                public int GetHashCode(GroupedVariable obj)
                {
                    return obj.Var.GetHashCode() ^ obj.Pow.GetHashCode();
                }
            }
        }

        internal class Variable
        {
            public Variable()
            {
            }

            public string Coef { get; set; }

            public string Pow { get; set; }

            public string Sign { get; set; }

            public string Var { get; set; }

            public static Variable Create(Match match)
            {
                return new Variable
                {
                    Sign = match.Groups["sign"].Value,
                    Coef = match.Groups["coef"].Value,
                    Var = match.Groups["var"].Value,
                    Pow = match.Groups["pow"].Value
                };
            }

            public IEnumerable<string> AsEnum()
            {
                return ToString().Split(' ');
            }

            public Variable Multiply(double factor, bool changeSign)
            {
                if (changeSign)
                {
                    Sign = Sign == Resources.Minus ? Resources.Plus : Resources.Minus;
                }

                Coef = string.IsNullOrWhiteSpace(Coef) ?
                    factor.ToString(CultureInfo.InvariantCulture) :
                    (double.Parse(Coef) * factor).ToString(CultureInfo.InvariantCulture);

                return this;
            }

            public override string ToString()
            {
                return string.Concat(Sign, " ", Coef, Var, Pow);
            }
        }

        public static string ToSimpleForm(string input)
        {
            if (Resources.Parenteces.IsMatch(input))
            {
                try
                {
                    input = OpenParenteces(input);
                }
                catch
                {
                    throw new ArgumentException("Не удалось расскрыть скобки");
                }
            }

            return string
                .Join(" ", GetSentenceList(input).MoveRight().GroupByRight())
                .Trim();
        }
    }
}