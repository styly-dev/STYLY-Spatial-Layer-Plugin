using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_SetParameterValuesToPrefab : MonoBehaviour
{

    public GameObject prefab;
    public string parameterValueJson;

    // Start is called before the first frame update
    void Start()
    {
        Styly.VisionOs.Plugin.VisualScriptingParameterUtility.SetParameterValuesToPrefab(prefab, parameterValueJson);
    }
}
