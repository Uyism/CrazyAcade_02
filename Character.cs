using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Jh_Lib;

public class Character : MonoBehaviour
{
    #region Parameter

    CharacterFactory mCharacterFactory;
    GameObject mSystemManager;
    public GameObject PlayerSign;

    [HideInInspector]
    public float mWalkSpeed = Const.WalkSpeed;
    float mOriWalkSpeed = Const.WalkSpeed;
    float mWaterBallTime = Const.WaterBallTime;
    float mOriWaterBallTime = Const.WaterBallTime;

    bool mIsAuto;
    [HideInInspector]
    public bool mIsPlayer;
    bool mIsPlayerTeam;

    // 폭탄 설치 순간은 자유로운 이동 가능
    // 이후로는 폭탄 있는 곳에서 걸을 수 없음
    bool mIsWalkBlockFreeTime = false;
    float mWalkingBlockFreeTime = Const.WalkBlockFreeTime;
    float mOriWalkingBlockFreeTime = Const.WalkBlockFreeTime;

    Animator anim;

    // Inspector
    [HideInInspector]
    public Const.EPlayerState mPlayerState = Const.EPlayerState.EWalk;

    [Header("캐릭터")]
    public int CurIndex = 0;

    [Header("보유 아이템")]
    [SerializeField]
    int NiddleCnt = Const.NiddleCount;
    public int BombCnt = Const.BombCount;
    public int BombSize = Const.BombSize;

    #endregion

    public int GetSetNiddleCnt { get => NiddleCnt; set { NiddleCnt = value; SetNiddleCntText(); } }

    private void Start()
    {
        mSystemManager = GameObject.Find("SystemManager");
        mCharacterFactory = mSystemManager.GetComponent<CharacterFactory>();

        anim = GetComponent<Animator>();
        GetSetNiddleCnt = 2;

        // 쉐이더로 팀 색상 구분해서 랜더링
        if (mIsPlayerTeam)
            this.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Material/character_red");
        else
            this.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Material/character_blue");
    }

    public void SetAutoControl(bool is_auto)
    {
        mIsAuto = is_auto;
        if (mIsAuto)
            gameObject.AddComponent<AutoController>();
        else
        {
            if (Application.platform == RuntimePlatform.Android)
                gameObject.AddComponent<PlayerController_mobile>();
            else
                gameObject.AddComponent<PlayerController>();
        }

        if (mIsPlayer)
            PlayerSign.SetActive(true);
    }

    void Update()
    {
        switch (mPlayerState)
        {
            case Const.EPlayerState.EWalk:
                CurIndex = TileMap.PosToIndex(transform.position);
                CheckTrapInWaterBall();
                break;

            case Const.EPlayerState.EWaterBall:
                CheckOpponentTouchWaterBall();
                break;

            case Const.EPlayerState.EDead:
                break;

        }
    }

    public void Move(Const.EDirection dir)
    {
        if (mPlayerState != Const.EPlayerState.EWalk) return;

        Vector3 pos_dir = Vector3.zero;
        switch (dir)
        {
            case Const.EDirection.UP: pos_dir = Vector3.up; break;
            case Const.EDirection.Down: pos_dir = Vector3.down; break;
            case Const.EDirection.Right: pos_dir = Vector3.right; break;
            case Const.EDirection.Left: pos_dir = Vector3.left; break;
        }
        SetMoveAnim(pos_dir);

        bool is_free = CheckIsWalkable(transform.position + pos_dir.normalized / 2);
        if (!is_free)
            return;

        transform.Translate(pos_dir * mWalkSpeed * Time.deltaTime);
    }

    public void Move(int index)
    {
        if (mPlayerState != Const.EPlayerState.EWalk) return;

        Vector3 target_pos = TileMap.IndexToPos(index);
        Vector3 cur_pos = transform.position;
        Vector3 dir = target_pos - cur_pos;
        SetMoveAnim(dir);

        bool is_free = CheckIsWalkable(transform.position + dir.normalized / 2);
        if (!is_free)
            return;

        transform.Translate(dir.normalized * mWalkSpeed * Time.deltaTime);
    }

    #region Inner Function

    void CheckTrapInWaterBall()
    {
        bool is_explode_tile = mSystemManager.GetComponent<BombExplodeFactory>().IsBombExplodeTile(TileMap.PosToIndex(transform.position));
        if (is_explode_tile == true)
        {
            mPlayerState = Const.EPlayerState.EWaterBall;
            anim.SetBool("water_ball", true);

            TestShaderStart();
        }
    }

    

