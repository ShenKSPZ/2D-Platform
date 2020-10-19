using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class SaveLoad : MonoBehaviour
{

    public GameManager GM;
    public AudioManager AM;

    private void Awake()
    {
        GM = GetComponent<GameManager>();
        AM = GetComponent<AudioManager>();
    }

    public bool SaveGame(Save save)
    {
        string filePath = Application.streamingAssetsPath + "/SaveData.json";
        string saveJsonStr = JsonMapper.ToJson(save);
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(saveJsonStr);
        sw.Close();
        AM.PlaySaveGame();
        return true;
    }

    public bool LoadGame()
    {
        string filePath = Application.streamingAssetsPath + "/SaveData.json";
        if (File.Exists(filePath))
        {
            StreamReader sr = new StreamReader(filePath);
            string jsonStr = sr.ReadToEnd();
            sr.Close();
            Save save = JsonMapper.ToObject<Save>(jsonStr);
            Debug.Log(save.HasSword);
            Debug.Log(save.CanWallJump);
            Debug.Log(save.SavePointNum);
            GM.SetGame(save);
            return true;
        }
        else
        {
            return false;
        }
    }
}

public class Save
{
    public int SavePointNum;
    public bool HasSword;
    public bool CanWallJump;
    public int AreaNum;
}
