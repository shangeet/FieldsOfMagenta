using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotAdditionalProperties : MonoBehaviour {

    Dictionary<string, string> additionalPropertiesDict = new Dictionary<string, string>();

    public void AddProperty(string key, string value) {
        additionalPropertiesDict[key] = value;
    }

    public string GetProperty(string key) {
        return additionalPropertiesDict[key];
    }
}
