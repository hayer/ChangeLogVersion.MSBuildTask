using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;

namespace ChangeLogVersioningTask
{
    // ReSharper disable once UnusedMember.Global -- Exposed as MSBuild task
    public class ChangeLogVersioningTask : Microsoft.Build.Utilities.Task
    {
        public bool CrashOnFailure { get; set; }

        public string ChangeLogDirectory { get; set; }

        public string ChangeLogFileName { get; set; }

        [Output]
        public string VersionPrefix { get; set; }

        protected Regex VersionMatchRegex = new Regex("(?:\\[)(?<Version>\\d+\\.\\d+\\.\\d+)(?:\\])",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public override bool Execute()
        {
            if (string.IsNullOrWhiteSpace(ChangeLogFileName))
            {
                LogDependingOnParameter("Parameter ChangeLogFileName is not set");
                return !CrashOnFailure;
            }

            if (string.IsNullOrWhiteSpace(ChangeLogDirectory))
            {
                LogDependingOnParameter("Failed to get project directory");
                return !CrashOnFailure;
            }

            var changeLogFullPath = Path.Combine(ChangeLogDirectory, ChangeLogFileName);

            if (!File.Exists(changeLogFullPath))
            {
                LogDependingOnParameter($"Could not find changelog file at {changeLogFullPath}");
                return !CrashOnFailure;
            }

            string versionString = null;
            using (var changeLogFile = File.OpenRead(changeLogFullPath))
            {
                using (var changeLogStream = new StreamReader(changeLogFile))
                {
                    while (!changeLogStream.EndOfStream)
                    {
                        var currentLine = changeLogStream.ReadLine();
                        if (currentLine is null)
                            continue;

                        var match = VersionMatchRegex.Match(currentLine);
                        if (match.Success)
                        {
                            versionString = match.Groups["Version"].Value;
                            break;
                        }
                    }
                }
            }

            if (versionString is null)
            {
                LogDependingOnParameter($"Failed to find version string in {changeLogFullPath}");
                return !CrashOnFailure;
            }

            VersionPrefix = versionString;
            Log.LogMessage(MessageImportance.High, $"First version found in {changeLogFullPath} is {versionString}");

            return true;
        }

        private void LogDependingOnParameter(string message)
        {
            if (CrashOnFailure)
            {
                Log.LogError(message);
            }
            else
            {
                Log.LogMessage(MessageImportance.High, message);
            }
        }
    }
}
