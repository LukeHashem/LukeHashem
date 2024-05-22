using System;
namespace MorseCodeTranslator
{
    public static class Utilities
    {
        public static string GetString(string text)
        {
            //New string template
            string newString = "";

            Console.Write(text);
            newString = Console.ReadLine() ?? "";

            return newString;
        }

        public static bool BoolChoice(string text)
        {
            bool bChoice = false;
            string sChoice = "";

            Console.WriteLine(text);
            Console.Write("Option: ");

            sChoice = (Console.ReadLine() ?? "").ToUpper();

            while (sChoice != "Y" && sChoice != "N")
            {
                Console.WriteLine("Error please either input Y or N to choose.");
                Console.WriteLine("");

                Console.WriteLine(text);
                Console.Write("Option: ");

                sChoice = (Console.ReadLine() ?? "").ToUpper();
            }

            if (sChoice == "Y")
            {
                bChoice = true;
            }

            return bChoice;
        }
    }
}
