using CareBoo.Serially;
using UnityEngine;

[ExecuteAlways]
public class SystemsHandler : MonoBehaviour
{

    [SerializeReference, ShowSerializeReference]
    IEffectSystem _effectSystem = new DummyEffectSystem();

    static SystemsHandler s_handler;

    public static IEffectSystem EffectSystem {
        get {
            return s_handler._effectSystem;
        }
    }

    protected void Awake() {
        s_handler = this;
    }

    // Start is called before the first frame update
    protected void Start() {

    }

    // Update is called once per frame
    protected void Update() {

    }
}
