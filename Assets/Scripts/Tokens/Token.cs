using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Token : MonoBehaviour
{
    [SerializeField] protected int countMax;
    [SerializeField] protected TextMeshProUGUI countText;

    protected int count;
    protected ISkillCaster tokenOwner;

    public int GetCount()
    {
        return count;
    }

    public virtual void Initialize(ISkillCaster skillCaster, int count)
    {
        this.count = count;
        tokenOwner = skillCaster;
        countText.text = count.ToString();
    }

    public virtual void IncreaseCount(int increaseCount)
    {
        if (count < countMax)
        {
            count += increaseCount;
            countText.text = count.ToString();
        }
    }

    public void DestroySelf()
    {
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
