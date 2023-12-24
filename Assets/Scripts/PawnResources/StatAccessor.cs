using System.Threading;

namespace TagFighter.Resources
{
    public interface IStatAccessor
    {
        public void AddCurrentModifier<TResource, TUnitType>(TResource resource, Unit<TUnitType> value) where TResource : Resource<TUnitType> where TUnitType : IUnitType;
        public void AddCapacityModifier<TResource, TUnitType>(TResource resource, Unit<TUnitType> value) where TResource : Resource<TUnitType> where TUnitType : IUnitType;
    }

    public class StatModifierAccessor : IStatAccessor
    {
        CancellationToken _cancellationToken = CancellationToken.None;

        public static StatModifierAccessor Permanent = new(CancellationToken.None);

        public StatModifierAccessor(CancellationToken cancellationToken) {
            _cancellationToken = cancellationToken;
        }
        public void AddCurrentModifier<TResource, TUnitType>(TResource resource, Unit<TUnitType> value) where TResource : Resource<TUnitType> where TUnitType : IUnitType {
            var modifier = new StatModifier<TUnitType> {
                Amount = value
            };
            resource.AddCurrentModifier(modifier, _cancellationToken);
        }
        public void AddCapacityModifier<TResource, TUnitType>(TResource resource, Unit<TUnitType> value) where TResource : Resource<TUnitType> where TUnitType : IUnitType {
            var modifier = new StatModifier<TUnitType> {
                Amount = value
            };
            resource.AddCapacityModifier(modifier, _cancellationToken);
        }
    }
}