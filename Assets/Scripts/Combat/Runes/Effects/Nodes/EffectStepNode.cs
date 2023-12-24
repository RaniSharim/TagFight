
namespace TagFighter.Effects
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    public enum PortCapacity
    {
        None = 0,
        Single = 1,
        Multi = 2,
    }
    public class EffectStepValidatedEventArgs : EventArgs
    {
        public string DisplayName;
        public EffectStepValidatedEventArgs(string displayName) {
            DisplayName = displayName;
        }
    }

    /// <summary>
    /// <see cref="FoldStep"/>              Input: Single Sequence<Double>  Output: Double
    /// <see cref="ZipStep"/>               Input: Multi  Sequence<Double>  Output: Sequence<Double>
    /// <see cref="RepeatStep"/>            Input: Single Double            Output: Sequence<Double>
    /// <see cref="OperatorStep"/>          Input: Multi  Double            Output: Double
    /// 
    /// <see cref="ConstValueStep"/>        Input: None Output: Double
    /// <see cref="ContextGetterStep"/>     Input: None Output: Double
    /// <see cref="PawnGetterStep"/>        Input: None Output: Sequence<Double>
    /// 
    /// <see cref="ContextSetterStep"/>     Input: Single Double
    /// <see cref="PawnSetterStep"/>        Input: Single Sequence<Double>
    /// <see cref="ConeAreaStep"/>          Input: 2xDouble
    /// </summary>
    /// 

    public interface IOperatorStep { };
    public interface IGetterStep { };
    public interface ISetterStep { };
    public abstract class EffectStepNode : ScriptableObject
    {
        public Vector2 Position;
        public string Guid;
        public EffectInput Data;
        public event EventHandler<EffectStepValidatedEventArgs> EffectStepValidated;


        public abstract IEnumerable<IPort<EffectStepNode>> Inputs { get; }

        public abstract string ShortName { get; }
        protected virtual void OnValidate() {
            OnValidated(this, new(ShortName));
        }
        protected virtual void OnValidated(object sender, EffectStepValidatedEventArgs e) {
            EffectStepValidated?.Invoke(sender, e);
        }
    }
}