using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.SAMS.Utilities
{
    public class HebrewWordInverter
    {
        private static readonly char[] HebrewChars = "אבגדהוזחטיכלמנסעפצקרשתםןףךץ".ToArray();

        private bool ReverseWord(string word, out string result)
        {
            result = string.Empty;
            var letters = new Stack<char>();
            var hasHebrewLetters = false;
            foreach (var character in word)
            {
                if (HebrewChars.Contains(character))
                {
                    hasHebrewLetters = true;
                    letters.Push(character);
                }
                else
                {
                    while (letters.Count > 0)
                    {
                        result += letters.Pop();
                    }
                    result += character;
                }
            }

            while (letters.Count > 0)
            {
                result += letters.Pop();
            }

            return hasHebrewLetters;
        }

        private void AddWord(StringBuilder builder, string word)
        {
            builder.Append(word);
            builder.Append(" ");
        }

        public string Invert(string text)
        {
            var result = new StringBuilder();
            var words = text.Split(' ');
            var reversedWords = new Stack<string>();

            foreach (var word in words)
            {
                string reversedWord;
                ReverseWord(word, out reversedWord);

                if (ReverseWord(word, out reversedWord))
                {
                    reversedWords.Push(reversedWord);
                }
                else
                {
                    while (reversedWords.Count > 0)
                    {
                        AddWord(result, reversedWords.Pop());
                    }
                    AddWord(result, word);
                }

            }

            while (reversedWords.Count > 0)
            {
                AddWord(result, reversedWords.Pop());
            }

            return result.ToString().Trim();
        }
    }
}
