using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIPanel : MonoBehaviour
{

    public Image ContentBG;
    public Text ContentText;
    public Image ContentPressKey;
    public Canvas canvas;
    public Text AreaTitle;
    public Image BlackScreen;
    public List<Button> Buttons;
    public Vector2 BGOffest;
    public SaveLoad SL;

    public GameObject worldPos;
    [SerializeField]
    RectTransform rectText;
    [SerializeField]
    RectTransform rectBG;
    [SerializeField]
    RectTransform rectTips;
    public Vector2 TextFollowOffset;
    public Vector2 BGFollowOffset;
    public Vector2 TipsFollowOffset;
    public Image Menu;
    public Slider YOffset;
    public Text YOffsetValue;
    public MainController MC;
    public GameManager GM;

    public bool AreaTitleShowing;
    float AlphaVelocity;
    float AreaTitleDuration;
    float AreaTitleFadeTime;
    float AreaTitleTimer;

    public bool BlackScreenShowing;
    float BlackVelocity;
    float BlackDuration;
    float BlackFadeTime;
    float BlackTimer;

    private void Awake()
    {
        MC = FindObjectOfType<MainController>();
        SL = FindObjectOfType<SaveLoad>();
        GM = FindObjectOfType<GameManager>();
        Menu.gameObject.SetActive(false);
    }

    public void ShowCanvas(bool enabled)
    {
        canvas.enabled = enabled;
    }

    public void ShowContentBG(bool enabled)
    {
        ContentBG.enabled = enabled;
    }

    public void ShowContentText(bool enabled)
    {
        ContentText.enabled = enabled;
    }

    public void ShowAll(bool enabled)
    {
        ContentBG.enabled = enabled;
        ContentText.enabled = enabled;
    }

    public void SetContentText(string Text)
    {
        ContentText.text = Text;
    }

    public void ShowAreaTitle(string Text, float Time, float FadeTime)
    {
        if(!AreaTitleShowing && AreaTitle.color.a == 0)
        {
            AreaTitleShowing = true;
            AreaTitle.text = Text;
            AreaTitleDuration = Time;
            AreaTitleFadeTime = FadeTime;
        }
    }

    public void ShowBlackScreen(float Time, float FadeTime)
    {
        if (!BlackScreenShowing && BlackScreen.color.a == 0)
        {
            BlackScreenShowing = true;
            BlackDuration = Time;
            BlackFadeTime = FadeTime;
        }
    }

    public void ShowTips(bool enable)
    {
        ContentPressKey.enabled = enable;
    }

    public void ShowMenu()
    {
        Menu.gameObject.SetActive(!Menu.gameObject.activeSelf);
        YOffset.value = MC.dirOffest.y;
    }

    public void YAxisOffset()
    {
        MC.dirOffest.y = YOffset.value;
        YOffsetValue.text = (Mathf.Round(YOffset.value * 1000)/1000).ToString();
    }

    public void ResetSavaData()
    {
        Save save = new Save
        {
            SavePointNum = 0,
            HasSword = false,
            CanWallJump = false,
            AreaNum = 0
        };
        SL.SaveGame(save);
        GM.ReloadScene();
    }

    public void Quit()
    {
        GM.Quit();
    }

    void Update()
    {
        //对话框对齐文字
        ContentBG.rectTransform.sizeDelta = ContentText.rectTransform.sizeDelta + BGOffest;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos.transform.position);
        rectBG.position = screenPos + BGFollowOffset;
        rectText.position = screenPos + TextFollowOffset;
        //rectTips.position = screenPos + TipsFollowOffset;

        if (AreaTitleShowing && AreaTitle.color.a < 0.98)
        {
            Debug.Log("b");
            Color AC = Color.white;
            AC.a = Mathf.SmoothDamp(AreaTitle.color.a, 1, ref AlphaVelocity, AreaTitleFadeTime);
            AreaTitle.color = AC;
        }
        else if (AreaTitleShowing && AreaTitle.color.a >= 0.98 && AreaTitleTimer < AreaTitleDuration)
        {
            AreaTitle.color = new Color(AreaTitle.color.r, AreaTitle.color.g, AreaTitle.color.b, 1);
            AreaTitleTimer += Time.deltaTime;
        }
        else if (AreaTitleShowing && AreaTitle.color.a >= 0.98 && AreaTitleTimer >= AreaTitleDuration)
        {
            AreaTitleShowing = false;
        }
        else if (!AreaTitleShowing && AreaTitle.color.a >= 0.01)
        {
            AreaTitle.color = new Color(AreaTitle.color.r, AreaTitle.color.g, AreaTitle.color.b, Mathf.SmoothDamp(AreaTitle.color.a, 0, ref AlphaVelocity, AreaTitleFadeTime));
        }
        else if (!AreaTitleShowing && AreaTitle.color.a < 0.01)
        {
            AreaTitle.color = new Color(AreaTitle.color.r, AreaTitle.color.g, AreaTitle.color.b, 0);
            AreaTitleTimer = 0;
        }

        if (BlackScreenShowing && BlackScreen.color.a < 0.98)
        {
            Color AC = Color.black;
            AC.a = Mathf.SmoothDamp(BlackScreen.color.a, 1, ref BlackVelocity, BlackFadeTime);
            BlackScreen.color = AC;
        }
        else if (BlackScreenShowing && BlackScreen.color.a >= 0.98 && BlackTimer < BlackDuration)
        {
            BlackScreen.color = Color.black;
            BlackTimer += Time.deltaTime;
        }
        else if (BlackScreenShowing && BlackScreen.color.a >= 0.98 && BlackTimer >= BlackDuration)
        {
            BlackScreenShowing = false;
        }
        else if (!BlackScreenShowing && BlackScreen.color.a >= 0.01)
        {
            BlackScreen.color = new Color(BlackScreen.color.r, BlackScreen.color.g, BlackScreen.color.b, Mathf.SmoothDamp(BlackScreen.color.a, 0, ref BlackVelocity, BlackFadeTime));
        }
        else if (!BlackScreenShowing && BlackScreen.color.a < 0.01)
        {
            BlackScreen.color = new Color(BlackScreen.color.r, BlackScreen.color.g, BlackScreen.color.b, 0);
            BlackTimer = 0;
        }
    }
}
