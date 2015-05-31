using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecLib
{
    public class vec2
    {
        public int x;
        public int y;
        public vec2() { }
        public vec2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static vec2 operator +(vec2 a, vec2 b)
        {
            return new vec2(a.x + b.x, a.y + b.y);
        }
        public static vec2 operator -(vec2 a, vec2 b)
        {
            return new vec2(a.x - b.x, a.y - b.y);
        }
        public static vec2 operator *(vec2 a, int scalar)
        {
            return new vec2(a.x * scalar, a.y * scalar);
        }
        public static vec2 operator *(vec2 a, float scalar)
        {
            return new vec2((int)(a.x * scalar), (int)(a.y * scalar));
        }
        public static vec2 operator *(vec2 a, double scalar)
        {
            return new vec2((int)(a.x * scalar), (int)(a.y * scalar));
        }
        public static double operator *(vec2 a, vec2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static vec2 operator /(vec2 a, int scalar)
        {
            return new vec2(a.x / scalar, a.y / scalar);
        }
        public static vec2 operator /(vec2 a, float scalar)
        {
            return new vec2((int)(a.x / scalar), (int)(a.y / scalar));
        }
        public static vec2 operator /(vec2 a, double scalar)
        {
            return new vec2((int)(a.x / scalar), (int)(a.y / scalar));
        }

        public double magnitude()
        {
            return Math.Sqrt(x * x + y * y);
        }
        public double angle()
        {
            return Math.Atan2(y, x);
        }

        public static vec2 operator -(vec2 a)
        {
            return new vec2(-a.x, -a.y);
        }
        public static double operator !(vec2 a)
        {
            return a.magnitude();
        }
        public static double operator ~(vec2 a)
        {
            return a.angle();
        }

        public vec2 rotate(double r)
        {
            return new vec2((int)(x * Math.Cos(r) - y * Math.Sin(r)), (int)(x * Math.Sin(r) + y * Math.Cos(r)));
        }

        public static readonly vec2 zero = new vec2(0, 0);
    }

    public class vec2f
    {
        public float x;
        public float y;
        public vec2f() { }
        public vec2f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static vec2f operator +(vec2f a, vec2f b)
        {
            return new vec2f(a.x + b.x, a.y + b.y);
        }
        public static vec2f operator -(vec2f a, vec2f b)
        {
            return new vec2f(a.x - b.x, a.y - b.y);
        }
        public static vec2f operator *(vec2f a, int scalar)
        {
            return new vec2f(a.x * scalar, a.y * scalar);
        }
        public static vec2f operator *(vec2f a, float scalar)
        {
            return new vec2f((a.x * scalar), (a.y * scalar));
        }
        public static vec2f operator *(vec2f a, double scalar)
        {
            return new vec2f((float)(a.x * scalar), (float)(a.y * scalar));
        }
        public static double operator *(vec2f a, vec2f b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static vec2f operator /(vec2f a, int scalar)
        {
            return new vec2f(a.x / scalar, a.y / scalar);
        }
        public static vec2f operator /(vec2f a, float scalar)
        {
            return new vec2f((a.x / scalar), (a.y / scalar));
        }
        public static vec2f operator /(vec2f a, double scalar)
        {
            return new vec2f((float)(a.x / scalar), (float)(a.y / scalar));
        }

        public double magnitude()
        {
            return Math.Sqrt(x * x + y * y);
        }
        public double angle()
        {
            return Math.Atan2(y, x);
        }

        public static vec2f operator -(vec2f a)
        {
            return new vec2f(-a.x, -a.y);
        }
        public static double operator !(vec2f a)
        {
            return a.magnitude();
        }
        public static double operator ~(vec2f a)
        {
            return a.angle();
        }

        public vec2f rotate(double r)
        {
            return new vec2f((float)(x * Math.Cos(r) - y * Math.Sin(r)), (float)(x * Math.Sin(r) + y * Math.Cos(r)));
        }

        public static readonly vec2f zero = new vec2f(0, 0);
    }
  
    public class vec2d
    {
        public double x;
        public double y;
        public vec2d() { }
        public vec2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static vec2d operator +(vec2d a, vec2d b)
        {
            return new vec2d(a.x + b.x, a.y + b.y);
        }
        public static vec2d operator -(vec2d a, vec2d b)
        {
            return new vec2d(a.x - b.x, a.y - b.y);
        }
        public static vec2d operator *(vec2d a, int scalar)
        {
            return new vec2d(a.x * scalar, a.y * scalar);
        }
        public static vec2d operator *(vec2d a, float scalar)
        {
            return new vec2d((a.x * scalar), (a.y * scalar));
        }
        public static vec2d operator *(vec2d a, double scalar)
        {
            return new vec2d((a.x * scalar), (a.y * scalar));
        }
        public static double operator *(vec2d a, vec2d b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static vec2d operator /(vec2d a, int scalar)
        {
            return new vec2d(a.x / scalar, a.y / scalar);
        }
        public static vec2d operator /(vec2d a, float scalar)
        {
            return new vec2d((a.x / scalar), (a.y / scalar));
        }
        public static vec2d operator /(vec2d a, double scalar)
        {
            return new vec2d((a.x / scalar), (a.y / scalar));
        }

        public double magnitude()
        {
            return Math.Sqrt(x * x + y * y);
        }
        public double angle()
        {
            return Math.Atan2(y, x);
        }

        public static vec2d operator -(vec2d a)
        {
            return new vec2d(-a.x, -a.y);
        }
        public static double operator !(vec2d a)
        {
            return a.magnitude();
        }
        public static double operator ~(vec2d a)
        {
            return a.angle();
        }

        public vec2d rotate(double r)
        {
            return new vec2d((x * Math.Cos(r) - y * Math.Sin(r)), (x * Math.Sin(r) + y * Math.Cos(r)));
        }

        public static readonly vec2d zero = new vec2d(0, 0);
    }
}
