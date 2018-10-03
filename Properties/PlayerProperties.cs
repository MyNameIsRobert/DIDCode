using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Written By Robert Clark
//Updates: 6/3/17 - Added Hitmarker variables & methods
public class PlayerProperties : MonoBehaviour {

    #region Variable Declaration
    [SerializeField]
    [Tooltip("For dev and testing purposes only. Should be set to true for builds and final releases")]
    bool canDie = false;

    public int ductTape = 0;

    [SerializeField]
    public int sceneToLoadOnRestart = 0;

    public float handSpeed;

    public float defaultAccuracyModifier = 1;
    [HideInInspector]
    public float accuracyModifier = 1;

    public float playerStrength = 1;

    bool isCrouching;

    bool isSprinting;


    bool encumbered;

    [SerializeField]
    public float detectionModifier = 1;

    public float quickMeleeDamage;

    
    public float appliedDetectionModifier = 1;

    [SerializeField]
    int mediumHitmarkerMinDamage = 50;

    [SerializeField]
    int largeHitmarkerMinDamage = 100;

    //Hitmarker
    public GameObject hitmarker;
    public GameObject criticalHitmarker;

    public AudioClip hitmarkerSound;
    public AudioClip criticalHitmarkerSound;

    public Camera cam;

    float alphaCValue;
    float alphaHValue;
    [SerializeField]
    float hitmarkerTimer;

    bool isCritHit = false;
    const float DEFAULT_HITMARKER_TIME = .04f;

    public bool aiming = false;

    bool bigHitmarker;
    bool mediumHitmarker;
    bool smallHitmarker;

    [SerializeField]
    Sprite smallHitmarkerSprite;
    [SerializeField]
    Sprite mediumHitmarkerSprite;
    [SerializeField]
    Sprite bigHitmarkerSprite;

    [SerializeField]
    int hitmarkerCounter;
    [SerializeField]
    int maxHitmarkersPerSecond = 10;

    [SerializeField]
    float hitmarkerCounterTimer = .1f;

    WeaponSwitch weaponSwitch;

    PlayerControllerSinglePlayer playerController;

    GunAimUpSinglePlayer gunAimUp;

    HealthSinglePlayer playerHealth;

    PauseGame pauseGame;

    AudioSource aud;
    

    [SerializeField]
    [Range(0, 300)]
    int frameRate = 60;

    [SerializeField]
    float XAimMomentum = 0;
    [SerializeField]
    float YAimMomentum = 0;

    float startXDirection = 0;
    float startYDirection = 0;

    [SerializeField]
    float maxMomentumX = 0;
    [SerializeField]
    float maxMomentumY = 0;

    int deathMenuOptions = -1;

    float totalMomentumX, totalMomentumY;

    [HideInInspector]
    public float handSpeedFloor = .20f;
    
    public float handSpeedCeiling = 3;

    public Image scopeReticleImage;

