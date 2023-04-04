using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Timeline : MonoBehaviour
{
    private Slider _slider;
    private TMP_InputField _input;

    [SerializeField]
    private bool _isPaused = false;

    public float timeScale = 0.01f;

    public double startDate = SatelliteUtils.startJulianDate;

    void Start()
    {
        _slider = GetComponentInChildren<Slider>();
        _input = GetComponentInChildren<TMP_InputField>();

        _slider.onValueChanged.AddListener(
            delegate(float value)
            {
                EventManager.OnTimeChanged(value);
                _input.text = SatelliteUtils.GetDateTime(value + startDate).ToString();
            }
        );

        _input.onEndEdit.AddListener(
            delegate(string input)
            {
                if (float.TryParse(input, out float result))
                {
                    _slider.value = Mathf.Clamp(result, _slider.minValue, _slider.maxValue);
                }
                else if (DateTime.TryParse(input, out DateTime dateTime))
                {
                    _slider.value = Mathf.Clamp(
                        (float)(SatelliteUtils.GetJulianDate(dateTime) - startDate),
                        _slider.minValue,
                        _slider.maxValue
                    );
                }
            }
        );

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
