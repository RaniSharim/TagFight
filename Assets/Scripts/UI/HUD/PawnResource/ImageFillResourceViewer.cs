using TagFighter.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace TagFighter.UI
{
    [RequireComponent(typeof(Image))]
    public class ImageFillResourceViewer : MonoBehaviour, IResourceViewer
    {
        Image _fill;
        protected void Awake() {
            _fill = GetComponent<Image>();
        }
        public void UpdateView(ResourceChangeArgs e) {
            _fill.fillAmount = (float)e.Current / e.Capacity;
        }
    }

}