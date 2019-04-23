using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxCanvas : MonoBehaviour {

    public VoxCanvasRenderer Renderer;

    public void Place(Vector3Int position, Color color) {
        Renderer.Place (position, color);
    }

    public void Erase(Vector3Int position) {
        Renderer.Erase (position);
    }

}