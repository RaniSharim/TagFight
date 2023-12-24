using System;
using System.Collections.Generic;
using UnityEngine;

public class PartyCompositionArgs : EventArgs
{
    public IEnumerable<PartyMember> PartyMembers { get; }
    public PartyCompositionArgs(List<PartyMember> partyMembers) {
        PartyMembers = partyMembers;
    }
}
public class Party : MonoBehaviour
{
    List<PartyMember> _partyMembers;
    public event EventHandler<PartyCompositionArgs> OnCompositionChanged;
    bool _shouldUpdate;

    protected void Awake() {
        _partyMembers = new List<PartyMember>();
        _shouldUpdate = false;
    }
    protected void Start() {
        InitParty();
    }

    void InitParty() {
        foreach (Transform child in transform) {
            if (child.gameObject.activeSelf) {
                if (child.TryGetComponent(out PartyMember partyMember)) {
                    _partyMembers.Add(partyMember);
                }
            }
        }
        _shouldUpdate = true;
    }

    protected void Update() {
        if (_shouldUpdate) {
            OnCompositionChanged?.Invoke(this, new PartyCompositionArgs(_partyMembers));
            _shouldUpdate = false;
        }
    }
}
