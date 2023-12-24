using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using TagFighter.Equipment;

namespace TagFighter
{
    [CreateAssetMenu(fileName = "AnimationClipSetBank", menuName = "Game/Animation/AnimationClipSetBank")]
    public class AnimationClipSetBank : ScriptableObject
    {
        [FormerlySerializedAs("animationClipSets")]
        public List<AnimationClipSetContainer> AnimationClipSets;
    }

    [Serializable]
    public class AnimationClipSetContainer
    {
        [FormerlySerializedAs("weapon")]
        public WeaponRef Weapon;

        public Item Item;

        [FormerlySerializedAs("clipSet")]
        public AnimationClipSet ClipSet;
    }
}