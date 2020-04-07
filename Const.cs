using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Const
{
    public const float StartPosX = 0;
    public const float StartPosY = 0;
    public const int TileCntX = 30;
    public const int TileCntY = 20;
    public const int BombLifeTime = 2;
    public const float WalkSpeed = 5.0f;
    public const float WaterBallTime = 5.0f;
    public const float WalkBlockFreeTime = 10;
    public const float ItemDropPossiblity= 0.5f;
    public const string MapDataName = "MapData";

    public const int BombSize = 1;
    public const int BombCount = 2;
    public const int NiddleCount = 2;



    public enum EXPLODE_EFFECT_TYPE { BOMB_CENTER, BOMB_END, BOMB_DIR, BOMB_DEFAULT } // ex) Bomb_Explode 애니메이션 타입 :방향, 중간, 끝
    public enum EPlayerState { EWalk, EWaterBall, EDead }

    public enum EDirection
    {
        Default,
        UP,
        Down,
        Right,
        Left
    }

    public enum ETileType

    {
        Default = 0,
        Rock,
        Forest_1,
        Flower,
        Basket
    }
}
