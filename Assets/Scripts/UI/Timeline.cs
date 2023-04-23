using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using SatSys;

public class Timeline : MonoBehaviour
{
    private Slider _slider;

    public TMP_InputField valueInput;
    public TMP_InputField timeScaleInput;

    [SerializeField]
    private bool _isPaused = false;

    public float timeScale = 0.001f;

    public double startDate = SatDate.startJulianDate;

    void Start()
    {
        _slider = GetComponentInChildren<Slider>();
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

        if (timeScaleInput != null)
        {
            timeScaleInput.text = timeScale.ToString();
            timeScaleInput.onEndEdit.AddListener(
                delegate(string input)
                {
                    if (float.TryParse(input, out float result))
                    {
                        timeScale = Mathf.Clamp(result, 0.00001f, 1f);
                        EventManager.OnTimeScaleChange(timeScale);
                    }
                }
            );
        }

        var toggle = GetComponentInChildren<Toggle>();
        toggle.isOn = false;
        toggle.onValueChanged.AddListener(
            delegate(bool flag)
            {
                _isPaused = flag;
            }
        );
    }

    void Update()
    {
        if (!_isPaused)
        {
            _slider.value += Time.deltaTime * timeScale;
        }
    }
}
