using System;
using System.Collections.Generic;
using TagFighter.Martial;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AnimationClipSet", menuName = "Game/Animation/AnimationClipSet")]
public class AnimationClipSet : ScriptableObject
{
    [FormerlySerializedAs("idle")]
    public List<AnimationClip> Idle;

    [FormerlySerializedAs("run")]
    public AnimationClip Run;

    [FormerlySerializedAs("moveContainers")]
    public List<CombatMoveContainer> Attacks;

}

[Serializable]
public class CombatMoveContainer
{
    [FormerlySerializedAs("combatMove")]
    public CombatMoveRef CombatMove;

    [FormerlySerializedAs("attackAnimation")]
    public AnimationClip AttackAnimation;

    [FormerlySerializedAs("runAnimationOverride")]
    public AnimationClip RunAnimationOverride;

}