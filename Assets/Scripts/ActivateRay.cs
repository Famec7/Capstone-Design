using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
public class ActivateRay : MonoBehaviour
{
    public GameObject RightTp;
    public InputActionProperty RightActivate;
    // Update is called once per frame
    void Update()
    {
        RightTp.SetActive(RightActivate.action.ReadValue<float>() > 0.1f);
    }
}
