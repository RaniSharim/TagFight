using System;
using System.Collections.Generic;
using System.Linq;
using CareBoo.Serially;
using TagFighter.Resources;

namespace TagFighter.Effects
{
    public interface IResourceOperator
    {
        public double Operate(params double[] values) => OperateEnum(values);
        public Unit<TUnit> Operate<TUnit>(params Unit<TUnit>[] values) where TUnit : IUnitType => OperateEnum(values);
        public Unit<TUnit> OperateEnum<TUnit>(IEnumerable<Unit<TUnit>> values) where TUnit : IUnitType => (Unit<TUnit>)OperateEnum(values.Select(value => value.AsPrimitive()));

        public double OperateEnum(IEnumerable<double> values);
        public int OperateEnum(IEnumerable<int> values);
    }

    namespace Operators
    {
        [ProvideSourceInfo]
        [Serializable]
        public class Sum : IResourceOperator
        {

            public double OperateEnum(IEnumerable<double> a) => a.Sum();

            public int OperateEnum(IEnumerable<int> a) => a.Sum();

            public override string ToString() => nameof(Sum);
        }

        [ProvideSourceInfo]
        [Serializable]
        public class Multiply : IResourceOperator
        {
            public double OperateEnum(IEnumerable<double> values) => values.Aggregate(1d, (acc, val) => acc * val);
            public int OperateEnum(IEnumerable<int> values) => values.Aggregate(1, (acc, val) => acc * val);

            public override string ToString() => nameof(Multiply);
        }

        [ProvideSourceInfo]
        [Serializable]
        public class Min : IResourceOperator
        {
            public double OperateEnum(IEnumerable<double> values) => values.Min();
            public int OperateEnum(IEnumerable<int> values) => values.Min();

            public override string ToString() => nameof(Min);
        }

        [ProvideSourceInfo]
        [Serializable]
        public class Max : IResourceOperator
        {
            public double OperateEnum(IEnumerable<double> values) => values.Max();
            public int OperateEnum(IEnumerable<int> values) => values.Max();

            public override string ToString() => nameof(Max);
        }

        [ProvideSourceInfo]
        [Serializable]
        public class Average : IResourceOperator
        {
            public double OperateEnum(IEnumerable<double> values) => values.Average();
            public int OperateEnum(IEnumerable<int> values) => (int)values.Average();

            public override string ToString() => nameof(Average);
        }
    }
}
