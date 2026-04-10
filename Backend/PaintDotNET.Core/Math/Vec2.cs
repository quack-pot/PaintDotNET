namespace PaintDotNET.Core.Math;

public struct Vec2
{
    public float X;
    public float Y;

    // *=================================================
    // *
    // * Constructors
    // *
    // *=================================================

    public Vec2() => (X, Y) = (0.0f, 0.0f);
    public Vec2(float x, float y) => (X, Y) = (x, y);
    public Vec2(in Vec2 other) => (X, Y) = (other.X, other.Y);

    // *=================================================
    // *
    // * Simple Math Copy-Operators
    // *
    // *=================================================

    public static Vec2 operator +(in Vec2 a, in Vec2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vec2 operator -(in Vec2 a, in Vec2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Vec2 operator *(in Vec2 vec, float scalar) => new(scalar * vec.X, scalar * vec.Y);
    public static Vec2 operator *(float scalar, in Vec2 vec) => new(scalar * vec.X, scalar * vec.Y);
    public static Vec2 operator /(in Vec2 vec, float scalar) => new(vec.X / scalar, vec.Y / scalar);

    // *=================================================
    // *
    // * Simple Math Assignment-Operators
    // *
    // *=================================================

    public void operator +=(in Vec2 other) { X += other.X; Y += other.Y; }
    public void operator -=(in Vec2 other) { X -= other.X; Y -= other.Y; }
    public void operator *=(float scalar) { X *= scalar; Y *= scalar; }
    public void operator /=(float scalar) { X /= scalar; Y /= scalar; }

    // *=================================================
    // *
    // * Misc. Math Methods
    // *
    // *=================================================

    public readonly float Dot(in Vec2 other) => (X * other.X) + (Y * other.Y);
    public readonly float LengthSquared() => Dot(this);
    public readonly float Length() => MathF.Sqrt(LengthSquared());

    public void Normalize()
    {
        float magnitude = Length();

        if (magnitude == 0.0f)
        {
            return;
        }

        this /= magnitude;
    }

    public readonly Vec2 Normalized()
    {
        Vec2 result = new(X, Y);
        result.Normalize();
        return result;
    }

    // *=================================================
    // *
    // * Conditional Methods
    // *
    // *=================================================

    public void Clamp(float min_x, float max_x, float min_y, float max_y)
    {
        X = MathGen.Clamp(X, min_x, max_x);
        Y = MathGen.Clamp(Y, min_y, max_y);
    }
}
