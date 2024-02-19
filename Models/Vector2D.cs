using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticFieldEpidEval
{
    public struct Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void ScaleToUnitLength()
        {
            double length = Length;
            if (length > 0.0)
            {
                X /= length;
                Y /= length;
            }
            else
            {
                X = 1.0;
                Y = 0.0;
            }
        }
        public double LengthSquared => X * X + Y * Y;

        public static Vector2D Normalize(Vector2D vector)
        {
            double length = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            return new Vector2D(vector.X / length, vector.Y / length);
        }
        public double Length => Math.Sqrt(LengthSquared);

        public static double Distance(Vector2D v1, Vector2D v2)
        {
            double dx = v2.X - v1.X;
            double dy = v2.Y - v1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static Vector2D operator *(Vector2D val, double mul)
        {
            return new Vector2D(val.X * mul, val.Y * mul);
        }

        public static Vector2D operator +(Vector2D left, Vector2D right)
        {
            return new Vector2D(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2D operator -(Vector2D left, Vector2D right)
        {
            return new Vector2D(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2D RotateVector(Vector2D vector, double angleInDegrees)
        {
            // Convert the angle to radians
            double angleInRadians = Math.PI * angleInDegrees / 180.0;

            // Calculate the rotated coordinates
            double rotatedX = vector.X * Math.Cos(angleInRadians) - vector.Y * Math.Sin(angleInRadians);
            double rotatedY = vector.X * Math.Sin(angleInRadians) + vector.Y * Math.Cos(angleInRadians);

            return new Vector2D(rotatedX, rotatedY);
        }
    }
}


