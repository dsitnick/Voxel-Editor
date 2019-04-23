using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSlider : MonoBehaviour {

    public float LocalLength;

    public float Value { get; private set; } = 1;

    private Vector3 grabPos;
    private float grabVal;

    public void Grab (Vector3 position) {
        grabPos = position;
        grabVal = Value;
    }

    public void SetPosition(Vector3 position) {
        Value = Mathf.Clamp01((GetValue (position) - GetValue(grabPos)) / LocalLength + grabVal);
    }

    public float GetValue(Vector3 position) {
        return transform.InverseTransformPoint (position).z;
    }
}
