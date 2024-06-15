using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int MaxNumberOfShots = 3;
    [SerializeField] private float _secondsToWaitBeforeDeathCheck = 3f;
    [SerializeField] private GameObject _restartScreenObject;
    [SerializeField] private SlingShotHandler _slingShotHandler;
    [SerializeField] private Image _levelButton;

    public int _usedNumberOfShots;

    private IconHandler _iconHandler;

    private List<Baddie> _baddies = new List<Baddie>();

    private void Awake()
    {
        if( instance == null)
        {
            instance = this;
        }

        _usedNumberOfShots = 0;

        _iconHandler = GameObject.FindObjectOfType<IconHandler>();

        Baddie[] baddies = FindObjectsOfType<Baddie>();

        foreach( Baddie baddie in baddies )
        {
            _baddies.Add(baddie);
        }

        _restartScreenObject.SetActive(false);
        _levelButton.enabled = false;
    }

    public void UseShot()
    {
        _iconHandler.UseShot(_usedNumberOfShots);
        _usedNumberOfShots++;

        CheckForLastShot();
    }

    public bool HasEnoughShots()
    {
        return _usedNumberOfShots < MaxNumberOfShots;
    }

    public void CheckForLastShot()
    {
        if( _usedNumberOfShots >= MaxNumberOfShots )
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    public IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(_secondsToWaitBeforeDeathCheck);

        if ( _baddies.Count == 0 )
        {
            WinGame();
        }
        else
        {
            RestartGame();
        }
    }

    public void RemoveBaddie(Baddie baddie)
    {
        _baddies.Remove(baddie);

        CheckForAllDeadBaddies();
    }

    private void CheckForAllDeadBaddies()
    {
        if( _baddies.Count == 0 )
        {
            WinGame();
        }
    }

    #region end game
    private void WinGame()
    {
        _restartScreenObject.SetActive(true);
        _slingShotHandler.enabled = false;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevels = SceneManager.sceneCountInBuildSettings;
        if( currentSceneIndex + 1 < maxLevels )
        {
            _levelButton.enabled = true;
        }
    }

    public void RestartGame()
    {
        DOTween.Clear(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    public void NextLevel()
    {
        DOTween.Clear(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
