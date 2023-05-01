using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using SatSys;
using UnityEngine.Events;

public class Timeline : MonoBehaviour
{
    private Slider _slider;

    public TMP_InputField valueInput;
    public TMP_InputField timeStepInput;

    [SerializeField]
    private bool _isPaused = false;

    public static float timeStep = 0.001f;

    public static double startDate = SatDate.startJulianDate;
    public static double endDate = SatDate.endJulianDate;

    static UnityAction<float, double, double, float> ChangeTimeProperties;

    public static void OnChangeTimeProperties(
        float timeStep,
        double startDate,
        double endDate,
        float epoch = 0
    ) => ChangeTimeProperties?.Invoke(timeStep, startDate, endDate, epoch);

    void Awake()
    {
        if (valueInput == null)
        {
            valueInput = transform.Find("ValueInput").GetComponent<TMP_InputField>();
        }
        if (timeStepInput == null)
        {
            timeStepInput = transform.Find("TimeStepInput").GetComponent<TMP_InputField>();
        }
    }

    void Start()
    {
        ChangeTimeProperties += UpdateTimeProperties;

        _slider = GetComponentInChildren<Slider>();
        _slider.minValue = 0;
        _slider.maxValue = (float)(endDate - startDate);
        _slider.onValueChanged.AddListener(
            delegate(float value)
            {
                EventManager.OnTimeChanged(value);
                valueInput.text = SatDate.GetDateTime(value + startDate).ToString();
            }
        );

        if (valueInput != null)
        {
            valueInput.onEndEdit.AddListener(
                delegate(string input)
                {
                    if (float.TryParse(input, out float result))
                    {
                        _slider.value = Mathf.Clamp(result, _slider.minValue, _slider.maxValue);
                    }
                    else if (DateTime.TryParse(input, out DateTime dateTime))
                    {
                        _slider.value = Mathf.Clamp(
                            (float)(SatDate.GetJulianDate(dateTime) - startDate),
                            _slider.minValue,
                            _slider.maxValue
                        );
                    }
                }
            );
        }

        if (timeStepInput != null)
        {
            timeStepInput.text = timeStep.ToString();
            timeStepInput.onEndEdit.AddListener(
                delegate(string input)
                {
                    if (float.TryParse(input, out float result))
                    {
                        timeStep = Mathf.Clamp(result, 0.00001f, 1f);
                        EventManager.OnTimeStepChange(timeStep);
                    }
                }
            );
        }

        // time pause
        var toggle = GetComponentInChildren<Toggle>();
        toggle.isOn = false;
        toggle.onValueChanged.AddListener(
            delegate(bool flag)
            {
                _isPaused = flag;
            }
        );
    }

    void FixedUpdate()
    {
        if (!_isPaused)
        {
            _slider.value += Time.deltaTime * timeStep;
        }
    }

    public void UpdateTimeProperties(
        float timeStep,
        double startDate,
        double endDate,
        float epoch = 0
    )
    {
        Timeline.timeStep = timeStep;
        Timeline.startDate = startDate;
        Timeline.endDate = endDate;
        _slider.maxValue = (float)(endDate - startDate);
        _slider.value = Mathf.Clamp(epoch, _slider.minValue, _slider.maxValue);
        valueInput.text = SatDate.GetDateTime(startDate + _slider.value).ToString();
        timeStepInput.text = timeStep.ToString();
        EventManager.OnTimeChanged(epoch);
        EventManager.OnTimeStepChange(timeStep);
    }
}
