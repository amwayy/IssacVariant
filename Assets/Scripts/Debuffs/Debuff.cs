using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Debuff : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject iconGameObject;
    [SerializeField] private float debuffMakeEffectTimerMax = .5f;   // Debuff生效动画的时间

    protected ISkillCaster skillCaster;

    private float setDebuffTimer;
    private float setDebuffTimerMax;
    private bool isSettingDebuff = true;
    private int countdown;

    private void Start()
    {
        if (skillCaster.IsPlayer())
        {
            TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
        } else
        {
            TurnManager.Instance.OnEnterEnemyTurn += TurnManager_OnEnterEnemyTurn;
        }
    }

    private void Update()
    {
        if (isSettingDebuff)
        {
            setDebuffTimer -= Time.deltaTime;
            if (setDebuffTimer <= 0f)
            {
                isSettingDebuff = false;
                ShowVisual();
            }
        }
    }

    public void IncreaseCountdown(int increaseAmount)
    {
        isSettingDebuff = true;
        setDebuffTimer = setDebuffTimerMax;
        countdown += increaseAmount;
    }

    public float GetDebuffMakeEffectTimerMax()
    {
        return debuffMakeEffectTimerMax;
    }

    private void ShowVisual()
    {
        countdownText.text = countdown.ToString();

        countdownText.gameObject.SetActive(true);
        iconGameObject.SetActive(true);
    }

    private void HideVisual()
    {
        countdownText.gameObject.SetActive(false);
        iconGameObject.SetActive(false);
    }

    public void Initialize(ISkillCaster skillCaster, int countdown, float setDebuffTimerMax)
    {
        this.countdown = countdown;
        this.skillCaster = skillCaster;
        countdownText.text = countdown.ToString();
        this.setDebuffTimerMax = setDebuffTimerMax;
        setDebuffTimer = setDebuffTimerMax;

        HideVisual();
    }

    private void TurnManager_OnEnterEnemyTurn(object sender, System.EventArgs e)
    {
        MakeEffect();
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        MakeEffect();
    }

    public virtual void MakeEffect()
    {
        countdown--;
        countdownText.text = countdown.ToString();
        if (countdown == 0)
        {
            DestroySelf();
        }
    }

    public void DestroySelf()
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
