using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jh_Lib;

public class ItemFactory : ObjectPool<ItemFactory, Item>
{
    public GameObject Item;

    float mPossiblity = Const.ItemDropPossiblity;
    Dictionary<int, Item> mItemMap = new Dictionary<int, Item>();


    public void SetItem(Vector3 pos)
    {
        // 아이템 드랍 여부
        bool drop_item = CheckSettingItemPossiblity();
        if (!drop_item) return;

        // @PoolObject Push
        GameObject item = Push(Item);
        item.transform.position = pos;
        item.SetActive(true);

        int index = TileMap.PosToIndex(pos);
        mItemMap[index] = item.GetComponent<Item>();

        // 어떤 아이템 드랍하는지
        int item_num = Random.Range(0, 4); // niddle, speed, bombsize, bombcount

        switch (item_num)
        {
            case 0:
                item.gameObject.AddComponent<Item_Niddle>();
                break;
            case 1:
                item.gameObject.AddComponent<Item_BombSize>();
                break;
            case 2:
                item.gameObject.AddComponent<Item_BombLength>();
                break;
            case 3:
                item.gameObject.AddComponent<Item_Speed>();
                break;
        }
        
    }

    bool CheckSettingItemPossiblity()
    {
        float rate = Random.Range(0.0f, 1.0f); //0.0f ~ 0.9~f
        return rate < mPossiblity;
    }

    public void RemoveItem(int index)
    {
        if (!mItemMap.ContainsKey(index)) return;

        mItemMap[index] = null;
    }

    public Item GetItembMap(int index)
    {
        if (!mItemMap.ContainsKey(index)) return null;

        return mItemMap[index];
    }
}
