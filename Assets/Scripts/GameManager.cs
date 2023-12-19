using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    private Mng_SpawnEnemies spawnManager;
    public int currentLevel = 1;

    public bool rewind = false;
    public bool ignoreInputs = false;
    public bool pause = false;
    public GameObject pausePanel;

    public bool isLost = false;
    public GameObject gameOverPanel;
    
    private bool isTogglingPause = false;
    
    public Volume volume;
    
    private ColorAdjustments colorAdjustments;
    private FilmGrain filmGrain;

    public Camera camara1;
    public float shakeIntensity = 2f;
    public float shakeDuration = 0.25f;
    
    public TextMeshProUGUI textoCentro;

    public int rewindsLeft = 3;
    public GameObject rewind1;
    public GameObject rewind2;
    public GameObject rewind3;
    public int rewindCharge = 0;
    public List<GameObject> rewindImages = new List<GameObject>();

    public Button RestartButton;
    public TextMeshProUGUI nivelActual;
    public TextMeshProUGUI nivelMaximo;
    public TextMeshProUGUI nivelAlcanzado;
    private static int maxNivelAlcanzado = 0;


    
    private void Awake()
    {
        spawnManager = FindObjectOfType<Mng_SpawnEnemies>();
        volume = FindObjectOfType<Volume>();
        camara1 = Camera.main;

        UpdateRewindImages();
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        GetVolumeData();
        
        RestartButton.onClick.AddListener(RestartGame); 
    }

    private void Start()
    {
        ShowText("GAME START");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Start")) //pause
        {
            if (pause)
            {
                UnPause();
            }
            else if (!isLost)
            {
                Pause();
            }
        }
    }

    public void ResetText()
    {
        textoCentro.color = new Color(textoCentro.color.r, textoCentro.color.g, textoCentro.color.b, 1);
        textoCentro.transform.localScale = new Vector3(1, 1, 1);
    }

    public void ShowText(string t)
    {
        SetText(t);
        StartCoroutine(FadeOutRoutine());
    }

    public void GetVolumeData()
    {
        if(volume.profile.TryGet<ColorAdjustments>(out colorAdjustments) &&
           volume.profile.TryGet<FilmGrain>(out filmGrain))
        {
            colorAdjustments.saturation.value = 0;
            filmGrain.intensity.value = 0.4f;
        }
    }
    
    public void SetText(string t)  {textoCentro.text = t;}
    
    private void ApplyShake()
    {
        float shakeIntensity = currentLevel > 11 ? this.shakeIntensity * 2 : this.shakeIntensity;
        textoCentro.transform.localPosition = new Vector3(Random.Range(-shakeIntensity, shakeIntensity), Random.Range(-shakeIntensity, shakeIntensity), 0);
    }
    public void CameraShake(float multiplier)
    {
        StartCoroutine(ShakeCamera(multiplier));
    }

    public void StartRewind()
    {
        if (rewindsLeft <= 0)
        {
            GameOver();
        }
        else
        {
            rewindsLeft--;
            checkRewinds();
            colorAdjustments.saturation.value = -100;
            filmGrain.intensity.value = 1;
            ignoreInputs = true;
            rewind = true;
            spawnManager.RewindLevel();
            UpdateRewindImages();
            
        }
    }

    public void StopRewind()
    {
        colorAdjustments.saturation.value = 0;
        filmGrain.intensity.value = 0.4f;
        ignoreInputs = false;
        rewind = false;
    }

    public void Pause()
    {
        spawnManager.Pause();
        pausePanel.SetActive(true);
        UpdateUI(); 
        colorAdjustments.saturation.value = -100;
        ignoreInputs = true;
        pause = true;
    }

    public void UnPause()
    {
        spawnManager.UnPause();
        pausePanel.SetActive(false);
        colorAdjustments.saturation.value = 0;
        ignoreInputs = false;
        pause = false;
    }

    public void TogglePause()
    {
        if (isTogglingPause) return; // Ignorar entradas adicionales

        isTogglingPause = true;
        if (pause)
        {
            UnPause();
        }
        else
        {
            Pause();
        }

        StartCoroutine(ResetPauseToggle());
    }
    private IEnumerator FadeOutRoutine()
    {
        float duration = 1.0f; 
        float currentTime = 0f;
        Color[] colors = new Color[]
        {
            Color.red, Color.magenta, Color.yellow, Color.red, Color.yellow, Color.white, 
            Color.red, Color.magenta, Color.yellow, Color.red, Color.yellow, Color.white,
            Color.red, Color.magenta, Color.yellow, Color.red, Color.yellow, Color.white,
            Color.red, Color.magenta, Color.yellow, Color.red, Color.yellow, Color.white,
            Color.red, Color.magenta, Color.yellow, Color.red, Color.yellow, Color.white,
        };

        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(1, 0, currentTime / duration);
            textoCentro.color = new Color(1,1, 1, alpha);
            textoCentro.transform.localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(1.2f, 1.2f, 1.2f), currentTime / duration);
            
            if (currentLevel > 2)
            {
                ApplyShake();
            }

            if (currentLevel > 6)
            {
                textoCentro.color = colors[(int)(currentTime / duration * colors.Length)] * new Color(1, 1, 1, alpha);
            }

            currentTime += Time.deltaTime;
            yield return null;
        }

        textoCentro.color = new Color(0, 0, 0, 0);
        textoCentro.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        textoCentro.transform.localPosition = Vector3.zero; //Resetear posición al centro
    }
    private IEnumerator ShakeCamera(float multiplier)
    {
        float elapsed = 0.0f;

        Vector3 originalCamPos = camara1.transform.position;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity * multiplier;
            float y = Random.Range(-1f, 1f) * shakeIntensity * multiplier;

            camara1.transform.position = new Vector3(originalCamPos.x + x, originalCamPos.y + y, originalCamPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }
        camara1.transform.position = originalCamPos;
    }
    private IEnumerator ResetPauseToggle()
    {
        yield return new WaitForSeconds(0.1f); // Un breve retardo
        isTogglingPause = false;
    }


    public void checkRewinds()
    {
        if (rewindsLeft > 3) { rewindsLeft = 3; }
        
        if (rewindsLeft == 3)
        {
            rewind1.SetActive(true);
            rewind2.SetActive(true);
            rewind3.SetActive(true);
        }
        else if (rewindsLeft == 2)
        {
            rewind1.SetActive(false);
            rewind2.SetActive(true);
            rewind3.SetActive(true);
        }

        else if (rewindsLeft == 1)
        {
            rewind1.SetActive(false);
            rewind2.SetActive(false);
            rewind3.SetActive(true);
        }

        else if (rewindsLeft <= 0)
        {
            rewind1.SetActive(false);
            rewind2.SetActive(false);
            rewind3.SetActive(false);
        }
    }

    public void chargeRewind()
    {
        if(rewindsLeft < 3)
        {
            if (rewindCharge >= 12)
            {
                rewindsLeft++;
                rewindCharge = 0;
                checkRewinds();
            }
            else
            {
                rewindCharge++;
            }

            UpdateRewindImages();
        }
    }

    public void GameOver()
    {
        isLost = true;
        gameOverPanel.SetActive(true);
        UpdateUI();
        colorAdjustments.saturation.value = -100;
        ignoreInputs = true;
        pause = true;
        spawnManager.Pause();
    }
    
    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void UpdateRewindImages()
    {
        if (rewindsLeft == 3)
        {
            foreach (var image in rewindImages)
            {
                image.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < rewindImages.Count; i++)
            {
                rewindImages[i].SetActive(i < rewindCharge);
            }
        }
    }
    
    private void UpdateUI()
    {
        nivelActual.text = currentLevel.ToString();
        nivelAlcanzado.text = currentLevel.ToString();

        if (currentLevel > maxNivelAlcanzado)
        {
            maxNivelAlcanzado = currentLevel;
        }
        
        nivelMaximo.text = maxNivelAlcanzado.ToString();
    }


}
