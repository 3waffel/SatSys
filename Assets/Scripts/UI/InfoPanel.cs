using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.ObjectInfoSent += UpdateInfo;
    }

    void UpdateInfo(ObjectLogic logic)
    {
        switch (logic)
        {
            case SatelliteLogic satellite:
                break;
            case StationLogic station:
                break;
            default:
                break;
        }
    }
}
