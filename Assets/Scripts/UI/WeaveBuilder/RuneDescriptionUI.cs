using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TagFighter.UI
{
    public class RuneDescriptionUI : MonoBehaviour
    {
        [UnityEngine.Serialization.FormerlySerializedAs("runeName")]
        [SerializeField] TextMeshProUGUI _runeName;

        [UnityEngine.Serialization.FormerlySerializedAs("runeCastTime")]
        [SerializeField] TextMeshProUGUI _runeCastTime;

        [UnityEngine.Serialization.FormerlySerializedAs("runeImage")]
        [SerializeField] Image _runeImage;

        public void OnRuneLeftClicked(RuneRef runeRef) {
            _runeImage.sprite = runeRef.RuneSprite;
            _runeName.text = runeRef.Rune.DisplayName;
            _runeCastTime.text = runeRef.Rune.Speed.ToString() + " Seconds";
        }
    }
}