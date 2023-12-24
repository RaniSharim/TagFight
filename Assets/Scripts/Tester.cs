using System;
using System.Collections;
using TagFighter;
using TagFighter.Events;
using TagFighter.Resources;
using TagFighter.Testing.Optics;
using Testing.Actions;
using Testing.Line;
using Testing.TimeDialation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace TagFighter.Testing
{


    public class Tester : MonoBehaviour
    {
        [SerializeField] Vector3 _forceToApply;
        [SerializeField] Transform _affected;
        [SerializeField] LayerMask _unitMovementLayerMask;
        [SerializeField] ForceMode _forceMode;
        [SerializeField] EventAggregator _eventAggregator;
        [SerializeField] bool _shouldTestTags = false;
        [SerializeField] bool _shouldTestPhysics = false;
        [SerializeField] bool _shouldTestPawnConditions = false;
        [SerializeField] bool _shouldRunAutoTests = false;



        Renderer _r;
        Rigidbody _rb;
        NavMeshAgent _nma;
        bool _pushed;
        Vector3 _resetPosition;

        protected void Start() {
            if (_affected) {
                _r = _affected.GetComponent<Renderer>();
                _rb = _affected.GetComponent<Rigidbody>();
                _nma = _affected.GetComponent<NavMeshAgent>();
                _resetPosition = _affected.transform.position;
            }
            Reset();
        }
        protected void Update() {
            if (_shouldTestTags) {
                TestTags();
            }

            if (_shouldTestPhysics) {
                TestPhysics();
            }

            if (_shouldTestPawnConditions) {
                TestAddPawnCondition();
            }

            if (_shouldRunAutoTests) {
                AutoTests();
            }
        }

        void TestAddPawnCondition() {
            if (Input.GetKeyDown(KeyCode.T)) {
                var condition = _affected.gameObject.AddComponent<TagFighter.Effects.PawnCondition>();
                condition.Apply();
            }
        }

        void TestTags() {
            if (Input.GetKeyDown(KeyCode.L)) {
                var color = UnityEngine.Random.Range(0, 3);
                switch (color) {
                    case 0: {
                            var modifier = new StatModifier<BlueTagUnit> {
                                Amount = (Unit<BlueTagUnit>)1
                            };
                            _affected.GetComponent<BlueTag>().AddCurrentModifier(modifier, System.Threading.CancellationToken.None);
                            break;
                        }
                    case 1: {
                            var modifier = new StatModifier<RedTagUnit> {
                                Amount = (Unit<RedTagUnit>)1
                            };
                            _affected.GetComponent<RedTag>().AddCurrentModifier(modifier, System.Threading.CancellationToken.None);
                            break;
                        }
                    case 2: {
                            var modifier = new StatModifier<GreenTagUnit> {
                                Amount = (Unit<GreenTagUnit>)1
                            };
                            _affected.GetComponent<GreenTag>().AddCurrentModifier(modifier, System.Threading.CancellationToken.None);
                            break;
                        }
                }
            }
        }
        void AutoTests() {
            if (Input.GetKeyDown(KeyCode.T)) {
                if (Input.GetKey(KeyCode.LeftShift)) {
                    Test();
                }
            }
        }
        void TestPhysics() {
            if (Input.GetKeyDown(KeyCode.T)) {
                if (!Input.GetKey(KeyCode.LeftShift)) {
                    _pushed = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.B) && !EventSystem.current.IsPointerOverGameObject()) {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit)) {
                    var objectHit = hit.transform;
                    if (_unitMovementLayerMask.IsLayerInMask(objectHit.gameObject.layer)) {
                        _nma.Warp(hit.point);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                Reset();
            }

            if (Input.GetKeyDown(KeyCode.P)) {
                print($"Velocity ({_rb.velocity}), Magnitude {_rb.velocity.magnitude}");
                print($"world bounds {_r.bounds} local bounds {_r.localBounds}");
            }
        }
        protected void FixedUpdate() {
            if (_rb != null && !_rb.isKinematic && _nma.isOnNavMesh && _rb.velocity.sqrMagnitude < 0.25f) {
                if (Physics.Raycast(_affected.transform.position, Vector3.down, _r.bounds.extents.y + 0.1f, _unitMovementLayerMask)) {
                    _rb.isKinematic = true;
                    _nma.updatePosition = true;
                    _nma.updateRotation = true;
                    _nma.Warp(_affected.transform.position);
                }
                //nma.enabled = true;
            }

            if (_pushed) {
                // nma.enabled = false;
                _nma.updatePosition = false;
                _nma.updateRotation = false;
                _rb.isKinematic = false;
                _rb.AddForce(_forceToApply, _forceMode);
                _pushed = false;
            }
        }

        void Reset() {
            if (_rb != null) {
                _rb.isKinematic = true;
            }
            if (_nma) {
                _nma.updatePosition = true;
                _nma.updateRotation = true;
                _nma.enabled = true;
                _pushed = false;
            }
            if (_affected) {
                _affected.transform.position = _resetPosition;
            }
        }

        [ContextMenuItem("TestEffectInput", "TestEffectInputOptics")]
        [SerializeField]
        OpticsTester _opticsTester;

        [SerializeField] ActionsTester _actionsTester;
        void Test() {
            TimeDialationTester timeDialationTester = new(_eventAggregator);
            timeDialationTester.TestAll();

            LineTester lineTester = new();
            lineTester.TestAll();

            _actionsTester.TestAll();

            _opticsTester.TestAll();
        }


        void TestEffectInputOptics() {
            _opticsTester.TestEffectInput();
        }


    }
}

