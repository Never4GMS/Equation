using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Equation
{
    internal static class Resources
    {
        public static Regex Variable = new Regex(
           @"(?<sign>[+-])?\s?((?<coef>\d+(?:.\d+)?)?(?<var>[a-z]+)(?<pow>\^\d+)?|(?<coef>\d+(?:.\d+)?)(\s|$))",
           RegexOptions.Compiled);

        public static Regex Parenteces = new Regex(
            @"(?<sign>[+-])?\s?((?<factor>\d+(?:.\d+)?)\s?\*)?\s?\((?<content>(?<=\()[^)(]+(?=\)))\)\s?(?:\*\s?(?<factor>\d+(?:.\d+)?))?",
            RegexOptions.Compiled);

        public static IList<string> Sign = new List<string>(2) { "+", "-" };

        public const string Minus = "-";

        public const string Plus = "+";

        public const string Equality = "=";

        public const string Zero = "0";

        public const string LastAlpha = "z";
    }
}
