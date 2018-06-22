using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

//tmp test potom peredelayoo todo
public class CustomProjectBuild : EditorWindow {
    private bool _deletePreviousBuilds = false;
    private bool _runAfterBuild = false;
    private BuildTarget _targetBuild = BuildTarget.StandaloneWindows64;

    public Dictionary<BuildTarget, string> FileExtension = new Dictionary<BuildTarget, string>()
    {
        { BuildTarget.StandaloneWindows64, ".exe"},
        { BuildTarget.StandaloneWindows, ".exe"},
        { BuildTarget.StandaloneOSX, ".app"},
        { BuildTarget.StandaloneLinux, ""},
        { BuildTarget.StandaloneLinux64, ""},
        { BuildTarget.StandaloneLinuxUniversal, ""},
        { BuildTarget.Android, ".apk"},
        { BuildTarget.WebGL, ""}
    };

    private KeyValuePair<string, bool>[] _testingNameToggles = new KeyValuePair<string, bool>[]
    {
        new KeyValuePair<string, bool>("alpha", true),
        new KeyValuePair<string, bool>("beta", false),
        new KeyValuePair<string, bool>("rc", false),
        new KeyValuePair<string, bool>("release", false)
    };
    private KeyValuePair<string, bool>[] _changesToggles = new KeyValuePair<string, bool>[]
    {
        new KeyValuePair<string, bool>("Bags or negligible update", true),
        new KeyValuePair<string, bool>("Minor update", false),
        new KeyValuePair<string, bool>("Major update", false),
        new KeyValuePair<string, bool>("Re-build", false)
    };
    public static string VersionNumber;
    public const string VersionPattern = @"^\d?[.]{1}\d+[a-z]{1,2}\d+";

    public static string BuildsPath;
    public static string ProjectSettingsPath;

    [MenuItem("File/Build Project &b")]
    public static void ShowWindow()
    {
        var window = (CustomProjectBuild) GetWindow(typeof(CustomProjectBuild), true, "Build Project");
        window.minSize = new Vector2(350, 280);
        window.Show();
        BuildsPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
        ProjectSettingsPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/ProjectSettings/";


        TryLoadSettings();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Path:", GUILayout.Width(35));
            
            EditorGUI.BeginChangeCheck();
            GUILayout.TextField(BuildsPath);
            if (GUILayout.Button("Browse...")) BuildsPath = OpenFolderWindow();
            if (EditorGUI.EndChangeCheck())
            {
                SaveSettings();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Current version:", GUILayout.Width(100));
            VersionNumber = GUILayout.TextField(VersionNumber);
        }
        GUILayout.EndHorizontal();

        if (!TryParseVersion())
            EditorGUILayout.HelpBox("Unavailable version format!", MessageType.Info);

        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Platform:", GUILayout.Width(60));
            _targetBuild = (BuildTarget)EditorGUILayout.EnumPopup(_targetBuild);
        }
        GUILayout.EndHorizontal();
        

        EditorGUILayoutExtensions.Line();
        EditorGUILayoutExtensions.HorizontalCheckBox(ref _testingNameToggles, 60);
        EditorGUILayoutExtensions.Line();
        EditorGUILayoutExtensions.VerticalCheckBox(ref _changesToggles, 250);
        EditorGUILayoutExtensions.Line();


        GUILayout.BeginHorizontal();
        {
            _deletePreviousBuilds = GUILayout.Toggle(_deletePreviousBuilds, "Delete previous builds");
            _runAfterBuild = GUILayout.Toggle(_runAfterBuild, "Run after building");
        }
        GUILayout.EndHorizontal();

