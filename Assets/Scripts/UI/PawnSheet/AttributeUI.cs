using TagFighter.Resources;
using TMPro;
using UnityEngine;

namespace TagFighter.UI
{
    public class AttributeUI : MonoBehaviour, IResourceViewer
    {
        [SerializeField] TextMeshProUGUI _name;
        [SerializeField] TextMeshProUGUI _currentTotal;
        [SerializeField] TextMeshProUGUI _capacityTotal;
        [SerializeField] TextMeshProUGUI _currentBase;
        [SerializeField] TextMeshProUGUI _capacityBase;
        [SerializeField] TextMeshProUGUI _currentModifier;
        [SerializeField] TextMeshProUGUI _capacityModifier;

        [SerializeField] Color _naturalColor = Color.grey;
        [SerializeField] Color _positiveColor = Color.blue;
        [SerializeField] Color _negativeColor = Color.magenta;

        protected void OnValidate() {
            _name ??= transform.Find("Title/Name").GetComponent<TextMeshProUGUI>();
            _currentTotal ??= transform.Find("Current/Total").GetComponent<TextMeshProUGUI>();
            _capacityTotal ??= transform.Find("Capacity/Total").GetComponent<TextMeshProUGUI>();

            _currentBase ??= transform.Find("Current/Base").GetComponent<TextMeshProUGUI>();
            _capacityBase ??= transform.Find("Capacity/Base").GetComponent<TextMeshProUGUI>();

            _currentModifier ??= transform.Find("Current/Modifier").GetComponent<TextMeshProUGUI>();
            _capacityModifier ??= transform.Find("Capacity/Modifier").GetComponent<TextMeshProUGUI>();
        }

        public AttributeUI SetName(string attributeName) {
            _name.text = attributeName;
            return this;
        }

        public void UpdateView(ResourceChangeArgs e) {
            _currentTotal.text = e.Current.ToString();
            _capacityTotal.text = e.Capacity.ToString();
            _currentBase.text = e.CurrentBase.ToString();
            _capacityBase.text = e.CapacityBase.ToString();
            _currentModifier.text = e.CurrentModifier.ToString();
            _capacityModifier.text = e.CapacityModifier.ToString();

            _currentTotal.color = GetColorForValue(e.CurrentModifier);
            _currentBase.color = GetColorForValue(e.CurrentBase);
            _currentModifier.color = _currentTotal.color;

            _capacityTotal.color = GetColorForValue(e.CapacityModifier);
            _capacityBase.color = GetColorForValue(e.CapacityBase);
            _capacityModifier.color = _capacityTotal.color;
        }

        Color GetColorForValue(int value) => value switch {
            < 0 => _negativeColor,
            > 0 => _positiveColor,
            _ => _naturalColor
        };
    }
}