namespace Logic
{
    public interface IInteractiveWatcher
    {
        void OnAreaCheckExecute(InteractiveEvtData data);
        void OnClickExecute(InteractiveEvtData data);
        void OnDoubleClickExecute(InteractiveEvtData data);
        void OnLongPressExecute(InteractiveEvtData data);
        void OnDistanceCheckExecute(InteractiveEvtData data);
        void OnUIButtonExecute(InteractiveEvtData data);
    }
}
