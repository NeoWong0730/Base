using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceTest : MonoBehaviour
{
    public Text text;
    private static PerformanceCheck performanceCheck;
    private void OnEnable()
    {
        performanceCheck = new PerformanceCheck();
        performanceCheck.Start();
    }

    private void Update()
    {
        performanceCheck.IsFinished();
        text.text = performanceCheck.nPerformanceScore.ToString();
    }

    private void OnDisable()
    {
        performanceCheck.End();
        performanceCheck = null;
    }
}