namespace TagFighter.Testing.Optics
{

    using System;

    [Serializable]
    public class OpticsTester
    {
        // [SerializeField] List<Transform> _transforms = new();
        // [SerializeField] EffectDataAccessor _from;
        // [SerializeField] EffectDataAccessor _to;
        // [SerializeField] ResourceInfoGet<IResourceTypeAccessor, Effects.ResourceLocationAccessors.Get.Context> _rg;
        // [SerializeField] ResourceInfoSet<IResourceTypeAccessor, Effects.ResourceLocationAccessors.Set.Context> _rs;

        public void TestAll() {
        }

        public void TestEffectInput() {
            //effectContext.SetResource<RedTag, RedTagUnit, Current>((Unit<RedTagUnit>)1);
            // var input = new EffectInput(new EffectContext(), _transforms, null);

            // foreach (var val in _from.Get(input)) {
            //     Debug.Log(val);
            // }

            // _to.Set(input, _from.Get(input));
            // _to.Set(input, Enumerable.Empty<int>().Append(20));
            // _to.Set(input, Enumerable.Empty<int>().Append(30));
        }
    }
}
namespace Testing.Actions
{
    [Serializable]
    public class ActionsTester
    {
        [SerializeField] Weaver _weaver;
        [SerializeField] Transform _transform1;
        [SerializeField] Transform _transform2;

        public void TestAll() {
            TestFollowActionConcreteEqual();
            TestFollowActionInterfaceEqual();
            TestActionInterfaceNotEqual();
            TestMoveToActionConcreteEqual();
            TestMoveToActionInterfaceEqual();
        }

        public void TestFollowActionConcreteEqual() {
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            var action1 = new FollowAction(_weaver, _transform1);
            var action2 = new FollowAction(_weaver, _transform1);
            if (action1.IsSimilarAction(action2)) {
                Debug.Log($"<color=green>PASSED: {testName}</color>");
            }
            else {
                Debug.Log($"<color=red>FAILED: {testName}</color>");
            }
        }

        public void TestFollowActionInterfaceEqual() {
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            IAction action1 = new FollowAction(_weaver, _transform1);
            IAction action2 = new FollowAction(_weaver, _transform1);

            if (action1.IsSimilarAction(action2)) {
                Debug.Log($"<color=green>PASSED: {testName}</color>");
            }
            else {
                Debug.Log($"<color=red>FAILED: {testName}</color>");
            }
        }


        public void TestActionInterfaceNotEqual() {
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            IAction action1 = new FollowAction(_weaver, _transform1);
            IAction action2 = new MoveToAction(_weaver, _transform1.position);

            if (!action1.IsSimilarAction(action2)) {
                Debug.Log($"<color=green>PASSED: {testName}</color>");
            }
            else {
                Debug.Log($"<color=red>FAILED: {testName}</color>");
            }
        }


        public void TestMoveToActionConcreteEqual() {
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            var action1 = new MoveToAction(_weaver, _transform1.position);
            var action2 = new MoveToAction(_weaver, _transform1.position);
            if (action1.IsSimilarAction(action2)) {
                Debug.Log($"<color=green>PASSED: {testName}</color>");
            }
            else {
                Debug.Log($"<color=red>FAILED: {testName}</color>");
            }
        }

