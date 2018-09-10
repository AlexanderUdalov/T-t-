using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace VersionBuilder
{
    //TODO: придумать, что сделать с расширениями файла.
    internal class Builder
    {
        private BuildOptions _options;
        private readonly Dictionary<BuildTarget, string> _fileExtension = new Dictionary<BuildTarget, string>()
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

        public Builder(BuildOptions options)
        {
            _options = options;
        }

        public void Build()
        {
            Build(_options);
        }

        public void Build(BuildOptions options)
        {
            if (string.IsNullOrEmpty(options.BuildPath)) throw new ArgumentException("Build path cannot be empty");
            if (options.DeletePreviousBuilds) DeleteBuilds(options.BuildPath);
            
            var fileName = PlayerSettings.productName + "_" + options.VersionNumber;
            var path = Path.Combine(options.BuildPath, options.TargetBuild.ToString(),
                    fileName, fileName + _fileExtension[options.TargetBuild]);

            BuildPipeline.BuildPlayer(
                    EditorBuildSettings.scenes, path, options.TargetBuild,
                    options.RunAfterBuild ?
                        UnityEditor.BuildOptions.AutoRunPlayer :
                        UnityEditor.BuildOptions.ShowBuiltPlayer);
        }
        
        private void DeleteBuilds(string path)
        {
            Directory.Delete(path, true);
            Directory.CreateDirectory(path);
        }
    }
}
