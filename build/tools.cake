var help = Argument("h", "null");

void init() {
    if (help != "null") {
        Information("Rerun with --help to get cake help");
        Environment.Exit(-1);
    }
}
bool IsValidReleaseNumber(string releaseNumber) {
    return System.Text.RegularExpressions.Regex.IsMatch(releaseNumber, @"[0-9]+\.[0-9]+");
}