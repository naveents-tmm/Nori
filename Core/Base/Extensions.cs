﻿// ────── ╔╗                                                                                   CORE
// ╔═╦╦═╦╦╬╣ Extensions.cs
// ║║║║╬║╔╣║ Various extension methods for common system types
// ╚╩═╩═╩╝╚╝ ───────────────────────────────────────────────────────────────────────────────────────
using static System.Math;
namespace Nori;

#region class Extensions ---------------------------------------------------------------------------
public static class Extensions {
   public static Span<T> AsSpan<T> (this List<T> list) => CollectionsMarshal.AsSpan (list);

   /// <summary>Clamps the given double to lie within min..max (inclusive)</summary>
   public static double Clamp (this double a, double min, double max) => a < min ? min : (a > max ? max : a);
   /// <summary>Clamps the given double to the range 0..1</summary>
   public static double Clamp (this double a) => a < 0 ? 0 : (a > 1 ? 1 : a);

   /// <summary>Clamps the given float to lie within min..max (inclusive)</summary>
   public static float Clamp (this float a, float min, float max) => a < min ? min : (a > max ? max : a);

   /// <summary>Convert an angle from degrees to radians</summary>
   public static double D2R (this double f) => f * RadiansPerDegree;
   /// <summary>Convert an angle from degrees to radians</summary>
   public static double D2R (this int n) => n * RadiansPerDegree;

   /// <summary>Compare two doubles for equality to within 1e-6</summary>
   public static bool EQ (this double a, double b) => Abs (a - b) < 1e-6;
   /// <summary>Compare two doubles for equality with the given epsilon</summary>
   public static bool EQ (this double a, double b, double epsilon) => Abs (a - b) < epsilon;
   /// <summary>Compare two floats for equality to within 1e-5</summary>
   public static bool EQ (this float a, float b) => Abs (a - b) < 1e-5;
   /// <summary>Compare two halfs for equality to within 1e-4</summary>
   public static bool EQ (this Half a, Half b) => Abs ((float)a - (float)b) < 1e-3;

   /// <summary>Compares two string for equality, ignoring case</summary>
   public static bool EqIC (this string a, string b) => a.Equals (b, StringComparison.OrdinalIgnoreCase);

   /// <summary>Performs an action on each element of a sequence</summary>
   public static void ForEach<T> (this IEnumerable<T> seq, Action<T> action) {
      foreach (var elem in seq) action (elem);
   }

   /// <summary>Gets a value from a dictionary, or adds a new one (synthesized by the maker function)</summary>
   public static U Get<T, U> (this IDictionary<T, U> dict, T key, Func<T, U> maker) {
      if (!dict.TryGetValue (key, out var value)) 
         dict[key] = value = maker (key); 
      return value;
   }

   /// <summary>Checks if a member (type / method / property / field) has the given custom attribute attached</summary>
   public static bool HasAttribute<T> (this MemberInfo mi) where T : Attribute => mi.GetCustomAttribute<T> () != null;

   /// <summary>Returns true if a string is null, empty or whitespace</summary>
   public static bool IsBlank (this string? s) => string.IsNullOrWhiteSpace (s);

   /// <summary>Checks if a double is NaN</summary>
   public static bool IsNaN (this double a) => double.IsNaN (a);
   /// <summary>Checks if a float is NaN</summary>
   public static bool IsNaN (this float a) => float.IsNaN (a);

   /// <summary>Checks if a given double is zero to within 1e-6</summary>
   public static bool IsZero (this double a) => Abs (a) < 1e-6;
   /// <summary>Checks if a given float is zero to within 1e-5</summary>
   public static bool IsZero (this float a) => Abs (a) < 1e-5;

   /// <summary>Returns a random bool</summary>
   public static bool NextBool (this Random r) => r.Next (10000) < 5000;

   /// <summary>Given a sequence, returns a 'numbered' version where each item is tagged with an ordinal (starting from 0)</summary>
   public static IEnumerable<(int No, T Data)> Numbered<T> (this IEnumerable<T> seq) {
      int c = 0;
      foreach (var elem in seq) yield return (c++, elem);
   }

   /// <summary>Returns a Half rounded off to 5 decimal places</summary>
   public static float R3 (this Half f) => (float)Math.Round ((float)f, 3);
   /// <summary>Returns a float rounded off to 5 decimal places</summary>
   public static float R5 (this float f) => (float)Math.Round (f, 5);
   /// <summary>Returns a double rounded off to 6 decimal places</summary>
   public static double R6 (this double f) => Math.Round (f, 6);

   /// <summary>Rounds a double to the given number of digits</summary>
   public static double Round (this double a, int digits) => Math.Round (a, digits);
   /// <summary>Rounds a float to the given number of digits</summary>
   public static double Round (this float a, int digits) => Math.Round (a, digits);

   /// <summary>Rounds up the given integer to the next multiple of the given chunk size</summary>
   public static int RoundUp (this int n, int chunk) => chunk * ((n + chunk - 1) / chunk);

   /// <summary>Convert an angle from radians to degrees</summary>
   public static double R2D (this double f) => f * DegreesPerRadian;

   public static string S6 (this double f) {
      string s = Round (f, 6).ToString ();
      return s == "-0" ? "0" : s;
   }

   public static double ToDouble (this string s) {
      double.TryParse (s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double f);
      return f;
   }

   const double DegreesPerRadian = 180 / PI;
   const double RadiansPerDegree = PI / 180;
}
#endregion
