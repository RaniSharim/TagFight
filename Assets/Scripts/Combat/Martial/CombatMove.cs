using System;

[Serializable]
public class CombatMove
{
    public string MoveName;
    public float Damage;
    public float Speed;
    public Type MatchingAoe;
    public float MatchingAoeReleaseMultiplier;
    public float NonMatchingAoeReleaseMultiplier;
    public float StartReleaseNormalized;
    public float EndReleaseNormalized;
}
