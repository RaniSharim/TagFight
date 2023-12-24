using System;
using System.Collections.Generic;
using UnityEngine;


public enum TagColor
{
    Red, Green, Blue

}

public class TagSlot
{
    public TagColor Color;
    public float Amount;

}

public class TagCounter
{
    TagSlot[] _currentTags;
    public const float MaxAmount = 9;
    public TagCounter() {
        _currentTags = new TagSlot[Enum.GetNames(typeof(TagColor)).Length];
        for (var i = 0; i < _currentTags.Length; i++) {
            var tagSlot = new TagSlot {
                Color = (TagColor)i,
                Amount = 0
            };
            _currentTags[i] = tagSlot;
        }
    }

    public IEnumerable<TagSlot> AsEnumerable() => _currentTags;

    public float this[string colorName] {
        get {
            var isValidEnum = Enum.TryParse(colorName, out TagColor color);
            if (!isValidEnum) {
                throw new IndexOutOfRangeException();
            }
            return this[color];
        }
        set {
            var isValidEnum = Enum.TryParse(colorName, out TagColor color);
            if (!isValidEnum) {
                throw new IndexOutOfRangeException();
            }
            this[color] = value;
        }
    }

    public float this[TagColor color] {
        get {
            var slotNum = (int)(color);
            return _currentTags[slotNum].Amount;
        }
        set {
            var slotNum = (int)(color);
            _currentTags[slotNum].Amount = value;
        }
    }

    public float AddTagSlot(TagSlot slot) {
        Mathf.Clamp(this[slot.Color] + slot.Amount, 0, 9);
        this[slot.Color] = Mathf.Clamp(this[slot.Color] + slot.Amount, 0, MaxAmount);
        return this[slot.Color];
    }
}

