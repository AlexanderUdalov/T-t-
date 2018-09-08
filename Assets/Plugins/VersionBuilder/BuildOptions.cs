using UnityEditor;

namespace VersionBuilder
{
    internal class BuildOptions
    {
        public bool DeletePreviousBuilds = false;
        public bool RunAfterBuild = false;
        public BuildTarget TargetBuild = BuildTarget.StandaloneWindows64;
        public string BuildPath;
        public string VersionNumber;
    }
}
