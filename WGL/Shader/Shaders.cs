// ────── ╔╗                                                                                    WGL
// ╔═╦╦═╦╦╬╣ Shaders.cs
// ║║║║╬║╔╣║ Final Shader classes, all inherited from Shader<Vertex, UBlock>
// ╚╩═╩═╩╝╚╝ ───────────────────────────────────────────────────────────────────────────────────────
namespace Nori;

#region class Bezier2DShader -----------------------------------------------------------------------
/// <summary>A specialization of Seg2DShader, used to draw curved segs (using beziers)</summary>
class Bezier2DShader : Seg2DShader {
   public Bezier2DShader () : base (ShaderImp.Bezier2D) { }
   public static readonly Bezier2DShader It = new ();
}
#endregion

class StencilLineShader : Shader<CMesh.Node, StencilLineShader.Settings> {
   public StencilLineShader () : base (ShaderImp.StencilLine) => Bind ();

   protected override void ApplyUniformsImp (ref readonly Settings settings) {
      Pgm.Set (muLineWidth, settings.LineWidth);
      Pgm.Set (muDrawColor, settings.Color);
   }

   protected override int OrderUniformsImp (ref readonly Settings a, ref readonly Settings b) {
      int n = a.LineWidth.CompareTo (b.LineWidth); if (n != 0) return n;
      return (int)(a.Color.Value - b.Color.Value);
   }

   protected override void SetConstantsImp () {
      Pgm.Set (muXfm, Lux.Xfm);
      Pgm.Set (muVPScale, Lux.VPScale);
   }

   protected override Settings SnapUniformsImp ()
      => new (3, Color4.Black);

   public readonly record struct Settings (float LineWidth, Color4 Color);

   // These contain the slot numbers of the uniforms
   int muXfm = 0, muVPScale = 0, muLineWidth = 0, muDrawColor = 0;
   public static readonly StencilLineShader It = new ();
}

#region class FacetShader --------------------------------------------------------------------------
abstract class FacetShader : Shader<CMesh.Node, Color4> {
   protected FacetShader (ShaderImp imp) : base (imp) => Bind ();

   protected override void ApplyUniformsImp (ref readonly Color4 settings)
      => Pgm.Set (muDrawColor, settings);

   protected override int OrderUniformsImp (ref readonly Color4 a, ref readonly Color4 b)
      => (int)(a.Value - b.Value);

   protected override void SetConstantsImp () {
      Pgm.Set (muXfm, Lux.Xfm);
      Pgm.Set (muNormalXfm, Lux.NormalXfm);
   }

   protected override Color4 SnapUniformsImp ()
      => Lux.DrawColor;

   int muXfm = 0, muNormalXfm = 0, muDrawColor = 0;
}
#endregion

#region class GouradShader -------------------------------------------------------------------------
class GouradShader : FacetShader {
   public GouradShader () : base (ShaderImp.Gourad) { }
   public static readonly GouradShader It = new ();
}
#endregion

#region class FlatFacetShader ----------------------------------------------------------------------
class FlatFacetShader : FacetShader {
   public FlatFacetShader () : base (ShaderImp.FlatFacet) { }
   public static readonly FlatFacetShader It = new ();
}
#endregion

#region class PhongShader --------------------------------------------------------------------------
class PhongShader : FacetShader {
   public PhongShader () : base (ShaderImp.Phong) { }
   public static readonly PhongShader It = new ();
}
#endregion

#region class Line2DShader -------------------------------------------------------------------------
/// <summary>A specialization of Seg2DShader, used to draw linear segs</summary>
class Line2DShader : Seg2DShader {
   public Line2DShader () : base (ShaderImp.Line2D) { }
   public static readonly Line2DShader It = new ();
}
#endregion

#region class Point2DShader ------------------------------------------------------------------------
/// <summary>Shader used to draw points</summary>
class Point2DShader : Shader<Vec2F, Point2DShader.Settings> {
   // Constructor --------------------------------------------------------------
   public Point2DShader () : base (ShaderImp.Point2D) => Bind ();

   // Overrides ----------------------------------------------------------------
   protected override void ApplyUniformsImp (ref readonly Settings a) {
      Pgm.Set (muPointSize, a.PointSize);
      Pgm.Set (muDrawColor, a.Color);
   }

