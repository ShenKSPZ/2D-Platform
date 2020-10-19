using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public MainController MC;
    public CameraManager Cam;
    public AudioManager AM;
    public UIPanel UI;
    public SaveLoad SL;
    public bool SaveFunctionSwitch;

    [Header("SavePoint")]
    public List<Transform> SavePoint = new List<Transform>();
    [Header("HasSword")]
    public Transform SwordItem;
    public Transform SwordTips;
    [Header("CanWallJump")]
    public Transform WallJumpDisk;

    // Start is called before the first frame update
    void Awake()
    {
        SL = GetComponent<SaveLoad>();
        Cam = GetComponent<CameraManager>();
        AM = GetComponent<AudioManager>();
        UI = FindObjectOfType<UIPanel>();
    }

    private void Start()
    {
        AM.PlayBGMusic();
        if (SaveFunctionSwitch)
        {
            bool LoadGameSuccess = SL.LoadGame();
            if (!LoadGameSuccess)
            {
                Save save = new Save
                {
                    SavePointNum = 0,
                    HasSword = false,
                    CanWallJump = false,
                    AreaNum = 0
                };
                MC.hasSword = false;
                MC.CanWallJump = false;
                SL.SaveGame(save);
                SetGame(save);
            }
        }
        Vector2 PPos = MC.transform.position;
        Camera.main.transform.position = PPos;
    }

    public void Quit()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UI.ShowMenu();
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetGame(Save save)
    {
        MC.transform.position = SavePoint[save.SavePointNum].position;
        MC.hasSword = save.HasSword;
        MC.CanWallJump = save.CanWallJump;
        Cam.AreaNum = save.AreaNum;
        if(save.HasSword)
        {
            if(SwordTips != null)
                Destroy(SwordTips.gameObject);
            if(SwordItem != null)
                Destroy(SwordItem.gameObject);
        }

        if(save.CanWallJump)
        {
            if (WallJumpDisk != null)
                Destroy(WallJumpDisk.gameObject);
        }
    }
}
