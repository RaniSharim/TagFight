
using System;
using UnityEngine;

namespace TagFighter.Resources
{
    public interface IUnit { public int AsPrimitive(); }
    [Serializable]
    public struct Unit<TUnitType> : IUnit, IEquatable<Unit<TUnitType>>, IComparable<Unit<TUnitType>> where TUnitType : IUnitType
    {
        [SerializeField] int _value;

        Unit(int value) => _value = value;
        public int AsPrimitive() => _value;
        public static explicit operator Unit<TUnitType>(int value) => new(value);
        public static explicit operator int(Unit<TUnitType> resource) => resource._value;
        public static bool operator ==(Unit<TUnitType> a, Unit<TUnitType> b) => a._value == b._value;
        public int CompareTo(Unit<TUnitType> other) => _value.CompareTo(other._value);
        public static bool operator >(Unit<TUnitType> a, Unit<TUnitType> b) => a._value > b._value;
        public static bool operator <(Unit<TUnitType> a, Unit<TUnitType> b) => a._value < b._value;
        public static bool operator !=(Unit<TUnitType> a, Unit<TUnitType> b) => !(a == b);
        public static bool operator >=(Unit<TUnitType> a, Unit<TUnitType> b) => !(a < b);
        public static bool operator <=(Unit<TUnitType> a, Unit<TUnitType> b) => !(a > b);
        public static Unit<TUnitType> operator ++(in Unit<TUnitType> a) => (Unit<TUnitType>)(a._value + 1);
        public static Unit<TUnitType> operator --(in Unit<TUnitType> a) => (Unit<TUnitType>)(a._value - 1);
        public static Unit<TUnitType> operator +(Unit<TUnitType> a, Unit<TUnitType> b) => (Unit<TUnitType>)(a._value + b._value);
        public static Unit<TUnitType> operator -(Unit<TUnitType> a, Unit<TUnitType> b) => (Unit<TUnitType>)(a._value - b._value);

        public static float operator /(Unit<TUnitType> a, Unit<TUnitType> b) => a._value / (float)b._value;
        public static Unit<TUnitType> operator /(Unit<TUnitType> a, int b) => a / (float)b;
        public static Unit<TUnitType> operator /(Unit<TUnitType> a, float b) => a / (float)b;

        public static Unit<TUnitType> operator *(Unit<TUnitType> a, float b) => (Unit<TUnitType>)(int)(a._value * b);

        public override bool Equals(object obj) => obj is Unit<TUnitType> other && _value.Equals(other._value);

        public bool Equals(Unit<TUnitType> other) => _value.Equals(other._value);

        public override int GetHashCode() => HashCode.Combine(_value);

        public override string ToString() => _value.ToString();

        public static Unit<TUnitType> Min(Unit<TUnitType> a, Unit<TUnitType> b) => (Unit<TUnitType>)Math.Min(a._value, b._value);
        public static Unit<TUnitType> Max(Unit<TUnitType> a, Unit<TUnitType> b) => (Unit<TUnitType>)Math.Max(a._value, b._value);
        public static Unit<TUnitType> Clamp(Unit<TUnitType> value, Unit<TUnitType> min, Unit<TUnitType> max) => (Unit<TUnitType>)Math.Clamp(value._value, min._value, max._value);

    }

    public interface IUnitType { }

    public sealed class PainUnit : IUnitType { }
    public sealed class FatigueUnit : IUnitType { }
    // public class StrainUnit : IUnit { }
    public sealed class RedTagUnit : IUnitType { }
    public sealed class BlueTagUnit : IUnitType { }
    public sealed class GreenTagUnit : IUnitType { }
    public sealed class Centimeter : IUnitType { }
    public sealed class RecklessFormKP : IUnitType { }
    public sealed class SwordKP : IUnitType { }
    public sealed class ShieldKP : IUnitType { }
}
