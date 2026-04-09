using PaintDotNET.Core.Math;

namespace PaintDotNET.Tests.Core.Math;

public class TestMathGen
{
    [Fact]
    public void TestGenericMin()
    {
        Assert.Equal(-256, MathGen.Min(5, -256));
        Assert.Equal(3, MathGen.Min(3, 19));
        Assert.Equal(-573, MathGen.Min(-573, -573));

        Assert.Equal(5u, MathGen.Min(5u, 256u));
        Assert.Equal(3u, MathGen.Min(19u, 3u));
        Assert.Equal(573u, MathGen.Min(573u, 573u));

        Assert.Equal(5.3f, MathGen.Min(5.3f, 256.5f));
        Assert.Equal(3.98f, MathGen.Min(19.008f, 3.98f));
        Assert.Equal(573.1294f, MathGen.Min(573.1294f, 573.1294f));

        Assert.Equal(5.3, MathGen.Min(5.3, 256.5));
        Assert.Equal(3.98, MathGen.Min(19.008, 3.98));
        Assert.Equal(573.1294, MathGen.Min(573.1294, 573.1294));
    }

    [Fact]
    public void TestGenericMax()
    {
        Assert.Equal(5, MathGen.Max(5, -256));
        Assert.Equal(19, MathGen.Max(3, 19));
        Assert.Equal(-573, MathGen.Max(-573, -573));

        Assert.Equal(256u, MathGen.Max(5u, 256u));
        Assert.Equal(19u, MathGen.Max(19u, 3u));
        Assert.Equal(573u, MathGen.Max(573u, 573u));

        Assert.Equal(256.5f, MathGen.Max(5.3f, 256.5f));
        Assert.Equal(19.008f, MathGen.Max(19.008f, 3.98f));
        Assert.Equal(573.1294f, MathGen.Max(573.1294f, 573.1294f));

        Assert.Equal(256.5, MathGen.Max(5.3, 256.5));
        Assert.Equal(19.008, MathGen.Max(19.008, 3.98));
        Assert.Equal(573.1294, MathGen.Max(573.1294, 573.1294));
    }

    [Fact]
    public void TestGenericClamp()
    {
        Assert.Equal(0, MathGen.Clamp(0, -10, 37));
        Assert.Equal(-10, MathGen.Clamp(-15, -10, 37));
        Assert.Equal(37, MathGen.Clamp(38, -10, 37));

        Assert.Equal(13u, MathGen.Clamp(13u, 5u, 47u));
        Assert.Equal(5u, MathGen.Clamp(0u, 5u, 47u));
        Assert.Equal(47u, MathGen.Clamp(50u, 5u, 47u));

        Assert.Equal(0.0f, MathGen.Clamp(0.0f, -10.525f, 37.894f));
        Assert.Equal(-10.525f, MathGen.Clamp(-15.0f, -10.525f, 37.894f));
        Assert.Equal(37.894f, MathGen.Clamp(38.5f, -10.525f, 37.894f));

        Assert.Equal(0.0, MathGen.Clamp(0.0, -10.525, 37.894));
        Assert.Equal(-10.525, MathGen.Clamp(-15.0, -10.525, 37.894));
        Assert.Equal(37.894, MathGen.Clamp(38.5, -10.525, 37.894));
    }
}
