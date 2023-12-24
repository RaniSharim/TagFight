
using TagFighter.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace TagFighter.UI
{
    public class TagViewer : MonoBehaviour, IResourceViewer
    {
        Image _foreground;
        Transform _background;
        protected void Awake() {
            _background = transform.Find("Background");
            _foreground = transform.Find("Foreground").GetComponent<Image>();
        }
        public void UpdateView(ResourceChangeArgs e) {
            _background.gameObject.SetActive(e.Current != 0);
            _foreground.fillAmount = (float)e.Current / e.Capacity;
        }
    }
}