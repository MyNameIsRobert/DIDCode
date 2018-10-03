using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


//Written By Robert
//One of the more complex/conveluted scripts. In addition to displaying the contents of the player's inventory, it handles
//what happens when any item is clicked on, including using Consumables, Equipping Armor, and Equipping Attachments
public class DisplayInventory : MonoBehaviour{

    #region Variables

    #region Components
    PlayerInventory playerInventory;
    PlayerAmmo playerAmmo;
    PlayerArmor playerArmor;
    PlayerInputControls control;
    GameObject player;
    PlayerProperties playerProperties;
    PlayerControllerSinglePlayer playerController;
    Stamina playerStamina;
    WeaponSwitch weaponSwitch;
    Shoot currentWeapon;
    Item clickedItem;
    Consumable currentConsumable;
    HealthSinglePlayer playerHealth;
    [SerializeField]
    WeaponAttachmentUI attachmentUI;
    Attachment tempAttachment = null;
    [SerializeField]
    UpdateItemTipText tipText;
    [SerializeField]
    MouseToolTip floatingToolTip;
    ChestContents openChest;
    [SerializeField]
    List<Text> amountOfText;
    [SerializeField]
    DisplayCurrentBuffs currentBuffs;
    [SerializeField]
    List<Item> arrayToShowInventory;

    [SerializeField]
    List<Consumable> usedConsumables = new List<Consumable>();
    [SerializeField]
    List<int> consumableUses = new List<int>();

    [SerializeField]
    public Consumable[] topThreeConsumables = new Consumable[3];
    #endregion

    bool attachCoroutineRunning = false;
    int weaponToAttachTo;

