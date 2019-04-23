using System;
using System.Windows;
using static BenLib.Literal;

namespace BenLib
{
    /// <summary>
    /// Représente une équation cartésienne de droite
    /// </summary>
    public struct LinearEquation
    {
        /// <summary>
        /// Coefficient de x
        /// </summary>
        public double A { get; }

        /// <summary>
        /// Coefficient de y
        /// </summary>
        public double B { get; }

        /// <summary>
        /// Constante de l'équation
        /// </summary>
        public double C { get; }

        /// <summary>
        /// Crée une équation de droite de la forme : ax + by + c = 0
        /// </summary>
        /// <param name="a">Coefficient de x</param>
        /// <param name="b">Coefficient de y</param>
        /// <param name="c">Constante de l'équation</param>
        public LinearEquation(double a, double b, double c)
        {
            if (a == 0 && b == 0) A = B = C = double.NaN;
            else
            {
                A = a;
                B = b;
                C = c;
            }
        }

        /// <summary>
        /// Crée une équation de droite de la forme : y = ax + b
        /// </summary>
        /// <param name="m">Coefficient directeur de l'équation</param>
        /// <param name="p">Ordonnée à l'origine de l'équation</param>
        public LinearEquation(double m, double p)
        {
            //(y = mx + p) <=> (mx - y + p = 0)
            A = m;
            B = -1;
            C = p;
        }

        /// <summary>
        /// Crée une équation de droite de la forme : x = k
        /// </summary>
        /// <param name="k">Constante de l'équation</param>
        public LinearEquation(double k)
        {
            //(x = k) <=> (x - k = 0)
            A = 1;
            B = 0;
            C = -k;
        }

        /// <summary>
        /// Si possible, obtient l'abscisse d'un point de la droite d'ordonnée donnée
        /// </summary>
        /// <param name="y">Ordonnée du point de la droite</param>
        public double X(double y) => Intersection(this, new LinearEquation(0, y)).Y;

        /// <summary>
        /// Si possible, obtient l'ordonnée d'un point de la droite d'abscisse donnée
        /// </summary>
        /// <param name="x">Abscisse du point de la droite</param>
        public double Y(double x) => Intersection(this, new LinearEquation(x)).Y;

        /// <summary>
        /// Convertit la valeur réduite de cette instance en sa représentation équivalente sous forme de chaîne
        /// </summary>
        /// <returns>Représentation sous forme de chaîne de la valeur réduite de cette instance</returns>
        public override string ToString() => ToString(true);

        /// <summary>
        /// Convertit la valeur de cette instance en sa représentation équivalente sous forme de chaîne
        /// </summary>
        /// <returns>Représentation sous forme de chaîne de la valeur de cette instance si reduce est <see langword="false"/>, ou de la valeur réduite de cette instance si reduce est <see langword="true"/></returns>
        public string ToString(bool reduce)
        {
            if (reduce)
            {
                if (B == 0)
                {
                    double k = -C / A;
                    return $"x = {k}";
                }
                else
                {
                    double m = -A / B;
                    double p = -C / B;
                    return "y = " + CoefsToString(true, (m, "x"), (p, string.Empty));
                }
            }
            else return CoefsToString(true, (A, "x"), (B, "y"), (C, string.Empty)) + " = 0";
        }

        /// <summary>
        /// Obtient la constante d'une <see cref="LinearEquation"/> à partir de ses coefficients et d'un de ses points
        /// </summary>
        /// <param name="a">Coefficient de x</param>
        /// <param name="b">Coefficient de y</param>
        /// <param name="point">Point de la droite</param>
        private static double GetC(double a, double b, Point point) => -(a * point.X + b * point.Y);

        /// <summary>
        /// Calcule l'équation d'une droite passant par un point et dirigée par un vecteur
        /// </summary>
        /// <param name="point">Point de la droite</param>
        /// <param name="vector">Vecteur directeur de la droite</param>
        /// <returns>Équation de la droite passant par le point et dirigée par le vecteur</returns>
        public static LinearEquation FromPointVector(Point point, Vector vector)
        {
            //point a pour coordonnées (m, n) et vector a pour coordonnées (-b, a)
            //=> result : ax + by = am + bn
            double a = vector.Y;
            double b = -vector.X;
            double c = GetC(a, b, point);
            return new LinearEquation(a, b, c);
        }

