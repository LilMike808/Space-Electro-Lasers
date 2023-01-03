using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //handle to text
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Image _LivesImg;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private GameManager _gameManager;
    
    //Slider AND percentage values to be assigned in the inspector
    [SerializeField]   
    private Slider _thrusterSlider;
    [SerializeField]
    private TMPro.TextMeshProUGUI _fuelPercentageText;
       
    [SerializeField]
    private Sprite[] _liveSprites;
    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score : 0";
        _ammoText.text = "Ammo: 15/15";
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is NULL");
        }

    }

    public void UpdateAmmo(int ammocount)

    {
        _ammoText.text = "Ammo: " + ammocount + "/15"; 
    }

   public void UpdateScore(int PlayerScore)
    {
        _scoreText.text = "Score:" + PlayerScore.ToString();
        
    }
    public void UpdateLives(int currentLives)
    {
        _LivesImg.sprite = _liveSprites[currentLives];

        if(currentLives == 0)
        {
            GameOverSequence();                   
        }
    }
    public void UpdateThruster (float fuelpercentage)
    {
        _thrusterSlider.value = fuelpercentage;
        _fuelPercentageText.text = Mathf.RoundToInt(fuelpercentage) + "%";
    }
    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());

    }
    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