    #region Images and Buttons
    int numOfItemSlots = 0;
    public GameObject itemSlot;
    [SerializeField]
    List<Image> itemImage;
    [SerializeField]
    List<Button> itemButtons;
    #endregion
    #endregion

    
    #region Coroutines
    IEnumerator StartCountDown(float countDownTime)
    {
        while (countDownTime > 0)
        {
            Debug.Log(countDownTime);
            yield return new WaitForSecondsRealtime(1);
            countDownTime--;
        }
        yield return null;
    }
    IEnumerator ApplyStealthPotion(float duration, float affectAmount, bool isPercentage)
    {
        float tempAmountSubtracted = .5f;
        if (isPercentage)
        {
            playerProperties.ChangeDetectionModifierPercentage(affectAmount, out tempAmountSubtracted);
        }
        else
        {
            playerProperties.ChangeDetectionModifierFlatNumber(affectAmount);
        }
        Debug.Log("Coroutine Started, going to wait for " + duration + " seconds");
        yield return new WaitForSecondsRealtime(duration);
        if (isPercentage)
        {
            playerProperties.ChangeDetectionModifierFlatNumber(0 - tempAmountSubtracted);
            Debug.Log("Aded " + tempAmountSubtracted + " to the detection modifier");
        }
        else
        {
            playerProperties.ChangeDetectionModifierFlatNumber(0 - affectAmount);
        }

        yield return null;
    }
    IEnumerator ApplySpeedPotion(float duration, float affectAmount, bool isPercentage)
    {
        float tempChangedAmount = 0;
        if (isPercentage)
            playerController.ChangeMoveSpeedModifierPercentage(affectAmount, out tempChangedAmount);
        else
            playerController.ChangeMoveSpeedModifierFlatNumber(affectAmount);
        yield return new WaitForSecondsRealtime(duration);
        if (isPercentage)
            playerController.ChangeMoveSpeedModifierFlatNumber(0 - tempChangedAmount);
        else
            playerController.ChangeMoveSpeedModifierFlatNumber(0 - affectAmount);
        yield return null;
    }
    IEnumerator ApplyAccuracyPotion(float duration, float affectAmount, bool isPercentage)
    {
        yield return null;
    }
    IEnumerator ApplyStaminaPotionOverTime(float duration, float affectAmount, bool isPercentage)
    {
        Debug.Log("Applied stamina over time");
        if (!isPercentage)
        {
            while (duration > 0)
            {
                playerStamina.IncreaseStamina(affectAmount * Time.deltaTime);
                duration -= Time.fixedDeltaTime;
                Debug.Log(duration);
                yield return null;
            }
        }
        else
        {
            affectAmount /= 100;
            float tempTotalToAffectBy = affectAmount * playerStamina.maxStamina;
            float amountPerSecond = tempTotalToAffectBy / duration;
            while (duration > 0)
            {
                playerStamina.IncreaseStamina(amountPerSecond * Time.deltaTime);
                duration -= Time.fixedDeltaTime;
                yield return null;
            }
        }
        yield return null;
    }
    IEnumerator ApplyStaminaPotionImmediately(float affectAmount, bool isPercentage)
    {
        Debug.Log("Applied stamina potion immediately");
        if (!isPercentage)
            playerStamina.IncreaseStamina(affectAmount);
        else
        {
            affectAmount /= 100;
            float tempAmount = affectAmount * playerStamina.maxStamina;
            playerStamina.IncreaseStamina(tempAmount);
        }
        yield return null;
    }
    IEnumerator SelectWeaponToAttach(int attachmentIndexInInventory)
    {
        attachCoroutineRunning = true;
        player.GetComponent<PauseGame>().HideInventory();
        player.GetComponent<PauseGame>().ShowAttachmentMenu();
        weaponToAttachTo = -1;
        yield return new WaitUntil(() => weaponToAttachTo != -1);
        player.GetComponent<PauseGame>().HideAttachmentMenu();
        player.GetComponent<PauseGame>().ShowInventory();
        attachmentUI.SetAttachment(null);
        Attachment returnedAttachment = null;
        currentWeapon = weaponSwitch.hands.GetChild(weaponToAttachTo).GetComponent<Shoot>();
        returnedAttachment = currentWeapon.AddAttachment(tempAttachment);
        #region Adding Attachment Effects
        if (returnedAttachment != null && returnedAttachment.itemName == "N/A")
        {

        }
        else 
        {
            for (int i = 0; i < tempAttachment.attachmentEffects.Length; i++)
                {
                    switch (tempAttachment.attachmentEffects[i])
                {
                    case Attachment.AttachmentEffect.AimingAngleDivider:
                        if (tempAttachment.attachmentEffectAmountTypes[i] == Attachment.AttachmentEffectNumberType.FlatNumber)
                        {
                            currentWeapon.aimingDivider += tempAttachment.attachmentEffectAmounts[i];
                        }
                        else
                        {
                            float tempAmount = (tempAttachment.attachmentEffectAmounts[i] / 100) * currentWeapon.aimingDivider;
                            currentWeapon.aimingDivider += tempAmount;
                        }
                        break;
                    case Attachment.AttachmentEffect.BulletSpeed:
                        //if (tempAttachment.attachmentEffectAmountTypes[i] == Attachment.AttachmentEffectNumberType.FlatNumber)
                        //{
                        //    currentWeapon.IncreaseProjectileSpeed(tempAttachment.attachmentEffectAmounts[i]);
                        //}
                        //else
                        //{
                        //    float tempAmount = (tempAttachment.attachmentEffectAmounts[i] / 100) * currentWeapon.GetProjectileSpeed();
                        //    currentWeapon.IncreaseProjectileSpeed(tempAmount);
                        //}
                        break;
                    case Attachment.AttachmentEffect.Damage:
                        if (tempAttachment.attachmentEffectAmountTypes[i] == Attachment.AttachmentEffectNumberType.FlatNumber)
                        {
                            currentWeapon.damage += tempAttachment.attachmentEffectAmounts[i];
                        }
                        else
                        {
                            float amountToIncrease = (tempAttachment.attachmentEffectAmounts[i] / 100) * currentWeapon.damage;
                            currentWeapon.damage += (amountToIncrease);
                        }
                        break;
                    case Attachment.AttachmentEffect.DefaultAngle:
                        if (tempAttachment.attachmentEffectAmountTypes[i] == Attachment.AttachmentEffectNumberType.FlatNumber)
                        {
                            currentWeapon.defaultAngle += (tempAttachment.attachmentEffectAmounts[i]);
                        }
                        else
                        {
                            float tempAmount = (tempAttachment.attachmentEffectAmounts[i] / 100) * currentWeapon.defaultAngle;
                            currentWeapon.defaultAngle += (tempAmount);
                        }
                        break;
                    case Attachment.AttachmentEffect.FireAmmo:
                        if (tempAttachment.attachmentEffectAmountTypes[i] == Attachment.AttachmentEffectNumberType.FlatNumber)
                        {

                        }
                        else
                        {

                        }
                        break;
                    case Attachment.AttachmentEffect.FireRate:
                        if (tempAttachment.attachmentEffectAmountTypes[i] == Attachment.AttachmentEffectNumberType.FlatNumber)
                        {
                            currentWeapon.fireRate += (0 - tempAttachment.attachmentEffectAmounts[i]);
                        }
                        else
                        {
                            float amountToIncrease = (tempAttachment.attachmentEffectAmounts[i] / 100) * currentWeapon.fireRate;
                            currentWeapon.fireRate += (0 - amountToIncrease);
                        }
                        break;
                    case Attachment.AttachmentEffect.IceAmmo:
                        if (tempAttachment.attachmentEffectAmountTypes[i] == Attachment.AttachmentEffectNumberType.FlatNumber)
                        {

                        }
                        else
                        {

                        }
                        break;
                    case Attachment.AttachmentEffect.MagazineSize:
                        if (tempAttachment.attachmentEffectAmountTypes[i] == Attachment.AttachmentEffectNumberType.FlatNumber)
                        {
                            currentWeapon.magazineSize += (int)(tempAttachment.attachmentEffectAmounts[i]);
                        }
                        else
                        {
                            float amountToIncrease = (tempAttachment.attachmentEffectAmounts[i] / 100) * currentWeapon.magazineSize;
                            currentWeapon.magazineSize += (int)(amountToIncrease);
                        }
                        break;
                    case Attachment.AttachmentEffect.Silence:
                        currentWeapon.isSilenced = true;
                        break;
                    case Attachment.AttachmentEffect.ShotGunAccuracy:
                        //if (tempAttachment.attachmentEffectAmountTypes[i] == Attachment.AttachmentEffectNumberType.FlatNumber)
                        //{
                        //    currentWeapon.IncreaseNumOfPellets(tempAttachment.attachmentEffectAmounts[i]);
                        //}
                        //else
                        //{
                        //    float tempAmount = (tempAttachment.attachmentEffectAmounts[i] / 100) * currentWeapon.GetNumOfPellets();
                        //    currentWeapon.IncreaseNumOfPellets(tempAmount);
                        //}
                        //break;
                    case Attachment.AttachmentEffect.CritHitMultiplier:
                        if (tempAttachment.attachmentEffectAmountTypes[i] == Attachment.AttachmentEffectNumberType.FlatNumber)
                        {
                            currentWeapon.critMultiplier += (tempAttachment.attachmentEffectAmounts[i]);
                        }
                        else
                        {
                            float tempAmount = (tempAttachment.attachmentEffectAmounts[i] / 100) * currentWeapon.critMultiplier;
                            currentWeapon.critMultiplier += (tempAmount);
                        }
                        break;
                    case Attachment.AttachmentEffect.ReloadSpeed:
                        if (tempAttachment.attachmentEffectAmountTypes[i] == Attachment.AttachmentEffectNumberType.FlatNumber)
                        {
                            currentWeapon.reloadSpeedModifier += (tempAttachment.attachmentEffectAmounts[i]);
                        }
                        else
                        {
                            float tempAmount = (tempAttachment.attachmentEffectAmounts[i] / 100) * currentWeapon.reloadSpeedModifier;
                            currentWeapon.reloadSpeedModifier += (tempAmount);
                        }
                        break;
                    case Attachment.AttachmentEffect.AimSpeed:
                        if (tempAttachment.attachmentEffectAmountTypes[i] == Attachment.AttachmentEffectNumberType.FlatNumber)
                        {
                            currentWeapon.aimSpeed += (tempAttachment.attachmentEffectAmounts[i]);
                        }
                        else
                        {
                            float tempAmount = (tempAttachment.attachmentEffectAmounts[i] / 100) * currentWeapon.aimSpeed;
                            currentWeapon.aimSpeed += (tempAmount);
                        }
                    
                        break;
                     case Attachment.AttachmentEffect.Kickback:
                        if (tempAttachment.attachmentEffectAmountTypes[i] == Attachment.AttachmentEffectNumberType.FlatNumber)
                        {
                            currentWeapon.kickBackAmount += (tempAttachment.attachmentEffectAmounts[i]);
                        }
                        else
                        {
                            float tempAmount = (tempAttachment.attachmentEffectAmounts[i] / 100) * currentWeapon.kickBackAmount;
                            currentWeapon.kickBackAmount += (tempAmount);
                        }
                        break;
                    case Attachment.AttachmentEffect.ZoomAmount:
                        if(tempAttachment.attachmentEffectAmountTypes[i] == Attachment.AttachmentEffectNumberType.FlatNumber)
                        {
                            currentWeapon.zoomAmount = tempAttachment.attachmentEffectAmounts[i];
                        }
                        else
                        {
                            float tempAmount = (tempAttachment.attachmentEffectAmounts[i] / 100) * currentWeapon.zoomAmount;
                            currentWeapon.zoomAmount += (tempAmount);
                        }
                        break;

                }
                }
            playerInventory.RemoveItem(attachmentIndexInInventory);
            //if(returnedAttachment != null)
            //{
            //    RemoveAttachmentEffects(returnedAttachment, currentWeapon);
            //    playerInventory.AddAttachmentToInventory(returnedAttachment);
            //}
        }
        
        #endregion
    }
    IEnumerator ApplyJumpPotion(float duration, float affectAmout, bool isPercentage)
    {
        float tempValue = 0;
        if (isPercentage)
        {
            playerController.ChangeJumpSpeedModifierPercentage(affectAmout, out tempValue);
        }
        else
        {
            playerController.ChangeJumpSpeedModifierFlat(affectAmout);
        }
        yield return new WaitForSecondsRealtime(duration);
        if (isPercentage)
        {
            playerController.ChangeJumpSpeedModifierFlat(0 - tempValue);
        }
        else
        {
            playerController.ChangeJumpSpeedModifierFlat(0 - affectAmout);
        }
        yield return null;
    }
    IEnumerator ApplyReloadSpeedPotion(float duration, float affectAmount, bool isPercentage)
    {
        float tempValue = 0;
        if(isPercentage)
        {
            tempValue = playerProperties.handSpeed * (affectAmount / 100);
            Debug.Log("tempValue is now: " + tempValue);
            if(playerProperties.handSpeed + tempValue < playerProperties.handSpeedFloor)
            {
                tempValue = -(playerProperties.handSpeed - playerProperties.handSpeedFloor);
                Debug.Log("tempValue hit floor, it is now: " + tempValue);
            }
            else if(playerProperties.handSpeed + tempValue > playerProperties.handSpeedCeiling)
            {
                tempValue = (playerProperties.handSpeedCeiling - playerProperties.handSpeed);
                Debug.Log("tempValue hit ceiling of: " + playerProperties.handSpeedCeiling + ", it is now: " + tempValue + ". Original handSpeed was " + playerProperties.handSpeed);
            }
            playerProperties.handSpeed += tempValue;
            Debug.Log("Increased handSpeed by "  +tempValue);
        }
        else
        {
            tempValue = affectAmount;
            if (playerProperties.handSpeed - tempValue < playerProperties.handSpeedFloor)
            {
                tempValue = playerProperties.handSpeed - playerProperties.handSpeedFloor;
            }
            playerProperties.handSpeed -= tempValue;
            Debug.Log("Decreased handSpeed by " + tempValue);
        }
        yield return new WaitForSeconds(duration);
        playerProperties.handSpeed += tempValue;
        yield return null;
    }
    #endregion


