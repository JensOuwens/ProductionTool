using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using SFB;


public class ExportFile : MonoBehaviour
{
    public void ExportFontWithDialog()
    {
        string folderPath = StandaloneFileBrowser.OpenFolderPanel("Select Export Folder", "", false)[0];

        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.LogWarning("No folder selected for export.");
            return;
        }

        ProjectSettings ps = ProjectSettingsManager.Instance.currentProjectSettings;
        if (ps == null || ps.characters == null || ps.characters.Count == 0)
        {
            Debug.LogWarning("No characters to export.");
            return;
        }

        string ttfPath = Path.Combine(folderPath, ps.projectName + ".ttf");

        // --- Step 1: Convert characters to SVG paths ---
        Dictionary<string, string> svgPaths = new Dictionary<string, string>();
        int counter = 1;

        foreach (var ch in ps.characters)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 1024 1024\">");
            sb.Append("<path d=\"");

            foreach (var stroke in ch.strokes)
            {
                sb.Append($"M {stroke.startX} {stroke.startY} L {stroke.endX} {stroke.endY} ");
            }

            sb.Append("\" stroke=\"black\" fill=\"none\" stroke-width=\"1\" />");
            sb.Append("</svg>");
            svgPaths[counter.ToString()] = sb.ToString();
            counter++;
        }

        // Save SVGs temporarily
        string svgFolder = Path.Combine(folderPath, "SVGs");
        Directory.CreateDirectory(svgFolder);
        foreach (var kv in svgPaths)
        {
            File.WriteAllText(Path.Combine(svgFolder, kv.Key + ".svg"), kv.Value);
        }

        // --- Step 2: Generate minimal TTF ---
        TTFWriter.WriteTTF(ttfPath, ps.projectName, ps.characters);
        Debug.Log($"TTF created at: {ttfPath}");

        // --- Step 3: Delete SVGs ---
        try
        {
            Directory.Delete(svgFolder, true);
            Debug.Log("Temporary SVGs deleted.");
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to delete SVG folder: " + e.Message);
        }
    }

    public static class TTFWriter
    {
        public const int UnitsPerEm = 2048;

        public static void WriteTTF(string path, string fontName, List<CharacterData> characters)
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                WriteUInt16(bw, 0x0001);
                WriteUInt16(bw, 1);
                WriteUInt16(bw, 16);
                WriteUInt16(bw, 0);
                WriteUInt16(bw, 16);

                File.WriteAllBytes(path, ms.ToArray());
            }
        }

        private static void WriteUInt16(BinaryWriter bw, ushort value)
        {
            bw.Write(BitConverter.IsLittleEndian ? ReverseBytes(value) : value);
        }

        private static ushort ReverseBytes(ushort value)
        {
            return (ushort)((value << 8) | (value >> 8));
        }
    }
}
