using GestionBD.Application.Abstractions.Services;
using System.Text.RegularExpressions;

namespace GestionBD.Infrastructure.Services;

public sealed class ScriptRegexService : IScriptRegexService
{
    #region Dictionary of SQL Object Patterns
    private static readonly Dictionary<string, Regex> SqlObjectPatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        // Tablas
        ["CREATE_TABLE"] = new Regex(@"CREATE\s+TABLE\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["ALTER_TABLE"] = new Regex(@"ALTER\s+TABLE\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["DROP_TABLE"] = new Regex(@"DROP\s+TABLE\s+(?:IF\s+EXISTS\s+)?(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),

        // Índices
        ["CREATE_INDEX"] = new Regex(@"CREATE\s+(?:UNIQUE\s+)?(?:CLUSTERED\s+|NONCLUSTERED\s+)?INDEX\s+\[?(?<name>\w+)\]?\s+ON\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<table>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["DROP_INDEX"] = new Regex(@"DROP\s+INDEX\s+(?:IF\s+EXISTS\s+)?\[?(?<name>\w+)\]?\s+ON\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<table>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),

        // Stored Procedures
        ["CREATE_PROCEDURE"] = new Regex(@"CREATE\s+(?:OR\s+ALTER\s+)?PROC(?:EDURE)?\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["ALTER_PROCEDURE"] = new Regex(@"ALTER\s+PROC(?:EDURE)?\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["DROP_PROCEDURE"] = new Regex(@"DROP\s+PROC(?:EDURE)?\s+(?:IF\s+EXISTS\s+)?(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["EXEC_PROCEDURE"] = new Regex(@"EXEC(?:UTE)?\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),

        // Funciones
        ["CREATE_FUNCTION"] = new Regex(@"CREATE\s+(?:OR\s+ALTER\s+)?FUNCTION\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["ALTER_FUNCTION"] = new Regex(@"ALTER\s+FUNCTION\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["DROP_FUNCTION"] = new Regex(@"DROP\s+FUNCTION\s+(?:IF\s+EXISTS\s+)?(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),

        // Vistas
        ["CREATE_VIEW"] = new Regex(@"CREATE\s+(?:OR\s+ALTER\s+)?VIEW\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["ALTER_VIEW"] = new Regex(@"ALTER\s+VIEW\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["DROP_VIEW"] = new Regex(@"DROP\s+VIEW\s+(?:IF\s+EXISTS\s+)?(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),

        // Triggers
        ["CREATE_TRIGGER"] = new Regex(@"CREATE\s+(?:OR\s+ALTER\s+)?TRIGGER\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?\s+ON\s+(?:\[?(?<schema_target>\w+)\]?\.)?\[?(?<table>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["DROP_TRIGGER"] = new Regex(@"DROP\s+TRIGGER\s+(?:IF\s+EXISTS\s+)?(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),

        // Referencias a objetos en SELECT, INSERT, UPDATE, DELETE
        ["SELECT_FROM"] = new Regex(@"FROM\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?(?:\s+(?:AS\s+)?\w+)?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["JOIN_TABLE"] = new Regex(@"(?:INNER\s+|LEFT\s+|RIGHT\s+|FULL\s+|CROSS\s+)?JOIN\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?(?:\s+(?:AS\s+)?\w+)?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["INSERT_INTO"] = new Regex(@"INSERT\s+INTO\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["UPDATE_TABLE"] = new Regex(@"UPDATE\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["DELETE_FROM"] = new Regex(@"DELETE\s+FROM\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),

        // Constraints
        ["ADD_CONSTRAINT"] = new Regex(@"ADD\s+CONSTRAINT\s+\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),
        ["DROP_CONSTRAINT"] = new Regex(@"DROP\s+CONSTRAINT\s+(?:IF\s+EXISTS\s+)?\[?(?<name>\w+)\]?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),

        // Foreign Keys
        ["FOREIGN_KEY_REFERENCES"] = new Regex(@"REFERENCES\s+(?:\[?(?<schema>\w+)\]?\.)?\[?(?<table>\w+)\]?\s*\(\s*\[?(?<column>\w+)\]?\s*\)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled),

        // Llamadas a funciones
        ["FUNCTION_CALL"] = new Regex(@"(?:\[?(?<schema>\w+)\]?\.)?\[?(?<name>\w+)\]?\s*\(",
            RegexOptions.IgnoreCase | RegexOptions.Compiled)
    };
    #endregion
    #region public methods
    public List<string> getRelatedObjects(string script)
    {
        if (string.IsNullOrWhiteSpace(script))
            return [];

        var relatedObjects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var cleanScript = CleanSqlScript(script);

        foreach (var (patternName, regex) in SqlObjectPatterns)
        {
            var matches = regex.Matches(cleanScript);

            foreach (Match match in matches)
            {
                var extractedObjects = ExtractObjectsFromMatch(match, patternName);
                foreach (var obj in extractedObjects)
                {
                    relatedObjects.Add(obj);
                }
            }
        }

        // Remover objetos comunes del sistema que no son relevantes
        var filteredObjects = relatedObjects
            .Where(obj => !IsSystemObject(obj))
            .OrderBy(obj => obj)
            .ToList();

        return filteredObjects;
    }
    #endregion
    #region private methods
    private static List<string> ExtractObjectsFromMatch(Match match, string patternName)
    {
        var objects = new List<string>();

        try
        {
            var schema = match.Groups["schema"].Success ? match.Groups["schema"].Value : "dbo";
            var name = match.Groups["name"].Success ? match.Groups["name"].Value : null;

            if (!string.IsNullOrWhiteSpace(name))
            {
                var fullName = $"{schema}.{name}";
                objects.Add(fullName);
            }

            if (patternName.Contains("INDEX") && match.Groups["table"].Success)
            {
                var tableSchema = match.Groups["schema"].Success ? match.Groups["schema"].Value : "dbo";
                var tableName = match.Groups["table"].Value;
                objects.Add($"{tableSchema}.{tableName}");
            }
            if (patternName == "FOREIGN_KEY_REFERENCES")
            {
                var refSchema = match.Groups["schema"].Success ? match.Groups["schema"].Value : "dbo";
                var refTable = match.Groups["table"].Value;
                objects.Add($"{refSchema}.{refTable}");
            }

            if (patternName == "CREATE_TRIGGER" && match.Groups["table"].Success)
            {
                var targetSchema = match.Groups["schema_target"].Success ? match.Groups["schema_target"].Value : "dbo";
                var targetTable = match.Groups["table"].Value;
                objects.Add($"{targetSchema}.{targetTable}");
            }
        }
        catch (Exception)
        {
            // Si hay error en la extracción, continuar con el siguiente match
        }

        return objects;
    }


    private static string CleanSqlScript(string script)
    {
        var cleanScript = Regex.Replace(script, @"--.*?$", "", RegexOptions.Multiline);
        cleanScript = Regex.Replace(cleanScript, @"/\*.*?\*/", "", RegexOptions.Singleline);
        cleanScript = Regex.Replace(cleanScript, @"\s+", " ");

        return cleanScript.Trim();
    }

    private static bool IsSystemObject(string objectName)
    {
        var systemPrefixes = new[]
        {
            "sys.", "information_schema.", "msdb.", "master.", "tempdb.", "model.",
            "sp_", "fn_", "xp_", "dm_", "INFORMATION_SCHEMA"
        };

        return systemPrefixes.Any(prefix =>
            objectName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }
    #endregion
}
