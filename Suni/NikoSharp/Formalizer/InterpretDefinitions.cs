using System.Reflection;
using Suni.Suni.NikoSharp.Core;
using Suni.Suni.NikoSharp.Core.Evaluator;
using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;

namespace Suni.Suni.NikoSharp.Formalizer;

public partial class FormalizingScript
{
    internal void InterpretDefinitionsBlock()
    {
        for (int i = 0; i < DefLines.Count; i++)
        {
            string currentLine = DefLines[i].Trim();
            if (!currentLine.StartsWith('~')) continue;

            var keyWord = Help.keywordLookahead(currentLine).Letters;
            Console.WriteLine($"Identified DefLine keyword: >>{keyWord}<<");
            
            switch (keyWord)
            {
                case "include":
                    ProcessInclude(currentLine);
                    break;
                
                default:
                    ProcessVariableDeclaration(currentLine, keyWord);
                    break;
            }
        }

        Console.WriteLine($">> Includes: {string.Join(", ", FormalizingDataContext.Includes.Keys)}");
        Console.WriteLine($">> Variables: {string.Join(", ", 
            FormalizingDataContext.Variables.Select(v => $"{v.Keys.First()}: {v.Values.First()}"))}");
    }

    private void ProcessInclude(string currentLine)
    {
        var includeName = currentLine.Substring(9).Trim();
        Console.WriteLine($"{includeName} included by line: {currentLine}");

        if (string.IsNullOrWhiteSpace(includeName) || 
            FormalizingDataContext.Includes.ContainsKey(includeName)) return;

        string classNameProper = $"{char.ToUpper(includeName[0])}{includeName[1..].ToLower()}Entitie";
        Type type = Type.GetType($"Suni.Suni.NikoSharp.Data.Classes.{classNameProper}");

        if (type == null)
        {
            Console.WriteLine($"Error: Library '{includeName}' class '{classNameProper}' not found.");
            FormalizingDataContext.LogDiagnostic(Diagnostics.IncludeNotFoundException, 
                $"Library '{includeName}' class '{classNameProper}' not found.");
            return;
        }

        var libMethodsProperty = type.GetProperty("LibMethods", 
            BindingFlags.Static | BindingFlags.Public);

        if (libMethodsProperty?.GetValue(null) is not List<string> methods || !methods.Any())
        {
            FormalizingDataContext.Includes[includeName] = new List<string>();
            Console.WriteLine(libMethodsProperty == null 
                ? $"Library '{includeName}' found, but 'LibMethods' property is missing." 
                : $"Library '{includeName}' registered but no methods found.");
            return;
        }

        FormalizingDataContext.Includes[includeName] = methods;
        Console.WriteLine($"Library '{includeName}' registered with methods: {string.Join(", ", methods)}");
    }

    private void ProcessVariableDeclaration(string currentLine, string keyWord)
    {
        if (!Enum.TryParse(keyWord, true, out STypes varType))
        {
            FormalizingDataContext.LogDiagnostic(Diagnostics.SyntaxException, 
                $"'{keyWord}' isn't a valid instruction to DefinitionsLines.");
            return;
        }

        string rest = currentLine.Substring(keyWord.Length + 1).Trim();
        if (!rest.StartsWith("set"))
        {
            FormalizingDataContext.LogDiagnostic(Diagnostics.SyntaxException, 
                $"After a '{varType}' statement, is expected a 'set' expression.");
            return;
        }

        ParseVariableAssignment(rest[4..].Trim(), varType);
    }

    private void ParseVariableAssignment(string assignment, STypes varType)
    {
        var parts = assignment.Split(' ', 2);
        if (parts.Length < 2)
        {
            FormalizingDataContext.LogDiagnostic(Diagnostics.SyntaxException, 
                $"Missing value for variable '{assignment}'");
            return;
        }

        var (varName, expression) = (parts[0], parts[1]);
        var evaluationResults = NptEvaluator.EvaluateExpression(expression, FormalizingDataContext);
        
        if (evaluationResults.diagnostic != Diagnostics.Success)
        {
            FormalizingDataContext.LogDiagnostic(evaluationResults.diagnostic, 
                evaluationResults.diagnosticMessage);
            return;
        }

        var variable = SType.Create(varType, evaluationResults.resultValue.Value);
        if (variable is NptError error)
            FormalizingDataContext.LogDiagnostic(error.Diagnostic, error.Message);
        
        FormalizingDataContext.Variables.Add(new Dictionary<string, SType> { { varName, variable } });
    }
}