using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public interface IAreaOfEffect
{
    public IEnumerable<Transform> GetAffectedUnits(Transform origin);
    public IAreaOfEffect ShallowCopy();
}

[Serializable]
public class SingleTarget : IAreaOfEffect
{
    public IEnumerable<Transform> GetAffectedUnits(Transform origin) {
        return new Transform[] { origin };
    }
    public IAreaOfEffect ShallowCopy() {
        return (IAreaOfEffect)MemberwiseClone();
    }
}

[Serializable]
public class CircleArea : IAreaOfEffect
{
    public float Radius;
    public IEnumerable<Transform> GetAffectedUnits(Transform origin) {
        var hits = Physics.SphereCastAll(origin.position, Radius, origin.forward, 0, LayerMask.NameToLayer("units"));

        return hits.Select(x => x.transform);
    }

    public IAreaOfEffect ShallowCopy() {
        return (IAreaOfEffect)MemberwiseClone();
    }
}

[Serializable]
public class ConeArea : IAreaOfEffect
{
    public float Radius;

    [Range(0f, 360f)]
    public float Angle;

    public IEnumerable<Transform> GetAffectedUnits(Transform origin) {

        // Its problematic to cast a real cone, so we cast the sphere and later
        // calculate if the potential target is within the cone.
        var possibleHits = Physics.SphereCastAll(origin.position, Radius, origin.forward, 0, LayerMask.NameToLayer("units"));

        if (possibleHits.Length == 0) {
            return Enumerable.Empty<Transform>();
        }

        foreach (var h in possibleHits) {
            Debug.Log($"ConeArea:GetAffectedUnits possbile hit: {h.transform.gameObject}");
        }

        var hits = possibleHits.Where(h => {
            var targetDir = (h.transform.position - origin.position).normalized;
            var targetAngle = Math.Abs(Vector3.Angle(targetDir, origin.forward));
            Debug.Log($"ConeArea:GetAffectedUnits target: {h.transform.gameObject} angle: {targetAngle}");

            return targetAngle <= Angle / 2;
        });

        foreach (var h in hits) {
            Debug.Log($"ConeArea:GetAffectedUnits hit: {h.transform.gameObject}");
        }


        return hits.Select(x => x.transform);
    }

    public IAreaOfEffect ShallowCopy() {
        return (IAreaOfEffect)MemberwiseClone();
    }
}

[Serializable]
public class PathArea : IAreaOfEffect
{
    public float Width;
    public float Length;
    public IEnumerable<Transform> GetAffectedUnits(Transform origin) {

        // Get potential collisions
        var hits = Physics.BoxCastAll(origin.position, new Vector3(Width / 2, 0, 0), origin.forward, origin.rotation, Length, LayerMask.NameToLayer("units"));

        foreach (var h in hits) {
            Debug.Log($"PathArea:GetAffectedUnits hit: {h.transform.gameObject}");
        }

        return hits.Select(x => x.transform);
    }

    public IAreaOfEffect ShallowCopy() {
        return (IAreaOfEffect)MemberwiseClone();
    }
}
