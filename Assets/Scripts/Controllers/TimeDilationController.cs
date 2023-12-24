using System;
using TagFighter.Events;
using UnityEngine;

namespace TagFighter
{
    public class TimeDilationController : MonoBehaviour
    {
        [SerializeField] float _dilationStep = 0.25f;
        public const float MaxTimeDilation = 4f;
        const float MinTimeDilation = 0f;
        const int TimeDilationPercision = 2;
        float _baseTimeScale;
        [SerializeField] EventAggregator _eventAggregator;

        protected void Start() {
            _eventAggregator.TimeDilationSpeedUp += OnTimeDilationSpeedUp;
            _eventAggregator.TimeDilationSpeedDown += OnTimeDilationSpeedDown;
            _eventAggregator.TimeDilationSpeedReset += OnTimeDilationSpeedReset;
            _eventAggregator.TimeDilationSpeedPause += OnTimeDilationSpeedPause;
            _baseTimeScale = Time.timeScale;
        }


        protected void OnDestroy() {
            _eventAggregator.TimeDilationSpeedUp -= OnTimeDilationSpeedUp;
            _eventAggregator.TimeDilationSpeedDown -= OnTimeDilationSpeedDown;
            _eventAggregator.TimeDilationSpeedReset -= OnTimeDilationSpeedReset;
            _eventAggregator.TimeDilationSpeedPause -= OnTimeDilationSpeedPause;
        }

        void OnTimeDilationSpeedPause(object sender, EventArgs e) {
            Time.timeScale = 0;
        }
        void OnTimeDilationSpeedReset(object sender, EventArgs e) {
            Time.timeScale = _baseTimeScale;
        }

        void OnTimeDilationSpeedDown(object sender, EventArgs e) {
            Time.timeScale = MathF.Max(MinTimeDilation, MathF.Round(Time.timeScale - _dilationStep, TimeDilationPercision));
        }

        void OnTimeDilationSpeedUp(object sender, EventArgs e) {
            Time.timeScale = MathF.Min(MaxTimeDilation, MathF.Round(Time.timeScale + _dilationStep, TimeDilationPercision));
        }

    }
}
