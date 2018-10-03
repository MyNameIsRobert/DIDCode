using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public  class PauseGame : MonoBehaviour {

    public GameObject menu;
    public GameObject inventory;
    public GameObject chestInventory;
    public GameObject hud;
    public GameObject attachmentMenu;
    public GameObject armorMenu;
    public GameObject deathMenu;
    public CanvasGroup extraOptions;
    public CanvasGroup ammoMenu;
    public CanvasGroup allInventoryPanel;
    public CanvasGroup consumablesPanel;
    public CanvasGroup weaponAttachmentsPanel;
    [SerializeField]
    DisplayInventory inventoryDisplay;
    public CanvasGroup displayQuests;
    bool curserLock = true;
    bool heldKey = false;
    Coroutine heldKeyRoutine;
    PlayerInputControls control;
    PlayerInventory playerInventory;
    public Toggle[] inventoryToggles;



    IEnumerator InputHoldCounter(float time)
    {
        heldKey = false;
        yield return new WaitForSeconds(time);
        heldKey = true;
        yield return null;
    }
    IEnumerator FadeCanvasAlpha(int fadeTo, CanvasGroup groupToFade, float speed)
    {
        if (groupToFade.alpha < fadeTo)
        {
            while(groupToFade.alpha < fadeTo)
            {
                groupToFade.alpha += Time.fixedDeltaTime * speed;
                yield return null;
            }
        }
        else if (groupToFade.alpha > fadeTo)
        {
            while (groupToFade.alpha > fadeTo)
            {
                groupToFade.alpha -= Time.deltaTime * speed;
                yield return null;
            }
        }
        else
        {
        }
        yield return null;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        control = GetComponent<PlayerInputControls>();

    }
    void Update () {
        if (Time.timeScale != 0) {
            if (Input.GetKeyDown(control.pauseMenu)){
                Time.timeScale = 0;
                menu.SetActive(true);
                hud.SetActive(false);
                inventory.GetComponent<CanvasGroup>().alpha = 0;
                inventory.GetComponent<CanvasGroup>().interactable = false;
                inventory.GetComponent<CanvasGroup>().blocksRaycasts = false;
                armorMenu.GetComponent<CanvasGroup>().alpha = 0;
                armorMenu.GetComponent<CanvasGroup>().interactable = false;
                armorMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
                chestInventory.GetComponent<CanvasGroup>().alpha = 0;
                chestInventory.GetComponent<CanvasGroup>().interactable = false;
                chestInventory.GetComponent<CanvasGroup>().blocksRaycasts = false;
                curserLock = false;
                ammoMenu.alpha = 0;
                ammoMenu.interactable = false;
                ammoMenu.blocksRaycasts = false;
            }
            else if (Input.GetKeyDown(control.inventory))
            {
                Time.timeScale = 0;
                inventory.GetComponent<CanvasGroup>().alpha = 1;
                inventory.GetComponent<CanvasGroup>().interactable = true;
                inventory.GetComponent<CanvasGroup>().blocksRaycasts = true;
                allInventoryPanel.alpha = 1;
                allInventoryPanel.interactable = true;
                allInventoryPanel.blocksRaycasts = true;
                hud.SetActive(false);
                menu.SetActive(false);
                ResetInventoryToggleGroup();
                GetComponent<PlayerInventory>().AutoSortInventory();
                curserLock = false;
                ammoMenu.alpha = 1;
                ammoMenu.interactable = true;
                ammoMenu.blocksRaycasts = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(control.pauseMenu)) {
                Time.timeScale = 1;
                menu.SetActive(false);
                HideExtraOptions();
                inventory.GetComponent<CanvasGroup>().alpha = 0;
                inventory.GetComponent<CanvasGroup>().interactable = false;
                chestInventory.GetComponent<CanvasGroup>().alpha = 0;
                chestInventory.GetComponent<CanvasGroup>().interactable = false;
                armorMenu.GetComponent<CanvasGroup>().alpha = 0;
                armorMenu.GetComponent<CanvasGroup>().interactable = false;
                armorMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
                ammoMenu.alpha = 0;
                ammoMenu.interactable = false;
                ammoMenu.blocksRaycasts = false;
                hud.SetActive(true);

                curserLock = true;
                ResetOpenChest();
            }
            else if (Input.GetKeyDown(control.inventory) && !menu.activeSelf)
            {
                Time.timeScale = 1;
                HideExtraOptions();
                inventory.GetComponent<CanvasGroup>().alpha = 0;
                inventory.GetComponent<CanvasGroup>().interactable = false;
                inventory.GetComponent<CanvasGroup>().blocksRaycasts = false;
                chestInventory.GetComponent<CanvasGroup>().alpha = 0;
                chestInventory.GetComponent<CanvasGroup>().interactable = false;
                chestInventory.GetComponent<CanvasGroup>().blocksRaycasts = false;
                armorMenu.GetComponent<CanvasGroup>().alpha = 0;
                armorMenu.GetComponent<CanvasGroup>().interactable = false;
                armorMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
                ammoMenu.alpha = 0;
                ammoMenu.interactable = false;
                ammoMenu.blocksRaycasts = false;
                hud.SetActive(true);
                curserLock = true;
                ResetOpenChest();

            }
        }
        if (Input.GetKeyDown(control.questMenu))
        {
            TogglePreviousQuests();            
        }
        if(Input.GetKeyDown(control.weaponMenu))
        {
            DisplayAttachmentMenu();
            DisplayAmmoMenu();
            heldKeyRoutine = StartCoroutine(InputHoldCounter(.2f));
        }
        if(Input.GetKeyUp(control.weaponMenu))
        {
            StopCoroutine(heldKeyRoutine);
            if(heldKey)
            {
                CanvasGroup tempGroup = attachmentMenu.GetComponent<CanvasGroup>();
                tempGroup.alpha = 0;
                ShowHUD();
                HideAmmoMenu();
            }
            else
            {
                CanvasGroup group = attachmentMenu.GetComponent<CanvasGroup>();
                if (group.interactable)
                {
                    HideAttachmentMenu();
                    HideAmmoMenu();
                }
                else
                {
                    ShowAttachmentMenu();
                    DisplayAmmoMenu();
                }
            }
        }
        CurserLock();


	}

    void CurserLock()
    {
        if (curserLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
        else if (!curserLock)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }

    public void ShowMenu()
    {
        Time.timeScale = 0;
        menu.SetActive(true);
        hud.SetActive(false);
        curserLock = false;
    }
    public void ShowInventory()
    {
        Time.timeScale = 0;
        inventory.GetComponent<CanvasGroup>().alpha = 1;
        inventory.GetComponent<CanvasGroup>().interactable = true;
        inventory.GetComponent<CanvasGroup>().blocksRaycasts = true;
        armorMenu.GetComponent<CanvasGroup>().alpha = 1;
        armorMenu.GetComponent<CanvasGroup>().interactable = true;
        armorMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;
        hud.SetActive(false);
        curserLock = false;
    }
    public void HideMenu()
    {
        Time.timeScale = 1;
        menu.SetActive(false);
        hud.SetActive(true);
        curserLock = true;
    }
    public void HideInventory()
    {
        Time.timeScale = 1;
        inventory.GetComponent<CanvasGroup>().alpha = 0;
        inventory.GetComponent<CanvasGroup>().interactable = false;
        inventory.GetComponent<CanvasGroup>().blocksRaycasts = false;
        armorMenu.GetComponent<CanvasGroup>().alpha = 0;
        armorMenu.GetComponent<CanvasGroup>().interactable = false;
        armorMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
        hud.SetActive(true);
        curserLock = true;
    }
    public void ShowAttachmentMenu()
    {
        Time.timeScale = 0;
        CanvasGroup group = attachmentMenu.GetComponent<CanvasGroup>();
        StartCoroutine(FadeCanvasAlpha(1, group, 5));
        group.interactable = true;
        group.blocksRaycasts = true;
        hud.SetActive(false);
        curserLock = false;
    }
    public void HideAttachmentMenu()
    {
        Time.timeScale = 1;
        CanvasGroup group = attachmentMenu.GetComponent<CanvasGroup>();
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
        hud.SetActive(true);
        curserLock = true;
    }
    public void DisplayAttachmentMenu()
    {
        CanvasGroup group = attachmentMenu.GetComponent<CanvasGroup>();
        group.alpha = .8f;
        HideHUD();
    }

    public void ShowChestAndInventory()
    {
        Time.timeScale = 0;
        hud.SetActive(false);
        inventory.GetComponent<CanvasGroup>().alpha = 1;
        inventory.GetComponent<CanvasGroup>().interactable = true;
        inventory.GetComponent<CanvasGroup>().blocksRaycasts = true;
        GetComponent<PlayerInventory>().AutoSortInventory();
        chestInventory.GetComponent<CanvasGroup>().alpha = 1;
        chestInventory.GetComponent<CanvasGroup>().interactable = true;
        chestInventory.GetComponent<CanvasGroup>().blocksRaycasts = true;
        curserLock = false;
        DisplayAmmoMenu();
        ShowChestAndInventoryPanel();
        ResetInventoryToggleGroup();
    }
    public void HideChestAndInventory()
    {
        Time.timeScale = 1;
        inventory.GetComponent<CanvasGroup>().alpha = 0;
        inventory.GetComponent<CanvasGroup>().interactable = false;
        inventory.GetComponent<CanvasGroup>().blocksRaycasts = false;
        chestInventory.GetComponent<CanvasGroup>().alpha = 0;
        chestInventory.GetComponent<CanvasGroup>().interactable = false;
        chestInventory.GetComponent<CanvasGroup>().interactable = false;
        curserLock = true;
        hud.SetActive(true);
        inventoryDisplay.SetOpenChest(null);
        HideChestAndInventoryPanel();
        HideAmmoMenu();
        ResetOpenChest();
    }
    public void HideHUD()
    {
        hud.SetActive(false);
    }
    public void ShowHUD()
    {
        hud.SetActive(true);
    }
    public void ShowDeathMenu()
    {        
        HideChestAndInventory();
        HideAttachmentMenu();
        HideMenu();
        hud.SetActive(false);
        CanvasGroup group;
        try
        {
            group = deathMenu.GetComponent<CanvasGroup>();
        }
        catch (MissingComponentException)
        {
            Debug.Log("Canvas Group Not attached! Attaching one now");
            deathMenu.AddComponent<CanvasGroup>();
            group = deathMenu.GetComponent<CanvasGroup>();
            deathMenu.SetActive(true);
        }
        StartCoroutine(FadeCanvasAlpha(1, group, .5f));
        group.interactable = true;
        group.blocksRaycasts = true;
        Time.timeScale = 0;
        curserLock = false;
    }

    public void ShowExtraOptions()
    {
        menu.SetActive(false);
        extraOptions.alpha = 1;
        extraOptions.interactable = true;
        extraOptions.blocksRaycasts = true;
    }
    public void HideExtraOptions()
    {
        extraOptions.alpha = 0;
        extraOptions.interactable = false;
        extraOptions.blocksRaycasts = false;
    }
    public void Pause()
    {
        Time.timeScale = 0;
    }
    public void Unpause()
    {
        Time.timeScale = 1;
    }
    public void LockCursor()
    {
        curserLock = true;
    }
    public void UnlockCursor()
    {
        curserLock = false;
    }
    public void TogglePreviousQuests()
    {
        if(displayQuests.alpha == 1)
        {
            displayQuests.alpha = 0;
            displayQuests.interactable = false;
            displayQuests.blocksRaycasts = false;
            ShowHUD();
            Unpause();
            curserLock = true;
        }
        else
        {
            displayQuests.alpha = 1;
            displayQuests.interactable = true;
            displayQuests.blocksRaycasts = true;
            HideHUD();
            Pause();
            curserLock = false;
        }
    }
    public void DisplayAmmoMenu()
    {
        ammoMenu.alpha = 1;
        ammoMenu.interactable = true;
        ammoMenu.blocksRaycasts = true;
    }
    public void HideAmmoMenu()
    {
        ammoMenu.alpha = 0;
        ammoMenu.interactable = false;
        ammoMenu.blocksRaycasts = false;
    }

    public void ResetOpenChest()
    {
        inventoryDisplay.SetOpenChest(null);
    }
    public void ToggleInventoryAndChest(Toggle toggle)
    {
        if (toggle.isOn)
        {
            if(inventoryDisplay.GetOpenChest() != null)
                ShowChestAndInventoryPanel();
            else
            {
                allInventoryPanel.alpha = 1;
                allInventoryPanel.interactable = true;
                allInventoryPanel.blocksRaycasts = true;
            }
        }
        else
            HideChestAndInventoryPanel();
    }
    public void ToggleArmorPanel(Toggle toggle)
    {
        CanvasGroup group = armorMenu.GetComponent<CanvasGroup>();
        if(toggle.isOn)
        {
            group.alpha = 1;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        else
        {
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }
    public void ToggleConsumablePanel(Toggle toggle)
    {
        if (toggle.isOn)
        {
            consumablesPanel.alpha = 1;
            consumablesPanel.interactable = true;
            consumablesPanel.blocksRaycasts = true;
        }
        else
        {
            consumablesPanel.alpha = 0;
            consumablesPanel.interactable = false;
            consumablesPanel.blocksRaycasts = false;
        }
    }
    public void ToggleWeaponAttachments(Toggle toggle)
    {
        if(toggle.isOn)
        {
            weaponAttachmentsPanel.alpha = 1;
            weaponAttachmentsPanel.interactable = true;
            weaponAttachmentsPanel.blocksRaycasts = true;
        }
        else
        {
            weaponAttachmentsPanel.alpha = 0;
            weaponAttachmentsPanel.interactable = false;
            weaponAttachmentsPanel.blocksRaycasts = false;
        }
    }
    void HideChestAndInventoryPanel()
    {
        CanvasGroup group = chestInventory.GetComponent<CanvasGroup>();
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
        allInventoryPanel.alpha = 0;
        allInventoryPanel.interactable = false;
        allInventoryPanel.blocksRaycasts = false;
    }
    void ShowChestAndInventoryPanel()
    {
        CanvasGroup group = chestInventory.GetComponent<CanvasGroup>();
        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;
        allInventoryPanel.alpha = 1;
        allInventoryPanel.interactable = true;
        allInventoryPanel.blocksRaycasts = true;
    }
    void ResetInventoryToggleGroup()
    {
        inventoryToggles[0].isOn = true;
        for(int i = 1; i < 4; i++)
        {
            inventoryToggles[i].isOn = false;
        }
    }
}
