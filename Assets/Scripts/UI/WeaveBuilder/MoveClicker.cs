using TagFighter.Martial;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TagFighter.UI
{
    [RequireComponent(typeof(Image))]
    public class MoveClicker : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] MovePicker _movePicker;

        public CombatMoveRef MoveRef { get; set; }
        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    HandleMoveViewInfo();
                    break;
                case PointerEventData.InputButton.Right:
                    HandleMoveAddToSequence();
                    break;
            }
        }

        void HandleMoveAddToSequence() {
            _movePicker.OnMoveAddToSequence(this);
        }

        void HandleMoveViewInfo() {
            _movePicker.OnSelectedMove(this);
        }
    }
}