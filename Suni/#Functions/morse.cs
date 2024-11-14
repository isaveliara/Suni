using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sun.Functions
{
    public partial class Functions
    {
        private static readonly Dictionary<string, char> morseCodeDictionary = new Dictionary<string, char>()
        {
            { ".-", 'A' }, { "-...", 'B' }, { "-.-.", 'C' }, { "-..", 'D' }, { ".", 'E' }, 
            { "..-.", 'F' }, { "--.", 'G' }, { "....", 'H' }, { "..", 'I' }, { ".---", 'J' }, 
            { "-.-", 'K' }, { ".-..", 'L' }, { "--", 'M' }, { "-.", 'N' }, { "---", 'O' }, 
            { ".--.", 'P' }, { "--.-", 'Q' }, { ".-.", 'R' }, { "...", 'S' }, { "-", 'T' }, 
            { "..-", 'U' }, { "...-", 'V' }, { ".--", 'W' }, { "-..-", 'X' }, { "-.--", 'Y' }, 
            { "--..", 'Z' }, { "-----", '0' }, { ".----", '1' }, { "..---", '2' }, { "...--", '3' }, 
            { "....-", '4' }, { ".....", '5' }, { "-....", '6' }, { "--...", '7' }, { "---..", '8' }, 
            { "----.", '9' }, { ".-.-.-", '.' }, { "--..--", ',' }, { "..--..", '?' }, { "-.-.--", '!' }, 
            { "-.--.", '(' }, { "-.--.-", ')' }, { ".----.", '\'' }, { "-..-.", '/' }, { "-.-.-.", ';' }, 
            { "-...-", '=' }, { ".-.-.", '+' }, { "-....-", '-' }, { ".-..-.", '"' }, { ".--.-.", '@' }
        };

        private string TranslateMorse(string text)
        {
            string[] morseCharacters = text.Split(' ');
            char[] translatedCharacters = new char[morseCharacters.Length];

            for (int i = 0; i < morseCharacters.Length; i++)
            {
                if (morseCodeDictionary.TryGetValue(morseCharacters[i], out char translatedChar))
                    translatedCharacters[i] = translatedChar;
                else
                    translatedCharacters[i] = '?'; //unknown char
            }

            return new string(translatedCharacters);
        }

        public (string, List<string>) GetMorsePart(string content)
        {
            string morseExp = @"(?:[.-]{1,6}(?:\s[.-]{1,6})*)";

            MatchCollection morseOccurrences = Regex.Matches(content, morseExp);
            List<string> translations = new List<string>();

            if (morseOccurrences.Count > 0)
            {
                for (int i = 0; i < morseOccurrences.Count; i++)
                {
                    string morseCode = morseOccurrences[i].Value;
                    translations.Add(TranslateMorse(morseCode));
                }
            }

            string final = Regex.Replace(content, morseExp, match => $"**{TranslateMorse(match.Value)}**");
            return (final, translations);
        }
    }
}
