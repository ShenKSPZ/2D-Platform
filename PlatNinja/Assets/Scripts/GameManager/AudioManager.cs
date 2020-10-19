using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioSource AS;
    public AudioSource Music;
    public AudioClip BGMusic;
    public AudioClip BossStageBridge;
    public AudioClip BossStage1;
    public AudioClip BossStage2;
    public AudioClip WindSound;
    public AudioClip LeftFoot;
    public AudioClip RightFoot;
    public AudioClip Dash;
    public AudioClip TouchGround;
    public AudioClip Slash;
    public AudioClip Hit;
    public AudioClip SaveGame;
    public AudioClip Voice;
    public AudioClip Explosion;
    public AudioClip BigFall;
    public AudioClip Victory;

    public void PlaySaveGame()
    {
        AS.PlayOneShot(SaveGame);
    }

    public void PlayVoice()
    {
        AS.PlayOneShot(Voice);
    }

    public void PlayExplosion()
    {
        AS.PlayOneShot(Explosion);
    }

    public void PlayLeft()
    {
        AS.PlayOneShot(LeftFoot);
    }

    public void PlayRight()
    {
        AS.PlayOneShot(RightFoot);
    }

    public void PlayDash()
    {
        AS.PlayOneShot(Dash);
    }

    public  void PlayTouchedGround()
    {
        AS.PlayOneShot(TouchGround);
    }

    public void PlayBigFallTouchedGround()
    {
        AS.PlayOneShot(BigFall);
    }

    public void PlaySwordSlash()
    {
        AS.PlayOneShot(Slash);
    }

    public void PlayHitSomeThing()
    {
        AS.PlayOneShot(Hit);
    }

    public void PlayBossStageBridge()
    {
        Music.volume = .6f;
        Music.PlayOneShot(BossStageBridge);
    }

    public void PlayBGMusic()
    {
        Music.volume = 0.2f;
        Music.clip = BGMusic;
        Music.Play();
    }

    public void PlayBossStageOne()
    {
        Music.clip = BossStage1;
        Music.Play();
    }

    public void PlayBossStageTwo()
    {
        Music.clip = BossStage2;
        Music.Play();
    }

    public void StopMusic()
    {
        Music.Stop();
    }

    public void MusicFade(float time)
    {

    }

    public void PlayVictory()
    {
        Music.PlayOneShot(Victory);
    }
}
