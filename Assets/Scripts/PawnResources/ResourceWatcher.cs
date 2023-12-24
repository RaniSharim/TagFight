using CareBoo.Serially;
using TagFighter.Resources;
using UnityEngine;

namespace TagFighter.UI
{
    [RequireComponent(typeof(IResourceViewer))]
    public class ResourceWatcher : MonoBehaviour
    {
        IWatchableResource _watchedResource;

        IResourceViewer _resourceDisplay;
        static readonly ResourceChangeArgs s_default = new(0, 0, 0, 1, 0, 0, null);

        [TypeFilter(derivedFrom: typeof(IWatchableResource))]
        [SerializeField]
        SerializableType _watch;

        public IWatchableResource WatchedResource {
            get => _watchedResource;
            set {
                if (_watchedResource != null) {
                    _watchedResource.ResourceChanged -= OnChanged;
                }
                _watchedResource = value;

                var status = s_default;
                if (_watchedResource != null) {
                    _watchedResource.ResourceChanged += OnChanged;
                    status = _watchedResource.Status;
                }
                _resourceDisplay.UpdateView(status);
            }
        }
        protected void Awake() {
            _resourceDisplay = GetComponent<IResourceViewer>();
        }

        protected void Start() {
            if ((_watch.Type != null) && (WatchedResource == null)) {
                WatchedResource = GetComponentInParent(_watch.Type) is IWatchableResource resource ? resource : null;
            }
        }

        protected virtual void OnChanged(object sender, ResourceChangeArgs e) {
            _resourceDisplay.UpdateView(e);
        }

        protected void OnDestroy() {
            WatchedResource = null;
            _resourceDisplay = null;
        }

    }
}