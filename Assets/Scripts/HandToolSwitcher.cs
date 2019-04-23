using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandToolSwitcher : MonoBehaviour {

    public Pointer ColorPointer;
    public PencilTool Pencil;

    public VRInput.Button SwitchButton;
    public VRInput.Hand InputHand;

    private bool isPicking;

    void Start () {
        setPicking (false);
    }

    void Update () {
        if (VRInput.GetButtonDown(SwitchButton, InputHand)) {
            setPicking (!isPicking);
        }
    }

    private void setPicking(bool isPicking) {
        this.isPicking = isPicking;
        Pencil.SetActive (!isPicking);
        ColorPointer.SetActive (isPicking);
    }

}
