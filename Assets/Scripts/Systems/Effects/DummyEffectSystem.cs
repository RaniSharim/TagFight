using System.Collections.Generic;
using TagFighter.Resources;
using UnityEngine;
using System;

class DummyEffectSystem : IEffectSystem
{
    public void ApplyTagsEffect(IEnumerable<(Type, IUnit)> tags, Transform origin, Quaternion direction, IAreaOfEffect areaOfEffect) {
        Debug.Log("Dummy:ApplyTagsEffect");
    }
    public Color GetEffectColor(IEnumerable<(Type, IUnit)> tags) {
        Debug.Log("Dummy:GetEffectColor");
        return Color.cyan;
    }
    public Mesh CreateArcMesh(Vector3 direction, float arc, float length, int numberOfVerticesInArc, Vector3 rotationAxis) {
        Debug.Log("Dummy:CreateArcMesh");
        return null;
    }
    public Mesh CreateQuadMesh(Vector3 direction, float width, float length, Vector3 rotationAxis) {
        Debug.Log("Dummy:CreateQuadMesh");
        return null;
    }

}
