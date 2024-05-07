using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Effects : SkillBase
{
    public GameObject gameClip;
    Player player;

    ParticleSystem particleSystem;

    GameObject obj;
    public Skill_Effects(Player _player)
    {
        player = _player;

    }

    public void SetGameClip(GameObject _effectClip)
    {
        gameClip = _effectClip;
        if (gameClip.GetComponent<ParticleSystem>())
        {
            obj = GameObject.Instantiate(gameClip, player.effectsparent);
            particleSystem = obj.GetComponent<ParticleSystem>();
            particleSystem.Stop();
        }
        name = _effectClip.name;
    }

    public override void Play()
    {
        base.Play();
        starttime = Time.time;
        isBegin = true;

    }
    public override void Update(float times)
    {
        if ((times - starttime) > float.Parse(trigger) && isBegin)
        {
            isBegin = false;
            Begin();
        }
    }

    private void Begin()
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }
    public override void Init()
    {
        if (gameClip.GetComponent<ParticleSystem>())
        {
            particleSystem = obj.GetComponent<ParticleSystem>();
            particleSystem.Stop();
        }
    }
    public override void Stop()
    {
        base.Play();
        if (particleSystem != null)
            particleSystem.Stop();

    }
}
