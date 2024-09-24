using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SunFunctions
{
    public partial class Functions
    {
        private string Translate8bit(string text)
        {
            string[] binaryValues = text.Split(' ');
            char[] ASCIICharacteres = new char[binaryValues.Length];
            for (int i = 0; i < binaryValues.Length; i++)
                ASCIICharacteres[i] = (char)Convert.ToInt32(binaryValues[i], 2);
            
            return new string(ASCIICharacteres);
        }

        public (string, List<string>) Get8bitPart(string content)
        {
            string binaryExp = @"(?:\b[01]{8}\b(?:\s\b[01]{8}\b)*)";
            MatchCollection binaryOccurrences = Regex.Matches(content, binaryExp);
            List<string> translations = new List<string>();
            if (binaryOccurrences.Count > 0)
            {
                for (int i = 0; i < binaryOccurrences.Count; i++)
                {
                    string binaryCode = binaryOccurrences[i].Value;
                    translations.Add(Translate8bit(binaryCode));
                }
            }
            string final = Regex.Replace(content, binaryExp, match => $"**{Translate8bit(match.Value)}**");
            return (final, translations);
        }
    }
}