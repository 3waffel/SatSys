using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLogic : MonoBehaviour
{
    [SerializeField] protected Transform _targetPlanet;
    public Transform TargetPlanet { get => _targetPlanet; set => _targetPlanet = value; }
}
