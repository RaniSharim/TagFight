using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace TagFighter.UI
{
    [RequireComponent(typeof(Image))]
    public class RuneClicker : MonoBehaviour, IPointerClickHandler
    {
        public int GridLocation { get; set; }

        [UnityEngine.Serialization.FormerlySerializedAs("runeGridParent")]
        [SerializeField] PopulateRuneGrid _runeGridParent;

        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    HandleRuneViewInfo();
                    break;
                case PointerEventData.InputButton.Right:
                    HandleRuneAddToSequence();
                    break;
            }
        }

        public void HandleRuneViewInfo() {
            _runeGridParent.OnRuneViewInfo(GridLocation);
        }

        public void HandleRuneAddToSequence() {
            _runeGridParent.OnRuneAddToSequence(GridLocation);
        }
    }
}