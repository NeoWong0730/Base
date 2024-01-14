using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Action = System.Action;
using Object = UnityEngine.Object;

namespace Lib.Core
{
    /// <summary>
    /// Allows you to run events on a delay without the use of <see cref="Coroutine"/>s
    /// or <see cref="MonoBehaviour"/>s.
    ///
    /// To create and start a Timer, use the <see cref="Register"/> method.
    /// </summary>
    public class Timer
    {
        #region Public Properties/Fields

        /// <summary>
        /// How long the timer takes to complete from start to finish.
        /// </summary>
        public float duration { get; set; }

        /// <summary>
        /// Whether the timer will run again after completion.
        /// </summary>
        public bool isLooped { get; set; }

        /// <summary>
        /// Whether or not the timer completed running. This is false if the timer was cancelled.
        /// </summary>
        public bool isCompleted { get; private set; }

        /// <summary>
        /// Whether the timer uses real-time or game-time. Real time is unaffected by changes to the timescale
        /// of the game(e.g. pausing, slow-mo), while game time is affected.
        /// </summary>
        public bool usesRealTime { get; private set; }

        /// <summary>
        /// Whether the timer is currently paused.
        /// </summary>
        public bool isPaused
        {
            get { return this._timeElapsedBeforePause.HasValue; }
        }

        /// <summary>
        /// Whether or not the timer was cancelled.
        /// </summary>
        public bool isCancelled
        {
            get { return this._timeElapsedBeforeCancel.HasValue; }
        }

        /// <summary>
        /// Get whether or not the timer has finished running for any reason.
        /// </summary>
        public bool isDone
        {
            get { return this.isCompleted || this.isCancelled || this.isOwnerDestroyed; }
        }
#if DEBUG_MODE
        public string DebugName()
        {
            if(_onComplete != null)
            {
                return _onComplete.Method.ToString();
            }
            if(_onUpdate != null)
            {
                return _onUpdate.Method.ToString();
            }
            return null;
        }
#endif

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Register a new timer that should fire an event after a certain amount of time
        /// has elapsed.
        ///
        /// Registered timers are destroyed when the scene changes.
        /// </summary>
        /// <param name="duration">The time to wait before the timer should fire, in seconds.</param>
        /// <param name="onComplete">An action to fire when the timer completes.</param>
        /// <param name="onUpdate">An action that should fire each time the timer is updated. Takes the amount
        /// of time passed in seconds since the start of the timer's current loop.</param>
        /// <param name="isLooped">Whether the timer should repeat after executing.</param>
        /// <param name="useRealTime">Whether the timer uses real-time(i.e. not affected by pauses,
        /// slow/fast motion) or game-time(will be affected by pauses and slow/fast-motion).</param>
        /// <param name="autoDestroyOwner">An object to attach this timer to. After the object is destroyed,
        /// the timer will expire and not execute. This allows you to avoid annoying <see cref="NullReferenceException"/>s
        /// by preventing the timer from running and accessessing its parents' components
        /// after the parent has been destroyed.</param>
        /// <returns>A timer object that allows you to examine stats and stop/resume progress.</returns>
        public static Timer Register(float duration, Action onComplete, Action<float> onUpdate = null,
            bool isLooped = false, bool useRealTime = true, MonoBehaviour autoDestroyOwner = null)
        {
            // create a manager object to update all the timers if one does not already exist.
            if (Timer._manager == null)
            {
                TimerManager managerInScene = Object.FindObjectOfType<TimerManager>();
                if (managerInScene != null)
                {
                    Timer._manager = managerInScene;
                }
                else
                {
                    if (Application.isPlaying)
                    {
                        GameObject managerObject = new GameObject { name = "Root_TimerManager" };
                        GameObject.DontDestroyOnLoad(managerObject);
                        Timer._manager = managerObject.AddComponent<TimerManager>();
                    }
                }
            }

            Timer timer = new Timer(duration, onComplete, onUpdate, isLooped, useRealTime, autoDestroyOwner);
            Timer._manager.RegisterTimer(timer);
            return timer;
        }

