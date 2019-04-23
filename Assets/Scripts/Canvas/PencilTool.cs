using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PencilTool : MonoBehaviour {

    public MeshRenderer cursor;

    public Transform canvasRoot, pencilPoint;
    public VoxCanvas canvas;

    public ColorPicker colorPicker;

    public VRInput.Button PlaceButton, EraseButton;
    public VRInput.Hand Hand;

    private bool active;

    public void SetActive(bool active) {
        this.active = active;
        cursor.gameObject.SetActive (active);
    }

    private Vector3Int CurrentCoordinate { get {
        Vector3 p = canvasRoot.InverseTransformPoint (pencilPoint.position);
        return new Vector3Int (Mathf.RoundToInt (p.x), Mathf.RoundToInt (p.y), Mathf.RoundToInt (p.z));
    } }

    void Update () {
        if (!active)
            return;

        Vector3Int coordinate = CurrentCoordinate;

        cursor.transform.position = canvasRoot.TransformPoint (coordinate);
        cursor.transform.rotation = canvasRoot.rotation;

        if (VRInput.GetButtonDown (PlaceButton, Hand)) {
            canvas.Place (coordinate, colorPicker.CurrentColor);
        }
        if (VRInput.GetButtonDown (EraseButton, Hand)) {
            canvas.Erase (coordinate);
        }
    }
}
