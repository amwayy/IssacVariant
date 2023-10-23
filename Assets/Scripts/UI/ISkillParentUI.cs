using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillParentUI
{
    // 可以作为Skills的Parent的UI，包括BattleUI和BackpackWindowUI

    public List<EquippedSkillVisual> GetEquippedSkillList();

    public void ExchangeSkill(int equippedSkillIndex, int backupSkillIndex);

    public Transform GetTransform();
}
