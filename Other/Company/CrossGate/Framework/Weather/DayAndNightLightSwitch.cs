using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DayAndNightMaterialData
{
    public string sPropertyName;
    public Material[] mMaterials;
    [ColorUsage(true, true)]
    public Color mDayEmissives;
    [ColorUsage(true, true)]
    public Color mNightEmissives;
}

[System.Serializable]
public struct DayAndNightSwitchData
{
    public EDayStage eStage;
    public GameObject[] mObjects;
}

public class DayAndNightLightSwitch : MonoBehaviour
{
    [SerializeField]
    private DayAndNightSwitchData[] mSwitchData;

    private void OnEnable()
    {
        OnDayNightStageChange(EDayStage.Invalid, WeatherSystem.gDayStage);
        WeatherSystem.OnDayNightStageChange += OnDayNightStageChange;
    }

    private void OnDisable()
    {
        WeatherSystem.OnDayNightStageChange -= OnDayNightStageChange;
    }

    private void OnDayNightStageChange(EDayStage from, EDayStage to)
    {
        if (mSwitchData != null)
        {
            for (int i = 0; i < mSwitchData.Length; ++i)
            {
                DayAndNightSwitchData switchData = mSwitchData[i];
                if (switchData.mObjects == null)
                    continue;

                for (int j = 0; j < switchData.mObjects.Length; ++j)
                {
                    switchData.mObjects[j].SetActive(to == switchData.eStage);
                }
            }
        }
    }
}