    [SerializeField]
    float currentDPS = 0;
    public float dpsResetTime = 1;
    #endregion
    private void Awake()
    {

    }
    IEnumerator DisplayHitmarker(bool isCritical, float displayTime, int damageDone)
    {
        if (damageDone <= 0)
        {
            hitmarker.GetComponent<Image>().sprite = mediumHitmarkerSprite;
            criticalHitmarker.GetComponent<Image>().sprite = mediumHitmarkerSprite;
        }
        else if (damageDone < mediumHitmarkerMinDamage)
        {
            hitmarker.GetComponent<Image>().sprite = smallHitmarkerSprite;
            criticalHitmarker.GetComponent<Image>().sprite = smallHitmarkerSprite;
        }
        else if (damageDone >= mediumHitmarkerMinDamage && damageDone < largeHitmarkerMinDamage)
        {
            hitmarker.GetComponent<Image>().sprite = mediumHitmarkerSprite;
            criticalHitmarker.GetComponent<Image>().sprite = mediumHitmarkerSprite;
        }
        else
        {
            hitmarker.GetComponent<Image>().sprite = bigHitmarkerSprite;
            criticalHitmarker.GetComponent<Image>().sprite = bigHitmarkerSprite;
        }
        Color c = criticalHitmarker.GetComponent<Image>().color;
        Color h = hitmarker.GetComponent<Image>().color;
        if (isCritical)
        {
            alphaCValue = 1;
            c.a = alphaCValue;
            criticalHitmarker.GetComponent<Image>().color = c;
        }
        else
        {
            alphaHValue = 1;
            h.a = alphaHValue;
            hitmarker.GetComponent<Image>().color = h;
        }
        if (displayTime <= 0)
            displayTime = DEFAULT_HITMARKER_TIME;

        yield return new WaitForSeconds(displayTime);
        Debug.Log("Done waiting");
        while(c.a > 0 || h.a > 0)
        {
            c.a -= Time.deltaTime;
            h.a -= Time.deltaTime;
            criticalHitmarker.GetComponent<Image>().color = c;
            hitmarker.GetComponent<Image>().color = h;
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }
    IEnumerator AddToCurrentDPS(float amount)
    {
        currentDPS += amount;
        yield return new WaitForSeconds(dpsResetTime);
        currentDPS -= amount;
        yield return null;
    }
    IEnumerator DieAndRespawn()
    {
        pauseGame.ShowDeathMenu();
        yield return new WaitUntil(() => deathMenuOptions != -1);
        switch (deathMenuOptions)
        {
            case 0:
                SceneManager.LoadScene(sceneToLoadOnRestart, LoadSceneMode.Single);
                break;
            case 1:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
                Application.Quit();
                break;
        }
        yield return null;
    }
    private void Start()
    {
        aud = GetComponent<AudioSource>();
        weaponSwitch = GetComponent<WeaponSwitch>();
        playerController = GetComponent<PlayerControllerSinglePlayer>();
        gunAimUp = GetComponent<GunAimUpSinglePlayer>();
        playerHealth = GetComponent<HealthSinglePlayer>();
        pauseGame = GetComponent<PauseGame>();
    }

    #region Update Method
    void Update ()
    {
        if(canDie && playerHealth.isDead)
        {
            StartCoroutine(DieAndRespawn());
        }

        totalMomentumX = Input.GetAxis("Mouse X");
        totalMomentumY = Input.GetAxis("Mouse Y");

        
        Animator animator = null;
        if(weaponSwitch.equippedWeapon != null && Time.timeScale != 0)
        {
            animator = weaponSwitch.equippedWeapon.GetComponent<Animator>();
            animator.SetFloat("XDirection", totalMomentumX);
            animator.SetFloat("YDirection", totalMomentumY);
            totalMomentumY = 0;
            totalMomentumX = 0;
        }
        

        if(hitmarkerCounterTimer <= 0)
        {
            hitmarkerCounter = 0;
            hitmarkerCounterTimer = .1f;
        }
        hitmarkerCounterTimer -= Time.deltaTime;
        if(isSprinting && isCrouching)
        {
            appliedDetectionModifier = detectionModifier + .1f;
        }
        else if (isSprinting)
        {
            appliedDetectionModifier = detectionModifier + .4f;
        }
        else if (isCrouching)
        {
            appliedDetectionModifier = detectionModifier - .3f;
        }
        else
        {
            appliedDetectionModifier = detectionModifier;
        }
        if (encumbered)
        {
            accuracyModifier = defaultAccuracyModifier * 1.2f;
        }
        else
        {
            accuracyModifier = defaultAccuracyModifier;
        }
        Animator tempAnim = null;
        if(weaponSwitch.equippedWeapon != null)
        { 
            tempAnim = weaponSwitch.equippedWeapon.GetComponent<Animator>();

        if (isSprinting && playerController.GetIsMoving())
        {
            tempAnim.SetBool("Running", true);
            tempAnim.SetBool("Walking", false);
        }
        else if(playerController.GetIsMoving())
        {
            tempAnim.SetBool("Running", false);
            tempAnim.SetBool("Walking", true);
        }
        else
        {
            tempAnim.SetBool("Running", false);
            tempAnim.SetBool("Walking", false);
        }
        }


    }
        


	


    #endregion

    #region Change Functions
    public void ChangeHandSpeed(float newSpeed) {
        handSpeed = newSpeed;    
    }
    public void SetAiming(bool aim)
    {
        aiming = aim;
    }
    public void SetCrouching(bool crouch)
    {
        isCrouching = crouch;
    }
    public void SetSprinting(bool sprint)
    {
        isSprinting = sprint;
    }
    public void SetEncumbered(bool heavy)
    {
        encumbered = heavy;
    }
    public void ChangeDetectionModifierPercentage(float percentage, out float amountSubtracted)
    {
        float tempAmountToSubtract = (percentage/100) * detectionModifier;
        detectionModifier -= tempAmountToSubtract;
        amountSubtracted = tempAmountToSubtract;
    }
    public void ChangeDetectionModifierPercentage(float percentage)
    {
        float tempAmountToSubtract = (percentage / 100) * detectionModifier;
        detectionModifier -= tempAmountToSubtract;
    }
    public void ChangeDetectionModifierFlatNumber(float amountToSubtract)
    {
        detectionModifier -= amountToSubtract;
    }
    public void SetDetectionModifier(float amountToSetTo)
    {
        detectionModifier = amountToSetTo;
    }
    public void ChangeAccuracyModifierPercentage(float percentage, out float amountSubtracted)
    {
        float tempAmountToSubtract = (percentage / 100) * accuracyModifier;
        accuracyModifier -= tempAmountToSubtract;
        amountSubtracted = tempAmountToSubtract;
    }
    public void ChangeAccuracyModifierPercentage(float percentage)
    {
        float tempAmountToSubtract = (percentage / 100) * accuracyModifier;
        accuracyModifier -= tempAmountToSubtract;
    }
    public void ChangeAccuracyModifierFlatNumber(float amountToSubtract)
    {
        accuracyModifier -= amountToSubtract;
    }
    #endregion

    #region Hitmarker Functions
    public void ShowHitmarker()
    { 
        isCritHit = false;
        hitmarkerTimer = DEFAULT_HITMARKER_TIME;
        if (hitmarkerCounter < maxHitmarkersPerSecond)
        {
            aud.PlayOneShot(hitmarkerSound);
        }
        hitmarkerCounter++;

        StartCoroutine(DisplayHitmarker(isCritHit, hitmarkerTimer, (int)currentDPS));
    }
    public void ShowHitmarker(bool isCritical)
    {
        
        isCritHit = isCritical;
        hitmarkerTimer = DEFAULT_HITMARKER_TIME;
        if (hitmarkerCounter < maxHitmarkersPerSecond)
        {
            if (isCritical)
                aud.PlayOneShot(criticalHitmarkerSound);
            else
                aud.PlayOneShot(hitmarkerSound);
        }
        hitmarkerCounter++;
        StartCoroutine(DisplayHitmarker(isCritHit, hitmarkerTimer, (int)currentDPS));
    }
    public void ShowHitmarker(float specificTime) {
        
        isCritHit = false;
        hitmarkerTimer = specificTime;
        if(hitmarkerCounter < maxHitmarkersPerSecond)
        { 
        aud.PlayOneShot(criticalHitmarkerSound);
        }
        hitmarkerCounter++;
        StartCoroutine(DisplayHitmarker(isCritHit, hitmarkerTimer, (int)currentDPS));
    }
    public void ShowHitmarker(bool isCritical, float specificTime) {
        
        isCritHit = isCritical;
        hitmarkerTimer = specificTime;
        if(hitmarkerCounter < maxHitmarkersPerSecond)
        { 
        if (isCritical)
            aud.PlayOneShot(criticalHitmarkerSound);
        else
            aud.PlayOneShot(hitmarkerSound);
        }
        hitmarkerCounter++;
        StartCoroutine(DisplayHitmarker(isCritHit, hitmarkerTimer, (int)currentDPS));
    }
    public void ShowHitmarker(float specificTime, float damageDealt)
    {
        isCritHit = false;
        hitmarkerTimer = specificTime;

        if (hitmarkerCounter < maxHitmarkersPerSecond)
        {
            aud.PlayOneShot(hitmarkerSound);
        }
        hitmarkerCounter++;
        StartCoroutine(AddToCurrentDPS(damageDealt));
        StartCoroutine(DisplayHitmarker(isCritHit, hitmarkerTimer, (int)currentDPS));
    }
    public void ShowHitmarker(bool isCritical, int damageDealt)
    {
        isCritHit = isCritical;
        hitmarkerTimer = DEFAULT_HITMARKER_TIME;
        if (hitmarkerCounter < maxHitmarkersPerSecond)
        {
            if (isCritical)
                aud.PlayOneShot(criticalHitmarkerSound);
            else
                aud.PlayOneShot(hitmarkerSound);
        }
        hitmarkerCounter++;
        StartCoroutine(AddToCurrentDPS(damageDealt));
        StartCoroutine(DisplayHitmarker(isCritHit, hitmarkerTimer, (int)currentDPS));
    }
    public void ShowHitmarker(bool isCritical, bool playSound)
    {
        isCritHit = isCritical;
        hitmarkerTimer = DEFAULT_HITMARKER_TIME;
        if (playSound)
        {
            if (hitmarkerCounter < maxHitmarkersPerSecond)
            {
                if (isCritHit)
                    aud.PlayOneShot(criticalHitmarkerSound);
                else
                    aud.PlayOneShot(hitmarkerSound);
            }
        }
        StartCoroutine(DisplayHitmarker(isCritHit, hitmarkerTimer, (int)currentDPS));
    }
    #endregion

    public bool GetEncumbered()
    {
        return encumbered;
    }

    public void SetDeathOption(int num)
    {
        deathMenuOptions = num;
    }

    public void ToggleScopeReticle(bool show, Sprite spriteToChangeTo)
    {
        if(show)
        {
            scopeReticleImage.GetComponent<CanvasGroup>().alpha = 1;
            scopeReticleImage.sprite = spriteToChangeTo;
        }
        else
        {
            scopeReticleImage.GetComponent<CanvasGroup>().alpha = 0;
        }
        
    }
    
}
