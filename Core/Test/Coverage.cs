﻿// ────── ╔╗ Nori.Core
// ╔═╦╦═╦╦╬╣ Copyright © 2024 Arvind
// ║║║║╬║╔╣║ Coverage.cs ~ Implements the Coverage class, used to load coverage.xml files
// ╚╩═╩═╩╝╚╝ ───────────────────────────────────────────────────────────────────────────────────────
using System.Xml.Linq;
namespace Nori;

#region class Coverage -----------------------------------------------------------------------------
/// <summary>This type is used to load Coverage data from an XML file</summary>
public class Coverage {
   // Constructor --------------------------------------------------------------
   /// <summary>Load a coverage.xml file generated by the dotnet-coverage tool</summary>
   /// We run the Nori.Test routine using the dotnet-coverage tool, and that then
   /// generates a coverage.xml file that contains all the blocks in the code, along
   /// with a marker on whether that block has been covered or not. This class loads
   /// that file and can then provide the coverage blocks corresponding to a particular
   /// source file.
   public Coverage (string xmlfile) {
      XDocument doc = XDocument.Load (xmlfile);
      var results = doc.Element ("results")!;
      foreach (var module in results.Descendants ("module")) {
         Dictionary<int, string> tmpMap = [];
         foreach (var file in module.Descendants ("source_file")) {
            int id = AttrN (file, "id");
            string path = AttrS (file, "path").Replace (@"W:\Nori", @"N:").Replace (@"w:\Nori", @"N:");
            tmpMap.Add (id, path);
         }

         foreach (var function in module.Descendants ("function")) {
            foreach (var range in function.Descendants ("range")) {
               int idSource = GetFileID (tmpMap[AttrN (range, "source_id")]);
               int sline = AttrN (range, "start_line"), eline = AttrN (range, "end_line");
               int scolumn = AttrN (range, "start_column"), ecolumn = AttrN (range, "end_column");
               bool covered = AttrS (range, "covered") != "no";
               mBlocks.Add (new (idSource, (sline, scolumn), (eline, ecolumn), covered));
            }
         }
      }
      mBlocks.Sort ();
      for (int i = mBlocks.Count - 1; i >= 1; i--) {
         Block b0 = mBlocks[i - 1], b1 = mBlocks[i];
         if (b0.CompareTo (b1) == 0) {
            mBlocks[i - 1] = new Block (b0.FileId, b0.Start, b0.End, b0.Covered | b1.Covered);
            mBlocks.RemoveAt (i);
         }
      }
   }

   // Properties ---------------------------------------------------------------
   /// <summary>The set of blocks we loaded from the coverage.xml file</summary>
   /// Each block contains a file, start and end position (line + column) and a
   /// 'covered' state bit
   public IReadOnlyList<Block> Blocks => mBlocks;
   readonly List<Block> mBlocks = [];

   /// <summary>The list of files we've got coverage for</summary>
   public IReadOnlyList<string> Files => mFiles;
   List<string> mFiles = [];

   /// <summary>Gets the blocks related to a particular source file</summary>
   /// The blocks are sorted from top to bottom
   public IEnumerable<Block> GetBlocksFor (string file) {
      int id = GetFileID (file);
      return mBlocks.Where (a => a.FileId == id);
   }

   /// <summary>This is used by the caller to limit the blocks to only those within a given set of files</summary>
   public void SetFilesOfInterest (List<string> files) {
      HashSet<int> fileids = [];
      foreach (var f in files) fileids.Add (GetFileID (f));
      for (int i = mBlocks.Count - 1; i >= 0; i--)
         if (!fileids.Contains (mBlocks[i].FileId)) mBlocks.RemoveAt (i);
      mFiles = [.. files];
   }

   // Nested types -------------------------------------------------------------
   /// <summary>The Block structure maintains the data for each atomic code-execution block</summary>
   public readonly struct Block : IComparable<Block> {
      public Block (int file, (int L, int C) start, (int L, int C) end, bool covered)
         => (FileId, Start, End, Covered) = (file, start, end, covered);
      public override string ToString () => $"{FileId} : {Start.Line}:{Start.Col} - {End.Line}:{End.Col}";

      public readonly int FileId;
      public readonly (int Line, int Col) Start;
      public readonly (int Line, int Col) End;
      public readonly bool Covered;

      public int CompareTo (Block other) {
         int n = FileId - other.FileId; if (n != 0) return n;
         n = Start.Line - other.Start.Line; if (n != 0) return n;
         return Start.Col - other.Start.Col;
      }
   }

   // Implementation -----------------------------------------------------------
   // Gets the internal file-id for a given filename
   int GetFileID (string name) {
      if (!mSrcMap.TryGetValue (name, out int n)) {
         mSrcMap.Add (name, n = mFiles.Count);
         mFiles.Add (name);
      }
      return n;
   }
   readonly Dictionary<string, int> mSrcMap = new (StringComparer.OrdinalIgnoreCase);

   // Extract a string attribute from an XElement
   static string AttrS (XElement elem, string name) {
      XAttribute? attr = elem.Attribute (name);
      if (attr == null) return "";
      return attr.Value;
   }

   // Extract an integer attribute from an XElement
   static int AttrN (XElement elem, string name) {
      if (!int.TryParse (AttrS (elem, name), out int n)) return 0;
      return n;
   }
}
#endregion
