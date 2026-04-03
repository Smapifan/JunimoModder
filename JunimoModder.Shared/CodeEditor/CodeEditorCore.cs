using System;
using System.Collections.Generic;
using System.IO;

namespace JunimoModder.Shared.CodeEditor
{
    /// <summary>
    /// Kernklasse für den Code-Editor. Verwaltet Quelltext, unterstützte Dateitypen und Dateioperationen.
    /// </summary>
    public class CodeEditorCore
    {
        // ===================== Felder & Eigenschaften =====================

        /// <summary>
        /// Der aktuell geladene Quelltext im Editor.
        /// </summary>
        public string SourceCode { get; set; } = string.Empty;

        /// <summary>
        /// Liste aller unterstützten Dateitypen (z.B. .cs, .json, .xml).
        /// </summary>
        public List<string> SupportedFileTypes { get; } = new() { ".cs", ".json", ".xml" };

        // ===================== Methoden =====================

        /// <summary>
        /// Öffnet eine Datei und lädt deren Inhalt in den Editor.
        /// </summary>
        /// <param name="path">Pfad zur zu öffnenden Datei.</param>
        public void OpenFile(string path)
        {
            // TODO: Fehlerbehandlung und Encoding beachten!
            // Beispiel: SourceCode = File.ReadAllText(path);
        }

        /// <summary>
        /// Speichert den aktuellen Quelltext in eine Datei.
        /// </summary>
        /// <param name="path">Pfad zur Zieldatei.</param>
        public void SaveFile(string path)
        {
            // TODO: Fehlerbehandlung und Encoding beachten!
            // Beispiel: File.WriteAllText(path, SourceCode);
        }

        // ===================== Erweiterbar =====================

        // Hier können weitere Methoden für Suchen, Ersetzen, Syntax-Highlighting usw. ergänzt werden.
    }
}