        public void TestMoveToActionInterfaceEqual() {
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            IAction action1 = new MoveToAction(_weaver, _transform1.position);
            IAction action2 = new MoveToAction(_weaver, _transform1.position);

            if (action1.IsSimilarAction(action2)) {
                Debug.Log($"<color=green>PASSED: {testName}</color>");
            }
            else {
                Debug.Log($"<color=red>FAILED: {testName}</color>");
            }
        }

    }
}

namespace Testing.TimeDialation
{
    public class TimeDialationTester
    {
        EventAggregator _eventAggregator;
        public TimeDialationTester(EventAggregator eventAggregator) {
            _eventAggregator = eventAggregator;
        }

        public void TestAll() {
            TestTimeDialationSpeedUp();
            TestTimeDialationSpeedDown();
            TestTimeDialationSpeedPause();
            TestTimeDialationSpeedReset();
        }

        public void TestTimeDialationSpeedUp() {
            var currentTimeScale = Time.timeScale;
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            _eventAggregator.OnTimeDilationSpeedUp(this, System.EventArgs.Empty);
            if (Time.timeScale > currentTimeScale) {
                Debug.Log($"<color=green>PASSED: {testName}</color>");
            }
            else {
                Debug.Log($"<color=red>FAILED: {testName}</color>");
            }
        }
        public void TestTimeDialationSpeedDown() {
            var currentTimeScale = Time.timeScale;
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            _eventAggregator.OnTimeDilationSpeedDown(this, System.EventArgs.Empty);
            if (Time.timeScale < currentTimeScale) {
                Debug.Log($"<color=green>PASSED: {testName}</color>");
            }
            else {
                Debug.Log($"<color=red>FAILED: {testName}</color>");
            }
        }
        public void TestTimeDialationSpeedPause() {
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            _eventAggregator.OnTimeDilationSpeedPause(this, System.EventArgs.Empty);
            if (Time.timeScale == 0) {
                Debug.Log($"<color=green>PASSED: {testName}</color>");
            }
            else {
                Debug.Log($"<color=red>FAILED: {testName}</color>");
            }
        }
        public void TestTimeDialationSpeedReset() {
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            Time.timeScale = 4;
            _eventAggregator.OnTimeDilationSpeedReset(this, System.EventArgs.Empty);
            if (Time.timeScale == 1) {
                Debug.Log($"<color=green>PASSED: {testName}</color>");
            }
            else {
                Debug.Log($"<color=red>FAILED: {testName}</color>");
            }
        }
    }
}

namespace Testing.Line
{
    public class LineTester
    {
        public void TestAll() {
            TestWaitInLine();
            TestCutInLine();
            TestTryRemoveAt();
            TestCircularInsert();
        }

        public void TestWaitInLine() {
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            var anyFail = false;
            int insert;
            int[] insertArr = { 5, 4, 3, 2, 1 };

            Line<int> l = new(insertArr.Length);

            foreach (var item in insertArr) {
                if (l.TryWaitInLine(item) == false) {
                    Debug.Log($"<color=red>FAILED: {testName} {item} </color>");
                    anyFail = true;
                }
            }

            insert = 0;
            if (l.TryWaitInLine(insert) == true) {
                Debug.Log($"<color=red>FAILED: {testName} {insert} </color>");
                anyFail = true;
            }

            if (!l.TryGetFirstOutOfLine(out var n) || (n != insertArr[0])) {
                Debug.Log($"<color=red>FAILED: {testName} TryGetFirstOutOfLine {insertArr[0]} </color>");
                anyFail = true;
            }

            int[] resultArr = { 4, 3, 2, 1 };
            var count = 1;
            foreach (var item in l) {
                if (item != insertArr[count]) {
                    Debug.Log($"<color=red>FAILED: {testName} foreach {item} != {insertArr[count]} </color>");
                    anyFail = true;
                }
                count++;
            }

            if (count != insertArr.Length) {
                Debug.Log($"<color=red>FAILED: {testName} incorrect number of items in Line {count != resultArr.Length}</color>");
                anyFail = true;
            }

            if (anyFail == false) {
                Debug.Log($"<color=green>PASSED: {testName} </color>");
            }
        }
        public void TestCutInLine() {
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            var anyFail = false;
            int[] insertArr = { 1, 2, 3, 4, 5 };

            Line<int> l = new(insertArr.Length);
            foreach (var item in insertArr) {
                if (l.TryWaitInLine(item) == false) {
                    Debug.Log($"<color=red>FAILED: {testName} {item} </color>");
                    anyFail = true;
                }
            }

            var insert = 6;
            l.CutInLine(insert);

            if (l.TryWaitInLine(insert) == true) {

            }

            if (!l.TryGetFirstOutOfLine(out var n) || (n != insert)) {
                Debug.Log($"<color=red>FAILED: {testName} get out {insert} </color>");
                anyFail = true;
            }


            int[] resultArr = { 1, 2, 3, 4 };
            var count = 0;
            foreach (var item in l) {
                if (item != resultArr[count]) {
                    Debug.Log($"<color=red>FAILED: {testName} foreach {item} != {resultArr[count]} </color>");
                    anyFail = true;
                }
                count++;
            }

            if (count != resultArr.Length) {
                Debug.Log($"<color=red>FAILED: {testName} incorrect number of items in Line {count != resultArr.Length}</color>");
                anyFail = true;
            }

            if (anyFail == false) {
                Debug.Log($"<color=green>PASSED: {testName} </color>");
            }
        }

