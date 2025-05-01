using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandManager : Singleton<HandManager>
{
    [SerializeField] private XRBaseInteractor leftHandInteractor;
    [SerializeField] private XRBaseInteractor rightHandInteractor;


    protected override void Init()
    {
        ;   
    }


    public bool IsLeftHandHolding()
    {
        return leftHandInteractor.interactablesSelected.Count > 0;
    }

    public bool IsRightHandHolding()
    {
        return rightHandInteractor.interactablesSelected.Count > 0;
    }


    public GameObject GetLeftHeldObject()
    {
        if (IsLeftHandHolding())
            return leftHandInteractor.interactablesSelected[0].transform.gameObject;
        return null;
    }

    public GameObject GetRightHeldObject()
    {
        if (IsRightHandHolding())
            return rightHandInteractor.interactablesSelected[0].transform.gameObject;
        return null;
    }
}
