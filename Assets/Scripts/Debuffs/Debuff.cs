using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Debuff : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject iconGameObject;
    [SerializeField] private float debuffMakeEffectTimerMax = .5f;   // Debuff生效动画的时间

    protected ISkillCaster debuffOwner;
    protected int countdown;

    private float setDebuffTimer;
    private float setDebuffTimerMax;
    private bool isSettingDebuff = true;

    protected virtual void Start()
    {
        TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
        TurnManager.Instance.OnEnterEnemyTurn += TurnManager_OnEnterEnemyTurn;

        Player.Instance.OnQuitBattle += Player_OnQuitBattle;
    }

    private void Player_OnQuitBattle(object sender, System.EventArgs e)
    {
        DestroySelf();
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

    public void SetOwner(ISkillCaster debuffOwner)
    {
        this.debuffOwner = debuffOwner;
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

    protected virtual void ShowVisual()
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

    public virtual void Initialize(ISkillCaster skillCaster, int countdown, float setDebuffTimerMax, int extraCountdown)
    {
        this.countdown = countdown;
        debuffOwner = skillCaster;
        countdownText.text = countdown.ToString();
        this.setDebuffTimerMax = setDebuffTimerMax;
        setDebuffTimer = setDebuffTimerMax;

        HideVisual();
    }

    protected virtual void TurnManager_OnEnterEnemyTurn(object sender, System.EventArgs e)
    {

    }

    protected virtual void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {

    }

    protected virtual void DecreaseCountdown()
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

    public virtual void MakeEffect()
    {

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

        debuffOwner.UpdateStatContainer();

        Destroy(gameObject);
    }
}
