using System;
using System.Runtime.InteropServices;
using CareBoo.Serially;

namespace TagFighter.Resources
{
    [Guid("CF53C617-F396-4C4F-9317-F935188D525C")]
    [Serializable]
    [ProvideSourceInfo]
    public sealed class Range : Resource<Centimeter> { }

    public static class CentimeterExtensions
    {
        public static float ToMeter(this Unit<Centimeter> source) {
            return source.AsPrimitive() * 0.01f;
        }
    }
}
