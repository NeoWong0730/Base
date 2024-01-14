using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(TimelineDirector))]
[RequireComponent(typeof(PlayableDirector))]
public abstract class TimelineTrackSetter : MonoBehaviour
{
    public TimelineDirector timelineDirector;
    
    private void Awake()
    {
        if (timelineDirector == null)
        {
            timelineDirector = GetComponent<TimelineDirector>();
        }
    }

    public bool ExistTrack(string trackName, out PlayableBinding binding)
    {
        return timelineDirector.Exist(trackName, out binding);
    }
}
