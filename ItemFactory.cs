using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jh_Lib;
using System.Net.Sockets;

public class ItemFactory : ObjectPool<ItemFactory, Item>
{
    public GameObject Item;

    float mPossiblity = Const.ItemDropPossiblity;
    Dictionary<int, Item> mItemMap = new Dictionary<int, Item>();
    char[] mItemList;
    int mItemTryCnt = 0;
    public void Start()
    {
        // 로컬 데이터에 들어있는 유저 정보 저장
        string data = JsonFactory.Load(Const.UserDataName);
        StructUserData _data = JsonUtility.FromJson<StructUserData>(data);
        Const.UserData = _data;

        if (Const.UserData.isPVPMode)
            SetItemList();
    }

    public void SetItem(Vector3 pos)
    {
        int item_num = 0;
        GameObject item = null;
        if (!Const.UserData.isPVPMode)
        {
            // 아이템 드랍 여부
            bool drop_item = CheckSettingItemPossiblity();
            if (!drop_item) return;

            // 어떤 아이템 드랍하는지
            item_num = Random.Range(0, 4); // niddle, speed, bombsize, bombcount
        }
        else
        {
            string num = mItemList[mItemTryCnt].ToString();
            mItemTryCnt += 1;
            item_num = int.Parse(num);
            // 아이템 외 숫자는 무효 처리
            if (item_num == 5)
                return;
        }


        // @PoolObject Push
        item = Push(Item);
        item.transform.position = pos;
        item.SetActive(true);

        int index = TileMap.PosToIndex(pos);
        mItemMap[index] = item.GetComponent<Item>();

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

    public void SetItemList()
    {
        mItemList = Const.UserData.itemData.ToCharArray();
    }
}
