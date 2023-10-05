using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    #region Variables
    public Color hoverColor;
    public Shader wireframeShader;

    AudioSource notEnoughMoney;
    AudioSource buyAudio;
    AudioSource sellAudio;

    private Renderer rend;
    private Color startColor;

    public GameObject tower;
    private Shader towerOriginalShader;
    private int index = 0;
    private List<Shader> origShaders = new List<Shader>();
    private bool _towerBuilt = false;
    private bool active = false;
    bool previewing = false;
    public bool TRADER_NODE = false;

    Quaternion towerRot = Quaternion.Euler(new Vector3(0, 0, 0));

    private ResourceManager resourceManager;
    #endregion

    private void Start()
    {
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;

        buyAudio = GameObject.Find("Upgrade").GetComponent<AudioSource>();
        notEnoughMoney = GameObject.Find("NotEnoughMoney").GetComponent<AudioSource>();
        sellAudio = GameObject.Find("SellAudio").GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!active) { return; } 

        if (BuildManager.instance.getPurchaseMode() && !TRADER_NODE)
        {
            GameObject towerToBuild = BuildManager.instance.GetTowerToBuild();

            #region Preview
            if (!_towerBuilt && towerToBuild != null) { Preview(towerToBuild); }
            #endregion

            #region Buy
            if (correctBuyInput()) { TryToBuy(); }
            #endregion
        }
    }

    private void Preview(GameObject towerToBuild)
    {
        /* If preview tower is the same as before, we don't need to do anything */
        if (tower && tower.name == towerToBuild.name + "(Clone)") { return; }

        /* switching tower previews */
        if (tower) { Destroy(tower); }
        previewing = true;

        #region Place Wireframe Shader
        /* grab OG shader  << then >>  create the preview (wireframe) version of the tower */
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + .001f, transform.position.z);
        //tower = (GameObject)Instantiate(towerToBuild, pos, transform.rotation);
        tower = (GameObject)Instantiate(towerToBuild, pos, towerRot);
        towerOriginalShader = tower.GetComponent<Renderer>().material.shader;
        tower.GetComponent<Renderer>().material.shader = wireframeShader;
        foreach (Transform child in tower.transform)
        { 
            if(child.GetComponent<Renderer>() != null && child.name != "Cylinder")
            {
                origShaders.Add(child.GetComponent<Renderer>().material.shader);
                child.GetComponent<Renderer>().material.shader = wireframeShader;
            }

            if (child.name.Contains("Archer"))
            {
                foreach (Transform child2 in child)
                {
                    if (child2.name.Contains("Hand")) 
                    {
                        origShaders.Add(child2.GetComponent<Renderer>().material.shader);
                        child2.GetComponent<Renderer>().material.shader = wireframeShader;
                    }
                }
            }
        }
        ITower iTower = tower.GetComponent<ITower>();
        iTower.Disable();
        #endregion
    }

    public void TryToBuy()
    {
        if (tower && !_towerBuilt)
        {
            int cost = tower.GetComponent<ITower>().GetPrice();
            if (tower && resourceManager.hasEnoughResource(cost) && previewing)
            {
                previewing = false;
                buyAudio.Play();
                resourceManager.spendResource(cost);
                foreach (Transform child in tower.transform)
                {
                    if (child.GetComponent<Renderer>() != null && child.name != "Cylinder")
                    {
                        child.GetComponent<Renderer>().material.shader = origShaders[index];
                        index++;
                        if (child.name.Contains("Archer"))
                        {
                            foreach (Transform child2 in child)
                            {
                                if (child2.name.Contains("Hand"))
                                {
                                    child2.GetComponent<Renderer>().material.shader = origShaders[index];
                                    index++;
                                }
                            }
                        }
                    }
                }
                tower.GetComponent<Renderer>().material.shader = towerOriginalShader;
                ITower iTower = tower.GetComponent<ITower>();
                iTower.Enable();
                tower.tag = "Tower";
                _towerBuilt = true;
                BuildManager.instance.unPreviewTower();
            }
            else
            {
                notEnoughMoney.Play();
            }
        }
    }

    public void SellTower()
    {
        Destroy(tower);
        sellAudio.Play();

        tower = null;
        _towerBuilt = false;
    }

    bool correctBuyInput()
    {
        if (!tower) { return false; }
        if (Input.GetKeyDown(KeyCode.Mouse1)) { return checkRightClick(); }
        if (Input.GetKeyDown(KeyCode.Alpha1) && tower.GetComponent<ITower>().getTowerType() == TowerType.Archer) { return checkArcher(); }
        if (Input.GetKeyDown(KeyCode.Alpha2) && tower.GetComponent<ITower>().getTowerType() == TowerType.Mage) { return checkMage(); }
        if (Input.GetKeyDown(KeyCode.Alpha3) && tower.GetComponent<ITower>().getTowerType() == TowerType.Mortar) { return checkMortar(); }
        if (Input.GetKeyDown(KeyCode.Alpha4) && tower.GetComponent<ITower>().getTowerType() == TowerType.Crystal) { return checkFactory(); }
        return false;
        /* TODO: add future towers here */
    }

    bool checkRightClick()
    {
        if (GameManager.instance.tutorialMode && !BuildManager.instance.blockRightClick) 
        {
            TowerType tt = tower.GetComponent<ITower>().getTowerType();
            if (tt == TowerType.Archer) { return checkArcher(); }
            else if (tt == TowerType.Mage) { return checkMage(); }
            else if (tt == TowerType.Mortar) { return checkMortar(); }
            else if (tt == TowerType.Crystal) { return checkFactory(); }
            return true;
        }
        else if (!GameManager.instance.tutorialMode) { return true; }
        return false;
    }

    bool checkArcher()
    {
        if (GameManager.instance.tutorialMode && !BuildManager.instance.block1)
        {
            BuildManager.instance.purchasedArcher = true;
            return true;
        }
        else if (!GameManager.instance.tutorialMode) { return true; }
        return false;
    }

    bool checkMage()
    {
        if (GameManager.instance.tutorialMode && !BuildManager.instance.block2)
        {
            BuildManager.instance.purchasedMage = true;
            return true;
        }
        else if (!GameManager.instance.tutorialMode) { return true; }
        return false;
    }

    bool checkMortar()
    {
        if (GameManager.instance.tutorialMode && !BuildManager.instance.block3)
        {
            BuildManager.instance.purchasedMortar = true;
            return true;
        }
        else if (!GameManager.instance.tutorialMode) { return true; }
        return false;
    }

    bool checkFactory()
    {
        if (GameManager.instance.tutorialMode && !BuildManager.instance.block4)
        {
            BuildManager.instance.purchasedFactory = true;
            return true;
        }
        else if (!GameManager.instance.tutorialMode) { return true; }
        return false;
    }

    private void clearNode()
    {
        rend.material.color = startColor;

        /* if player walks off node while previewing tower */
        if (tower && !_towerBuilt) { Destroy(tower); }
        if (tower) { tower.GetComponent<ITower>().DisableRange(); }

        active = false;
        BuildManager.instance.setNodeTaken(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (active && other.gameObject.name == "Player")
        {
            clearNode();
            BuildManager.instance.setTowerBuilt(null);
        }

        if (tower) { tower.GetComponent<ITower>().DisableRange(); }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!BuildManager.instance.getNodeTaken() && !active)
        {
            if (other.gameObject.name == "Player" && BuildManager.instance.getPurchaseMode())
            {
                rend.material.color = hoverColor;
            }
            active = true;
            BuildManager.instance.setNodeTaken(true);
            BuildManager.instance.node = this.gameObject;
        }

        if (!BuildManager.instance.getPurchaseMode()) { clearNode(); BuildManager.instance.unPreviewTower(); }

        if (active && tower && _towerBuilt && BuildManager.instance.getPurchaseMode()) { BuildManager.instance.setTowerBuilt(tower.GetComponent<ITower>()); }
        if (active && tower && tower.GetComponent<ITower>().getEnabled()) {tower.GetComponent<ITower>().EnableRange(); }
    }
    public bool getTowerBuilt() { return _towerBuilt;}

    public ITower getTower() { return tower.GetComponent<ITower>(); }
    public void SetTower(GameObject _tower) { tower = _tower; }
}
