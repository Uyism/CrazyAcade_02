using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour
{
    void Awake()
    {
        //1. 맵 생성
        TileMapFactory factory_map = this.GetComponent<TileMapFactory>();
        Dictionary<int, Tile> map_data = factory_map.MakeTileMap();
        this.GetComponent<TileMap>().SetTileMap(map_data);

        //2. 캐릭터 생성
        CharacterFactory factory_character = this.GetComponent<CharacterFactory>();
        factory_character.SetMode();
    }

    private void Update()
    {
        // 뒤로 가기
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene("Intro");
            }
        }
    }

}
