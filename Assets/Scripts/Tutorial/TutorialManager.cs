using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour

{
    public TextMeshProUGUI TutorialGenText;
    public Canvas TutorialCanvas;
    public GameObject MainCamera;
    public GameObject PressF;
    public bool canPressF;
    public bool waitingKeyPress;
    public bool waitingArcher;
    public bool waitingMage;
    public bool waitingMortar;
    public bool waitingMortarTarget;
    public bool waitingMortarUpgrade;
    public bool waitingMortarUpgrade2;
    public bool waitingCrystal;
    public bool waitingSell;
    public AudioSource backgroundMusic;

    [SerializeField] GameObject chestManager;
    [SerializeField] GameObject blockage;

    private KeyCode nextPress;
    private bool next;
    private ResourceManager resourceManager;
    private CameraFollow cameraFollow;
    private BuildManager buildManager;

    // add Selling
    // add Arrows
    // rn techincally
    private string[] tutorialText = {
        "Use <color=#0070FF>WASD</color> to move your charater towards the <color=#E39100>Castle</color>", 
        "Welcome Builder!",
        "We have an <color=red>Urgent</color> Mission for You!",
        "The nearby Castle of <color=yellow>Goldcrest</color> has reported enemy <color=#FF5D0E>Krystallos</color> Soldiers amassing near their front gate",
        "If they manage to infiltrate their Castle, The <color=#FF5D0E>Krystallos</color> Commander <color=red>Xaakt</color> will have enough <color=green>Exousia Crystals</color> to take over the whole region",
        "With your help you can deliver our New <color=green>Exousia</color> powered Towers to help aid <color=yellow>Goldcrest</color> and Defend us all from those <color=#FF5D0E>Krystallos</color> Bastards!",
        "Let's practice your utilities!\n Press <color=#0070FF>SPACE</color> To Zoom In/Out",
        "Here are some <color=green>Crystals.</color>\n Crystals are gained by killing <color=red>enemies</color> and collecting <color=#E39100>chests</color>",
        "Press <color=#0070FF>TAB</color> To Open The <color=#E39100>Buy Menu</color>\n",
        "Let's try building an <color=#E39100>Archer Tower</color>. Click on 'Archer Tower' in the menu to preview, then again to Confirm",
        "Great! Now let's test a <color=#E39100>Mage Tower</color>. Note that you can use <color=#0070FF>Right Click</color> to Confirm as an additional method",
        "Now you're getting it! Towers can also be purchased using <color=#0070FF>1, 2, 3, 4</color> respectively. With the Buy Menu closed, Press <color=#0070FF>3</color> twice to preview & purchase a <color=#E39100>Mortar Tower</color>",
        "Each mortar's blast can be directed using its <color=#E39100>target</color>. Use <color=#0070FF>Left Click</color> anywhere to move the target and <color=#0070FF>Left Click</color> again to place it",
        "Let's upgrade this Mortar Tower a couple times to explore our options!",
        "Here you can see two paths are available. Each has its own advantages so choose wisely",
        "Finally we have the <color=#E39100>Crystal Factory</color>. This time, let's use the mouse scroll wheel to find the Crystal Factory, then Confirm using any method",
        "Towers <color=red>sell</color> for a fraction of their cost\n Let's sell this Crystal Factory to test",
        "Great work. To complete our preparation, there is one last thing you should know about.",
        "<color=#E39100>Chests</color> can spawn randomly within a round. Look out for them to get a boost in resources, or replenish your health when low!",
        "A 'pop' indicates one has spawned\n so be on the lookout! <color=#E39100>Chests</color> despawn after 15s so move quickly!",
        "When You Are Ready To Set Out For <color=yellow>Goldcrest</color> Walk Out The <color=#E39100>North Gate</color>",
        "Good Luck Builder, We Are All Counting On <color=#E39100>You!</color>"
    };

    private int prog = 0;
    // Start is called before the first frame update
    void Start()
    {
        TutorialGenText.text = tutorialText[0];
        next = false;
        waitingKeyPress = false;
        waitingArcher = false;
        waitingMage = false;
        waitingMortar = false;
        waitingMortarTarget = false;
        waitingMortarUpgrade = false;
        waitingMortarUpgrade2 = false;
        waitingCrystal = false;
        waitingSell = false;
        PressF.SetActive(false);
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
        cameraFollow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();
        cameraFollow.setTutorialView();
        buildManager = BuildManager.instance;
        buildManager.GetComponent<BuildManager>().setTutorialMode();
        GameManager.instance.tutorialMode = true;
        GameManager.instance.blockUpgrades = true;

        backgroundMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canPressF && !waitingKeyPress)
        {
            prog++;
            next = true;
        }
        else if (waitingKeyPress && Input.GetKeyDown(nextPress)) 
        {
            prog++;
            next = true;
            waitingKeyPress = false;
        }

        if (waitingArcher && BuildManager.instance.purchasedArcher)
        {
            prog++;
            next = true;
            waitingArcher = false;
        }
        if (waitingMage && BuildManager.instance.purchasedMage)
        {
            prog++;
            next = true;
            waitingMage = false;
        }
        if (waitingMortar && BuildManager.instance.purchasedMortar)
        {
            prog++;
            next = true;
            waitingMortar = false;
        }
        if (waitingMortarTarget && BuildManager.instance.usedMortarTarget)
        {
            prog++;
            next = true;
            waitingMortarTarget = false;
        }
        if (waitingMortarUpgrade && BuildManager.instance.upgradedMortar == 2)
        {
            prog++;
            next = true;
            waitingMortarUpgrade = false;
        }
        if (waitingMortarUpgrade2 && BuildManager.instance.upgradedMortar == 3)
        {
            prog++;
            next = true;
            waitingMortarUpgrade2 = false;
        }
        if (waitingCrystal && BuildManager.instance.purchasedFactory)
        {
            prog++;
            next = true;
            waitingCrystal = false;
        }
        if (waitingSell && BuildManager.instance.soldTower)
        {
            prog++;
            next = true;
            waitingSell = false;
        }

        if (prog == 1 && next)
        {
            PressF.gameObject.SetActive(true);
            canPressF = true;
            next = false;
            TutorialGenText.text = tutorialText[1];
        }
        else if(prog == 2 && next)
        {
            next = false;
            TutorialGenText.text = tutorialText[2];
        }
        else if (prog == 3 && next)
        {
            next = false;
            TutorialGenText.text = tutorialText[3];
        }
        else if (prog == 4 && next)
        {
            next = false;
            TutorialGenText.text = tutorialText[4];
        }
        else if (prog == 5 && next)
        {
            next = false;
            TutorialGenText.text = tutorialText[5];
        }
        else if (prog == 6 && next) // Zoom In/Out
        {
            next = false;
            TutorialGenText.text = tutorialText[6];

            canPressF = false;
            PressF.SetActive(false);
            nextPress = KeyCode.Space;
            waitingKeyPress = true;

        }
        else if (prog == 7 && next) // Here are some crystals
        {
            next = false;
            TutorialGenText.text = tutorialText[7];

            canPressF = true;
            PressF.SetActive(true);
            resourceManager.addResource(35000, true);
            // setActive(true) << arrow pointing to resources
        }
        else if (prog == 8 && next) // Open Buy Menu
        {
            next = false;
            TutorialGenText.text = tutorialText[8];

            canPressF = false;
            PressF.SetActive(false);
            nextPress = KeyCode.Tab;
            waitingKeyPress = true;
            buildManager.blockPurchaseMode = false;
        }
        else if (prog == 9 && next) // Purchase archer via Button, confirm via Button (any allowed)
        {
            next = false;
            TutorialGenText.text = tutorialText[9];

            buildManager.block1 = false;
            waitingArcher = true;
        }
        else if (prog == 10 && next) // Purchase mage via Button, confirm via Right Click (any allowed)
        {
            next = false;
            TutorialGenText.text = tutorialText[10];

            buildManager.block1 = true;
            buildManager.block2 = false;
            buildManager.blockRightClick = false;
            waitingMage = true;
        }
        else if (prog == 11 && next) // Purchase mortar via '3', confirm via any
        {
            next = false;
            TutorialGenText.text = tutorialText[11];

            buildManager.block2 = true;
            buildManager.block3 = false;

            waitingMortar = true;
        }
        else if (prog == 12) // Use Mortar Target
        {
            next = false;
            TutorialGenText.text = tutorialText[12];

            waitingMortarTarget = true;
            buildManager.block3 = true;
        }
        else if (prog == 13 && next) // wait for mortar upgrade
        {
            next = false;
            TutorialGenText.text = tutorialText[13];

            waitingMortarUpgrade = true;
        }
        else if (prog == 14 && next) // wait for final mortar upgrade
        {
            next = false;
            TutorialGenText.text = tutorialText[14];

            waitingMortarUpgrade2 = true;
        }
        else if (prog == 15 && next) // Purchase crystal
        {
            next = false;
            TutorialGenText.text = tutorialText[15];

            buildManager.block4 = false;
            buildManager.blockScroll = false;

            waitingCrystal = true;
        }
        else if (prog == 16 && next) // waiting Sell
        {
            next = false;
            TutorialGenText.text = tutorialText[16];

            buildManager.block4 = true;
            buildManager.blockSell = false;
            buildManager.blockScroll = true;

            waitingSell = true;
        }
        else if (prog == 17 && next) // chest info
        {
            next = false;
            TutorialGenText.text = tutorialText[17];

            PressF.SetActive(true);
            canPressF = true;
        }
        else if (prog == 18 && next) // chest info
        {
            next = false;
            TutorialGenText.text = tutorialText[18];
        }
        else if (prog == 19 && next) // chest info
        {
            next = false;
            TutorialGenText.text = tutorialText[19];

            chestManager.GetComponent<ChestManager>().TutorialChests();
        }
        else if (prog == 20) // chest info
        {
            next = false;
            TutorialGenText.text = tutorialText[20];
        }
        else if (prog == 21) // move to castle
        {
            next = false;
            TutorialGenText.text = tutorialText[21];
        }
        else if (prog == 22) // good luck!
        {
            TutorialCanvas.gameObject.SetActive(false);
            blockage.SetActive(false);
            BuildManager.instance.unsetTutorialMode();
            GameManager.instance.blockUpgrades = false;
        }
    }

    public void incrementProg() { prog++; next = true; }
}
