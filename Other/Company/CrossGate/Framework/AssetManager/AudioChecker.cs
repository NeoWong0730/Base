using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using UnityEditor;

public class AudioChecker : MonoBehaviour
{
    public List<AudioEntry> entries = new List<AudioEntry>();
    public int audioPoolCount = 0;

    private void Update()
    {
        entries = AudioManager.Instance.mAudioEntries;
        audioPoolCount = AudioManager.Instance.mAudioEntriesPool.Count;
    }
}