        if (_deletePreviousBuilds)
            EditorGUILayout.HelpBox("All files at " + BuildsPath + " will be deleted!", MessageType.Warning);

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Player Settings")) OpenPlayerSettings();
            if (GUILayout.Button("Build Settings")) OpenBuildSettings();
            if (GUILayout.Button("Build")) EditorApplication.delayCall += Build;
        }
        GUILayout.EndHorizontal();
    }

    private void Build()
    {
        if (_deletePreviousBuilds) DeletePreviousBuilds();

        var versionNumberFolder = CalculateNextVersionNumber();
        if (_runAfterBuild)
            BuildPipeline.BuildPlayer(
            EditorBuildSettings.scenes,
                BuildsPath + "/" +
                EditorUserBuildSettings.activeBuildTarget.ToString() + "/" +
                PlayerSettings.productName + "_" + versionNumberFolder +
                "/" + PlayerSettings.productName + "_" + versionNumberFolder +
                FileExtension[_targetBuild],
            _targetBuild,
            BuildOptions.AutoRunPlayer
            );
        else
            BuildPipeline.BuildPlayer(
            EditorBuildSettings.scenes,
                BuildsPath + "/" +
                EditorUserBuildSettings.activeBuildTarget.ToString() + "/" +
                PlayerSettings.productName + "_" + versionNumberFolder +
                "/" + PlayerSettings.productName + "_" + versionNumberFolder +
                FileExtension[_targetBuild],
            _targetBuild,
            BuildOptions.ShowBuiltPlayer
            );
        Close();
    }

    private string CalculateNextVersionNumber()
    {
        int majorVersion, minorVersion, bugfixVersion;
        string testingVersion;

        if (!TryParseVersion()) return DefaultVersion();
        else
        {
            majorVersion = int.Parse(VersionNumber.Substring(0, VersionNumber.IndexOf(".")));

            string minorVersionString = VersionNumber.Substring(VersionNumber.IndexOf(".") + 1);
            
            int i = 0;
            while (i < minorVersionString.Length && Char.IsDigit(minorVersionString[i]))
                i++;

            minorVersion = int.Parse(minorVersionString.Substring(0, i));

            string testVersionString = minorVersionString.Substring(i);

            i = 0;
            while (!Char.IsDigit(testVersionString[i]))
                i++;

            testingVersion = testVersionString.Substring(0, i);

            string bugfixVersionString = testVersionString.Substring(i);

            bugfixVersion = int.Parse(bugfixVersionString);
        }

        string testingToggle = _testingNameToggles.First(x => x.Value).Key;
        if (!(testingToggle.Length == 2))
            testingToggle = testingToggle.Substring(0, 1);

        if (testingToggle != testingVersion)
        {
            testingVersion = testingToggle;
            bugfixVersion = 1;

            if (_changesToggles[0].Value)
                bugfixVersion = 0;
        }

        if (_changesToggles[0].Value)
        {
            bugfixVersion++;
            if (minorVersion / 10 == 0)
                VersionNumber = majorVersion + ".0" + minorVersion + testingVersion + bugfixVersion;
            else
                VersionNumber = majorVersion + "." + minorVersion + testingVersion + bugfixVersion;
            SaveSettings();
            return VersionNumber;
        }
        else if (_changesToggles[1].Value)
        {
            bugfixVersion = 1;
            minorVersion++;
            if (minorVersion / 10 == 0)
                VersionNumber = majorVersion + ".0" + minorVersion + testingVersion + bugfixVersion;
            else
                VersionNumber = majorVersion + "." + minorVersion + testingVersion + bugfixVersion;

            SaveSettings();
            return VersionNumber;
        }
        else if (_changesToggles[2].Value)
        {
            bugfixVersion = 1;
            minorVersion = 0;
            majorVersion++;

            if (minorVersion / 10 == 0)
                VersionNumber = majorVersion + ".0" + minorVersion + testingVersion + bugfixVersion;
            else
                VersionNumber = majorVersion + "." + minorVersion + testingVersion + bugfixVersion;
            SaveSettings();
            return VersionNumber;
        }
        else if (_changesToggles[3].Value)
            return VersionNumber;
        else throw new Exception();
    }

    private bool TryParseVersion()
    {
        if (string.IsNullOrEmpty(VersionNumber))
            return false;
        return Regex.IsMatch(VersionNumber, VersionPattern);
    }

    private string DefaultVersion()
    {
        if (_testingNameToggles[0].Value)
            return "0.01a1";
        if (_testingNameToggles[1].Value)
            return "0.01b1";
        if (_testingNameToggles[2].Value)
            return "0.01r1";
        if (_testingNameToggles[3].Value)
            return "0.01rc1";
        return "";
    }

    private void DeletePreviousBuilds()
    {
        Directory.Delete(BuildsPath, true);
        Directory.CreateDirectory(BuildsPath);
    }

    private void OpenBuildSettings()
    {
        EditorApplication.ExecuteMenuItem("File/Build Settings...");
    }

    private void OpenPlayerSettings()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
    }

    private string OpenFolderWindow()
    {
        return EditorUtility.OpenFolderPanel("Choose builds folder", BuildsPath, "Builds");
    }

    private static void TryLoadSettings()
    {
        BuildsPath = EditorPrefs.GetString("Builds path");

        try
        {
            using (FileStream fstream = File.OpenRead(ProjectSettingsPath + "VersionNumber.txt"))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                VersionNumber = System.Text.Encoding.Default.GetString(array) ?? "0.1a1";
            }
        }
        catch (FileNotFoundException) { }
    }

    private static void SaveSettings()
    {
        EditorPrefs.SetString("Builds path", BuildsPath);

        using (FileStream fstream = new FileStream(ProjectSettingsPath + "VersionNumber.txt", FileMode.OpenOrCreate))
        {
            fstream.Flush();
            byte[] array = System.Text.Encoding.Default.GetBytes(VersionNumber);
            fstream.Write(array, 0, array.Length);
        }
    }
}

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
        var trueIndex = toggles.ToList().IndexOf(toggles.First(x => x.Value));

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
