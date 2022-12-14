using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float _speed = 1.75f;
    [SerializeField]
    private float _speedMultiplier = 2;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private int _shieldStrength;
    SpriteRenderer _shieldColor;
    private bool _isShieldsActive = false;
    
    [SerializeField]
    private GameObject _shieldVisualizer;
   
    
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;

    [SerializeField]
    private int _score;

    private UIManager _uiManager;
    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioSource _audioSource;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");

            if (_audioSource == null)
            {
                Debug.LogError("AudioSource on the Player is NULL.");
            }
            else
            {
                _audioSource.clip = _laserSoundClip;
            }

        }
        _shieldColor = _shieldVisualizer.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLazer();
        }
       
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);
        transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if(transform.position.x > 11)
        {
            transform.position = new Vector3(-11, transform.position.y, 0);
        }
        else if (transform.position.x < -11)
        {
            transform.position = new Vector3(11, transform.position.y, 0);
        }
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(Vector3.right * horizontalInput * (_speed * 1.5f) * 1.8f * Time.deltaTime);
            transform.Translate(Vector3.up * verticalInput * (_speed * 1.5f) * 1.8f * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);
            transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);
        }
    }

    void FireLazer()
    {
        _canFire = Time.time + _fireRate;
        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }
        _audioSource.Play();
    }
   


    public void Damage()
    {
        if(_isShieldsActive == true)
        {
            _shieldStrength--;
            if (_shieldStrength == 0)
            {
                _isShieldsActive = false;
                _shieldVisualizer.SetActive(false);
                return;
            }
            else if (_shieldStrength == 1)
            {
                _shieldColor.color = Color.red;
                return;
            }
            else if (_shieldStrength == 2)
            {
                _shieldColor.color = Color.green;
                return;
            }
           
        }
        _lives--;       
        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }
    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }
    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }
    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }
    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
    }
    public void ShieldsActive()
    {
        
        _isShieldsActive = true;
        _shieldVisualizer.SetActive(true);

        if (_shieldStrength < 3)
        {
            _shieldStrength ++;
            _isShieldsActive = true;
            _shieldVisualizer.SetActive(true);
        }
        if (_shieldStrength == 1)
        {
            _shieldColor.color = Color.red;
        }
        else if (_shieldStrength == 2)
        {
            _shieldColor.color = Color.green;
        }
        else if (_shieldStrength ==3)
        {
            _shieldColor.color = Color.cyan;
        }                
    }
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
 

   
}









  