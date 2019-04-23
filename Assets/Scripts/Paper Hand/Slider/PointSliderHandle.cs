using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSliderHandle : Pointable {

    public PointSlider slider;
    [Range(0, 2)]
    public float HighlightScale = 1.1f;

    private Vector3 scale;

    void Awake () {
        scale = transform.localScale;
    }

    void Start () {
        UpdatePosition ();
    }

    public override void SetSelected (bool IsSelected, Pointer pointer) {
        base.SetSelected (IsSelected, pointer);
        if (IsSelected) {
            slider.Grab (pointer.HitPosition);
        }
    }

    public override void PointerUpdate (Vector3 hitPos, bool isHit) {
        if (isHit) {
            slider.SetPosition (hitPos);
            UpdatePosition ();
        }
    }

    protected override void RefreshState () {
        transform.localScale = scale * (IsHighlight ? HighlightScale : 1);
    }

    private void UpdatePosition () {
        transform.localPosition = Vector3.forward * (slider.Value - 0.5f) * slider.LocalLength;
    }
}
