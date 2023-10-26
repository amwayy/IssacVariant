using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterProperty : MonoBehaviour
{
    [SerializeField] private float extraHpPercentage = .25f;
    [SerializeField] private int singleSetTokenCount = 1;
    [SerializeField] private Transform oceanHeartTokenPrefab;

    private void Start()
    {
        Player.Instance.OnEnterBattle += Player_OnEnterBattle;
        Player.Instance.OnTakeDamage += Player_OnTakeDamage;
    }

    private void Player_OnTakeDamage(object sender, int e)
    {
        SetOceanHeartToken();
    }

    private void SetOceanHeartToken()
    {
        bool hasOceanHeartToken = false;
        OceanHeartToken oceanHeartToken = null;
        Transform tokenContainerTransform = Player.Instance.GetTokenContainerTransform();
        foreach (Transform tokenTransform in tokenContainerTransform)
        {
            if (tokenTransform.TryGetComponent(out oceanHeartToken))
            {
                hasOceanHeartToken = true;
                break;
            }
        }

        if (hasOceanHeartToken)
        {
            oceanHeartToken.IncreaseCount(singleSetTokenCount);
        }
        else
        {
            Player.Instance.SetToken(oceanHeartTokenPrefab, singleSetTokenCount);
        }
    }

    // ¿ªÆô»¤¶Ü
    private void Player_OnEnterBattle(object sender, Enemy e)
    {
        Player.Instance.SetExtraHp((int)(Player.Instance.GetHPMaxAmount() * extraHpPercentage));
        PlayerStatUI.Instance.ShowExtraHP();
    }

    private void OnDestroy()
    {
        Player.Instance.OnEnterBattle -= Player_OnEnterBattle;
        Player.Instance.OnTakeDamage -= Player_OnTakeDamage;
    }
}
