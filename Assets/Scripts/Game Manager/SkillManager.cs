using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    #region Singletone
    public static SkillManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Skill Manager found.");
            return;
        }

        instance = this;
    }
    #endregion

    public List<Skill> items = new List<Skill>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
