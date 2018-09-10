using System;
using System.Collections.Generic;

namespace VersionBuilder.Numbering
{
    internal class PatternParser
    {
        public static void Parse(string pattern, out Token[] tokens)
        {
            if (string.IsNullOrEmpty(pattern)) throw new ArgumentException();

            var result = new List<Token>();

            int tokenIndex = 0;

            var tokensWithBracket = pattern.Split(new [] { '[' });

            if (tokensWithBracket.Length == 1) throw new ArgumentException("Pattern does not contains variables!");

            if (tokensWithBracket[0].Length != 0)
                result.Add(new Token(tokenIndex++, tokensWithBracket[0]));

            for (int i = 1; i < tokensWithBracket.Length; i++)
            {
                var stringTokens = tokensWithBracket[i].Split(new [] { ']' });
                
                var bracketIndex = stringTokens[0].IndexOf('{');
                if (bracketIndex != -1)
                {
                    var enumString = stringTokens[0].Substring(bracketIndex + 1);
                    enumString = enumString.Substring(0, enumString.IndexOf('}'));
                    var enumVariants = enumString.Split(new[] { ',' });

                    int valueIndex = 0;
                    for (int j = 0; j < enumVariants.Length; j++)
                    {
                        if (enumVariants[j][enumVariants[j].Length - 1] == '!')
                        {
                            valueIndex = j;
                            enumVariants[j] = enumVariants[j].Substring(0, enumVariants[j].Length - 1);
                            break;
                        }
                    }
                    result.Add(new EnumToken(tokenIndex++, stringTokens[0].Substring(0, bracketIndex), enumVariants, valueIndex));
                }
                else if (stringTokens[0].Contains("="))
                {
                    var equalIndex = stringTokens[0].IndexOf("=");
                    result.Add(new NumberToken(tokenIndex++, stringTokens[0].Substring(0, equalIndex),
                        stringTokens[0].Substring(equalIndex + 1, stringTokens[0].Length - equalIndex - 1)));
                }
                else result.Add(new NumberToken(tokenIndex++, stringTokens[0]));

                if (stringTokens.Length == 2)
                    if (!string.IsNullOrWhiteSpace(stringTokens[1]))
                        result.Add(new Token(tokenIndex++, stringTokens[1]));
            }

            tokens = result.ToArray();
        }
    }
}