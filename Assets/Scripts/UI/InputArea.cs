using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using TMPro;
using System;

public class InputArea : MonoBehaviour
{
    [SerializeField]
    private List<Transform> rows = new List<Transform>();

    private List<TMP_InputField> inputFields = new List<TMP_InputField>();
    private List<TMP_Text> texts = new List<TMP_Text>();

    [SerializeField]
    private List<string> values = new List<string>();

    string[] satParams =
    {
        "SemiMajorAxis",
        "Eccentricity",
        "Inclination",
        "Periapsis",
        "AscendingNode",
        "MeanAnomaly",
        "Name",
        "TargetStation",
        "ReceiverStation",
    };
    string[] sttParams = { "Longitude", "Latitude", "Name", };

    void Start()
    {
        values.AddRange(new string[satParams.Length]);
        foreach (var row in rows)
        {
            var input = row.GetComponentInChildren<TMP_InputField>();
            inputFields.Add(input);
            var text = row.GetComponentInChildren<TMP_Text>();
            texts.Add(text);
        }

        IEnumerator enumerator;
        if (rows.Count == satParams.Length)
        {
            enumerator = satParams.GetEnumerator();
        }
        else if (rows.Count == sttParams.Length)
        {
            enumerator = sttParams.GetEnumerator();
        }
        else
            return;

        foreach (var text in texts)
        {
            enumerator.MoveNext();
            text.text = enumerator.Current as string;
        }
    }

    public List<string> GetInputValues()
    {
        foreach (var input in inputFields)
        {
            int index = inputFields.IndexOf(input);
            values[index] = input.text;
        }
        return values;
    }
}
