using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace BenLib.WPF
{
    public class GeometryHelper
    {
        public static PathFigure InterpolatePointWithBeizerCurves(IEnumerable<Point> points, bool closed, double smoothValue = 0.75) => InterpolatePointWithBeizerCurves(points.ToList(), closed, smoothValue);

        public static PathFigure InterpolatePointWithBeizerCurves(List<Point> points, bool closed, double smoothValue = 0.75)
        {
            if (points.Count == 2) return new PathFigure(points[0], new PathSegmentCollection(new[] { new LineSegment(points[1], true) }), closed);
            else if (points.Count < 3) return null;

            //if is close curve then add the first point at the end
            if (closed) points.Add(points[0]);

            var segments = new PathSegmentCollection(points.Count - 1);

            for (int i = 0; i < points.Count - 1; i++) //iterate for points but the last one
            {
                // Assume we need to calculate the control
                // points between (x1,y1) and (x2,y2).
                // Then x0,y0 - the previous vertex,
                //      x3,y3 - the next one.
                double x1 = points[i].X;
                double y1 = points[i].Y;

                double x2 = points[i + 1].X;
                double y2 = points[i + 1].Y;

                double x0;
                double y0;

                if (i == 0) //if is first point
                {
                    if (closed)
                    {
                        var previousPoint = points[points.Count - 2]; //last Point, but one (due inserted the first at the end)
                        x0 = previousPoint.X;
                        y0 = previousPoint.Y;
                    }
                    else //Get some previouse point
                    {
                        var previousPoint = points[i]; //if is the first point the previous one will be it self
                        x0 = previousPoint.X;
                        y0 = previousPoint.Y;
                    }
                }
                else
                {
                    x0 = points[i - 1].X; //Previous Point
                    y0 = points[i - 1].Y;
                }

                double x3, y3;

                if (i == points.Count - 2) //if is the last point
                {
                    if (closed)
                    {
                        var nextPoint = points[1]; //second Point (due inserted the first at the end)
                        x3 = nextPoint.X;
                        y3 = nextPoint.Y;
                    }
                    else //Get some next point
                    {
                        var nextPoint = points[i + 1]; //if is the last point the next point will be the last one
                        x3 = nextPoint.X;
                        y3 = nextPoint.Y;
                    }
                }
                else
                {
                    x3 = points[i + 2].X; //Next Point
                    y3 = points[i + 2].Y;
                }

                double xc1 = (x0 + x1) / 2.0;
                double yc1 = (y0 + y1) / 2.0;
                double xc2 = (x1 + x2) / 2.0;
                double yc2 = (y1 + y2) / 2.0;
                double xc3 = (x2 + x3) / 2.0;
                double yc3 = (y2 + y3) / 2.0;

                double len1 = Math.Sqrt((x1 - x0).Pow(2) + (y1 - y0).Pow(2));
                double len2 = Math.Sqrt((x2 - x1).Pow(2) + (y2 - y1).Pow(2));
                double len3 = Math.Sqrt((x3 - x2).Pow(2) + (y3 - y2).Pow(2));

                double k1 = len1 / (len1 + len2);
                double k2 = len2 / (len2 + len3);

                double xm1 = xc1 + (xc2 - xc1) * k1;
                double ym1 = yc1 + (yc2 - yc1) * k1;

                double xm2 = xc2 + (xc3 - xc2) * k2;
                double ym2 = yc2 + (yc3 - yc2) * k2;

                // Resulting control points. Here smooth_value is mentioned
                // above coefficient K whose value should be in range [0...1].
                double ctrl1_x = xm1 + (xc2 - xm1) * smoothValue + x1 - xm1;
                double ctrl1_y = ym1 + (yc2 - ym1) * smoothValue + y1 - ym1;

                double ctrl2_x = xm2 + (xc2 - xm2) * smoothValue + x2 - xm2;
                double ctrl2_y = ym2 + (yc2 - ym2) * smoothValue + y2 - ym2;

                segments.Add(new BezierSegment(i == 0 && !closed ? new Point(x1, y1) : new Point(ctrl1_x, ctrl1_y), i == points.Count - 2 && !closed ? new Point(x2, y2) : new Point(ctrl2_x, ctrl2_y), new Point(x2, y2), true));
            }

            return new PathFigure(points[0], segments, closed);
        }

        public static PathGeometry GetCurve(Point[] points, bool closed, bool smooth, double smoothValue = 0.75) => smooth ? new PathGeometry(new[] { InterpolatePointWithBeizerCurves(points, false, smoothValue) }) : new PathGeometry(new[] { new PathFigure(points[0], new[] { new PolyLineSegment(points.Skip(1), true) }, closed) });

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
            PathGeometry pg = cg.GetFlattenedPathGeometry();
            Point[] result = new Point[pg.Figures.Count];
            for (int i = 0; i < pg.Figures.Count; i++)
            {
                Rect fig = new PathGeometry(new PathFigure[] { pg.Figures[i] }).Bounds;
                result[i] = new Point(fig.Left + fig.Width / 2.0, fig.Top + fig.Height / 2.0);
            }
            return result;
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
                pathGeometry.GetPointAtFractionLength(i / divisor, out var point, out var tangent);
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
    }
}
