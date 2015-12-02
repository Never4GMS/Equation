using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Equation
{
    class Program
    {
        private static Regex Word = new Regex(@"\w+", RegexOptions.Compiled);
        private const string help =
@"Справка по работе с программой:
EPConsole [ОПЦИИ] [файл]

Опции:
   /file=путь_к_файлу - путь к файлу для обработки
   /help              - вывод этой справки

Интерактивный режим:
    >x^2 + 3.5xy + y = y^2 - xy + y[Enter]
    ==>x^2 - y^2 + 4.5xy = 0
    >[ожидание пользовательского ввода]
";
        private const string Invite = ">";
        private const string Output = "==>{0}";


        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                InteractiveMode();
                return;
            }

            var terms = ParseArgs(args);
            if (terms.ContainsKey("help"))
            {
                Console.WriteLine(help);
                Console.ReadKey(true);
                return;
            }

            string filePath = null;
            if (terms.ContainsKey("file"))
            {
                filePath = terms["file"];
            }
            else
            {
                filePath = args[0];
            }
            
            if (!File.Exists(filePath))
            {
                DisplayError("Указанный файл не существует");                
                Console.ReadKey(true);
                return;
            }

            var outputPath = filePath + ".out";
            File.WriteAllLines(outputPath, ParseEquations(File.ReadLines(filePath)));

            DisplayOk(string.Format("Файл успешно обработан. Результат сохранен в файл {0}", outputPath));
            Console.ReadKey(true);
        }

        private static IEnumerable<string> ParseEquations(IEnumerable<string> equations)
        {
            return equations.Select(TryParseEquation);
        }

        private static string TryParseEquation(string input)
        {
            try
            {
                return EquationParser.ToSimpleForm(input);
            }
            catch (Exception ex)
            {
                return string.Format("[ERROR]{0}[ExMsg: {1}] ", input, ex.Message);
            }
        }

        private static void DisplayError(string message)
        {           
            Display(message, ConsoleColor.Red);
        }

        private static void InteractiveMode()
        {
            do
            {
                Console.Write(Invite);
                var input = Console.ReadLine();
                var output = TryParseEquation(input);
                if (output.Contains("ERROR"))
                {
                    DisplayError(string.Format(Output, output));
                }
                else
                {
                    DisplayOk(string.Format(Output, output));
                }                
            }
            while (true);
        }

        private static void DisplayOk(string message)
        {            
            Display(message, ConsoleColor.DarkGreen);
        }

        private static void Display(string message, ConsoleColor newColor)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = newColor;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }

        private static Dictionary<string, string> ParseArgs(string[] args)
        {
            var result = new Dictionary<string, string>();

            foreach (var arg in args)
            {
                var terms = arg.Split('=');
                var value = string.Empty;

                if (string.IsNullOrWhiteSpace(terms[0])) continue;

                if (terms.Length > 1) value = terms[1];

                result.Add(Word.Match(terms[0]).Value, value);
            }

            return result;
        }
    }
}