        public void TestTryRemoveAt() {
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            var anyFail = false;
            int[] insertArr = { 1, 2, 3, 4, 5 };

            Line<int> l = new(insertArr.Length);
            foreach (var item in insertArr) {
                if (l.TryWaitInLine(item) == false) {
                    Debug.Log($"<color=red>FAILED: TryWaitInLine foreach {testName} {item} </color>");
                    anyFail = true;
                }
            }

            var position = 2;
            if (!l.TryRemoveAt(position, out var n) || n != insertArr[position]) {
                Debug.Log($"<color=red>FAILED: TryRemoveAt {testName} {insertArr[position]} </color>");
                anyFail = true;
            }
            if (l.TryWaitInLine(insertArr[position]) == false) {
                Debug.Log($"<color=red>FAILED: TryWaitInLine {testName} {insertArr[position]} </color>");
                anyFail = true;
            }
            l.CutInLine(insertArr[position]);

            int[] resultArr = { 3, 1, 2, 4, 5 };
            position = 4;
            if (!l.TryRemoveAt(position, out n) || n != resultArr[position]) {
                Debug.Log($"<color=red>FAILED: TryRemoveAt {testName} {n} != {resultArr[position]} </color>");
                anyFail = true;
            }

            if (l.TryWaitInLine(insertArr[position]) == false) {
                Debug.Log($"<color=red>FAILED: TryWaitInLine {testName} {insertArr[position]} </color>");
                anyFail = true;
            }

            resultArr = new int[] { 3, 1, 2, 4, insertArr[position] };
            if (!l.TryRemoveAt(position, out n) || n != resultArr[position]) {
                Debug.Log($"<color=red>FAILED: TryRemoveAt {testName} {n} != {resultArr[position]} </color>");
                anyFail = true;
            }

            resultArr = new int[] { 3, 1, 2, 4 };
            var count = 0;
            foreach (var item in l) {
                if (item != resultArr[count]) {
                    Debug.Log($"<color=red>FAILED: {testName} foreach {item} != {resultArr[count]} </color>");
                    anyFail = true;
                }
                count++;
            }

            if (count != resultArr.Length) {
                Debug.Log($"<color=red>FAILED: {testName} incorrect number of items in Line {count != resultArr.Length}</color>");
                anyFail = true;
            }

            if (anyFail == false) {
                Debug.Log($"<color=green>PASSED: {testName} </color>");
            }
        }

        public void TestCircularInsert() {
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            var anyFail = false;
            int[] insertArr = { 1, 2, 3, 4, 5 };

            Line<int> l = new(insertArr);

            l.TryGetFirstOutOfLine(out var _);
            l.TryGetFirstOutOfLine(out var _);
            l.TryWaitInLine(6);
            l.TryWaitInLine(7);

            int[] resultArr = { 3, 4, 5, 6, 7 };
            var count = 0;
            foreach (var item in l) {
                if (item != resultArr[count]) {
                    Debug.Log($"<color=red>FAILED: {testName} foreach {item} != {resultArr[count]} </color>");
                    anyFail = true;
                }
                count++;
            }

            if (count != resultArr.Length) {
                Debug.Log($"<color=red>FAILED: {testName} incorrect number of items in Line {count != resultArr.Length} </color>");
                anyFail = true;
            }


            if (anyFail == false) {
                Debug.Log($"<color=green>PASSED: {testName} </color>");
            }
        }

    }

}
