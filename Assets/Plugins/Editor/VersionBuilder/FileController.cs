using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace VersionBuilder
{
    internal class FileController
    {
        public string ProjectSettingsPath;

        public string ProductName;
        public string CompanyName;
        public DateTime Updated;
        public string VersioningPattern;

        public VersionInfo[] Versions;
        public VersionInfo NewVersion;

        public FileController()
        {
            ProjectSettingsPath = Application.dataPath
                .Substring(0, Application.dataPath.LastIndexOf("/")) + "/ProjectSettings/"; ;
            ProductName = Application.productName;
            CompanyName = Application.companyName;
        }

        public bool TryLoad()
        {
            try
            {
                using (FileStream fstream = File.OpenRead(ProjectSettingsPath + "VersionHistory.txt"))
                {
                    byte[] array = new byte[fstream.Length];
                    fstream.Read(array, 0, array.Length);
                    var versionHistory = Encoding.Unicode.GetString(array).Split(new[] { '\n' });
                    
                    if (versionHistory.Length < 7) return false;

                    ProductName = versionHistory[0];
                    CompanyName = versionHistory[2];
                    Updated = DateTime.Parse(versionHistory[3].Substring(versionHistory[3].IndexOf(':') + 2));
                    VersioningPattern = versionHistory[4].Substring(versionHistory[4].IndexOf(':') + 2);

                    var versions = new List<VersionInfo>();
                    for (int i = 6; i < versionHistory.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(versionHistory[i - 1])) continue;
                        if (string.IsNullOrWhiteSpace(versionHistory[i])) continue;

                        var separatorIndex = versionHistory[i].LastIndexOf(" - ");
                        var versionNumber = versionHistory[i].Substring(0, separatorIndex);
                        var versionComment = versionHistory[i].Substring(
                                separatorIndex + 3, versionHistory[i].Length - (separatorIndex + 3));

                        while (i + 1 < versionHistory.Length && !string.IsNullOrWhiteSpace(versionHistory[i + 1]))
                        {
                            i++;
                            versionComment += "\n" + versionHistory[i];
                        }

                        versions.Add(new VersionInfo(versionNumber, versionComment));
                    }
                    Versions = versions.ToArray();
                    NewVersion = new VersionInfo("default", Versions[Versions.Length - 1].Comment);
                }
                return true;
            }
            catch (FileNotFoundException) {
                return false;
            }
        }

        public void Save()
        {
            Updated = DateTime.Now;
            
            StringBuilder sb = new StringBuilder();
            using (FileStream fstream = new FileStream(ProjectSettingsPath + "VersionHistory.txt", FileMode.Create))
            {
                fstream.Flush();

                sb.AppendLine(ProductName);
                sb.AppendLine("by");
                sb.AppendLine(CompanyName);
                sb.AppendLine("Updated: " + Updated.ToString("u"));
                sb.AppendLine("VersioningPattern: " + VersioningPattern);
                sb.AppendLine();

                foreach (var version in Versions)
                {
                    version.Comment.Replace(" - ", " -- ");
                    sb.AppendLine(version.Number + " - " + version.Comment);
                    sb.AppendLine();
                }
                NewVersion.Comment.Replace(" - ", " -- ");
                sb.AppendLine(NewVersion.Number + " - " + NewVersion.Comment);


                var result = sb.ToString();
                byte[] array = Encoding.Unicode.GetBytes(result);
                fstream.Write(array, 0, array.Length);
            }
        }
    }
}