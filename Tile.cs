using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour
{
    int mIndex;
    public int Index { get => mIndex; set => mIndex = value; }

    int mDurability;
    public int Durability { get => mDurability; set => mDurability = value; }

    bool mWalkable = true;
    public bool Walkable { get => mWalkable; set => mWalkable = value; }

    bool mIsBlocked = false;

    public Const.ETileType mTileType;
    Animator mAnimCrush;
    static public Dictionary<Const.ETileType, string> TILE_TEXTURE = new Dictionary<Const.ETileType, string>();

    private void Awake()
    {
        TILE_TEXTURE[Const.ETileType.Default] = null;
        TILE_TEXTURE[Const.ETileType.Forest_1] = "forest1";
        TILE_TEXTURE[Const.ETileType.Flower] = "flower";
        TILE_TEXTURE[Const.ETileType.Rock] = "rock";
        TILE_TEXTURE[Const.ETileType.Basket] = "basket";
        mAnimCrush = GetComponent<Animator>();
    }

    public void SetTileType(Const.ETileType type)
    {
        SetTileTexture(this.gameObject, type);
        mTileType = type;

        switch (type)
        {
            case Const.ETileType.Default:
                SetDefault();
                break;

            case Const.ETileType.Forest_1:
                SetForest1();
                break;

            case Const.ETileType.Rock:
                SetRock();
                break;

            case Const.ETileType.Basket:
                SetBasket();
                break;

            case Const.ETileType.Flower:
                SetFlower();
                break;
        }
    }

    public void CreateTile(int index)
    {
        Index = index;
        Vector3 pos = TileMap.IndexToPos(index);
        this.gameObject.transform.position = pos;
    }

    public static void SetTileTexture(GameObject obj, Const.ETileType type)
    {
        obj.SetActive(false);
        string res = TILE_TEXTURE[type];

        obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(res);
        obj.SetActive(type != Const.ETileType.Default);
    }

    #region Inner Function

    public void SetDefault()
    {
        Walkable = true;
        SetTileTexture(this.gameObject, Const.ETileType.Default);
    }

    public void SetForest1()
    {
        Durability = 1;
        Walkable = false;
    }

    public void SetFlower()
    {
        Durability = 1;
        Walkable = false;
    }

    public void SetBasket()
    {
        Durability = 1;
        Walkable = false;
    }

    public void SetRock()
    {
        mIsBlocked = true;
        Walkable = false;
        Durability = int.MaxValue;
    }

    public void HitTile()
    {
        Durability -= 1;
        CheckTile();
    }

    void CheckTile()
    {
        if (Durability <= 0) StartCrush();
    }

    public void StartCrush()
    {
        if (!this.gameObject.activeSelf) return;

        mAnimCrush.SetTrigger("dead");
        GameObject.Find("SystemManager").GetComponent<ItemFactory>().SetItem(transform.position);
    }

    public Const.ETileType GetTileType()
    {
        return mTileType;
    }

    public bool IsBlocked()
    {
        return mIsBlocked;
    }
    #endregion
}
