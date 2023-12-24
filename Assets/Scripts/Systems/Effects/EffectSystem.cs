using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CareBoo.Serially;
using TagFighter.Resources;
using UnityEngine;

[ProvideSourceInfo]
[Serializable]
public class EffectSystem : IEffectSystem
{
    ParticleSystem _rippleOutVfx;
    [SerializeField] TagFighter.Effects.EffectColors _colorMapping;

    const string PulseMaterial = "Materials/PulseMaterial2";

    class EffectRunner : MonoBehaviour
    {
        public GameObject EffectObj;
        public Material Material;
        public float PlayTime;
        public void Run(GameObject effectObj, Material material, float playTime) {
            EffectObj = effectObj;
            Material = material;
            PlayTime = playTime;

            Material.SetFloat("_Phase", 0);
            EffectObj.SetActive(true);

            StartCoroutine("PlayEffect");
        }

        public IEnumerator PlayEffect() {
            float totalTime = 0;
            float phase = 0;

            while (phase < 0.98) {
                totalTime += Time.deltaTime;
                phase = totalTime / PlayTime;
                Material.SetFloat("_Phase", phase);
                yield return null;
            }
            Destroy(EffectObj);
        }
    }


    public EffectSystem() {
    }

    public void ApplyTagsEffect(IEnumerable<(Type, IUnit)> tags, Transform origin, Quaternion direction, IAreaOfEffect areaOfEffect) {
        switch (areaOfEffect) {
            case SingleTarget: ApplyTagsEffectSingle(); break;
            case CircleArea aoe: ApplyTagsEffectRadius(tags, origin, direction, aoe); break;
            case ConeArea aoe: ApplyTagsEffectCone(tags, origin, direction, aoe); break;
            case PathArea aoe: ApplyTagsEffectPath(tags, origin, direction, aoe); break;
        }
    }

    void ApplyTagsEffectSingle() {
        Debug.Log("ApplyTagsEffectSingle");

    }

    void ApplyTagsEffectPath(IEnumerable<(Type, IUnit)> tags, Transform origin, Quaternion direction, PathArea areaOfEffect) {
        var color = GetEffectColor(tags);

        var mesh = CreateQuadMeshForUnit(origin, areaOfEffect.Width, areaOfEffect.Length);

        // Add a new to clone the shared resource
        Material material = new(Resources.Load<Material>(PulseMaterial));
        material.SetColor("_Color", color);

        DisplayEffect(origin, direction, mesh, material);
    }
    void ApplyTagsEffectRadius(IEnumerable<(Type, IUnit)> tags, Transform origin, Quaternion direction, CircleArea areaOfEffect) {
        var color = GetEffectColor(tags);

        var mesh = CreateArcMeshForUnit(origin, 360, areaOfEffect.Radius);

        // Add a new to clone the shared resource
        Material material = new(Resources.Load<Material>(PulseMaterial));
        material.SetColor("_Color", color);

        DisplayEffect(origin, direction, mesh, material);
    }

    void ApplyTagsEffectCone(IEnumerable<(Type, IUnit)> tags, Transform origin, Quaternion direction, ConeArea areaOfEffect) {
        Debug.Log("ApplyTagsEffectCone");
        var color = GetEffectColor(tags);

        var mesh = CreateArcMeshForUnit(origin, areaOfEffect.Angle, areaOfEffect.Radius);
        // Add a new to clone the shared resource
        Material material = new(Resources.Load<Material>(PulseMaterial));
        material.SetColor("_Color", color);

        DisplayEffect(origin, direction, mesh, material);
    }

    public Color GetEffectColor(IEnumerable<(Type, IUnit)> tags) {
        Debug.Log("GetEffectColor");

        var color = new Color();

        Debug.Log($"GetEffectColor length {tags.Count()}");

        if (_colorMapping != null) {
            foreach (var tag in tags) {
                Debug.Log("GetEffectColor tag");

                if (_colorMapping.ContainsKey(tag.Item1)) {
                    Debug.Log("GetEffectColor mapping found");
                    var mappedColor = _colorMapping[tag.Item1];
                    // TODO: scale color by amount
                    color += mappedColor;
                }
            }
        }

        Debug.Log("GetEffectColor Finished");


        return color;
    }

    public Mesh CreateArcMeshForUnit(Transform unit, float arc, float length, int numberOfVerticesInArc = 100) {
        return CreateArcMesh(unit.forward, arc, length, numberOfVerticesInArc, Vector3.up);
    }

    public Mesh CreateQuadMeshForUnit(Transform unit, float width, float length) {
        return CreateQuadMesh(unit.forward, width, length, Vector3.up);
    }


    void DisplayEffect(Transform originUnit, Quaternion direction, Mesh mesh, Material material) {
        GameObject effectObj = new();
        effectObj.SetActive(false);
        effectObj.transform.position = originUnit.position;
        effectObj.transform.rotation = direction;

        var meshRenderer = effectObj.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;
        meshRenderer.shadowCastingMode = 0;
        meshRenderer.receiveShadows = false;

        var meshFilter = effectObj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        effectObj.AddComponent<EffectRunner>();
        var effectRunner = effectObj.GetComponent<EffectRunner>();
        effectRunner.Run(effectObj, material, 1f);
    }



    public Mesh CreateArcMesh(Vector3 direction, float arc, float length, int numberOfVerticesInArc, Vector3 rotationAxis) {

        var origin = Vector3.zero;

        Mesh mesh = new();

        var n = numberOfVerticesInArc + 1;


        var arcStep = arc / (n - 2);
        var currentDegrees = -arc / 2;

        var vertices = new Vector3[n];
        var uv = new Vector2[n];

        vertices[0] = origin;
        uv[0] = origin;

        for (var i = 1; i < n; i++) {
            vertices[i] = Quaternion.AngleAxis(currentDegrees, rotationAxis) * direction * length;
            uv[i] = new Vector2(0, 1);

            //Debug.Log($"Current Degrees: {currentDegrees} -> {vertices[i]}");
            currentDegrees += arcStep;
        }
        mesh.vertices = vertices;
        mesh.uv = uv;

        var tris = new int[(n - 2) * 3];
        for (var i = 0; i < n - 2; i++) {
            tris[i * 3] = 0;
            tris[i * 3 + 1] = i + 1;
            tris[i * 3 + 2] = i + 2;
        }

        mesh.triangles = tris;

        var normals = new Vector3[n];
        for (var i = 0; i < n; i++) {
            normals[i] = rotationAxis;
        }
        mesh.normals = normals;

        return mesh;
    }

    public Mesh CreateQuadMesh(Vector3 direction, float width, float length, Vector3 rotationAxis) {

        Mesh mesh = new();
        var origin = Vector3.zero;


        // TODO: how to calculate local rotation? direction
        var vertices = new Vector3[4]
        {
            new Vector3(-width/2, 0, 0) + origin,
            new Vector3(width/2, 0, 0) + origin,
            new Vector3(-width/2, 0, length) + origin,
            new Vector3(width/2, 0, length) + origin
        };
        mesh.vertices = vertices;

        var tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        var normals = new Vector3[4]
        {
            rotationAxis,
            rotationAxis,
            rotationAxis,
            rotationAxis
        };
        mesh.normals = normals;

        var uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;


        return mesh;
    }

}
