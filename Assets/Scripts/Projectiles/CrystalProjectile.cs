using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalProjectile : MonoBehaviour
{
    private Vector3 target;

    //Speed of projectile
    public float speed = 50f;

    public float destroyTimer = 25;
    private int resourceAmount;

    private ResourceManager resourceManager;

    bool playerInRange = false;

    GameObject player;


    // Fading Stuff
    public float fadeSpeed = 5f;
    public float fadeAmount = 0f;
    float originalOpacity;
    Material Mat;
    public bool DoFade = false;
    private bool FadingIn = false;

    float targetEmissionIntensity = -5f;
    float currentEmissionIntensity = 0f;
    float originalEmissionIntensity;
    public Color originalEmissionColor;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
        Mat = GetComponent<Renderer>().material;
        originalEmissionIntensity = 0;
        //originalEmissionColor = Color.white;
        originalOpacity = Mat.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange) { pickup(); }
        else { placement(); }

        if (DoFade) {
            if (FadingIn)
                FadeIn();
            else
                FadeOut();
        }
    }


    void FadeOut()
    {
        Color currentColor = Mat.color;
        Color smoothColor = new Color(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(currentColor.a, fadeAmount, fadeSpeed * Time.deltaTime));
        Mat.color = smoothColor;

        currentEmissionIntensity = Mathf.Lerp(currentEmissionIntensity, targetEmissionIntensity, fadeSpeed * Time.deltaTime);
        Mat.SetColor("_EmissionColor", originalEmissionColor * currentEmissionIntensity);

        if (currentColor.a <= (255 * (fadeAmount + 0.001f)))
        {
            FadingIn = true;
        }
    }

    void FadeIn()
    {
        Color currentColor = Mat.color;
        Color smoothColor = new Color(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(currentColor.a, originalOpacity, fadeSpeed * Time.deltaTime));
        Mat.color = smoothColor;

        currentEmissionIntensity = Mathf.Lerp(currentEmissionIntensity, originalEmissionIntensity, fadeSpeed * Time.deltaTime);
        Mat.SetColor("_EmissionColor", originalEmissionColor * currentEmissionIntensity);

        if (currentColor.a >= (originalOpacity - 0.1f))
            FadingIn = false;
    }

    void pickup()
    {
        target = player.transform.position;
        Vector3 targetMiddle = new Vector3(target.x, 0, target.z);

        Vector3 dir = targetMiddle - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        //when projectile hits target
        if (dir.magnitude <= distanceThisFrame)
        {
            resourceManager.addResource(resourceAmount, true);
            Destroy(gameObject);
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void placement()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (destroyTimer < 0)
        {
            Destroy(gameObject);
        }
        destroyTimer -= Time.deltaTime;

        // do fading at last 5s
        if (destroyTimer <= 5)
            DoFade = true;

        Vector3 targetMiddle = new Vector3(target.x, 0, target.z);

        Vector3 dir = targetMiddle - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        //when projectile hits target
        if (dir.magnitude <= distanceThisFrame)
        {
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    public void Seek(Vector3 _target, int _resourceAmount)
    {
        target = _target;
        resourceAmount = _resourceAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerInRange = true;
            //resourceManager.addResource(resourceAmount, true);
            //Destroy(gameObject);
        }
    }
}
