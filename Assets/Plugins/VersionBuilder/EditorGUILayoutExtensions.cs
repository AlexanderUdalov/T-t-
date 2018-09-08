using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace VersionBuilder
{
    public static class EditorGUILayoutExtensions
    {
        public static void Line(int height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        public static void HorizontalCheckBox(ref KeyValuePair<string, bool>[] toggles, int labelWidth)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            VerticalCheckBox(ref toggles, labelWidth);
            EditorGUILayout.EndHorizontal();
        }

        public static void VerticalCheckBox(ref KeyValuePair<string, bool>[] toggles, int labelWidth)
        {
            var trueIndex = toggles.ToList().IndexOf(toggles.FirstOrDefault(x => x.Value));

            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < toggles.Length; i++)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(10 + labelWidth));

                toggles[i] = new KeyValuePair<string, bool>(toggles[i].Key,
                    EditorGUILayout.Toggle(toggles[i].Value, GUILayout.Width(10)));
                EditorGUILayout.LabelField(toggles[i].Key, GUILayout.Width(labelWidth));

                EditorGUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (toggles.Where(x => x.Value).Count() == 2)
                    toggles[trueIndex] = new KeyValuePair<string, bool>(toggles[trueIndex].Key, false);
                else if (toggles.Where(x => x.Value).Count() == 0)
                    toggles[trueIndex] = new KeyValuePair<string, bool>(toggles[trueIndex].Key, true);
            }
        }
    }
}
