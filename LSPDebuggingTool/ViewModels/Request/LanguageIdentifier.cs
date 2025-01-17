using System.Collections.Generic;

namespace LSPDebuggingTool.ViewModels;

public class LanguageIdentifier
{
    public LanguageIdentifier(string languageName, string languageId)
    {
        LanguageName = languageName;
        LanguageId = languageId;
    }

    public string LanguageName { get; }
    public string LanguageId { get; }

    public static List<LanguageIdentifier> LanguageIds { get; } =
    [
        new("ABAP", "abap"),
        new("Windows Bat", "bat"),
        new("BibTeX", "bibtex"),
        new("Clojure", "clojure"),
        new("Coffeescript", "coffeescript"),
        new("C", "c"),
        new("C++", "cpp"),
        new("C#", "csharp"),
        new("CSS", "css"),
        new("Diff", "diff"),
        new("Dart", "dart"),
        new("Dockerfile", "dockerfile"),
        new("Elixir", "elixir"),
        new("Erlang", "erlang"),
        new("F#", "fsharp"),
        new("Git(commit)", "git-commit"),
        new("Git(rebase)", "git-rebase"),
        new("Go", "go"),
        new("Groovy", "groovy"),
        new("Handlebars", "handlebars"),
        new("HTML", "html"),
        new("Ini", "ini"),
        new("Java", "java"),
        new("JavaScript", "javascript"),
        new("JavaScript React", "javascriptreact"),
        new("JSON", "json"),
        new("LaTeX", "latex"),
        new("Less", "less"),
        new("Lua", "lua"),
        new("Makefile", "makefile"),
        new("Markdown", "markdown"),
        new("Objective-C", "objective-c"),
        new("Objective-C++", "objective-cpp"),
        new("Perl", "perl"),
        new("Perl 6", "perl6"),
        new("PHP", "php"),
        new("Powershell", "powershell"),
        new("Pug", "jade"),
        new("Python", "python"),
        new("R", "r"),
        new("Razor (cshtml)", "razor"),
        new("Ruby", "ruby"),
        new("Rust", "rust"),
        new("SCSS(syntax using curly brackets)", "scss"),
        new("SCSS(indented syntax)", "sass"),
        new("Scala", "scala"),
        new("ShaderLab", "shaderlab"),
        new("Shell Script (Bash)", "shellscript"),
        new("SQL", "sql"),
        new("Swift", "swift"),
        new("TypeScript", "typescript"),
        new("TypeScript React", "typescriptreact"),
        new("TeX", "tex"),
        new("Visual Basic", "vb"),
        new("XML", "xml"),
        new("XSL", "xsl"),
        new("YAML", "yaml"),
    ];
}