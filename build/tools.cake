using System.IO;
using System.Text.RegularExpressions;

var help = Argument("h", "null");

void init() {
    if (help != "null") {
        Information("Rerun with --help to get cake help");
        Environment.Exit(-1);
    }
}
bool IsValidReleaseNumber(string releaseNumber) {
    return Regex.IsMatch(releaseNumber, @"[0-9]+\.[0-9]+");
}

void writeReleaseFile(string releaseDirectory, string releaseNumber) {
    if (System.IO.File.Exists($"{releaseDirectory}/version")) {
        Debug($"version file already exists at {releaseDirectory}/version, deleting");
        System.IO.File.Delete($"{releaseDirectory}/version");
    }
    System.IO.File.WriteAllText($"{releaseDirectory}/version", $"version={releaseNumber}" + Environment.NewLine);
}