var help = Argument("h", "null");

void init() {
    if (help != "null") {
        Information("Rerun with --help to get cake help");
        Environment.Exit(-1);
    }
}