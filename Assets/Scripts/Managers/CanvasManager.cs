using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class CanvasManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button startButton;
    public Button settingsButton;
    public Button backButton;
    public Button quitButton;
    public Button returnToMenuButton;
    public Button returnToGameButton;

    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject pauseMenu;

    [Header("Text")]
    public Text livesText;
    public Text volSliderText;

    [Header("Slider")]
    public Slider volSlider;
    public AudioMixer mixer;

    [Header("UI")]
    public HorizontalLayoutGroup healthBar;
    public Image hp;

    public void StartGame()
    {
        PlayButtonFX();
        SceneManager.LoadScene("Level");
    }

    public void ShowMainMenu()
    {
        PlayButtonFX();
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ShowSettingsMenu()
    {
        PlayButtonFX();
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    void OnSliderValueChanged(float value)
    {
        if (volSliderText)
            volSliderText.text = value.ToString();
        
        if (mixer)
            mixer.SetFloat("MasterVol", value);
    }

    void OnLifeValueChange()
    {
        if (livesText)
            livesText.text = GameManager.instance.lives.ToString();

        if (healthBar)
        {
            for (int i = 0; i < healthBar.transform.childCount; i++)
                Destroy(healthBar.transform.GetChild(i).gameObject);

            for (int i = 0; i < GameManager.instance.lives; i++)
            {
                Instantiate(hp, healthBar.transform);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (startButton)
            startButton.onClick.AddListener(() => StartGame());

        if (settingsButton)
            settingsButton.onClick.AddListener(() => ShowSettingsMenu());

        if (backButton)
            backButton.onClick.AddListener(() => ShowMainMenu());

        if (volSlider)
        {
            volSlider.onValueChanged.AddListener((value) => OnSliderValueChanged(value));
            if (volSliderText)
                volSliderText.text = volSlider.value.ToString();
        }

        if (quitButton)
            quitButton.onClick.AddListener(() => QuitGame());

        if (returnToMenuButton)
            returnToMenuButton.onClick.AddListener(() => ReturnToMenu());

        if (returnToGameButton)
            returnToGameButton.onClick.AddListener(() => ReturnToGame());


        //Add Listener to Lives value change
        if (livesText)
            GameManager.instance.onLifeValueChanged.AddListener((value) => OnLifeValueChange());
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseMenu)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                pauseMenu.SetActive(!pauseMenu.activeSelf);
                GameManager.instance.sfxManager.Play(GameManager.instance.pauseSound, GameManager.instance.soundFXGroup);

                //HINT FOR THE LAB
                if (pauseMenu.activeSelf)
                {
                    //do something to pause
                    Time.timeScale = 0;
                    GameManager.instance.playerInstance.GetComponent<PlayerController>().enabled = false;
                    GameManager.instance.playerInstance.GetComponent<ShootProjectile>().enabled = false;
                }
                else
                {
                    Time.timeScale = 1;
                    GameManager.instance.playerInstance.GetComponent<PlayerController>().enabled = true;
                    GameManager.instance.playerInstance.GetComponent<ShootProjectile>().enabled = true;
                    //do something to unpause
                }
            }
        }
    }

    public void ReturnToGame()
    {
        PlayButtonFX();
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        GameManager.instance.playerInstance.GetComponent<PlayerController>().enabled = true;
        GameManager.instance.playerInstance.GetComponent<ShootProjectile>().enabled = true;
    }

    public void ReturnToMenu()
    {
        PlayButtonFX();
        SceneManager.LoadScene("Start");
        Time.timeScale = 1;
    }

    public void PlayButtonFX()
    {
        GameManager.instance.sfxManager.Play(GameManager.instance.buttonFX, GameManager.instance.soundFXGroup);
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
