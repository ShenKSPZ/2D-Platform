using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("目标")]
    public Transform Camera;
    public Camera cameraCom;
    public Transform Target;
    public UIManager UIMana;
    MainController Player;

    [Header("属性")]
    public float Z = -7.5f;
    public float SmoothTimeX = 0.42f;
    public float MaxSpeedX = 33;
    public float SmoothTimeY = 0.25f;
    public float MaxSpeedY = 33;
    public Vector2 offset;
    public List<CameraBoxRestrict> cameraBoxRestricts;
    public Transform LevelCenter;
    public bool HasRestrict;
    public int AreaNum;
    [Header("调整时长")]
    public float FOVChangeTime = 2f;
    public float OffsetXChangeTime = 2f;
    public float OffsetYChangeTime = 2f;

    public TargetRecord targetRecord;
    float velocityA;
    float velocityB;
    float FOVvelocity;
    float OffsetXVelocity;
    float OffsetYVelocity;

    // Use this for initialization
    void Awake()
    {
        Player = Target.gameObject.GetComponent<MainController>();
        cameraCom = Camera.GetComponent<Camera>();
        UIMana = GetComponent<UIManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cameraCom.fieldOfView = Mathf.SmoothDamp(cameraCom.fieldOfView, cameraBoxRestricts[AreaNum].TargetFOV, ref FOVvelocity, FOVChangeTime);
        offset.x = Mathf.SmoothDamp(offset.x, cameraBoxRestricts[AreaNum].CamOffest.x, ref OffsetXVelocity, OffsetXChangeTime);
        offset.y = Mathf.SmoothDamp(offset.y, cameraBoxRestricts[AreaNum].CamOffest.y, ref OffsetYVelocity, OffsetYChangeTime);
        if (HasRestrict)
        {
            if (Target.position.x > LevelCenter.position.x + cameraBoxRestricts[AreaNum].BoxOffset.x - (cameraBoxRestricts[AreaNum].BoxSize.x / 2) && Target.position.x < LevelCenter.position.x + cameraBoxRestricts[AreaNum].BoxOffset.x + (cameraBoxRestricts[AreaNum].BoxSize.x / 2) && Player.currentHP != 0)
            {
                if (Player.Flip)
                {
                    Camera.transform.position = new Vector3(Mathf.SmoothDamp(Camera.transform.position.x, Target.position.x - offset.x, ref velocityA, SmoothTimeX, MaxSpeedX), Camera.transform.position.y, Z);
                }
                else
                {
                    Camera.transform.position = new Vector3(Mathf.SmoothDamp(Camera.transform.position.x, Target.position.x + offset.x, ref velocityA, SmoothTimeX, MaxSpeedX), Camera.transform.position.y, Z);
                }
                targetRecord.position = new Vector2(Target.position.x, targetRecord.position.y);
                targetRecord.flip = Player.Flip;
                targetRecord.AreaNum = AreaNum;
            }
            else if(targetRecord.AreaNum != AreaNum)
            {
                if (Player.Flip)
                {
                    Camera.transform.position = new Vector3(Mathf.SmoothDamp(Camera.transform.position.x, Target.position.x - offset.x, ref velocityA, SmoothTimeX, MaxSpeedX), Camera.transform.position.y, Z);
                }
                else
                {
                    Camera.transform.position = new Vector3(Mathf.SmoothDamp(Camera.transform.position.x, Target.position.x + offset.x, ref velocityA, SmoothTimeX, MaxSpeedX), Camera.transform.position.y, Z);
                }
            }
            else
            {
                if (targetRecord.flip)
                {
                    Camera.transform.position = new Vector3(Mathf.SmoothDamp(Camera.transform.position.x, targetRecord.position.x - offset.x, ref velocityA, SmoothTimeX, MaxSpeedX), Camera.transform.position.y, Z);
                }
                else
                {
                    Camera.transform.position = new Vector3(Mathf.SmoothDamp(Camera.transform.position.x, targetRecord.position.x + offset.x, ref velocityA, SmoothTimeX, MaxSpeedX), Camera.transform.position.y, Z);
                }
            }

            if (Target.position.y > LevelCenter.position.y + cameraBoxRestricts[AreaNum].BoxOffset.y - (cameraBoxRestricts[AreaNum].BoxSize.y / 2) && Target.position.y < LevelCenter.position.y + cameraBoxRestricts[AreaNum].BoxOffset.y + (cameraBoxRestricts[AreaNum].BoxSize.y / 2) && Player.currentHP != 0)
            {
                targetRecord.position = new Vector2(targetRecord.position.x, Target.position.y);
                Camera.transform.position = new Vector3(Camera.transform.position.x, Mathf.SmoothDamp(Camera.transform.position.y, Target.position.y + offset.y, ref velocityB, SmoothTimeY, MaxSpeedY), Z);
            }
            else if (targetRecord.AreaNum != AreaNum)
            {
                targetRecord.position = new Vector2(targetRecord.position.x, Target.position.y);
                Camera.transform.position = new Vector3(Camera.transform.position.x, Mathf.SmoothDamp(Camera.transform.position.y, Target.position.y + offset.y, ref velocityB, SmoothTimeY, MaxSpeedY), Z);
            }
            else
            {
                Camera.transform.position = new Vector3(Camera.transform.position.x, Mathf.SmoothDamp(Camera.transform.position.y, targetRecord.position.y + offset.y, ref velocityB, SmoothTimeY, MaxSpeedY), Z);
            }
        }
        else
        {
            if (Player.currentHP != 0)
            {
                //X
                if (Player.Flip)
                {
                    Camera.transform.position = new Vector3(Mathf.SmoothDamp(Camera.transform.position.x, Target.position.x - offset.x, ref velocityA, SmoothTimeX, MaxSpeedX), Camera.transform.position.y, Z);
                }
                else
                {
                    Camera.transform.position = new Vector3(Mathf.SmoothDamp(Camera.transform.position.x, Target.position.x + offset.x, ref velocityA, SmoothTimeX, MaxSpeedX), Camera.transform.position.y, Z);
                }

                //Y
                targetRecord.position = new Vector2(targetRecord.position.x, Target.position.y);
                Camera.transform.position = new Vector3(Camera.transform.position.x, Mathf.SmoothDamp(Camera.transform.position.y, Target.position.y + offset.y, ref velocityB, SmoothTimeY, MaxSpeedY), Z);
            }
            else
            {

            }
        }
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach(var box in cameraBoxRestricts)
        {
            Gizmos.DrawWireCube(new Vector2(LevelCenter.position.x + box.BoxOffset.x, LevelCenter.position.y + box.BoxOffset.y), box.BoxSize);
        }
        
    }
}

[System.Serializable]
public class CameraBoxRestrict
{
    public Vector2 BoxOffset;
    public Vector2 BoxSize;
    public float TargetFOV;
    public Vector2 CamOffest;
}

[System.Serializable]
public class TargetRecord
{
    public Vector2 position;
    public bool flip;
    public int AreaNum;
}