    void CheckOpponentTouchWaterBall()
    {
        // 적과 만났을  경우 Dead
        List<Character> opponent_list = mCharacterFactory.GetOpponentList(mIsPlayerTeam);
        if (opponent_list == null)
            return;

        foreach (Character charac in opponent_list)
        {
            if (charac.mPlayerState == Const.EPlayerState.EWalk)
            {
                if (CurIndex == charac.CurIndex)
                    SetDead();
            }
        }

        // 아군과 만났을 경우 부활
        List<Character> my_team_list = mCharacterFactory.GetOpponentList(!mIsPlayerTeam);
        foreach (Character charac in my_team_list)
        {
            if (charac.mPlayerState == Const.EPlayerState.EWalk)
            {
                if (CurIndex == charac.CurIndex)
                    SetResurect();
            }
        }
    }

    void SetMoveAnim(Vector3 dir) 
    {
        Const.EDirection e_dir = JUtils.GetDir(dir, Vector3.zero);
        switch (e_dir)
        {
            case Const.EDirection.UP:  anim.SetTrigger("up"); break;
            case Const.EDirection.Down:  anim.SetTrigger("down"); break;
            case Const.EDirection.Right:  anim.SetTrigger("right"); break;
            case Const.EDirection.Left: anim.SetTrigger("left"); break;
        }
    }

    public bool CheckIsWalkable(Vector3 pos_dir)
    {
        // 물폭탄 구역 지나가도 되는 타임인가
        if (mIsWalkBlockFreeTime)
        {
            mWalkingBlockFreeTime -= 1;
            if (mWalkingBlockFreeTime < 0) mIsWalkBlockFreeTime = false;
        }
        else
        {
            mWalkingBlockFreeTime = 0;
            bool isWalkable = mSystemManager.GetComponent<TileMap>().IsWalkable(pos_dir);
            if (!isWalkable) return false;
        }

        return true;
    }

    public virtual void BombAttack()
    {
        if (mPlayerState != Const.EPlayerState.EWalk) return;

        if (BombCnt <= 0) return;

        int cur_index = TileMap.PosToIndex(this.transform.position);
        mSystemManager.GetComponent<BombFactory>().MakeBomb(cur_index, BombSize, this);

        BombCnt -= 1;
        mWalkingBlockFreeTime = mOriWalkingBlockFreeTime;
        mIsWalkBlockFreeTime = true;
    }

    public void PickUpBombCnt()
    {
        BombCnt += 1;
    }

    public void SpeedUp()
    {
        mWalkSpeed += 1f;
    }

    public void BombSizeUp()
    {
        BombCnt += 1;
    }

    public void BombLengthUp()
    {
        BombSize += 1;
    }

    public void AddNiddle()
    {
        GetSetNiddleCnt += 1;
    }

    void SetNiddleCntText()
    {
        if (!mIsAuto)
            GameObject.Find("NiddleBtn").GetComponentInChildren<Text>().text = NiddleCnt.ToString();
    }

    public void IsPlayerTeam(bool is_player_team)
    {
        mIsPlayerTeam = is_player_team;
    }

    public void SetDead()
    {
        mPlayerState = Const.EPlayerState.EDead;
        anim.SetBool("water_ball", false);
        anim.SetTrigger("dead");

        mSystemManager.GetComponent<AudioManager>().PlayWaterBallEndAudio();
    }

    public void RemoveCharacter()
    {
        gameObject.SetActive(false);
    }

    // 바늘로 부활
    public void SetResurect()
    {
        if (GetSetNiddleCnt <= 0) return;

        GetSetNiddleCnt -= 1;
        mPlayerState = Const.EPlayerState.EWalk;
        mWalkSpeed = mOriWalkSpeed;

        anim.SetTrigger("resurrect");
        anim.SetBool("water_ball", false);

        mSystemManager.GetComponent<AudioManager>().PlayWaterBallEndAudio();
    }

    void TestShaderStart()
    {
        if (!mIsPlayerTeam)
        {
            this.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Material/character_blue_blue");
            StartCoroutine("TestShaderReturn");
        }
    }

    IEnumerator TestShaderReturn()
    {
        yield return new WaitForSeconds(2);
        this.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Material/character_blue");
    }

    #endregion
}
