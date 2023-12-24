using System.Collections.Generic;
using TagFighter.Resources;
using UnityEngine;
using System;

public interface IEffectSystem
{
    void ApplyTagsEffect(IEnumerable<(Type, IUnit)> tags, Transform origin, Quaternion direction, IAreaOfEffect areaOfEffect);
    Color GetEffectColor(IEnumerable<(Type, IUnit)> tags);
    Mesh CreateArcMesh(Vector3 direction, float arc, float length, int numberOfVerticesInArc, Vector3 rotationAxis);
    Mesh CreateQuadMesh(Vector3 direction, float width, float length, Vector3 rotationAxis);
}
