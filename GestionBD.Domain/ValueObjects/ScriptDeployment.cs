using GestionBD.Domain.Entities;
using System.IO.Compression;
using System.Text;

namespace GestionBD.Domain.ValueObjects
{
    public sealed class ScriptDeployment
    {
        public string ScriptName { get; }
        public string ScriptContent { get; }
        public int ExecutionOrder { get; }
        public Encoding Encoding { get; }

        private ScriptDeployment(string scriptName, string scriptContent, int executionOrder, Encoding encoding)
        {
            ScriptName = scriptName;
            ScriptContent = scriptContent;
            ExecutionOrder = executionOrder;
            Encoding = encoding;
        }

        public static ScriptDeployment Create(string scriptName, string scriptContent, int executionOrder, string codificacion)
        {
            if (string.IsNullOrWhiteSpace(scriptName))
                throw new ArgumentException("El nombre del script no puede estar vacío", nameof(scriptName));

            if (string.IsNullOrWhiteSpace(scriptContent))
                throw new ArgumentException("El contenido del script no puede estar vacío", nameof(scriptContent));

            var encoding = GetEncoding(codificacion);

            return new ScriptDeployment(scriptName, scriptContent, executionOrder, encoding);
        }

        public static IEnumerable<ScriptDeployment> ExtractScriptsFromZip(
            string zipPath,
            IEnumerable<TblArtefacto> artefactos)
        {
            if (string.IsNullOrWhiteSpace(zipPath))
                throw new ArgumentException("La ruta del ZIP no puede estar vacía", nameof(zipPath));

            if (!File.Exists(zipPath))
                throw new FileNotFoundException($"No se encontró el archivo ZIP en la ruta: {zipPath}");

            var artefactosList = artefactos.Where(a => !a.EsReverso).OrderBy(a => a.OrdenEjecucion).ToList();

            if (!artefactosList.Any())
                throw new InvalidOperationException("No hay artefactos para desplegar");

            var scripts = new List<ScriptDeployment>();

            using var zipArchive = ZipFile.OpenRead(zipPath);

            foreach (var artefacto in artefactosList)
            {
                var entry = zipArchive.Entries.FirstOrDefault(e =>
                    e.FullName.Replace('/', '\\').Equals(artefacto.NombreArtefacto, StringComparison.OrdinalIgnoreCase));

                if (entry == null)
                    throw new FileNotFoundException($"No se encontró el archivo {artefacto.NombreArtefacto} en el ZIP");

                using var stream = entry.Open();
                using var reader = new StreamReader(stream, GetEncoding(artefacto.Codificacion));
                var content = reader.ReadToEnd();

                scripts.Add(Create(artefacto.NombreArtefacto, content, artefacto.OrdenEjecucion, artefacto.Codificacion));
            }

            return scripts;
        }

        public IEnumerable<string> SplitIntoBatches()
        {
            var batches = new List<string>();
            var lines = ScriptContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var currentBatch = new StringBuilder();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.Equals("GO", StringComparison.OrdinalIgnoreCase))
                {
                    if (currentBatch.Length > 0)
                    {
                        batches.Add(currentBatch.ToString());
                        currentBatch.Clear();
                    }
                }
                else
                {
                    currentBatch.AppendLine(line);
                }
            }

            if (currentBatch.Length > 0)
            {
                batches.Add(currentBatch.ToString());
            }

            return batches.Where(b => !string.IsNullOrWhiteSpace(b));
        }

        private static Encoding GetEncoding(string codificacion)
        {
            return codificacion?.ToUpperInvariant() switch
            {
                "UTF8" or "UTF-8" => Encoding.UTF8,
                "UTF16" or "UTF-16" => Encoding.Unicode,
                "ASCII" => Encoding.ASCII,
                "LATIN1" or "ISO-8859-1" => Encoding.Latin1,
                _ => Encoding.UTF8
            };
        }
    }
}
