using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;
    
    static bool purchaseMode;
    static bool nodeTaken = false;

    public GameObject node = null;
    public bool menuChange = false;

    public GameObject archerTower1;
    public GameObject mageTower1;
    public GameObject mortarTower1;
    public GameObject crystalTower1;

    [SerializeField] Image UpgradeMenuImage;
    [SerializeField] Sprite archerImage;
    [SerializeField] Sprite mageImage;
    [SerializeField] Sprite mortarImage;
    [SerializeField] Sprite crystalImage;

    public Canvas UpgradeMenu;
    public Canvas BuyMenu;
    public Canvas TraderMenu = null;

    // tutorial features
    public bool block1 = false;
    public bool block2 = false;
    public bool block3 = false;
    public bool block4 = false;
    public bool blockScroll = false;
    public bool blockPurchaseMode = false;
    public bool blockRightClick = false;
    public bool blockSell = false;

    public bool purchasedArcher = false;
    public bool purchasedMage = false;
    public bool purchasedMortar = false;
    public bool usedMortarTarget = false;
    public int upgradedMortar = 0;
    public bool purchasedFactory = false;
    public bool soldTower = false;

    public ITower towerBuilt;
    int towerPreviewing = -1;

    GameObject towerToBuild;


    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    private void Start()
    {
        towerToBuild = null;
        purchaseMode = false;
        towerBuilt = null;
    }

    #region Setter/Getters
    public bool getPurchaseMode() { return purchaseMode; }
    public bool getNodeTaken() { return nodeTaken; }
    public void setNodeTaken(bool setter) { nodeTaken = setter; menuChange = true; }
    public GameObject GetTowerToBuild() { return towerToBuild; }
    public void setTowerBuilt(ITower tower) { towerBuilt = tower; }
    public ITower getTowerBuilt() { return towerBuilt; }
    #endregion

    private void Update()
    {
        #region togglePurchaseMode
        if (Input.GetKeyDown(KeyCode.Tab) && !blockPurchaseMode)
        {
            purchaseMode = !purchaseMode;
            if (!purchaseMode)
            {
                towerToBuild = null;
            }
        }
        #endregion

        if (nodeTaken && node.GetComponent<Node>().getTowerBuilt()) { towerBuilt = node.GetComponent<Node>().getTower(); }

        checkBuyInputs();

        #region Menus
        if (purchaseMode)
        {
            if (towerBuilt != null) 
            {
                if (!towerBuilt.getEnabled())
                {
                    closeAllMenus();
                }
                else
                {
                    if (menuChange) { showUpgradeMenu(); menuChange = false; }
                }
            }
            else {
                if (nodeTaken && !node.GetComponent<Node>().getTowerBuilt()) {
                    if (node.GetComponent<Node>().TRADER_NODE && GameManager.instance.TraderActive() && TraderMenu != null)
                        showTraderMenu();
                    else
                        showBuyMenu(); 
                }
                
            }
        }
        else
        {
            closeAllMenus();
            towerBuilt = null;
            //checkBuyInputs(); /* allow user to open menu via tower keybinds directly */
        }
        #endregion
    }


    #region DisplayMenuFunctionality
    public void closeAllMenus()
    {
        UpgradeMenu.gameObject.SetActive(false);
        BuyMenu.gameObject.SetActive(false);
        if (TraderMenu != null) { TraderMenu.gameObject.SetActive(false); }
        purchaseMode = false;
        towerToBuild = null;
    }

    void showTraderMenu()
    {
        TraderMenu.gameObject.SetActive(true);
        BuyMenu.gameObject.SetActive(false);
        UpgradeMenu.gameObject.SetActive(false);
    }

    void showBuyMenu()
    {
        BuyMenu.gameObject.SetActive(true);
        UpgradeMenu.gameObject.SetActive(false);
        if (TraderMenu != null) { TraderMenu.gameObject.SetActive(false); }
    }

    void showUpgradeMenu()
    {
        BuyMenu.gameObject.SetActive(false);
        UpgradeMenu.gameObject.SetActive(true);
        if (TraderMenu != null) { TraderMenu.gameObject.SetActive(false); }
        UpgradeMenu.GetComponent<UniversalUpgradeMenu>().UpdateText(towerBuilt);

        if (towerBuilt.getTowerType() == TowerType.Archer) { UpgradeMenuImage.sprite = archerImage; }
        else if (towerBuilt.getTowerType() == TowerType.Mage) { UpgradeMenuImage.sprite = mageImage; }
        else if (towerBuilt.getTowerType() == TowerType.Mortar) { UpgradeMenuImage.sprite = mortarImage; }
        else if (towerBuilt.getTowerType() == TowerType.Crystal) { UpgradeMenuImage.sprite = crystalImage; }
    }
    #endregion

    #region CheckBuyInputs
    void checkBuyInputs()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1) && !block1)
        {
            previewTower(archerTower1, 1);
        }

        if (Input.GetKeyUp(KeyCode.Alpha2) && !block2)
        {
            previewTower(mageTower1, 2);
        }

        if (Input.GetKeyUp(KeyCode.Alpha3) && !block3)
        {
            previewTower(mortarTower1, 3);
        }

        if (Input.GetKeyUp(KeyCode.Alpha4) && !block4)
        {
            previewTower(crystalTower1, 4);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && !blockScroll) // forward
        {
            if (purchaseMode)
            {
                if (towerToBuild == archerTower1) { previewTower(crystalTower1, 4);}
                else if (towerToBuild == mageTower1) { previewTower(archerTower1, 1); }
                else if (towerToBuild == mortarTower1) { previewTower(mageTower1, 2); }
                else if (towerToBuild == crystalTower1) { previewTower(mortarTower1, 3); }
                else { previewTower(archerTower1, 1); }
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && !blockScroll) // backward
        {
            if (purchaseMode)
            {
                if (towerToBuild == archerTower1) { previewTower(mageTower1, 2); }
                else if (towerToBuild == mageTower1) { previewTower(mortarTower1, 3); }
                else if (towerToBuild == mortarTower1) { previewTower(crystalTower1, 4); }
                else if (towerToBuild == crystalTower1) { previewTower(archerTower1, 1); }
                else { previewTower(archerTower1, 1); }
            }
        }
    }
    #endregion

    public void set_towerToBuild(int idx)
    {
         switch (idx)
        {
            case 1:
                if ((GameManager.instance.tutorialMode && !block1) || !GameManager.instance.tutorialMode)
                {
                    if (towerPreviewing == 1 && node != null) { node.GetComponent<Node>().TryToBuy(); if (GameManager.instance.tutorialMode) { purchasedArcher = true; } }
                    else { previewTower(archerTower1, 1); }
                }
                break;
            case 2:
                if ((GameManager.instance.tutorialMode && !block2) || !GameManager.instance.tutorialMode)
                {
                    if (towerPreviewing == 2 && node != null) { node.GetComponent<Node>().TryToBuy(); if (GameManager.instance.tutorialMode) { purchasedMage = true; } }
                    else { previewTower(mageTower1, 2); }
                }
                break;
            case 3:
                if ((GameManager.instance.tutorialMode && !block3) || !GameManager.instance.tutorialMode)
                {
                    if (towerPreviewing == 3 && node != null) { node.GetComponent<Node>().TryToBuy(); if (GameManager.instance.tutorialMode) { purchasedMortar = true; } }
                    else { previewTower(mortarTower1, 3); }
                }
                break;
            case 4:
                if ((GameManager.instance.tutorialMode && !block4) || !GameManager.instance.tutorialMode)
                {
                    if (towerPreviewing == 4 && node != null) { node.GetComponent<Node>().TryToBuy(); if (GameManager.instance.tutorialMode) { purchasedFactory = true; } }
                    else { previewTower(crystalTower1, 4); }
                }
                break;
            default:
                Debug.Log("buy menu error");
                break;
        }

    }

    void previewTower(GameObject tower, int towerNum)
    {
        purchaseMode = true;
        towerToBuild = tower;
        towerPreviewing = towerNum;
        towerToBuild.tag = "Preview";
    }

    public void unPreviewTower()
    {
        towerPreviewing = -1;
        towerToBuild = null;
    }

    public void SellTower()
    {
        node.GetComponent<Node>().SellTower();
        closeAllMenus();
    }

    public void setTutorialMode()
    {
        block1 = true;
        block2 = true;
        block3 = true;
        block4 = true;
        blockScroll = true;
        blockPurchaseMode = true;
        blockRightClick = true;
        blockSell = true;
    }

    public void unsetTutorialMode()
    {
        block1 = false;
        block2 = false;
        block3 = false;
        block4 = false;
        blockScroll = false;
        blockPurchaseMode = false;
        blockRightClick = false;
        blockSell = false;
    }

}