        public static Timer RegisterOrReuse(ref Timer timer, float duration, Action onComplete, Action<float> onUpdate = null,
            bool isLooped = false, bool useRealTime = true, MonoBehaviour autoDestroyOwner = null)
        {
            if (timer == null)
            {
                timer = Register(duration, onComplete, onUpdate, isLooped, useRealTime, autoDestroyOwner);
            }
            else
            {
                timer?.Cancel();
                Timer._manager.UnRegisterTimer(timer);
                timer.Reuse(duration, onComplete, onUpdate, isLooped, useRealTime, autoDestroyOwner);
                Timer._manager.RegisterTimer(timer);
            }
            return timer;
        }

        // 调整server时间的时候，需要矫正timer
        public static Timer Correct(ref Timer timer, long diff)
        {
            if (timer != null)
            {
                timer = RegisterOrReuse(ref timer, timer.duration - diff, timer._onComplete, timer._onUpdate, timer.isLooped, timer.usesRealTime, timer._autoDestroyOwner);
            }
            return timer;
        }

        /// <summary>
        /// Cancels a timer. The main benefit of this over the method on the instance is that you will not get
        /// a <see cref="NullReferenceException"/> if the timer is null.
        /// </summary>
        /// <param name="timer">The timer to cancel.</param>
        public static void Cancel(Timer timer)
        {
            if (timer != null)
            {
                timer.Cancel();
            }
        }

        /// <summary>
        /// Pause a timer. The main benefit of this over the method on the instance is that you will not get
        /// a <see cref="NullReferenceException"/> if the timer is null.
        /// </summary>
        /// <param name="timer">The timer to pause.</param>
        public static void Pause(Timer timer)
        {
            if (timer != null)
            {
                timer.Pause();
            }
        }

        /// <summary>
        /// Resume a timer. The main benefit of this over the method on the instance is that you will not get
        /// a <see cref="NullReferenceException"/> if the timer is null.
        /// </summary>
        /// <param name="timer">The timer to resume.</param>
        public static void Resume(Timer timer)
        {
            if (timer != null)
            {
                timer.Resume();
            }
        }

