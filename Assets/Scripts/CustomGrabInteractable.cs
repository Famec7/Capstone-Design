    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomGrabInteractable : XRGrabInteractable
{
    // Use this instead of XR Grab Interactable to prevent hands fighting over objects.
    // Fighting hands can cause hand animations to break, and causes rapid-fire grab sounds.
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        bool isAlreadyGrabbed = false;

        if (isSelected && !interactor.Equals(firstInteractorSelecting))
        {
            var grabber = firstInteractorSelecting as XRDirectInteractor;
            if (grabber != null)
            {
                // the grabber is a direct interactor
                isAlreadyGrabbed = true;
            }
        }

        return base.IsSelectableBy(interactor) && !isAlreadyGrabbed;
    }
}