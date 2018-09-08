using UnityEditor;
using UnityEngine;
using VersionBuilder.Numbering;

namespace VersionBuilder
{
    public class BuildWindow : EditorWindow
    {
        private BuildOptions _options;
        private VersionController _versionController;
        private FileController _fileController;

        [MenuItem("File/Version Builder &b")]
        public static void ShowWindow()
        {
            var window = (BuildWindow)GetWindow(typeof(BuildWindow), true, "Build Project");
            window.minSize = new Vector2(320, 400);
            window.Init();
            window.Show();
        }

        private void Init()
        {
            _options = new BuildOptions();
            _fileController = new FileController();
            _fileController.TryLoad();
            _versionController = new VersionController(_fileController.VersioningPattern);
        }

        private void OnProjectChange()
        {
            Init();
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Path:", GUILayout.Width(35));

                EditorGUI.BeginChangeCheck();
                _options.BuildPath = GUILayout.TextField(EditorPrefs.GetString("Builds path", _options.BuildPath));
                if (GUILayout.Button("Browse...")) _options.BuildPath = OpenFolderWindow();
                if (EditorGUI.EndChangeCheck())
                    EditorPrefs.SetString("Builds path", _options.BuildPath);
            }
            GUILayout.EndHorizontal();
            
            EditorGUILayout.LabelField("Last version: " + _fileController.Versions[_fileController.Versions.Length - 1].Number);
            EditorGUILayout.LabelField("Updated: " + _fileController.Updated.ToString("u"));

            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Platform:", GUILayout.Width(60));
                _options.TargetBuild = (BuildTarget)EditorGUILayout.EnumPopup(_options.TargetBuild);
            }
            GUILayout.EndHorizontal();
            
            EditorGUILayoutExtensions.Line();

            _versionController.OnGUI();

            EditorGUILayout.LabelField("Comment:");
            _fileController.NewVersion.Comment = EditorGUILayout.TextArea(
                _fileController.NewVersion.Comment, GUILayout.Height(60));

            EditorGUILayoutExtensions.Line();

            GUILayout.BeginHorizontal();
            {
                _options.DeletePreviousBuilds = 
                    GUILayout.Toggle(_options.DeletePreviousBuilds, "Delete previous builds");

                _options.RunAfterBuild = 
                    GUILayout.Toggle(_options.RunAfterBuild, "Run after building");
            }
            GUILayout.EndHorizontal();

            if (_options.DeletePreviousBuilds)
                EditorGUILayout.HelpBox("All files at " + _options.BuildPath + " will be deleted!", MessageType.Warning);

            GUILayout.Space(20);
            
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Player Settings")) OpenPlayerSettings();
                if (GUILayout.Button("Build Settings")) OpenBuildSettings();
                if (GUILayout.Button("Build"))
                {
                    _options.VersionNumber = _versionController.CalculateNextVersion();
                    _fileController.VersioningPattern = _versionController.VersioningPattern;
                    _fileController.NewVersion.Number = _options.VersionNumber;
                    _fileController.Save();
                    EditorApplication.delayCall += new Builder(_options).Build;
                    Close();
                }
            }
            GUILayout.EndHorizontal();
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
            return EditorUtility.OpenFolderPanel("Choose builds folder", _options.BuildPath, "Builds");
        }
    }

}
