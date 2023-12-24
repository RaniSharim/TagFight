using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRuneBank", menuName = "Game/Combat/RuneBank")]
public class RuneBank : ScriptableObject
{
    public List<RuneRef> Runes;
}