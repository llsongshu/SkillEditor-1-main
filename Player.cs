using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Xml;
using System.IO;
//using LitJson; 
using Newtonsoft.Json;

public class Player : MonoBehaviour
{
    public Dictionary<string, List<SkillBase>> skillsList = new Dictionary<string, List<SkillBase>>();

    RuntimeAnimatorController controller;

    public AnimatorOverrideController overrideController;

    public Transform effectsparent;

    AudioSource audioSource;
    //Player

    Animator anim;


    public List<SkillBase> currSkillComponets = new List<SkillBase>();
    /*    public float animSpeed = 1;

        public float AnimSpeed
        {
            set
            {
                if (anim != null)
                {
                    anim.speed = value;
                }
                animSpeed = value;
            }
            get

            {
                return animSpeed;
            }

        }*/
    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
    }
    public void Update()
    {
        foreach (var item in currSkillComponets)
        {
            item.Update(Time.time);
        }
    }
    


    public void play()
    {
        foreach (var item in currSkillComponets)
        {
            item.Play();
        }

    }

    private Skill_Anim _Anim;
    private Skill_Audio _Aduio;
    private Skill_Effects _Effect;



    public static Player Init(string path)
    {
        if (path != null)
        {
            string str = "Assets/aaa/" + path + ".prefab";
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(str);
            if (obj != null)
            {
                Player player = Instantiate(obj).GetComponent<Player>();
                player.controller = Resources.Load<RuntimeAnimatorController>("Player");
                player.anim.runtimeAnimatorController = player.overrideController;
                player.overrideController = new AnimatorOverrideController();
                player.overrideController.runtimeAnimatorController = player.controller;

                player.audioSource = player.gameObject.AddComponent<AudioSource>();
                player.effectsparent = player.transform.Find("effectsparent");
                player.gameObject.name = path;
                player.LoadAllSkill();
                return player;
            }
        }
        return null;
    }
    public List<SkillBase> GetSkill(string skillName)
    {
        if (skillsList.ContainsKey(skillName))
        {
            return skillsList[skillName];//List<skillbase>
        }
        return null;
    }

    public List<SkillBase> AddNewSkill(string newSkillName)
    {
        if (skillsList.ContainsKey(newSkillName))
        {
            return skillsList[newSkillName];
        }
        skillsList.Add(newSkillName, new List<SkillBase>());
        return skillsList[newSkillName];
    }
    public void RevSkill(string newSkillName)
    {
        if (skillsList.ContainsKey(newSkillName))
        {
            skillsList.Remove(newSkillName);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
    void LoadAllSkill()
    {
        if (File.Exists("Assets/" + gameObject.name + ".txt"))
        {
            string str = File.ReadAllText("Assets/" + gameObject.name + ".txt");
            List<SkillXml> skills = JsonConvert.DeserializeObject<List<SkillXml>>(str);
            foreach (var item in skills)
            {
                skillsList.Add(item.name, new List<SkillBase>());
                foreach (var ite in item.skillComponents)
                {
                    foreach (var it in ite.Value)
                    {
                        if (ite.Key.Equals("动画"))
                        {
                            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/GameDate/Anim/" + it.ComponentName + ".anim");;
                            Skill_Anim _Anim = new Skill_Anim(this);
                            _Anim.SetAnimClip(clip);
                            _Anim.SetTrigger(it.trigger);
                            skillsList[item.name].Add(_Anim);
                        }
                        else if (ite.Key.Equals("音效"))
                        {
                            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/GameDate/Audio/" + it.ComponentName + ".mp3");
                            Skill_Audio _Anim = new Skill_Audio(this);
                            _Anim.SetAnimClip(clip);
                            _Anim.SetTrigger(it.trigger);
                            skillsList[item.name].Add(_Anim);
                        }
                        else if (ite.Key.Equals("特效"))
                        {
                            GameObject clip = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GameDate/Effect/Skill/" + it.ComponentName + ".prefab");
                            Skill_Effects _Anim = new Skill_Effects(this);
                            _Anim.SetGameClip(clip);
                            _Anim.SetTrigger(it.trigger);
                            skillsList[item.name].Add(_Anim);
                        }
                    }
                }
            }
        }
    }
    private void SavaAllSkill()
    {
        List<SkillXml> skills = new List<SkillXml>();
        foreach (var item in skillsList)
        {
            SkillXml skillXml = new SkillXml();
            skillXml.name = item.Key;
            foreach (var ite in item.Value)
            {
                if (ite is Skill_Anim)
                {
                    if (!skillXml.skillComponents.ContainsKey("动画"))
                    {
                        skillXml.skillComponents.Add("动画", new List<SkillComponentsData>());
                    }
                    skillXml.skillComponents["动画"].Add(new SkillComponentsData(ite.name, ite.trigger));
                }
                else if (ite is Skill_Audio)
                {
                    if (!skillXml.skillComponents.ContainsKey("音效"))
                    {
                        skillXml.skillComponents.Add("音效", new List<SkillComponentsData>());
                    }
                    skillXml.skillComponents["音效"].Add(new SkillComponentsData(ite.name, ite.trigger));
                }
                else if (ite is Skill_Effects)
                {
                    if (!skillXml.skillComponents.ContainsKey("特效"))
                    {
                        skillXml.skillComponents.Add("特效", new List<SkillComponentsData>());
                    }
                    skillXml.skillComponents["特效"].Add(new SkillComponentsData( ite.name , ite.trigger));
                }

            }
            skills.Add(skillXml);
        }
        string str = JsonConvert.SerializeObject(skills);
        File.WriteAllText("Assets/" + gameObject.name + ".txt", str);
    }
    #region 游戏
    public void InitData()
    {
        overrideController = new AnimatorOverrideController();
        controller = Resources.Load<RuntimeAnimatorController>("Player");
        overrideController.runtimeAnimatorController = controller;
        anim.runtimeAnimatorController = overrideController;
        audioSource = gameObject.AddComponent<AudioSource>();
        effectsparent = transform.Find("effectsparent");
        //gameObject.name = path;
        //LoadAllSkill();
    }
    public void SetData(string skillName)
    {
        List<SkillXml> skillList = GameData.Instance.GetSkillsByRoleName("Teddy");
        //Debug.Log(skillList[0]);
        foreach (var item in skillList)
        {
            if (item.name == skillName)
            {
                currSkillComponets.Clear();
                foreach (var ite in item.skillComponents)
                {
                    foreach (var it in ite.Value)
                    {
                        if (ite.Key.Equals("动画"))
                        {
                            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/GameDate/Anim/" + it.ComponentName + ".anim");
                            if (_Anim == null) _Anim = new Skill_Anim(this);
                            _Anim.SetAnimClip(clip);
                            _Anim.SetTrigger(it.trigger);
                            currSkillComponets.Add(_Anim);
                        }
                        else if (ite.Key.Equals("音效"))
                        {
                            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/GameDate/Audio/" + it.ComponentName + ".mp3");
                            if (_Aduio == null) _Aduio = new Skill_Audio(this);
                            _Aduio.SetAnimClip(clip);
                            _Aduio.SetTrigger(it.trigger);
                            currSkillComponets.Add(_Aduio);
                        }
                        else if (ite.Key.Equals("特效"))
                        {
                            GameObject clip = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GameDate/Effect/Skill/" + it.ComponentName + ".prefab");
                            if (_Effect == null) _Effect = new Skill_Effects(this);
                            _Effect.SetGameClip(clip);
                            _Effect.SetTrigger(it.trigger);
                            currSkillComponets.Add(_Effect);
                        }
                    }
                }
            }
        }

    }
    #endregion
}
public class SkillXml
{
    public string name;
    public Dictionary<string, List<SkillComponentsData>> skillComponents = new Dictionary<string, List<SkillComponentsData>>();
}
public class SkillComponentsData
{
    public string ComponentName;
    public string trigger;
    public SkillComponentsData(string cn,string tri)
    {
        ComponentName = cn;
        trigger = tri;
    }
}
