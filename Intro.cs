using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    bool mIsPVPMode = false;
    public GameObject PVPMenu;
    public GameObject PVEMenu;
    public GameObject SelectMenu;

    public void GotoGame()
    {
        if (!mIsPVPMode)
            SceneManager.LoadScene("GameScene");
        else
            this.GetComponent<NetSetting>().GotoPVPGame();
    }

    public void GotoMapTool()
    {
        SceneManager.LoadScene("MapTool");
    }

    public void SetPVPMode()
    {
        mIsPVPMode = true;
        gameObject.AddComponent<NetSetting>();
        SetMode();
    }


    public void SetPVEMode()
    {
        mIsPVPMode = false;
        SetMode();

        StructUserData user_data = new StructUserData();
        user_data.uid = "";
        user_data.isPVPMode = false;

        // PVE 모드 저장
        JsonFactory.Write(Const.UserDataName, user_data);
    }

    void SetMode()
    {
        SelectMenu.SetActive(false);
        PVPMenu.SetActive(mIsPVPMode);
        PVEMenu.SetActive(!mIsPVPMode);
    }
}
