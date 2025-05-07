using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTool : MonoBehaviour
{
    // Update is called once per frame

    [SerializeField]
    private ToolSlot _toolSlot;

    public GameObject Player;
    public float LimitDistance = 10f;

    void Update()
    {
        float distance = Vector3.Distance(Player.transform.position, transform.position);
        if (distance > LimitDistance)
        {
            _toolSlot.ToolUI.AbleSlot(_toolSlot);
            gameObject.SetActive(false);
        }
    }
}
