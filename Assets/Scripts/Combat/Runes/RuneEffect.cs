namespace TagFighter.Effects
{
    using System.Collections.Generic;
    using UnityEngine;

    public class RuneEffect : ScriptableObject
    {
        public string Guid;
        [SerializeField]
        List<EffectStepNode> _nodes = new();
        public List<EffectStepNode> Nodes {
            get { return _nodes; }
        }

        public void AddNode(EffectStepNode node) {
            _nodes.Add(node);
        }

        public void DeleteNode(EffectStepNode node) {
            _nodes.Remove(node);

        }

    }
}