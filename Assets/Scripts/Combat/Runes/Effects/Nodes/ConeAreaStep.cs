
namespace TagFighter.Effects
{
    using System.Collections.Generic;

    public class ConeAreaStep : SetterNode, ISetterStep
    {
        [UnityEngine.SerializeField]
        SinglePort<SingleGetterStep> _radius = new() {
            DisplayName = "Radius"
        };

        [UnityEngine.SerializeField]
        SinglePort<SingleGetterStep> _angle = new() {
            DisplayName = "Angle"
        };

        public override IEnumerable<IPort<EffectStepNode>> Inputs {
            get {
                yield return _radius;
                yield return _angle;
            }
        }

        public override string ShortName => "Cone AOE";

        public override void Set() {
            Data.Context.AreaOfEffect = new ConeArea() {
                Radius = (float)_radius.Node.Get(),
                Angle = (float)_angle.Node.Get(),
            };
        }
    }
}