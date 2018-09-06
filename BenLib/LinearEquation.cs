using System;
using System.Windows;

namespace BenLib
{
    /// <summary>
    /// Représente une équation de droite
    /// </summary>
    public struct LinearEquation
    {
        /// <summary>
        /// Crée une équation de droite de la forme x=k
        /// </summary>
        /// <param name="k">Constante de l'équation</param>
        public LinearEquation(double k) : this()
        {
            K = k;
            Type = LinearEquationType.X;
        }

        /// <summary>
        /// Crée une équation de droite de la forme y=ax+b
        /// </summary>
        /// <param name="a">Coefficient directeur de l'équation</param>
        /// <param name="b">Ordonnée à l'origine de l'équation</param>
        public LinearEquation(double a, double b) : this()
        {
            A = a;
            B = b;

            YFunction = x => a * x + b;

            Type = LinearEquationType.Y;
        }

        /// <summary>
        /// Coefficient directeur de l'équation si elle est de la forme y=ax+b
        /// </summary>
        public double A { get; }

        /// <summary>
        /// Ordonnée à l'origine de l'équation si elle est de la forme y=ax+b
        /// </summary>
        public double B { get; }

        /// <summary>
        /// Si l'équation est de la forme y=ax+b, obtient y en fonction de x
        /// </summary>
        public Func<double, double> YFunction { get; }

        /// <summary>
        /// Constante de l'équation si elle est de la forme x=k
        /// </summary>
        public double K { get; }

        /// <summary>
        /// Forme de l'équation
        /// </summary>
        public LinearEquationType Type { get; }

        /// <summary>
        /// Calcule l'intersection de deux droites avec de leurs équations
        /// </summary>
        /// <param name="equation1">Équation de la première droite</param>
        /// <param name="equation2">Équation de la seconde droite</param>
        /// <returns>Point d'intersection des deux droites</returns>
        public static Point Intersection(LinearEquation equation1, LinearEquation equation2)
        {
            var type1 = equation1.Type;
            var type2 = equation2.Type;

            if (type1 == LinearEquationType.X && equation2.Type == LinearEquationType.X) return new Point(double.NaN, double.NaN);
            if (type1 == LinearEquationType.Y && equation2.Type == LinearEquationType.Y)
            {
                if (equation1.A == equation2.A) return new Point(double.NaN, double.NaN);
                else return IntersectYY(equation1, equation2);
            }

            if (type1 == LinearEquationType.X) return IntersectYX(equation2, equation1);
            else return IntersectYX(equation1, equation2);

            Point IntersectYY(LinearEquation yEquation1, LinearEquation yEquation2)
            {
                double x = (yEquation2.B - yEquation1.B) / (yEquation1.A - yEquation2.A);
                double y = yEquation1.YFunction(x);

                return new Point(x, y);
            }

            Point IntersectYX(LinearEquation yEquation, LinearEquation xEquation) => new Point(xEquation.K, yEquation.YFunction(xEquation.K));
        }

        public override string ToString() => Type == LinearEquationType.X ? $"x = {K}" : "y = " + (A != 0 ? $"{A}x" : string.Empty) + (B != 0 ? $" + {B}" : string.Empty);

        /// <summary>
        /// Calcule l'équation d'une droite passant par deux points
        /// </summary>
        /// <param name="point1">Premier point de la droite</param>
        /// <param name="point2">Second point de la droite</param>
        /// <returns>Équation de la droite passant par les deux points</returns>
        public static LinearEquation FromPoints(Point point1, Point point2)
        {
            if (point1.X == point2.X) return new LinearEquation(point1.X);

            double a = (point1.Y - point2.Y) / (point1.X - point2.X);
            double b = point1.Y - a * point1.X;

            return new LinearEquation(a, b);
        }

        /// <summary>
        /// Calcule l'équation d'une droite passant par un point et parallèle à une autre
        /// </summary>
        /// <param name="equation">Équation de la droite parallèle</param>
        /// <param name="point">Point de la droite</param>
        /// <returns>Équation de la droite et parallèle à l'autre et passant par le point</returns>
        public static LinearEquation Parallel(LinearEquation equation, Point point)
        {
            if (equation.Type == LinearEquationType.X) return new LinearEquation(point.X);
            else
            {
                double a = equation.A;
                double b = point.Y - a * point.X;
                return new LinearEquation(a, b);
            }
        }

        /// <summary>
        /// Calcule l'équation d'une droite passant par un point et perpendiculaire à une autre
        /// </summary>
        /// <param name="equation">Équation de la droite perpendiculaire</param>
        /// <param name="point">Point de la droite</param>
        /// <returns>Équation de la droite perpendiculaire à l'autre et passant par le point</returns>
        public static LinearEquation Perpendicular(LinearEquation equation, Point point)
        {
            if (equation.Type == LinearEquationType.X) return new LinearEquation(0.0, point.Y);
            else
            {
                double a = equation.A;
                if (a == 0.0) return new LinearEquation(point.X, 0.0);

                a = -1 / a;
                double b = point.Y - a * point.X;
                return new LinearEquation(a, b);
            }
        }
    }

    /// <summary>
    /// Forme d'une équation de droite
    /// </summary>
    public enum LinearEquationType
    {
        /// <summary>
        /// Forme x=k
        /// </summary>
        X,

        /// <summary>
        /// Forme y=ax+b
        /// </summary>
        Y
    }
}