   protected override int OrderUniformsImp (ref readonly Settings a, ref readonly Settings b) {
      int n = a.PointSize.CompareTo (b.PointSize); if (n != 0) return n;
      return (int)(a.Color.Value - b.Color.Value);
   }

   protected override void SetConstantsImp () {
      Pgm.Set (muVPScale, Lux.VPScale);
      Pgm.Set (muXfm, Lux.Xfm);
   }

   protected override Settings SnapUniformsImp ()
      => new (Lux.PointSize, Lux.DrawColor);

   // Nested types -------------------------------------------------------------
   public readonly record struct Settings (float PointSize, Color4 Color);

   // Private data -------------------------------------------------------------
   // These contain the slot numbers of the uniforms
   int muVPScale = 0, muXfm = 0, muPointSize = 0, muDrawColor = 0;

   public static readonly Point2DShader It = new ();
}
#endregion

#region class Seg2DShader --------------------------------------------------------------------------
/// <summary>Base class for the Line2DShader and Bezier2DShader</summary>
class Seg2DShader : Shader<Vec2F, Seg2DShader.Settings> {
   // Constructor --------------------------------------------------------------
   public Seg2DShader (ShaderImp shader) : base (shader) => Bind ();

   // Overrides ----------------------------------------------------------------
   protected override void ApplyUniformsImp (ref readonly Settings a) {
      Pgm.Set (muLineWidth, a.LineWidth);
      Pgm.Set (muDrawColor, a.Color);
   }

   protected override int OrderUniformsImp (ref readonly Settings a, ref readonly Settings b) {
      int n = a.LineWidth.CompareTo (b.LineWidth); if (n != 0) return n;
      return (int)(a.Color.Value - b.Color.Value);
   }

   protected override void SetConstantsImp () {
      Pgm.Set (muVPScale, Lux.VPScale);
      // TODO: Lux.Xfm will not be a constant across the frame
      Pgm.Set (muXfm, Lux.Xfm);
   }

   protected override Settings SnapUniformsImp () 
      => new (Lux.LineWidth, Lux.DrawColor);

   // Nested types -------------------------------------------------------------
   public readonly record struct Settings (float LineWidth, Color4 Color);

   // Private data -------------------------------------------------------------
   // These contain the slot numbers of the uniforms
   int muVPScale = 0, muXfm = 0, muLineWidth = 0, muDrawColor = 0;
}
#endregion

#region class TriQuad2DShader ----------------------------------------------------------------------
/// <summary>TriQuad2DShader is the base class for Triangle2DShader and Quad2DShader</summary>
abstract class TriQuad2DShader : Shader<Vec2F, Color4> {
   protected TriQuad2DShader (ShaderImp imp) : base (imp) => Bind ();

   // Overrides ----------------------------------------------------------------
   protected override void ApplyUniformsImp (ref readonly Color4 color)
      => Pgm.Set (muDrawColor, color);

   protected override int OrderUniformsImp (ref readonly Color4 a, ref readonly Color4 b)
      => (int)(a.Value - b.Value);

   protected override void SetConstantsImp ()
      => Pgm.Set (muXfm, Lux.Xfm);

   protected override Color4 SnapUniformsImp ()
      => Lux.DrawColor;

   // Private data -------------------------------------------------------------
   int muXfm = 0, muDrawColor = 0;
}
#endregion

#region class Triangle2DShader ---------------------------------------------------------------------
/// <summary>Shadernori. to draw simple triangles in 2D (specified in world space, no anti-aliasing)</summary>
class Triangle2DShader : TriQuad2DShader {
   public Triangle2DShader () : base (ShaderImp.Triangle2D) { }
   public static readonly Triangle2DShader It = new ();
}
#endregion

#region class Quad2DShader -------------------------------------------------------------------------
/// <summary>Shader to draw simple quads in 2D (specified in world space, no anti-aliasing)</summary>
class Quad2DShader : TriQuad2DShader {
   public Quad2DShader () : base (ShaderImp.Quad2D) { }
   public static readonly Quad2DShader It = new ();
}
#endregion