    #region Start Function
    void Start()
    {
        player = transform.root.gameObject;
        control = player.GetComponent<PlayerInputControls>();
        playerInventory = player.GetComponent<PlayerInventory>();
        playerProperties = player.GetComponent<PlayerProperties>();
        playerController = player.GetComponent<PlayerControllerSinglePlayer>();
        playerStamina = player.GetComponent<Stamina>();
        playerAmmo = player.GetComponent<PlayerAmmo>();
        for (int i = 0; i < transform.childCount; i++)
        {
            int tempInt = i;
            itemImage.Add(transform.GetChild(i).transform.GetChild(1).GetComponent<Image>());
            itemButtons.Add(transform.GetChild(i).GetComponent<Button>());
            amountOfText.Add(transform.GetChild(i).transform.GetChild(2).GetComponent<Text>());
            itemImage[i].rectTransform.localPosition = new Vector3(12.8f, -12.8f, 0);
            itemButtons[i].onClick.AddListener(() => ButtonClickFunction(tempInt));
            EventTrigger trigger = itemButtons[tempInt].GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data,tempInt); });
            trigger.triggers.Add(entry);
            numOfItemSlots++;
            UnityEvent rightClickTrigger = transform.GetChild(i).GetComponent<RightClickButton>().rightClick;
            rightClickTrigger.AddListener(() => OnRightClick(tempInt));
        }
        for(int i = 0; i < 6; i++)
        {
            AddInventorySlot();
        }
        playerArmor = player.GetComponent<PlayerArmor>();
        playerHealth = player.GetComponent<HealthSinglePlayer>();
        weaponSwitch = player.GetComponent<WeaponSwitch>();
    }
    #endregion

    #region Update Function
    void Update()
    {
        if (attachCoroutineRunning)
        {
            if (Input.GetKey(control.inventory) || Input.GetKey(control.pauseMenu))
            {
                StopCoroutine("SelectWeaponToAttach");
                tempAttachment = null;
                weaponToAttachTo = -1;
                player.GetComponent<PauseGame>().HideAttachmentMenu();
                attachCoroutineRunning = false;
            }
        }

        arrayToShowInventory = playerInventory.inventory;
        if(playerInventory.inventory.Count > numOfItemSlots)
        {
            for (int i = 0; i < 3; i++)//Adds three inventory slots i.e a row of slots
                AddInventorySlot();
        }

        for (int i = 0; i < playerInventory.inventory.Count; i++)
        {
            if (playerInventory.inventory[i] != null)
            {
                itemImage[i].sprite = playerInventory.inventory[i].sprite;
                itemImage[i].preserveAspect = true;
                itemImage[i].gameObject.SetActive(true);
                itemButtons[i].interactable = true;
                if (playerInventory.itemNumberInStack[i] > 1)
                    amountOfText[i].text = playerInventory.itemNumberInStack[i].ToString();
                else
                    amountOfText[i].text = "";
            }
            else
            {
                itemImage[i].gameObject.SetActive(false);
                itemButtons[i].interactable = false;
                amountOfText[i].text = "";
            }
        }
        for (int i = playerInventory.inventory.Count; i < itemImage.Count; i++)
        {
            itemImage[i].gameObject.SetActive(false);
            itemButtons[i].interactable = false;
        }
    }
    #endregion

    #region Apply Item Effect
    public void ApplyItemEffect(int itemIndex)
    {
        //Sets clickedItem to the item in player's inventory 
        clickedItem = playerInventory.inventory[itemIndex];
        currentBuffs.AddBuff(clickedItem);
        switch (clickedItem.itemType)
        {

            #region Item is a consumable
            case Item.ItemType.Consumable:               
                currentConsumable = (Consumable)playerInventory.inventory[itemIndex];

                #region Setting most used Consumables
                if (usedConsumables.Contains(currentConsumable))
                {
                    consumableUses[usedConsumables.IndexOf(currentConsumable)]++;
                }
                else
                {
                    usedConsumables.Add(currentConsumable);
                    consumableUses.Add(1);
                }
                topThreeConsumables = new Consumable[3];
                int tempMax = 0, firstIndex = 0, secondIndex = 0;
                for (int i = 0; i < usedConsumables.Count; i++) //Setting the total max value from the uses Counter. At the end, firstIndex should point the the most used consumable
                {
                    if (consumableUses[i] > tempMax)
                    {
                        firstIndex = i;
                        tempMax = consumableUses[i];
                    }
                }
                topThreeConsumables[0] = usedConsumables[firstIndex];
                tempMax = 0;
                for (int i = 0; i < usedConsumables.Count; i++) //Sees which value is the highest, while still being lower than the highest value. secondIndex should point to the second highest value
                {
                    if (consumableUses[i] > tempMax && usedConsumables[i] != usedConsumables[firstIndex])
                    {
                        secondIndex = i;
                        tempMax = consumableUses[i];
                    }
                }
                topThreeConsumables[1] = usedConsumables[secondIndex];
                tempMax = 0;
                for (int i = 0; i < usedConsumables.Count; i++)
                {
                    if (consumableUses[i] > tempMax && usedConsumables[i] != usedConsumables[secondIndex])
                    {
                        firstIndex = 1;
                        tempMax = consumableUses[1];
                    }
                }
                topThreeConsumables[2] = usedConsumables[firstIndex];

                if (topThreeConsumables[1] == topThreeConsumables[0])
                {
                    topThreeConsumables[1] = null;
                }
                if (topThreeConsumables[2] == topThreeConsumables[0])
                {
                    topThreeConsumables[2] = null;
                }
                if (topThreeConsumables[2] == topThreeConsumables[1])
                {
                    topThreeConsumables[2] = null;
                }
                if (topThreeConsumables[2] == topThreeConsumables[1] && topThreeConsumables[1] == topThreeConsumables[0])
                {
                    topThreeConsumables[1] = null;
                    topThreeConsumables[2] = null;
                } 
                #endregion

                switch (currentConsumable.consumableType)
                {
                    case Consumable.Type.HealthPotion:
                        HealthSinglePlayer health = playerInventory.gameObject.GetComponent<HealthSinglePlayer>();
                        if (currentConsumable.amountNumberType == Consumable.NumberType.Percentage)
                        {
                            float tempPercentage;
                            if (currentConsumable.amountToAffectBy > 100)
                                tempPercentage = 100;
                            else
                                tempPercentage = currentConsumable.amountToAffectBy;
                            health.HealPercentage(tempPercentage);
                        }
                        else
                        {
                            health.HealNumber(currentConsumable.amountToAffectBy);
                        }
                        break;
                    case Consumable.Type.StealthPotion:
                        if (currentConsumable.amountNumberType == Consumable.NumberType.Percentage)
                        {
                            StartCoroutine(ApplyStealthPotion(currentConsumable.effectTime, currentConsumable.amountToAffectBy, true));
                        }
                        else
                        {
                            StartCoroutine(ApplyStealthPotion(currentConsumable.effectTime, currentConsumable.amountToAffectBy, false));
                        }
                        break;
                    case Consumable.Type.SpeedPotion:
                        if (currentConsumable.amountNumberType == Consumable.NumberType.Percentage)
                        {
                            StartCoroutine(ApplySpeedPotion(currentConsumable.effectTime, currentConsumable.amountToAffectBy, true));
                        }
                        else
                        {
                            StartCoroutine(ApplySpeedPotion(currentConsumable.effectTime, currentConsumable.amountToAffectBy, false));
                        }
                        break;
                    case Consumable.Type.StaminaPotion:
                        if (currentConsumable.effectTime == -1)
                        {
                            if (currentConsumable.amountNumberType == Consumable.NumberType.FlatNumber)
                            {
                                StartCoroutine(ApplyStaminaPotionImmediately(currentConsumable.amountToAffectBy, false));
                            }
                            else
                            {
                                StartCoroutine(ApplyStaminaPotionImmediately(currentConsumable.amountToAffectBy, true));
                            }
                        }
                        else
                        {
                            if (currentConsumable.amountNumberType == Consumable.NumberType.FlatNumber)
                            {
                                StartCoroutine(ApplyStaminaPotionOverTime(currentConsumable.effectTime, currentConsumable.amountToAffectBy, false));
                            }
                            else
                            {
                                StartCoroutine(ApplyStaminaPotionOverTime(currentConsumable.effectTime, currentConsumable.amountToAffectBy, true));
                            }
                        }
                        break;
                    case Consumable.Type.AccuracyPotion:
                        break;
                    case Consumable.Type.JumpPotion:
                        if(currentConsumable.amountNumberType == Consumable.NumberType.FlatNumber)
                        {
                            StartCoroutine(ApplyJumpPotion(currentConsumable.effectTime, currentConsumable.amountToAffectBy, false));
                        }
                        else
                        {
                            StartCoroutine(ApplyJumpPotion(currentConsumable.effectTime, currentConsumable.amountToAffectBy, true));
                        }
                        break;
                    case Consumable.Type.ReloadSpeed:
                        if (currentConsumable.amountNumberType == Consumable.NumberType.FlatNumber)
                        {
                            StartCoroutine(ApplyReloadSpeedPotion(currentConsumable.effectTime, currentConsumable.amountToAffectBy, false));
                        }
                        else
                        {
                            StartCoroutine(ApplyReloadSpeedPotion(currentConsumable.effectTime, currentConsumable.amountToAffectBy, true));
                        }
                        break;
                    
                        
                }

                break;
            #endregion

            #region Item is armor
            case Item.ItemType.Armor:
                Armor currentArmor = (Armor)clickedItem;
                Armor returnedArmor = playerArmor.EquipArmor(currentArmor);
                playerHealth.ChangeDamageReduction(currentArmor.damageReductionAmount);
                switch (currentArmor.additionalEffectType)
                {
                    case Armor.AdditionalEffectType.IncreaseAccuracy:
                        if (currentArmor.additionalEffectNumberType == Armor.AdditionalEffectNumberType.FlatNumber)
                        {
                            float tempNum = currentArmor.additionalEffectAmount;
                            playerProperties.ChangeAccuracyModifierFlatNumber(tempNum);
                        }
                        else
                        {
                            playerProperties.ChangeAccuracyModifierPercentage(currentArmor.additionalEffectAmount);
                        }
                        break;
                    case Armor.AdditionalEffectType.IncreaseStealth:
                        if (currentArmor.additionalEffectNumberType == Armor.AdditionalEffectNumberType.FlatNumber)
                        {
                            playerProperties.ChangeDetectionModifierFlatNumber(currentArmor.additionalEffectAmount);
                        }
                        else
                        {
                            playerProperties.ChangeDetectionModifierPercentage(currentArmor.additionalEffectAmount);
                        }
                        break;
                    case Armor.AdditionalEffectType.MaxHealth:
                        if (currentArmor.additionalEffectNumberType == Armor.AdditionalEffectNumberType.FlatNumber)
                        {
                            playerHealth.IncreaseMaxHealthFlatNumber(currentArmor.additionalEffectAmount);
                        }
                        else
                        {
                            playerHealth.IncreaseMaxHealthPercentage(currentArmor.additionalEffectAmount);
                        }
                        break;
                    case Armor.AdditionalEffectType.MaxStamina:
                        if (currentArmor.additionalEffectNumberType == Armor.AdditionalEffectNumberType.FlatNumber)
                        {
                            playerStamina.IncreaseMaxStaminaFlatNumber(currentArmor.additionalEffectAmount);
                        }
                        else
                        {
                            playerStamina.IncreaseMaxStaminaPercentage(currentArmor.additionalEffectAmount);
                        }
                        break;
                    case Armor.AdditionalEffectType.MaxWeight:
                        break;
                    case Armor.AdditionalEffectType.NoAdditionalEffect:
                        break;
                }
                if (returnedArmor != null)
                {
                    playerHealth.ChangeDamageReduction(0 - returnedArmor.damageReductionAmount);
                    switch (returnedArmor.additionalEffectType)
                    {
                        case Armor.AdditionalEffectType.IncreaseAccuracy:
                            if (returnedArmor.additionalEffectNumberType == Armor.AdditionalEffectNumberType.FlatNumber)
                            {
                                playerProperties.ChangeAccuracyModifierFlatNumber(0 - returnedArmor.additionalEffectAmount);
                            }
                            else
                            {
                                float tempAmountToIncrease, tempPercentage;

                                tempPercentage = returnedArmor.additionalEffectAmount;
                                tempPercentage = 100 - tempPercentage;
                                tempPercentage /= 100;
                                tempAmountToIncrease = playerProperties.accuracyModifier / tempPercentage;
                                tempAmountToIncrease = tempAmountToIncrease - playerProperties.accuracyModifier;
                                playerProperties.ChangeAccuracyModifierFlatNumber(0 - tempAmountToIncrease);
                            }
                            break;
                        case Armor.AdditionalEffectType.IncreaseStealth:
                            if (returnedArmor.additionalEffectNumberType == Armor.AdditionalEffectNumberType.FlatNumber)
                            {
                                playerProperties.ChangeDetectionModifierFlatNumber(0 - returnedArmor.additionalEffectAmount);
                            }
                            else
                            {
                                float tempAmountToIncrease, tempPercentage;

                                tempPercentage = returnedArmor.additionalEffectAmount;
                                tempPercentage = 100 - tempPercentage;
                                tempPercentage /= 100;
                                tempAmountToIncrease = playerProperties.detectionModifier / tempPercentage;
                                tempAmountToIncrease = tempAmountToIncrease - playerProperties.detectionModifier;
                                playerProperties.ChangeDetectionModifierFlatNumber(0 - tempAmountToIncrease);
                            }
                            break;
                        case Armor.AdditionalEffectType.MaxHealth:
                            if (returnedArmor.additionalEffectNumberType == Armor.AdditionalEffectNumberType.FlatNumber)
                            {
                                playerHealth.IncreaseMaxHealthFlatNumber(0 - returnedArmor.additionalEffectAmount);
                            }
                            else
                            {
                                float tempAmountToIncrease, tempPercentage;
                                tempPercentage = returnedArmor.additionalEffectAmount;
                                tempPercentage /= 100;
                                tempAmountToIncrease = playerHealth.maxHealth / (tempPercentage + 1);
                                tempAmountToIncrease = playerHealth.maxHealth - tempAmountToIncrease;
                                playerHealth.IncreaseMaxHealthFlatNumber(0 - tempAmountToIncrease);
                            }
                            break;
                        case Armor.AdditionalEffectType.MaxStamina:
                            if (returnedArmor.additionalEffectNumberType == Armor.AdditionalEffectNumberType.FlatNumber)
                            {
                                playerStamina.IncreaseMaxStaminaFlatNumber(0 - returnedArmor.additionalEffectAmount);
                            }
                            else
                            {
                                float tempAmountToIncrease, tempPercentage;
                                tempPercentage = returnedArmor.additionalEffectAmount;
                                tempPercentage /= 100;
                                tempAmountToIncrease = playerStamina.maxStamina / (tempPercentage + 1);
                                tempAmountToIncrease = playerStamina.maxStamina - tempAmountToIncrease;
                                playerStamina.IncreaseMaxStaminaFlatNumber(0 - tempAmountToIncrease);
                            }
                            break;
                        case Armor.AdditionalEffectType.MaxWeight:

                            break;
                    }
                }
                break;
            #endregion

            #region Item is Attachment
            case Item.ItemType.Attachment:
                tempAttachment = (Attachment)clickedItem;
                attachmentUI.SetAttachment(tempAttachment);
                StartCoroutine("SelectWeaponToAttach", itemIndex);
                
                break;
            #endregion

            case Item.ItemType.Ammo:
                Ammo ammo = (Ammo)clickedItem;
                playerAmmo.AddAmmo(playerInventory.itemNumberInStack[itemIndex], ammo.ammoType);
                break;

            default:
                break;
        }
        if(clickedItem.itemType != Item.ItemType.Attachment)
            playerInventory.itemNumberInStack[itemIndex]--;
        if (playerInventory.itemNumberInStack[itemIndex] <= 0)
        {
            playerInventory.inventory.RemoveAt(itemIndex);
            playerInventory.itemNumberInStack.RemoveAt(itemIndex);
        }

    }
    #endregion

    #region Remove Armor Effects
    public void RemoveArmorEffects(Armor returnedArmor)
    {
        playerHealth.ChangeDamageReduction(0 - returnedArmor.damageReductionAmount);
        switch (returnedArmor.additionalEffectType)
        {
            case Armor.AdditionalEffectType.IncreaseAccuracy:
                if (returnedArmor.additionalEffectNumberType == Armor.AdditionalEffectNumberType.FlatNumber)
                {
                    playerProperties.ChangeAccuracyModifierFlatNumber(0 - returnedArmor.additionalEffectAmount);
                }
                else
                {
                    float tempAmountToIncrease, tempPercentage;

                    tempPercentage = returnedArmor.additionalEffectAmount;
                    tempPercentage = 100 - tempPercentage;
                    tempPercentage /= 100;
                    tempAmountToIncrease = playerProperties.accuracyModifier / tempPercentage;
                    tempAmountToIncrease = tempAmountToIncrease - playerProperties.accuracyModifier;
                    playerProperties.ChangeAccuracyModifierFlatNumber(0 - tempAmountToIncrease);
                }
                break;
            case Armor.AdditionalEffectType.IncreaseStealth:
                if (returnedArmor.additionalEffectNumberType == Armor.AdditionalEffectNumberType.FlatNumber)
                {
                    playerProperties.ChangeDetectionModifierFlatNumber(0 - returnedArmor.additionalEffectAmount);
                }
                else
                {
                    float tempAmountToIncrease, tempPercentage;

                    tempPercentage = returnedArmor.additionalEffectAmount;
                    tempPercentage = 100 - tempPercentage;
                    tempPercentage /= 100;
                    tempAmountToIncrease = playerProperties.detectionModifier / tempPercentage;
                    tempAmountToIncrease = tempAmountToIncrease - playerProperties.detectionModifier;
                    playerProperties.ChangeDetectionModifierFlatNumber(0 - tempAmountToIncrease);
                }
                break;
            case Armor.AdditionalEffectType.MaxHealth:
                if (returnedArmor.additionalEffectNumberType == Armor.AdditionalEffectNumberType.FlatNumber)
                {
                    playerHealth.IncreaseMaxHealthFlatNumber(0 - returnedArmor.additionalEffectAmount);
                }
                else
                {
                    float tempAmountToIncrease, tempPercentage;
                    tempPercentage = returnedArmor.additionalEffectAmount;
                    tempPercentage /= 100;
                    tempAmountToIncrease = playerHealth.maxHealth / (tempPercentage + 1);
                    tempAmountToIncrease = playerHealth.maxHealth - tempAmountToIncrease;
                    playerHealth.IncreaseMaxHealthFlatNumber(0 - tempAmountToIncrease);
                }
                break;
            case Armor.AdditionalEffectType.MaxStamina:
                if (returnedArmor.additionalEffectNumberType == Armor.AdditionalEffectNumberType.FlatNumber)
                {
                    playerStamina.IncreaseMaxStaminaFlatNumber(0 - returnedArmor.additionalEffectAmount);
                }
                else
                {
                    float tempAmountToIncrease, tempPercentage;
                    tempPercentage = returnedArmor.additionalEffectAmount;
                    tempPercentage /= 100;
                    tempAmountToIncrease = playerStamina.maxStamina / (tempPercentage + 1);
                    tempAmountToIncrease = playerStamina.maxStamina - tempAmountToIncrease;
                    playerStamina.IncreaseMaxStaminaFlatNumber(0 - tempAmountToIncrease);
                }
                break;
            case Armor.AdditionalEffectType.MaxWeight:

                break;
        }
    } 
    #endregion

    public void SetOpenChest(ChestContents chest)
    {
        openChest = chest;
    }

    public void SetWeaponNumber(int index)
    {
        weaponToAttachTo = index;
    }

    void AddInventorySlot()
    {
        if(itemImage == null)
        {
            itemImage = new List<Image>(1);
            itemButtons = new List<Button>(1);
            amountOfText = new List<Text>(1);
        }
        GameObject tempSpawned = Instantiate(itemSlot);
        tempSpawned.transform.SetParent(transform);
        tempSpawned.transform.localScale = new Vector3(.5f, .5f);
        numOfItemSlots++;
        int tempInt = numOfItemSlots - 1;
        itemImage.Add(tempSpawned.transform.GetChild(1).GetComponent<Image>());
        itemImage[tempInt].rectTransform.localPosition = new Vector3(12.8f, -12.8f, 0);
        itemButtons.Add(tempSpawned.GetComponent<Button>());
        amountOfText.Add(tempSpawned.transform.GetChild(2).GetComponent<Text>());

        tempSpawned.GetComponent<Button>().onClick.AddListener(() => ButtonClickFunction(tempInt));

        EventTrigger trigger = tempSpawned.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data, tempInt); });
        trigger.triggers.Add(entry);

        UnityEvent rightClickTrigger = tempSpawned.GetComponent<RightClickButton>().rightClick;
        rightClickTrigger.AddListener(() => OnRightClick(tempInt));
    }

    void ButtonClickFunction(int buttonNum)
    {
        ApplyItemEffect(buttonNum);
        tipText.SetToolTipItemIndex(buttonNum);
        OnPointerEnterDelegate(null, buttonNum);
    }

    void OnPointerEnterDelegate(PointerEventData data, int buttonNum)
    {
        if (buttonNum > playerInventory.inventory.Count)
        {
            floatingToolTip.ResetText();
            tipText.ResetToolTipText();
            return;
        }
        tipText.SetToolTipItemIndex(buttonNum);
        string tempString = "";
        switch(playerInventory.inventory[buttonNum].itemType)
        {
            case Item.ItemType.Armor:
                tempString = "<size=10><b><color=red>Equip</color></b> To <color=green>Body</color></size>";
                break;
            case Item.ItemType.Attachment:
                tempString = "<size=10><b><color=red>Equip</color></b> To A <color=blue>Weapon</color></size>";
                break;
            case Item.ItemType.Consumable:
                tempString = "<size=10><b><color=red>Use</color></b></size>";
                break;
        }
        floatingToolTip.SetToolTipText("<b><color=navy><size=13>" + playerInventory.inventory[buttonNum].itemName + "</size></color></b>" + "\n\n" + tempString);
    }
    void OnPointerExitDelegate(PointerEventData data)
    {
        tipText.ResetToolTipText();
        floatingToolTip.ResetText();
    }
    public void OnRightClick(int buttomNum)
    {
        Debug.Log("Right clicked on " + playerInventory.inventory[buttomNum].itemName);
        if (buttomNum > playerInventory.inventory.Count)
            return;
        if (openChest != null)
        {
            openChest.AddItemToChest(playerInventory.inventory[buttomNum], playerInventory.itemNumberInStack[buttomNum]);
            playerInventory.RemoveItem(buttomNum);
        }
        else
        {
            Debug.Log("A chest isn't open!");
        }
    }
    public ChestContents GetOpenChest()
    {
        return openChest;
    }
}
