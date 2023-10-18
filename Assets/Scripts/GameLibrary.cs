using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLibrary : MonoBehaviour
{
    // 储存游戏中各种数据，如技能、Buff、Debuff等

    [SerializeField] private List<Skill> allSkillList;
    [SerializeField] private List<Skill> grassSkillList;
    [SerializeField] private List<Skill> fireSkillList;
    [SerializeField] private List<Skill> waterSkillList;
    [SerializeField] private List<Skill> lightSkillList;
    [SerializeField] private List<Skill> darkSkillList;
    [SerializeField] private Color grassColor;
    [SerializeField] private Color fireColor;
    [SerializeField] private Color waterColor;
    [SerializeField] private Color lightColor;
    [SerializeField] private Color darkColor;

    public static GameLibrary Instance { get; private set; }

    public enum Element
    {
        Grass,
        Fire,
        Water,
        Light,
        Dark
    }

    private void Awake()
    {
        Instance = this;
    }

    public Color GetElementColor(Element element)
    {
        switch (element)
        {
            case Element.Grass:
                return grassColor;
            case Element.Fire:
                return fireColor;
            case Element.Water:
                return waterColor;
            case Element.Light:
                return lightColor;
            case Element.Dark:
                return darkColor;
        }
        return Color.black;
    }

    public List<Skill> GetElementSkillList(Element element)
    {
        switch (element)
        {
            case Element.Grass:
                return grassSkillList;
            case Element.Fire:
                return fireSkillList;
            case Element.Water:
                return waterSkillList;
            case Element.Light:
                return lightSkillList;
            case Element.Dark:
                return darkSkillList;
        }
        return null;
    }

    public List<Skill> GetAllSkillList()
    {
        return allSkillList;
    }
}
