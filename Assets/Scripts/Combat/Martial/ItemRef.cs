// using System;
// using UnityEngine;


// namespace TagFighter.Items
// {
//     [CreateAssetMenu(fileName = "Item", menuName = "Game/Equipment/Item")]
//     [Serializable]
//     public class ItemRef : ScriptableObject
//     {
//         [SerializeField] ItemTemp _item;
//         public static implicit operator ItemTemp(ItemRef itemRef) => itemRef.Value;

//         public ItemTemp Value => _item;
//     }

// }