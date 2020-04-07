using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory : MonoBehaviour
{
    public GameObject Character;
    TileMap mTileMap;
    GameObject mCharacterFactoryObj;

    int mPlayMode; // AI / Contrl VS Contrl
    List<Character> mPlayerCharacList = new List<Character>();
    List<Character> mEnemyCharacList = new List<Character>();

    public void SetMode()
    {
        mTileMap = GameObject.Find("SystemManager").GetComponent<TileMap>();
        mCharacterFactoryObj = new GameObject("CharacterFactoryObj");

        MakePlayer();
        MakePlayerTeam();
        MakePlayerTeam();
        
        MakeOpponent(mPlayMode);
        MakeOpponent(mPlayMode);
        MakeOpponent(mPlayMode);
    }

    #region Inner Function

    void MakePlayer()
    {
        
        GameObject player = CreateCharacter();
        Character charac = player.GetComponent<Character>();
        charac.mIsPlayer = true;

        // 설정 : 수동, 플레이어 팀
        charac.SetAutoControl(false);
        charac.IsPlayerTeam(true);      

        mPlayerCharacList.Add(charac);
    }

    void MakePlayerTeam()
    {
        GameObject player = CreateCharacter();
        Character charac = player.GetComponent<Character>();

        charac.SetAutoControl(true);
        charac.IsPlayerTeam(true);
        mPlayerCharacList.Add(charac);
    }

    GameObject CreateCharacter()
    {
        GameObject _character = Instantiate(Character);
        _character.transform.parent = mCharacterFactoryObj.transform;
        _character.transform.position = GetRandomPos();

        return _character;
    }

    void MakeOpponent(int player_mode)
    {
        switch (player_mode)
        {
            default:
                GameObject obj = CreateCharacter();
                Character charac = obj.GetComponent<Character>();

                charac.SetAutoControl(true);
                charac.IsPlayerTeam(false);
                mEnemyCharacList.Add(charac);
                break;
        }
    }

    public Character GetPlayer()
    {
        return mPlayerCharacList[0];
    }

    public List<Character> GetEnemyList()
    {
        return mEnemyCharacList;
    }
    // @TODO 무조건 첫번째 적을 반환하는 상태
    public Character GetOpponent(bool is_player_team)
    {
        if (!is_player_team)
            return GetPlayer();
        else
            if (GetEnemyList().Count >= 1)
            return GetEnemyList()[0];

        return null;
    }

    public List<Character> GetOpponentList(bool is_player_team)
    {
        if (!is_player_team)
            return mPlayerCharacList;
        else
            if (GetEnemyList().Count >= 1)
            return GetEnemyList();

        return null;
    }

    Vector3 GetRandomPos()
    {
        List<int> walkable_index_list = new List<int>();
        for (int i = 0; i < Const.TileCntY; i++)
        {
            for (int j = 0; j < Const.TileCntX; j++)
            {
                int index = i + j * Const.TileCntX;
                if (mTileMap.IsWalkable(index))
                    walkable_index_list.Add(index);
            }
        }

        int random = Random.Range(0, walkable_index_list.Count);
        int random_index = walkable_index_list[random];
        Vector3 random_pos = TileMap.IndexToPos(random_index);
        random_pos.z = -1;
        return random_pos;
    }

    #endregion
}