        public static void CancelAllRegisteredTimers()
        {
            if (Timer._manager != null)
            {
                Timer._manager.CancelAllTimers();
            }

            // if the manager doesn't exist, we don't have any registered timers yet, so don't
            // need to do anything in this case
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Stop a timer that is in-progress or paused. The timer's on completion callback will not be called.
        /// </summary>
        public void Cancel()
        {
            if (this.isDone)
            {
                return;
            }

            this._timeElapsedBeforeCancel = this.GetTimeElapsed();
            this._timeElapsedBeforePause = null;
        }

        /// <summary>
        /// Pause a running timer. A paused timer can be resumed from the same point it was paused.
        /// </summary>
        public void Pause()
        {
            if (this.isPaused || this.isDone)
            {
                return;
            }

            this._timeElapsedBeforePause = this.GetTimeElapsed();
        }

        /// <summary>
        /// Continue a paused timer. Does nothing if the timer has not been paused.
        /// </summary>
        public void Resume()
        {
            if (!this.isPaused || this.isDone)
            {
                return;
            }

            this._timeElapsedBeforePause = null;
        }

        /// <summary>
        /// Get how many seconds have elapsed since the start of this timer's current cycle.
        /// </summary>
        /// <returns>The number of seconds that have elapsed since the start of this timer's current cycle, i.e.
        /// the current loop if the timer is looped, or the start if it isn't.
        ///
        /// If the timer has finished running, this is equal to the duration.
        ///
        /// If the timer was cancelled/paused, this is equal to the number of seconds that passed between the timer
        /// starting and when it was cancelled/paused.</returns>
        public float GetTimeElapsed()
        {
            if (this.isCompleted || this.GetWorldTime() >= this.GetFireTime())
            {
                return this.duration;
            }

            return this._timeElapsedBeforeCancel ??
                   this._timeElapsedBeforePause ??
                   this.GetWorldTime() - this._startTime;
        }

        /// <summary>
        /// Get how many seconds remain before the timer completes.
        /// </summary>
        /// <returns>The number of seconds that remain to be elapsed until the timer is completed. A timer
        /// is only elapsing time if it is not paused, cancelled, or completed. This will be equal to zero
        /// if the timer completed.</returns>
        public float GetTimeRemaining()
        {
            return this.duration - this.GetTimeElapsed();
        }

        /// <summary>
        /// Get how much progress the timer has made from start to finish as a ratio.
        /// </summary>
        /// <returns>A value from 0 to 1 indicating how much of the timer's duration has been elapsed.</returns>
        public float GetRatioComplete()
        {
            return this.GetTimeElapsed() / this.duration;
        }

        /// <summary>
        /// Get how much progress the timer has left to make as a ratio.
        /// </summary>
        /// <returns>A value from 0 to 1 indicating how much of the timer's duration remains to be elapsed.</returns>
        public float GetRatioRemaining()
        {
            return this.GetTimeRemaining() / this.duration;
        }

        #endregion

        #region Private Static Properties/Fields

        // responsible for updating all registered timers
        public static TimerManager _manager { get; private set; }

        #endregion

        #region Private Properties/Fields

        private bool isOwnerDestroyed
        {
            get { return this._hasAutoDestroyOwner && this._autoDestroyOwner == null; }
        }

        private Action _onComplete;
        private Action<float> _onUpdate;
        private float _startTime;
        private float _lastUpdateTime;

        // for pausing, we push the start time forward by the amount of time that has passed.
        // this will mess with the amount of time that elapsed when we're cancelled or paused if we just
        // check the start time versus the current world time, so we need to cache the time that was elapsed
        // before we paused/cancelled
        private float? _timeElapsedBeforeCancel;
        private float? _timeElapsedBeforePause;

        // after the auto destroy owner is destroyed, the timer will expire
        // this way you don't run into any annoying bugs with timers running and accessing objects
        // after they have been destroyed
        private MonoBehaviour _autoDestroyOwner;
        private bool _hasAutoDestroyOwner;

        #endregion

        #region Private Constructor (use static Register method to create new timer)

        private Timer(float duration, Action onComplete, Action<float> onUpdate = null,
            bool isLooped = false, bool usesRealTime = false, MonoBehaviour autoDestroyOwner = null)
        {
            this.duration = duration;
            this._onComplete = onComplete;
            this._onUpdate = onUpdate;

            this.isLooped = isLooped;
            this.usesRealTime = usesRealTime;

            this._autoDestroyOwner = autoDestroyOwner;
            this._hasAutoDestroyOwner = autoDestroyOwner != null;

            this._startTime = this.GetWorldTime();
            this._lastUpdateTime = this._startTime;
        }
        public Timer Reuse(float duration, Action onComplete, Action<float> onUpdate = null,
            bool isLooped = false, bool usesRealTime = false, MonoBehaviour autoDestroyOwner = null)
        {

            this.isCompleted = false;

            this.duration = duration;
            this._onComplete = onComplete;
            this._onUpdate = onUpdate;

            this.isLooped = isLooped;
            this.usesRealTime = usesRealTime;

            this._autoDestroyOwner = autoDestroyOwner;
            this._hasAutoDestroyOwner = autoDestroyOwner != null;

            this._timeElapsedBeforePause = null;
            this._timeElapsedBeforeCancel = null;

            this._startTime = this.GetWorldTime();
            this._lastUpdateTime = this._startTime;

            return this;
        }

        #endregion

        #region Private Methods

        private float GetWorldTime()
        {
            return this.usesRealTime ? Time.realtimeSinceStartup : Time.time;
        }

        private float GetFireTime()
        {
            return this._startTime + this.duration;
        }

        public float GetRemainTime()
        {
            return GetFireTime() - GetWorldTime();
        }

        private float GetTimeDelta()
        {
            return this.GetWorldTime() - this._lastUpdateTime;
        }

        private void Update()
        {
            if (this.isDone)
            {
                return;
            }

            if (this.isPaused)
            {
                this._startTime += this.GetTimeDelta();
                this._lastUpdateTime = this.GetWorldTime();
                return;
            }

            this._lastUpdateTime = this.GetWorldTime();

            this._onUpdate?.Invoke(this.GetTimeElapsed());

            if (this.GetWorldTime() >= this.GetFireTime())
            {

                this._onComplete?.Invoke();

                if (this.isLooped)
                {
                    this._startTime = this.GetWorldTime();
                }
                else
                {
                    this.isCompleted = true;
                }
            }
        }

        #endregion

        #region Manager Class (implementation detail, spawned automatically and updates all registered timers)

        /// <summary>
        /// Manages updating all the <see cref="Timer"/>s that are running in the application.
        /// This will be instantiated the first time you create a timer -- you do not need to add it into the
        /// scene manually.
        /// </summary>
        public class TimerManager : MonoBehaviour
        {
            public int Count
            {
                get
                {
                    return this._timers.Count;
                }
            }
#if DEBUG_MODE
            public static int LastExecuteTime;
            public static int LastExecuteFrame;
            public static int LastExecuteCount;

            public static List<string> LastExecuteNames = new List<string>();
            public static List<int> LastExecuteTimes = new List<int>();
            public static List<int> LastExecuteFrames = new List<int>();
#endif

            private List<Timer> _timers = new List<Timer>();

            // buffer adding timers so we don't edit a collection during iteration
            private List<Timer> _timersToAdd = new List<Timer>();

            public void RegisterTimer(Timer timer)
            {
                this._timersToAdd.Add(timer);
            }

            public void UnRegisterTimer(Timer timer)
            {
                int index = this._timersToAdd.IndexOf(timer);
                int lastIndex = 0;
                if (index != -1)
                {
                    lastIndex = this._timersToAdd.Count - 1;
                    if (index != lastIndex)
                    {
                        this._timersToAdd[index] = this._timersToAdd[lastIndex];
                    }
                    this._timersToAdd.RemoveAt(lastIndex);
                }

                index = this._timers.IndexOf(timer);
                lastIndex = 0;
                if (index != -1)
                {
                    lastIndex = this._timers.Count - 1;
                    if (index != lastIndex)
                    {
                        this._timers[index] = this._timers[lastIndex];
                    }
                    this._timers.RemoveAt(lastIndex);
                }
            }

            public void CancelAllTimers()
            {
                foreach (Timer timer in this._timers)
                {
                    timer.Cancel();
                }

                this._timers = new List<Timer>();
                this._timersToAdd = new List<Timer>();
            }

            // update all the registered timers on every frame
            [UsedImplicitly]
            private void Update()
            {
#if DEBUG_MODE
                int count = _timersToAdd.Count + _timers.Count;
                if (count > 0)
                {
                    LastExecuteFrame = Time.frameCount;
                    LastExecuteCount = count;
                }

                float t = UnityEngine.Time.realtimeSinceStartup;
#endif
                this.UpdateAllTimers();
#if DEBUG_MODE
                LastExecuteTime = (int)((UnityEngine.Time.realtimeSinceStartup - t) * 1000000);
#endif
            }

            private void UpdateAllTimers()
            {
                if (this._timersToAdd.Count > 0)
                {
                    this._timers.AddRange(this._timersToAdd);
                    this._timersToAdd.Clear();
                }

#if DEBUG_MODE
                LastExecuteTimes.Clear();
                LastExecuteFrames.Clear();
                LastExecuteNames.Clear();
#endif                    
                Timer timer;
                for (int i = this._timers.Count - 1; i >= 0; --i)
                {
#if DEBUG_MODE                    
                    float t = UnityEngine.Time.realtimeSinceStartup;
#endif                    
                    timer = this._timers[i];
                    try
                    {
                        timer.Update();
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogException(e);
                        //timer.Cancel();
                    }

#if DEBUG_MODE
                    //LastExecuteTimes.Add();
                    LastExecuteTimes.Add((int)((UnityEngine.Time.realtimeSinceStartup - t) * 1000000));
                    LastExecuteFrames.Add(Time.frameCount);
                    LastExecuteNames.Add(timer.DebugName());
#endif

                    if (timer.isDone)
                    {
                        this._timers.RemoveAt(i);
                    }
                }
            }
        }

        #endregion
    }
}
