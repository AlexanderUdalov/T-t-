using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace VersionBuilder.Numbering
{
    //TODO: сброс меньшего номера при обновлении большего
    public class VersionController
    {
        public string VersioningPattern;
        private List<NumberToken> _numbers;
        private List<EnumToken> _enums;
        private List<Token> _textTokens;

        private KeyValuePair<string, bool>[] _numberToggles;
        private KeyValuePair<string, bool>[][] _enumToggles;
        
        public VersionController(string versionPattern)
        {
            VersioningPattern = versionPattern;
            Initialize();
        }

        public void Initialize()
        {
            Token[] _tokens;
            PatternParser.Parse(VersioningPattern, out _tokens);

            _numbers = new List<NumberToken>(_tokens
                .Where(token => token.GetType() == typeof(NumberToken))
                .Select(numberToken => (NumberToken)numberToken));

            _enums = new List<EnumToken>(_tokens
                .Where(token => token.GetType() == typeof(EnumToken))
                .Select(enumToken => (EnumToken)enumToken));

            _textTokens = new List<Token>(_tokens
                .Where(token => token.GetType() == typeof(Token))
                .Select(numberToken => numberToken));


            _numberToggles = new KeyValuePair<string, bool>[_numbers.Count];
            for (int i = 0; i < _numbers.Count; i++)
                _numberToggles[i] = new KeyValuePair<string, bool>(_numbers[i].Text, false);

            _numberToggles[0] = new KeyValuePair<string, bool>(_numbers[0].Text, true);

            _enumToggles = new KeyValuePair<string, bool>[_enums.Count][];
            for (int i = 0; i < _enumToggles.Length; i++)
            {
                _enumToggles[i] = new KeyValuePair<string, bool>[_enums[i].Variants.Length];

                for (int j = 0; j < _enumToggles[i].Length; j++)
                    _enumToggles[i][j] = new KeyValuePair<string, bool>(_enums[i].Variants[j],
                        j == _enums[i].ValueIndex ? true : false);
            }
        }

        public void OnGUI()
        {
            EditorGUILayout.LabelField("Update:");
            RenderVariableToggles();
            RenderEnums();
        }

        private void RenderVariableToggles()
        {
            EditorGUILayoutExtensions.VerticalCheckBox(ref _numberToggles, 60 * _numberToggles.Length);
            EditorGUILayoutExtensions.Line();
        }

        private void RenderEnums()
        {
            for (int i = 0; i < _enumToggles.Length; i++)
            {
                GUILayout.Label(_enums[i].Text + ":");
                EditorGUILayoutExtensions.HorizontalCheckBox(ref _enumToggles[i], 60);
                EditorGUILayoutExtensions.Line();
            }
        }

        public string CalculateNextVersion()
        {
            int resetIndex = int.MaxValue;
            var numberIndex = Array.IndexOf(_numberToggles, _numberToggles.FirstOrDefault(x => x.Value));
            _numbers[numberIndex]++;


            for (int i = 0; i < _enumToggles.Length; i++)
            {
                var enumValue = Array.IndexOf(_enumToggles[i], _enumToggles[i].FirstOrDefault(x => x.Value));
                if (enumValue != _enums[i].ValueIndex)
                {
                    _enums[i].ValueIndex = enumValue;
                    resetIndex = _enums[i].Index;
                }
            }
            
            var tokens = new List<Token>();
            tokens.AddRange(_textTokens);
            tokens.AddRange(_numbers);
            tokens.AddRange(_enums);
            tokens = new List<Token>(tokens.OrderBy(x => x.Index));

            resetIndex = Math.Min(resetIndex, tokens.IndexOf(_numbers[numberIndex]));
            for (int i = resetIndex + 1; i < tokens.Count(); i++)
            {
                if (tokens[i].GetType() == typeof(NumberToken))
                {
                    var index = _numbers.IndexOf(tokens[i] as NumberToken);

                    NumberToken token = new NumberToken(tokens[i].Index, tokens[i].Text);
                    tokens[i] = token;
                    _numbers[index] = token;
                }
            }

            StringBuilder numberBuilder = new StringBuilder();
            StringBuilder patternBuilder = new StringBuilder();
            foreach (var s in tokens)
            {
                numberBuilder.Append(s);
                patternBuilder.Append(s.ToPatternString());
            }
            VersioningPattern = patternBuilder.ToString();
            
            return numberBuilder.ToString();
        }
    }
}