        /// <summary>
        /// Calcule l'équation d'une droite passant par deux points
        /// </summary>
        /// <param name="point1">Premier point de la droite</param>
        /// <param name="point2">Second point de la droite</param>
        /// <returns>Équation de la droite passant par les deux points</returns>
        public static LinearEquation FromPoints(Point point1, Point point2) => FromPointVector(point1, point2 - point1);

        /// <summary>
        /// Calcule l'équation d'une droite passant par un point et parallèle à une autre
        /// </summary>
        /// <param name="equation">Équation de la droite parallèle</param>
        /// <param name="point">Point de la droite</param>
        /// <returns>Équation de la droite et parallèle à l'autre et passant par le point</returns>
        public static LinearEquation Parallel(LinearEquation equation, Point point)
        {
            //point a pour coordonnées (m, n)
            //=> result : ax + by = am + bn
            double a = equation.A;
            double b = equation.B;
            double c = GetC(a, b, point);
            return new LinearEquation(a, b, c);
        }

        /// <summary>
        /// Détermine si deux droites sont parallèles
        /// </summary>
        /// <param name="equation1">Équation de la première droite</param>
        /// <param name="equation2">Équation de la seconde droite</param>
        /// <returns><see langword="true"/> si les droites sont parallèles; sinon <see langword="false"/></returns>
        public static bool AreParallel(LinearEquation equation1, LinearEquation equation2) => equation1.B * equation2.A == equation1.A * equation2.B; //Les vecteurs directeurs sont-ils colinéaires ?

        /// <summary>
        /// Calcule l'équation d'une droite passant par un point et perpendiculaire à une autre
        /// </summary>
        /// <param name="equation">Équation de la droite perpendiculaire</param>
        /// <param name="point">Point de la droite</param>
        /// <returns>Équation de la droite perpendiculaire à l'autre et passant par le point</returns>
        public static LinearEquation Perpendicular(LinearEquation equation, Point point)
        {
            //point a pour coordonnées (m, n)
            //=> result : ay - bx = an - bm
            double a = -equation.B;
            double b = equation.A;
            double c = GetC(a, b, point);
            return new LinearEquation(a, b, c);
        }

        /// <summary>
        /// Détermine si deux droites sont perpendiculaires
        /// </summary>
        /// <param name="equation1">Équation de la première droite</param>
        /// <param name="equation2">Équation de la seconde droite</param>
        /// <returns><see langword="true"/> si les droites sont perpendiculaires; sinon <see langword="false"/></returns>
        public static bool ArePerpendicular(LinearEquation equation1, LinearEquation equation2) => equation1.A * equation2.A + equation1.B * equation2.B == 0; //Les vecteurs directeurs sont-ils orthogonaux ?

        /// <summary>
        /// Calcule l'intersection de deux droites avec de leurs équations
        /// </summary>
        /// <param name="equation1">Équation de la première droite</param>
        /// <param name="equation2">Équation de la seconde droite</param>
        /// <returns>Point d'intersection des deux droites</returns>
        public static Point Intersection(LinearEquation equation1, LinearEquation equation2)
        {
            if (AreParallel(equation1, equation2)) return new Point(double.NaN, double.NaN);
            //Les droites ne sont pas parallèles donc leurs vecteurs directeurs ne sont pas colinéaires
            //Donc (b * A != a * B) <=> (A != a * B / b) <=> (B != b * A / a)

            double a = equation1.A;
            double b = equation1.B;
            double c = equation1.C;

            double A = equation2.A;
            double B = equation2.B;
            double C = equation2.C;

            if (a != 0) return InterX();
            else return InterY(); //On ne peut pas avoir (A, B) == (0, 0) puisqu'une exception est levée dans ce cas

            Point InterX()
            {
                double coef = A / a;
                double y = (coef * c - C) / (B - coef * b); //Donc pas de division par 0
                double x = -(b * y + c) / a;
                return new Point(x, y);
            }

            Point InterY()
            {
                double coef = B / b;
                double x = (coef * c - C) / (A - coef * a); //Donc pas de division par 0
                double y = -(a * x + c) / b;
                return new Point(x, y);
            }
        }
    }
}
