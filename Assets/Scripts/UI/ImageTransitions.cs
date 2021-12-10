using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageTransitions : MonoBehaviour
{
    public void TranslateAcrossScreen(Vector2 startPosition, Vector2 endPosition, float time) {
        GameObject objectToTransform = this.gameObject;
        objectToTransform.transform.position = Vector2.Lerp(startPosition, endPosition, time);
    }
}
