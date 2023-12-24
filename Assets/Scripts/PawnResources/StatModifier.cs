
namespace TagFighter.Resources
{
    public interface IStatModifier<TUnitType> where TUnitType : IUnitType
    {
        public Unit<TUnitType> Amount { get; set; }
    }
    public class StatModifier<TUnitType> : IStatModifier<TUnitType> where TUnitType : IUnitType
    {
        public Unit<TUnitType> Amount { get; set; }

    }
}