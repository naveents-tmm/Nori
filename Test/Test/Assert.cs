﻿// ────── ╔╗ Nori.Test
// ╔═╦╦═╦╦╬╣ Copyright © 2024 Arvind
// ║║║║╬║╔╣║ Assert.cs ~ Assert implements various assertions used for testing
// ╚╩═╩═╩╝╚╝ ───────────────────────────────────────────────────────────────────────────────────────
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace Nori.Testing;

#region class Assert -------------------------------------------------------------------------------
/// <summary>Various Assert variants, used in [Test] methods</summary>
static class Assert {
   /// <summary>Check two objects for equality</summary>
   public static void Is (this object obj, object check,
                          [CallerFilePath] string? file = null,
                          [CallerLineNumber] int line = 0,
                          [CallerMemberName] string? member = null,
                          [CallerArgumentExpression (nameof (obj))] string? expr1 = null,
                          [CallerArgumentExpression (nameof (check))] string? expr2 = null) {
      if (check is string && obj is not string) obj = obj.ToString ()!;
      bool passed = (obj, check) switch {
         (double a, double b) => a.EQ (b),
         (float a, float b) => a.EQ (b),
         (double a, float b) => a.EQ (b),
         (float a, double b) => b.EQ (a),
         (double a, int b) => a.EQ (b),
         (float a, int b) => a.EQ (b),
         (Point2 a, Point2 b) => a.EQ (b),
         (Point3 a, Point3 b) => a.EQ (b),
         (Vector2 a, Vector2 b) => a.EQ (b),
         (Vector3 a, Vector3 b) => a.EQ (b),
         _ => obj.Equals (check),
      };
      if (!passed) Throw (obj, check, expr1, expr2, member, line, file);
   }

   /// <summary>Check two objects for equality</summary>
   public static void Is (this double obj, double check, double epsilon,
                          [CallerFilePath] string? file = null,
                          [CallerLineNumber] int line = 0,
                          [CallerMemberName] string? member = null,
                          [CallerArgumentExpression (nameof (obj))] string? expr1 = null,
                          [CallerArgumentExpression (nameof (check))] string? expr2 = null) {
      if (!obj.EQ (check, epsilon)) Throw (obj, check, expr1, expr2, member, line, file);
   }

   /// <summary>Asserts that the given expression is true</summary>
   public static void IsTrue (this bool obj,
                              [CallerFilePath] string? file = null,
                              [CallerLineNumber] int line = 0,
                              [CallerMemberName] string? member = null,
                              [CallerArgumentExpression (nameof (obj))] string? expr1 = null) {
      if (!obj) Throw (obj, true, expr1, "true", member, line, file);
   }

   /// <summary>Asserts that the given expression is false</summary>
   public static void IsFalse (this bool obj,
                              [CallerFilePath] string? file = null,
                              [CallerLineNumber] int line = 0,
                              [CallerMemberName] string? member = null,
                              [CallerArgumentExpression (nameof (obj))] string? expr1 = null) {
      if (obj) Throw (obj, false, expr1, "false", member, line, file);
   }

   /// <summary>Checks if two text files are equal</summary>
   /// <param name="reference">The reference file for comparision</param>
   /// <param name="test">The file generated by the test</param>
   public static void TextFilesEqual (string reference, string test) {
      if (!File.Exists (reference)) { File.Copy (test, reference, true); return; }
      byte[] data1 = File.ReadAllBytes (reference), data2 = File.ReadAllBytes (test);
      if (data1.SequenceEqual (data2)) return;
      Process.Start ("winmergeu.exe", $"{reference} {test}").WaitForExit ();
      throw new TestException ($"Files different: {reference} and {test}");
   }

   // Helper used to throw a TestException
   static void Throw (object obj, object check, string? expr1, string? expr2, string? member, int line, string? file)
      => throw new TestException ($"""
               Found | {obj}
            Expected | {check}
                Code | "{expr1} == {expr2};"
                  In | {member}(), at line {line} of {file}
            """);
}
#endregion

#region class NT -----------------------------------------------------------------------------------
/// <summary>Module class representing the Nori.Test system</summary>
static class NT {
   /// <summary>Path to test data (typically Nori/Test)</summary>
   public static readonly string Data = "N:/Test/Data";

   public static string File (string input) => Path.Combine (Data, input);

   public static readonly string TmpTxt = $"{Data}/tmpfile.txt";
   public static readonly string TmpNim = $"{Data}/tmpfile.nim";
   public static readonly string TmpDXF = $"{Data}/tmpfile.dxf";
}
#endregion
