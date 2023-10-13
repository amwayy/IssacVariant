using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private List<Skill> allSkillList;

    public static SkillManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public List<Skill> getAllSkillList()
    {
        return allSkillList;
    }
}
