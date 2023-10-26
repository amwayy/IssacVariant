using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Buff : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject iconGameObject;
    [SerializeField] private float buffMakeEffectTimerMax = .5f;   // Buff生效动画的时间

    protected ISkillCaster buffOwner;
    protected int countdown;

    private float setBuffTimer;
    private float setBuffTimerMax;
    private bool isSettingBuff = true;

    protected virtual void Start()
    {
        if (buffOwner.IsPlayer())
        {
            TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
        }
        else
        {
            TurnManager.Instance.OnEnterEnemyTurn += TurnManager_OnEnterEnemyTurn;
        }

        Player.Instance.OnQuitBattle += Player_OnQuitBattle;
    }

    private void Player_OnQuitBattle(object sender, System.EventArgs e)
    {
        DestroySelf();
    }

    private void Update()
    {
        if (isSettingBuff)
        {
            setBuffTimer -= Time.deltaTime;
            if (setBuffTimer <= 0f)
            {
                isSettingBuff = false;
                ShowVisual();
            }
        }
    }

    public void SetOwner(ISkillCaster buffOwner)
    {
        this.buffOwner = buffOwner;
    }

    public void IncreaseCountdown(int increaseAmount)
    {
        isSettingBuff = true;
        setBuffTimer = setBuffTimerMax;
        countdown += increaseAmount;
    }

    public float GetDebuffMakeEffectTimerMax()
    {
        return buffMakeEffectTimerMax;
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

    public virtual void Initialize(ISkillCaster skillCaster, int countdown, float setBuffTimerMax)
    {
        this.countdown = countdown;
        buffOwner = skillCaster;
        countdownText.text = countdown.ToString();
        this.setBuffTimerMax = setBuffTimerMax;
        setBuffTimer = setBuffTimerMax;

        HideVisual();
    }

    protected virtual void TurnManager_OnEnterEnemyTurn(object sender, System.EventArgs e)
    {
        DecreaseCountdown();
    }

    protected virtual void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        DecreaseCountdown();
    }

    public virtual void MakeEffect()
    {

    }

    protected void DecreaseCountdown()
    {
        countdown--;
        countdownText.text = countdown.ToString();

        CheckCountdown();
    }

    protected virtual void CheckCountdown()
    {
        if (countdown == 0)
        {
            DestroySelf();
        }
    }

    protected virtual void OnDestroy()
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;
        Player.Instance.OnQuitBattle -= Player_OnQuitBattle;
    }

    public void DestroySelf()
    {
        transform.SetParent(null);

        buffOwner.UpdateStatContainer();

        Destroy(gameObject);
    }
}
