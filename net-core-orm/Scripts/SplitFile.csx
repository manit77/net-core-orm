#!/usr/bin/env dotnet-script
#nullable enable

// install: dotnet tool install -g dotnet-script
// run: dotnet script SplitFile.csx <SourceFile> <OutputFolder>

using System;
using System.IO;
using System.Text.RegularExpressions;

if (Args.Count < 2) 
{
    Console.WriteLine("Error: You must provide 2 arguments.");
    Console.WriteLine("Usage: dotnet script SplitFile.csx <SourceFile> <OutputFolder>");
    return;
}

// FIX: Use Args here as well
string sourceFile = Args[0];
string outputDir = Args[1];

Console.WriteLine($"Starting file split operation...");
Console.WriteLine($"Source: {sourceFile}");
Console.WriteLine($"Output: {outputDir}");

try 
{
    SplitTemplateIntoFiles(sourceFile, outputDir);
    Console.WriteLine("File split operation completed successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Critical Error: {ex.Message}");
}

void SplitTemplateIntoFiles(string templateFilePath, string outputFolder)
{
    if (!File.Exists(templateFilePath))
    {
        Console.WriteLine($"Error: File not found at {templateFilePath}");
        return;
    }

    string sourceContent = File.ReadAllText(templateFilePath);

    if (!Directory.Exists(outputFolder))
    {
        Directory.CreateDirectory(outputFolder);
    }

    // Regex Explanation:
    // This pattern looks for <file name="..."> ... </file> blocks.
    string pattern = @"<file name=""(?<filename>.*?)"">(?<content>.*?)</file>";
    
    // RegexOptions.Singleline is CRITICAL here so the dot (.) matches newlines inside the file content
    var matches = Regex.Matches(sourceContent, pattern, RegexOptions.Singleline);

    if (matches.Count == 0)
    {
        Console.WriteLine("Warning: No matching <file> tags found in the source.");
    }

    foreach (Match match in matches)
    {
        string fileName = match.Groups["filename"].Value;
        string content = match.Groups["content"].Value;

        // Clean up the newline right after the opening tag so the file doesn't start with a blank line
        // We use TrimStart to only remove the initial whitespace/newline
        content = content.TrimStart('\r', '\n'); 

        string fullPath = Path.Combine(outputFolder, fileName);
        
        // Ensure the directory for the specific file exists (in case filename includes a folder path like 'Validations/MyValidator.cs')
        string? fileDirectory = Path.GetDirectoryName(fullPath);
        if (fileDirectory != null && !Directory.Exists(fileDirectory))
        {
             Directory.CreateDirectory(fileDirectory);
        }

        File.WriteAllText(fullPath, content);
        Console.WriteLine($"  - Extracted: {fileName}");
    }
}