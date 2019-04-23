using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour {

    public MeshRenderer Indicator;

    public PointSlider hueSlider, satSlider, valSlider;

    public Color CurrentColor { get; private set; }

    void LateUpdate () {
        CurrentColor = Color.HSVToRGB (hueSlider.Value, satSlider.Value, valSlider.Value);
        Indicator.material.color = CurrentColor;
    }

}
