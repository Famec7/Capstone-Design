using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : HarvestableObject
{
    public override void DropLoot()
    {
        int randomIdx = Random.Range(4, 19);
        BaseItem item = ItemFactory.Instance.CreateItem(randomIdx);
        item.gameObject.transform.position = transform.parent.position;
        Destroy(gameObject.transform.parent.gameObject);
    }
}
