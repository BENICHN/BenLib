using BenLib.Framework;
using BenLib.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BenLib.WPF
{
    public static class GeometryHelper
    {
        public static StreamGeometry InterpolatePointWithBeizerCurves(IEnumerable<Point> points, bool closed, double smoothValue = 0.75) => InterpolatePointWithBeizerCurves(points.ToList(), closed, smoothValue);
        public static StreamGeometry InterpolatePointWithBeizerCurves(List<Point> points, bool closed, double smoothValue = 0.75)
        {
            int count = points.Count;
            var result = new StreamGeometry();

            if (points.Count < 2) return (StreamGeometry)Geometry.Empty;

            using (var context = result.Open())
            {
                if (points.Count == 2)
                {
                    context.BeginFigure(points[0], true, false);
                    context.LineTo(points[1], true, true);
                }
                else
                {
                    if (closed)
                    {
                        points.Add(points[0]);
                        count++;
                    }

                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        var (x1, y1) = points[i].Deconstruct();
                        var (x2, y2) = points[i + 1].Deconstruct();

                        var (x0, y0) = i == 0 ? closed ? points[count - 2].Deconstruct() : (x1, y1) : points[i - 1].Deconstruct();
                        var (x3, y3) = i == count - 2 ? closed ? points[1].Deconstruct() : (x2, y2) : points[i + 2].Deconstruct();

                        double xc1 = (x0 + x1) / 2.0;
                        double yc1 = (y0 + y1) / 2.0;
                        double xc2 = (x1 + x2) / 2.0;
                        double yc2 = (y1 + y2) / 2.0;
                        double xc3 = (x2 + x3) / 2.0;
                        double yc3 = (y2 + y3) / 2.0;

                        double len1 = Num.Distance((x1, y1), (x0, y0));
                        double len2 = Num.Distance((x2, y2), (x1, y1));
                        double len3 = Num.Distance((x3, y3), (x2, y2));

                        double k1 = len1 / (len1 + len2);
                        double k2 = len2 / (len2 + len3);

                        double xm1 = xc1 + (xc2 - xc1) * k1;
                        double ym1 = yc1 + (yc2 - yc1) * k1;

                        double xm2 = xc2 + (xc3 - xc2) * k2;
                        double ym2 = yc2 + (yc3 - yc2) * k2;

                        double ctrl1_x = xm1 + (xc2 - xm1) * smoothValue + x1 - xm1;
                        double ctrl1_y = ym1 + (yc2 - ym1) * smoothValue + y1 - ym1;

                        double ctrl2_x = xm2 + (xc2 - xm2) * smoothValue + x2 - xm2;
                        double ctrl2_y = ym2 + (yc2 - ym2) * smoothValue + y2 - ym2;

                        context.BezierTo(i == 0 && !closed ? new Point(x1, y1) : new Point(ctrl1_x, ctrl1_y), i == points.Count - 2 && !closed ? new Point(x2, y2) : new Point(ctrl2_x, ctrl2_y), new Point(x2, y2), true, true);
                    }
                }
            }

            return result;
        }

        public static StreamGeometry GetCurve(IList<Point> points, bool closed, bool smooth, double smoothValue = 0.75)
        {
            int count = points.Count;
            if (count > 1)
            {
                if (smooth) return InterpolatePointWithBeizerCurves(points, closed, smoothValue);
                else
                {
                    var result = new StreamGeometry();
                    using (var context = result.Open())
                    {
                        context.BeginFigure(points[0], true, closed);
                        for (int i = 1; i < count; i++) context.LineTo(points[i], true, true);
                    }
                    return result;
                }
            }
            else return (StreamGeometry)Geometry.Empty;
        }

        public static GeometryGroup Group(IEnumerable<Geometry> children) => new GeometryGroup() { Children = new GeometryCollection(children) };
        public static GeometryGroup Group(params Geometry[] children) => new GeometryGroup() { Children = new GeometryCollection(children) };
        public static GeometryGroup Group(GeometryCollection children) => new GeometryGroup() { Children = children };

        /// <summary>
        /// https://loune.net/2009/08/getting-the-intersection-points-of-two-path-geometries-in-wpf/
        /// </summary>
        public static Point[] GetIntersectionPoints(Geometry g1, Geometry g2)
        {
            var og1 = g1.GetWidenedPathGeometry(new Pen(Brushes.Black, 1.0));
            var og2 = g2.GetWidenedPathGeometry(new Pen(Brushes.Black, 1.0));
            var cg = Geometry.Combine(og1, og2, GeometryCombineMode.Intersect, null, 0, ToleranceType.Absolute);
            var pg = cg.GetFlattenedPathGeometry();
            var result = new Point[pg.Figures.Count];
            for (int i = 0; i < pg.Figures.Count; i++)
            {
                var fig = new PathGeometry(new PathFigure[] { pg.Figures[i] }).Bounds;
                result[i] = new Point(fig.Left + fig.Width / 2.0, fig.Top + fig.Height / 2.0);
            }
            return result;
        }

        public static StreamGeometry ClipGeometry(PathGeometry geometry, Rect clipRect)
        {
            var result = new StreamGeometry();
            using (var context = result.Open()) { foreach (var figure in geometry.Figures) GetFigures(context, figure, clipRect); }
            return result;

            static (Point A, Point B)? LineRectIntersectionEndpoints(Point lineStart, Point lineEnd, Rect clipRect)
            {
                var endpoints = LineRectIntersection(lineStart, lineEnd, clipRect).GetEnumerator();
                if (!endpoints.MoveNext()) return null;
                else
                {
                    var a = endpoints.Current;
                    return endpoints.MoveNext() ? ((Point A, Point B)?)(a, endpoints.Current) : null;
                }
            }

            static IEnumerable<(Point A, Point B)> GetFigurePoints(PathFigure figure, Rect clipRect)
            {
                var lastPoint = figure.StartPoint;
                var resSeg = new PolyLineSegment();
                foreach (var segment in figure.Segments)
                {
                    if (segment is LineSegment lineSegment)
                    {
                        var endpoints = LineRectIntersectionEndpoints(lastPoint, lineSegment.Point, clipRect);
                        if (endpoints.HasValue) yield return endpoints.Value;
                        lastPoint = lineSegment.Point;
                    }
                    else if (segment is PolyLineSegment polyLineSegment)
                    {
                        foreach (var point in polyLineSegment.Points)
                        {
                            var endpoints = LineRectIntersectionEndpoints(lastPoint, point, clipRect);
                            if (endpoints.HasValue) yield return endpoints.Value;
                            lastPoint = point;
                        }
                    }
                }
            }

            static void GetFigures(StreamGeometryContext context, PathFigure figure, Rect clipRect)
            {
                var enumerator = GetFigurePoints(figure, clipRect).GetEnumerator();
                if (!enumerator.MoveNext()) return;

                Point? startPoint = null;
                var last = enumerator.Current;

                while (enumerator.MoveNext())
                {
                    var (a, b) = enumerator.Current;

                    if (Com(out var d))
                    {
                        if (!startPoint.HasValue)
                        {
                            if (last.A == d)
                            {
                                startPoint = last.B;
                                context.BeginFigure(startPoint.Value, true, false);
                                context.LineTo(last.A, true, true);
                            }
                            else
                            {
                                startPoint = last.A;
                                context.BeginFigure(startPoint.Value, true, false);
                                context.LineTo(last.B, true, true);
                            }
                        }
                        context.LineTo(d, true, true);
                    }
                    else
                    {
                        if (!startPoint.HasValue)
                        {
                            context.BeginFigure(last.A, true, false);
                            context.LineTo(last.B, true, true);
                        }
                        else startPoint = null;
                    }

                    last = (a, b);

                    bool Com(out Point diff)
                    {
                        if (a == last.A || a == last.B)
                        {
                            diff = b;
                            return true;
                        }
                        if (b == last.A || b == last.B)
                        {
                            diff = a;
                            return true;
                        }
                        diff = default;
                        return false;
                    }
                }

                if (!startPoint.HasValue)
                {
                    //context.BeginFigure(startPoint.Value, true, false);
                    context.BeginFigure(last.A, true, false);
                    context.LineTo(last.B, true, true);
                }
                /*yield return startPoint.HasValue
                    ? new PathFigure(startPoint.Value, new[] { new PolyLineSegment(points, true) }, false)
                    : new PathFigure(last.A, new[] { new LineSegment(last.B, true) }, false);*/
            }
        }

        public static IEnumerable<Point> LineRectIntersection(Point lineStart, Point lineEnd, Rect rect)
        {
            if (rect.Contains(lineStart) && rect.Contains(lineEnd))
            {
                yield return lineStart;
                yield return lineEnd;
            }
            else
            {
                byte count = 0;

                if (Intersects(lineStart, lineEnd, rect.TopLeft, rect.TopRight, out var point))
                {
                    yield return point;
                    count++;
                }

                if (Intersects(lineStart, lineEnd, rect.BottomLeft, rect.BottomRight, out point))
                {
                    yield return point;
                    count++;
                    if (count == 2) yield break;
                }

                if (Intersects(lineStart, lineEnd, rect.TopLeft, rect.BottomLeft, out point))
                {
                    yield return point;
                    count++;
                    if (count == 2) yield break;
                }

                if (Intersects(lineStart, lineEnd, rect.TopRight, rect.BottomRight, out point))
                {
                    yield return point;
                    count++;
                    if (count == 2) yield break;
                }

                yield return rect.Contains(lineStart) ? lineStart : lineEnd;
            }
        }

        public static bool Intersects(Point a1, Point a2, Point b1, Point b2, out Point intersection)
        {
            intersection = new Point(0, 0);

            var b = a2 - a1;
            var d = b2 - b1;
            double bDotDPerp = b.X * d.Y - b.Y * d.X;

            if (bDotDPerp == 0) return false;

            var c = b1 - a1;
            double t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1) return false;

            double u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1) return false;

            intersection = a1 + t * b;

            return true;
        }
    }

    public class OptimizedStreamGeometryContext
    {
        public StreamGeometryContext StreamGeometryContext { get; }
        //public Rect Clip { get; set; }
        public double MinimalSquaredDistance { get; set; }

        public OptimizedStreamGeometryContext(StreamGeometryContext streamGeometryContext/*, Rect clip*/, double minimalSquaredDistance)
        {
            StreamGeometryContext = streamGeometryContext;
            //Clip = clip;
            MinimalSquaredDistance = minimalSquaredDistance;
        }

        //private bool m_beyondLeft;
        //private bool m_beyondTop;
        //private bool m_beyondRight;
        //private bool m_beyondBottom;
        private Point m_previousPoint;

        /*private bool IsPointValid(Point point)
        {
            //var clip = Clip;
            //if (clip.Left > point.X)
            //{
            //    if (m_beyondLeft) return false;
            //    m_beyondLeft = true;
            //}
            //if (clip.Top > point.Y)
            //{
            //    if (m_beyondTop) return false;
            //    m_beyondTop = true;
            //}
            //if (clip.Right < point.X)
            //{
            //    if (m_beyondRight) return false;
            //    m_beyondRight = true;
            //}
            //if (clip.Bottom < point.Y)
            //{
            //    if (m_beyondBottom) return false;
            //    m_beyondBottom = true;
            //}
            return true;
        }*/

        public void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked, bool isSmoothJoin)
        {
            if (NumFramework.SquaredDistance(m_previousPoint, point) > MinimalSquaredDistance)
            {
                StreamGeometryContext.ArcTo(point, size, rotationAngle, isLargeArc, sweepDirection, isStroked, isSmoothJoin);
                m_previousPoint = point;
            }
        }
        public void BeginFigure(Point startPoint, bool isFilled, bool isClosed)
        {
            /*IsPointValid(startPoint);*/
            StreamGeometryContext.BeginFigure(startPoint, isFilled, isClosed);
        }
        public void BezierTo(Point point1, Point point2, Point point3, bool isStroked, bool isSmoothJoin)
        {
            if (/*IsPointValid(point3) && */NumFramework.SquaredDistance(m_previousPoint, point3) > MinimalSquaredDistance)
            {
                StreamGeometryContext.BezierTo(point1, point2, point3, isStroked, isSmoothJoin);
                m_previousPoint = point3;
            }
        }
        public void Close() => StreamGeometryContext.Close();
        public void LineTo(Point point, bool isStroked, bool isSmoothJoin)
        {
            if (/*IsPointValid(point) && */NumFramework.SquaredDistance(m_previousPoint, point) > MinimalSquaredDistance)
            {
                StreamGeometryContext.LineTo(point, isStroked, isSmoothJoin);
                m_previousPoint = point;
            }
        }
        public void PolyBezierTo(IEnumerable<Point> points, bool isStroked, bool isSmoothJoin)
        {
            Point? p1 = null;
            Point? p2 = null;
            foreach (var point in points)
            {
                if (!p1.HasValue) p1 = point;
                else if (!p2.HasValue) p2 = point;
                else
                {
                    if (/*IsPointValid(point) && */NumFramework.SquaredDistance(m_previousPoint, point) > MinimalSquaredDistance)
                    {
                        StreamGeometryContext.BezierTo(p1.Value, p2.Value, point, isStroked, isSmoothJoin);
                        m_previousPoint = point;
                    }
                    p1 = p2 = null;
                }
            }
        }
        public void PolyLineTo(IEnumerable<Point> points, bool isStroked, bool isSmoothJoin)
        {
            foreach (var point in points)
            {
                if (/*IsPointValid(point) && */NumFramework.SquaredDistance(m_previousPoint, point) > MinimalSquaredDistance)
                {
                    StreamGeometryContext.LineTo(point, isStroked, isSmoothJoin);
                    m_previousPoint = point;
                }
            }
        }
        public void PolyQuadraticBezierTo(IEnumerable<Point> points, bool isStroked, bool isSmoothJoin)
        {
            Point? p1 = null;
            foreach (var point in points)
            {
                if (!p1.HasValue) p1 = point;
                else
                {
                    if (/*IsPointValid(point) && */NumFramework.SquaredDistance(m_previousPoint, point) > MinimalSquaredDistance)
                    {
                        StreamGeometryContext.QuadraticBezierTo(p1.Value, point, isStroked, isSmoothJoin);
                        m_previousPoint = point;
                    }
                    p1 = null;
                }
            }
        }
        public void QuadraticBezierTo(Point point1, Point point2, bool isStroked, bool isSmoothJoin)
        {
            if (/*IsPointValid(point2) && */NumFramework.SquaredDistance(m_previousPoint, point2) > MinimalSquaredDistance)
            {
                StreamGeometryContext.QuadraticBezierTo(point1, point2, isStroked, isSmoothJoin);
                m_previousPoint = point2;
            }
        }
    }

    public static partial class Extensions
    {
        public static double StrokeLength(this Geometry geometry, double tolerance = -1, ToleranceType type = ToleranceType.Absolute)
        {
            if (geometry is EllipseGeometry ellipse && ellipse.RadiusX == ellipse.RadiusY) return 2 * ellipse.RadiusX * Math.PI;
            if (geometry is RectangleGeometry rectangle) return 2 * rectangle.Rect.Width + 2 * rectangle.Rect.Height;
            if (geometry is LineGeometry line) return Math.Sqrt(Math.Pow(line.StartPoint.X - line.EndPoint.X, 2) + Math.Pow(line.StartPoint.Y - line.EndPoint.Y, 2));

            double result = 0;
            var f = tolerance < 0 ? geometry.GetFlattenedPathGeometry() : geometry.GetFlattenedPathGeometry(tolerance, type);

            foreach (var figure in f.Figures)
            {
                var previousPoint = figure.StartPoint;

                foreach (var segment in figure.Segments)
                {
                    if (segment is PolyLineSegment polyLineSegment)
                    {
                        foreach (var point in polyLineSegment.Points)
                        {
                            result += Math.Sqrt(Math.Pow(previousPoint.X - point.X, 2) + Math.Pow(previousPoint.Y - point.Y, 2));
                            previousPoint = point;
                        }
                    }
                    else if (segment is LineSegment lineSegment) result += Math.Sqrt(Math.Pow(previousPoint.X - lineSegment.Point.X, 2) + Math.Pow(previousPoint.Y - lineSegment.Point.Y, 2));
                }
            }

            return result;
        }

        public static IEnumerable<Point> GetPoints(this Geometry geometry, int pointsCount) => GetPoints(PathGeometry.CreateFromGeometry(geometry), pointsCount);
        public static IEnumerable<Point> GetPoints(this PathGeometry pathGeometry, int pointsCount)
        {
            double divisor = pointsCount - 1;
            for (int i = 0; i < pointsCount; i++)
            {
                pathGeometry.GetPointAtFractionLength(i / divisor, out var point, out _);
                yield return point;
            }
        }

        public static IEnumerable<Point> GetPoints(this Geometry geometry, double tolerance = -1, ToleranceType type = ToleranceType.Absolute)
        {
            var f = tolerance < 0 ? geometry.GetFlattenedPathGeometry() : geometry.GetFlattenedPathGeometry(tolerance, type);

            foreach (var figure in f.Figures)
            {
                yield return figure.StartPoint;

                foreach (var segment in figure.Segments)
                {
                    if (segment is PolyLineSegment polyLineSegment) foreach (var point in polyLineSegment.Points) yield return point;
                    else if (segment is LineSegment lineSegment) yield return lineSegment.Point;
                }
            }
        }

        public static void Transform(this Geometry geometry, Transform transform)
        {
            if (geometry.Transform == null) geometry.Transform = transform;
            else if (geometry.Transform is TransformGroup transformGroup) transformGroup.Children.Add(transform);
            else geometry.Transform = new TransformGroup() { Children = new TransformCollection(new[] { geometry.Transform, transform }) };
        }

        public static Geometry ToGeometry(this Shape shape)
        {
            var result = shape is Ellipse ellipse ? new EllipseGeometry(new Rect(0, 0, ellipse.Width, ellipse.Height)) :
                shape is Line line ? new LineGeometry(new Point(line.X1, line.Y1), new Point(line.X2, line.Y2)) :
                shape is Path path ? path.Data.CloneCurrentValue() :
                shape is Polygon polygon ? GeometryHelper.GetCurve(polygon.Points.ToArray(), true, false) :
                shape is Polyline polyline ? GeometryHelper.GetCurve(polyline.Points.ToArray(), false, false) :
                shape is Rectangle rectangle ? new RectangleGeometry(new Rect(0, 0, rectangle.Width, rectangle.Height), rectangle.RadiusX, rectangle.RadiusY) :
                shape.RenderedGeometry;

            var transform = result.Transform.Value;
            double left = Canvas.GetLeft(shape);
            double top = Canvas.GetTop(shape);
            transform.Translate(double.IsNaN(left) ? 0 : left, double.IsNaN(top) ? 0 : top);
            result.Transform = new MatrixTransform(transform);

            return result;
        }
    }
}
