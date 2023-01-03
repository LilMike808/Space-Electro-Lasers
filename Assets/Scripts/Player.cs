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
    private int _ammoCount = 15;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    
    //left shift thruster variables
    [SerializeField]
    private GameObject _thruster;
    [SerializeField]
    private float _fuelPercentage = 100f;
    [SerializeField]
    private float _refuelSpeed;
    private bool _isThrusterActive;

    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    
    private bool _isRarePowerupActive = false;
    [SerializeField]
    private GameObject _missilePrefab;
    [SerializeField]
    private int _missileCount = 3;
    [SerializeField]
    private bool _isHomingMissileActive = false;

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
    private AudioClip _noAmmoAudio;
    [SerializeField]
    private AudioSource _audioSource;
    //Camera shake reference
    private CameraShake _camShake;
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

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        _camShake = Camera.main.GetComponent<CameraShake>();

        if(_camShake is null)
        {
            Debug.LogError("Shake Camera script is NULL");
        }
    }

    void Update()
    {
        CalculateMovement();
        if (_isRarePowerupActive != true)
        {
            if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            {
                if (_ammoCount == 0)
                {
                    AudioSource.PlayClipAtPoint(_noAmmoAudio, transform.position);
                    return;
                }
                FireLazer();
            }
        }
        HomingMissileFire();
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

        if (Input.GetKey(KeyCode.LeftShift) && _fuelPercentage > 0)
        {
            if (_isSpeedBoostActive)
            {
                StopCoroutine(ActivateRefuel());
                ActivateThruster();
            }
            else
            {
                StopCoroutine(ActivateRefuel());
                ActivateThruster();
                transform.Translate(Vector3.right * horizontalInput * (_speed * 1.5f) * 1.8f * Time.deltaTime);
                transform.Translate(Vector3.up * verticalInput * (_speed * 1.5f) * 1.8f * Time.deltaTime);

            }
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isThrusterActive = false;
            if (_isSpeedBoostActive)
            {
                _thruster.SetActive(false);
                StartCoroutine(ActivateRefuel());
            }
            else
            {
                _thruster.SetActive(false);
                StartCoroutine(ActivateRefuel());
                transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);
                transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);
            }
        }
    }
    void ActivateThruster()
    {
        _isThrusterActive = true;
        if(_fuelPercentage > 0)
        {
            _thruster.SetActive(true);
            _fuelPercentage -= 15 * 2 * Time.deltaTime;
            _uiManager.UpdateThruster(_fuelPercentage);
        }
        else if(_fuelPercentage <= 0)
        {
            _thruster.SetActive(false);
            _fuelPercentage = 0.0f;
            _uiManager.UpdateThruster(_fuelPercentage);
        }
    }
    void HomingMissileFire()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isHomingMissileActive == true)
        {
            Instantiate(_missilePrefab, transform.position, Quaternion.identity);
            _missileCount = _missileCount - 1;

            if (_missileCount == 0)
            {
                _isRarePowerupActive = false;
                _isHomingMissileActive = false;
            }
        }
    }
    void FireLazer()
    {
        if(_ammoCount <= 15)
        {
            if (_isTripleShotActive == true)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
                _ammoCount = _ammoCount - 3;
                _uiManager.UpdateAmmo(_ammoCount);
            }
            else
            {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
                _ammoCount = _ammoCount - 1;
                _uiManager.UpdateAmmo(_ammoCount);
            }
            AmmoCap();
        }
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

    void AmmoCap()
    {
        if(_ammoCount <= 0)
        {
            _ammoCount = 0;
        }
        else if(_ammoCount >= 15)
        {
            _ammoCount = 15;
        }
    }
    public void HomingMissileActive()
    {
        _isRarePowerupActive = true;
        _isHomingMissileActive = true;
        _missileCount = 3;
    }
    
    public void AmmoCollected()
    {
        _ammoCount = 15;
        AmmoCap();
        _uiManager.UpdateAmmo(_ammoCount);
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

        if (_lives >= 0)
        {
            _uiManager.UpdateLives(_lives);
        }
        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        } 
        
        StartCoroutine(_camShake.ShakeCamera());
    }
    public void HealthCollected()
    {
        _lives = _lives + 1;

        if(_lives >= 3)
        {
            _lives = 3;
            _leftEngine.SetActive(false);          
        }
        else if(_lives == 2)
        {
            _rightEngine.SetActive(false);
        }
        _uiManager.UpdateLives(_lives);
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
    IEnumerator ActivateRefuel()
    {
        while(_fuelPercentage != 100 && _isThrusterActive == false)
        {
            yield return new WaitForSeconds(1f);
            _fuelPercentage += 30 * _refuelSpeed * Time.deltaTime;
            _uiManager.UpdateThruster(_fuelPercentage);
            
            if(_fuelPercentage >= 100)
            {
                _fuelPercentage = 100;               
                _uiManager.UpdateThruster(_fuelPercentage);
                break;
            }
            else if(_fuelPercentage <= 0)
            {
                _fuelPercentage = 0;
                _uiManager.UpdateThruster(_fuelPercentage);
                break;
            }            
        }
    }
}









  