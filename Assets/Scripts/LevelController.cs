using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{

    public static LevelController instance;

    [Header("Prefab and objects")]
    public GameObject dnaButton;
    public Transform canvas;

    [Header("Continents")]
    public Continent[] continents;
    public Button[] continentButtons;

    [Header("UI")]
    public Text infectedText;
    public Text deathText;
    public Text dnaPointsText;
    public Text timerText;
    public Text cureText;
    public Slider cureSlider;
    public Text finalMessage;

    [Header("Unlock info")]
    public GameObject infoPanel;
    public Text skillNameText;
    public Text descriptionText;


    [Header("Info rates")]
    public float transmissionPerMinute = 100;
    public float deathRate;
    public float infectionRate;
    public float cureRate;
    public int dnaPoints;

    [Header("Audios")]
    public AudioClip clickSound;
    public AudioClip dnaSFX;
    private AudioSource audioSource;

    [Header("Control values")]
    private float infecteds;
    private float deads;
    private float timer;
    private float cureProgress;

    private UnlockButton myUnlockButton;

    private bool started;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CheckWinCondition", 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
            return;

        timer += Time.deltaTime;
        timerText.text = "Timer: " + timer.ToString("0");

        for (int i = 0; i < continents.Length; i++)
        {
            if (continents[i].infected)
            {
                if(continents[i].infecteds < continents[i].totalPopulation)
                {
                    continents[i].infecteds += transmissionPerMinute * (Time.deltaTime / 60);
                    continents[i].infectedFill.fillAmount = continents[i].infecteds / continents[i].totalPopulation;
                }

                if(continents[i].deaths < continents[i].totalPopulation)
                {
                    continents[i].deaths += continents[i].infecteds * (deathRate / 100) * (Time.deltaTime / 60);
                    continents[i].deathFill.fillAmount = continents[i].deaths / continents[i].totalPopulation;
                }
            }
        }

        cureProgress += cureRate * (Time.deltaTime / 60);
        cureText.text = cureProgress.ToString("0") + "%";
        cureSlider.value = cureProgress / 100;
        if(cureProgress >= 100)
        {
            GameOver("Sua doença infectou " + infecteds.ToString("N0") + " e matou " + deads.ToString("N0") + " pessoas em " + timer.ToString("0") + " dias");
        }

        infecteds = 0;
        deads = 0;
        for (int i = 0; i < continents.Length; i++)
        {
            infecteds += continents[i].infecteds;
            deads += continents[i].deaths;
        }

        infectedText.text = "Infected: " + infecteds.ToString("N0");
        deathText.text = "Deads: " + deads.ToString("N0");

    }

    public void PlayAudio(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    void CheckWinCondition()
    {
        for (int i = 0; i < continents.Length; i++)
        {
            if (continents[i].infected)
            {
                if (continents[i].deaths < continents[i].totalPopulation)
                    return;
            }
            else
            {
                return;
            }
        }

        GameOver("Sua doença dizimou a humanidade em " + timer.ToString("0") + " dias");

    }

    void GameOver(string text)
    {
        started = false;
        finalMessage.text = text;
        finalMessage.transform.parent.gameObject.SetActive(true);
    }

    public void StartGame(int index)
    {
        started = true;
        continents[index].infected = true;
        continents[index].animator.SetTrigger("Infected");
        

        for (int i = 0; i < continentButtons.Length; i++)
        {
            continentButtons[i].enabled = false;
        }
        Invoke("SpawnDnaPoint", Random.Range(20f, 30f));
    }

    void SpawnDnaPoint()
    {
        if (!started)
            return;

        GameObject newDnaButton = Instantiate(dnaButton, canvas);
        newDnaButton.transform.localPosition = new Vector3(Random.Range(-400f, 400f), Random.Range(-150f, 230f));
        newDnaButton.transform.SetSiblingIndex(1);

        Invoke("InfectOtherContinent", 0.5f);
        Invoke("SpawnDnaPoint", Random.Range(20f, 30f));
    }

    void InfectOtherContinent()
    {
        List<Continent> notInfecteds = new List<Continent>();
        for (int i = 0; i < continents.Length; i++)
        {
            if (!continents[i].infected)
            {
                notInfecteds.Add(continents[i]);
            }
        }

        if(notInfecteds.Count < 1)
        {
            return;
        }

        float newInfectionRate = infectionRate / 100;

        float randomValue = Random.value;

        Debug.Log(randomValue);

        if(randomValue < newInfectionRate)
        {
            cureRate += 3;
            int randomContinent = Random.Range(0, notInfecteds.Count);
            notInfecteds[randomContinent].infected = true;
            notInfecteds[randomContinent].animator.SetTrigger("Infected");
            GameObject newDnaButton = Instantiate(dnaButton, canvas);
            newDnaButton.transform.localPosition = new Vector3(Random.Range(-400f, 400f), Random.Range(-150f, 230f));
            newDnaButton.transform.SetSiblingIndex(1);
        }
    }

    public void SetSkillInfo(UnlockButton unlockButton)
    {
        myUnlockButton = unlockButton;
        skillNameText.text = unlockButton.skillName;
        descriptionText.text = unlockButton.description + "\nCusto: " + unlockButton.cost + " DNA Points";
        infoPanel.SetActive(true);
        PlayAudio(clickSound);

    }

    public void UnlockSkill()
    {
        if (myUnlockButton.cost > dnaPoints)
            return;

        dnaPoints -= myUnlockButton.cost;
        dnaPointsText.text = "DNA Points: " + dnaPoints;
        myUnlockButton.GetComponent<Button>().interactable = false;
        if(myUnlockButton.unlockButton != null)
        {
            myUnlockButton.unlockButton.SetActive(true);
        }

        transmissionPerMinute += myUnlockButton.transmission;
        deathRate += myUnlockButton.death;
        infectionRate += myUnlockButton.infection;
        cureRate += myUnlockButton.cure;

        if(cureRate <= 0)
        {
            cureRate = 0;
        }

        infoPanel.SetActive(false);
    }

    public void GetDnaPoints()
    {
        dnaPoints++;
        dnaPointsText.text = "DNA Points: " + dnaPoints;
        PlayAudio(dnaSFX);
    }
}
