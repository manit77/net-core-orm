using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CoreORM;

public static class Utils
{
    /// <summary>
    /// Splits a single string containing multiple <file name="...">...</file> blocks 
    /// into individual files.
    /// </summary>
    /// <param name="sourceContent">The raw string content to parse.</param>
    /// <param name="outputFolder">The directory where files should be saved.</param>
    public static void SplitTemplateIntoFiles(string templateFilePath, string outputFolder)
    {
        if (string.IsNullOrWhiteSpace(templateFilePath))
        {
            return;
        }

        string sourceContent = File.ReadAllText(templateFilePath);
        // Ensure directory exists
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        // Regex pattern:
        // (?<filename>.*?) -> Named capture group for the file name
        // (?<content>.*?)  -> Named capture group for the actual code/text
        // RegexOptions.Singleline -> Allows the dot (.) to match newline characters
        string pattern = @"<file name=""(?<filename>.*?)"">(?<content>.*?)</file>";
        var matches = Regex.Matches(sourceContent, pattern, RegexOptions.Singleline);

        foreach (Match match in matches)
        {
            string fileName = match.Groups["filename"].Value;
            string content = match.Groups["content"].Value;

            // Optional: Trim leading/trailing whitespace if you want to clean up 
            // the gap between the <file> tag and the code.
            content = content.TrimStart('\r', '\n');

            // Construct safe path
            string fullPath = Path.Combine(outputFolder, fileName);
            
            // Write the file (preserves original formatting)
            File.WriteAllText(fullPath, content);
            
            Console.WriteLine($"Successfully extracted: {fileName}");
        }
    }
}