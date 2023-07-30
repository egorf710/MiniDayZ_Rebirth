using UnityEngine;

namespace FletcherLibraries
{
    public static class StringExtensions
    {
        public const string EMPTY = "";
        public const string UNDERSCORE = "_";
        public const string SPACE = " ";
        public const string EXCLAMATION = "!";
        public const string PERIOD = ".";
        public const string SLASH = "/";
        public const string INFINITY = "∞";
        public const string NEWLINE = "\n";
        public const string AT = "@";
        public const char BAD_CHARACTER = '\u000b';
        public const char NEWLINE_CHARACTER = '\n';
        public const string JSON_SUFFIX = ".json";
        public const string DATA_SUFFIX = ".dat";

        public static bool IsEmpty(string s)
        {
            return s == null || s == EMPTY;
        }

        public static string RemoveAllInstancesOfCharacter(this string self, char c)
        {
            int index = self.IndexOf(c);
            string finalString = self;

            while (index >= 0)
            {
                finalString = finalString.Remove(index, 1);
                index = finalString.IndexOf(c);
            }

            return finalString;
        }

        public static string ROT(string value, int index)
        {
            char[] array = value.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                int number = (int)array[i];
                if (number >= 'a' && number <= 'z')
                {
                    number = rotHelper('a', number, index);
                }
                else if (number >= 'A' && number <= 'Z')
                {
                    number = rotHelper('A', number, index);
                }
                array[i] = (char)number;
            }
            return new string(array);
        }

        private static int rotHelper(int baseNum, int number, int index)
        {
            int normNum = number - baseNum;
            normNum += index;
            while (normNum > 25)
                normNum -= 26;
            while (normNum < 0)
                normNum += 26;
            return baseNum + normNum;
        }
    }
}
