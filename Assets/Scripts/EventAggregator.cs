using System;
using UnityEngine;


namespace TagFighter.Events
{

    [CreateAssetMenu(fileName = "NewEventAggregator", menuName = "Game/Misc/EventAggregator")]
    public class EventAggregator : ScriptableObject
    {
        public event EventHandler<UnitActionEventArgs> UnitSelected;
        public event EventHandler<UnitMoveEventArgs> UnitMove;
        public event EventHandler<TryRemovePlannedActionEventArgs> RemovePlannedActionSelected;
        public event EventHandler<UnitActionEventArgs> TargetSelected;

        public event EventHandler TimeDilationSpeedUp;
        public event EventHandler TimeDilationSpeedDown;
        public event EventHandler TimeDilationSpeedReset;
        public event EventHandler TimeDilationSpeedPause;

        public event EventHandler WeaveBuilderScreenToggleSelected;

        public event EventHandler<RuneWeavingBankChangedEventArgs> RuneWeavingBankChanged;

        public event EventHandler<WeaveCastEventArgs> RuneWeavingCastSelected;
        public event EventHandler PlannedActionsClearSelected;
        public event EventHandler<PawnSheetToggleSelectedEventArgs> PawnSheetToggleSelected;
        public event EventHandler<PawnSheetShowedEventArgs> PawnSheetShowed;
        public event EventHandler<PawnSheetHidEventArgs> PawnSheetHid;

        public event EventHandler<UnitControllerTargetStartedEventArgs> UnitControllerStarted;


        public void OnUnitSelected(object sender, UnitActionEventArgs e) {
            UnitSelected?.Invoke(sender, e);
        }
        public void OnUnitMove(object sender, UnitMoveEventArgs e) {
            UnitMove?.Invoke(sender, e);
        }

        public void OnRemovePlannedActionSelected(object sender, TryRemovePlannedActionEventArgs e) {
            RemovePlannedActionSelected?.Invoke(sender, e);
        }
        public void OnTargetSelected(object sender, UnitActionEventArgs e) {
            TargetSelected?.Invoke(sender, e);
        }
        public void OnTimeDilationSpeedUp(object sender, EventArgs e) {
            TimeDilationSpeedUp?.Invoke(sender, e);
        }
        public void OnTimeDilationSpeedDown(object sender, EventArgs e) {
            TimeDilationSpeedDown?.Invoke(sender, e);
        }
        public void OnTimeDilationSpeedReset(object sender, EventArgs e) {
            TimeDilationSpeedReset?.Invoke(sender, e);
        }
        public void OnTimeDilationSpeedPause(object sender, EventArgs e) {
            TimeDilationSpeedPause?.Invoke(sender, e);
        }
        public void OnWeaveBuilderScreenToggleSelected(object sender, EventArgs e) {
            WeaveBuilderScreenToggleSelected?.Invoke(sender, e);
        }
        public void OnRuneWeavingBankChanged(object sender, RuneWeavingBankChangedEventArgs e) {
            RuneWeavingBankChanged?.Invoke(sender, e);
        }

        public void OnRuneWeavingCastSelected(object sender, WeaveCastEventArgs e) {
            RuneWeavingCastSelected?.Invoke(sender, e);
        }
        public void OnPlannedActionsClearSelected(object sender, EventArgs e) {
            PlannedActionsClearSelected?.Invoke(sender, e);
        }

        public void OnPawnSheetToggleSelected(object sender, PawnSheetToggleSelectedEventArgs e) {
            PawnSheetToggleSelected?.Invoke(sender, e);
        }
        public void OnPawnSheetShowed(object sender, PawnSheetShowedEventArgs e) {
            PawnSheetShowed?.Invoke(sender, e);
        }
        public void OnPawnSheetHid(object sender, PawnSheetHidEventArgs e) {
            PawnSheetHid?.Invoke(sender, e);
        }

        public void OnUnitControllerStarted(object sender, UnitControllerTargetStartedEventArgs e) {
            UnitControllerStarted?.Invoke(sender, e);
        }

        #region events
        public class TryRemovePlannedActionEventArgs : EventArgs
        {
            public int Index { get; }
            public TryRemovePlannedActionEventArgs(int index) {
                if (index < 0) {
                    throw new IndexOutOfRangeException("index");
                }
                Index = index;
            }
        }
        public class WeaveCastEventArgs : EventArgs
        {
            public RuneWeavingContainer Weave { get; }
            public WeaveCastEventArgs(RuneWeavingContainer weave) {
                Weave = weave;
            }
        }
        public class RuneWeavingBankChangedEventArgs : EventArgs
        {
            public RuneWeavingBank Bank { get; }
            public RuneWeavingBankChangedEventArgs(RuneWeavingBank bank) {
                Bank = bank;
            }
        }

        public class PawnSheetToggleSelectedEventArgs : EventArgs
        {
            public PawnSheetToggleSelectedEventArgs() {
            }
        }
        public class PawnSheetShowedEventArgs : EventArgs
        {
            public PawnSheetShowedEventArgs() {
            }
        }
        public class PawnSheetHidEventArgs : EventArgs
        {
            public PawnSheetHidEventArgs() {
            }
        }
        #endregion
    }


}
