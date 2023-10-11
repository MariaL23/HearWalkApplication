using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    private float startTime;
    private float elapsedTime;
    public TextMeshProUGUI timeText;

    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time - startTime;

        timeText.text = elapsedTime.ToString("0:00");

    }
}
