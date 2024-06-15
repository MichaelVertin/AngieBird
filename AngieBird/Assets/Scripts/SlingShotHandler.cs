using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SlingShotHandler : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer _leftLineRender;
    [SerializeField] private LineRenderer _rightLineRender;

    [Header("Transform Renderers")]
    [SerializeField] private Transform _leftStartPosition;
    [SerializeField] private Transform _rightStartPosition;
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _idlePosition;
    [SerializeField] private Transform _elasticTransform;

    [Header("Slingshot Stats")]
    [SerializeField] private float _maxDistance = 3.5f;
    [SerializeField] private float _shotForce = 9f;
    [SerializeField] private float _timeBetweenBirdSpawns = 2f;
    [SerializeField] private float _elasicDivider = 1.2f;
    [SerializeField] private AnimationCurve _elastiveCurve;
    [SerializeField] private float _maxAnimationTime = 1f;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea _slingShotArea;
    [SerializeField] private CameraManager _cameraManager;

    [Header("Sounds")]
    [SerializeField] private AudioClip[] _elasticPullClips;
    [SerializeField] private AudioClip _elasticReleaseClip;


    [Header("Bird")]
    [SerializeField] private AngieBird angieBirdPrefab;
    [SerializeField] private AngieBird blueBirdPrefab;
    [SerializeField] private float _angieBirdPositionOffset = .275f;

    private Vector2 _slingShotLinesPosition;

    private Vector2 _direction;
    private Vector2 _directionNormalized;

    private bool _clickedWithinArea;
    private bool _birdOnSlingShot;

    private AngieBird _spawnedAngieBird;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _leftLineRender.enabled = false;
        _rightLineRender.enabled = false;

        SpawnAngieBird();
    }

    private void Update()
    {
        if( Mouse.current.leftButton.wasPressedThisFrame && _slingShotArea.isWithinSlingShotArea())
        {
            _clickedWithinArea = true;

            if( _birdOnSlingShot )
            {
                SoundManager.instance.PlayRandomClip(_elasticPullClips, _audioSource);
                _cameraManager.SwitchToFollowCam(_spawnedAngieBird.transform);
            }
        }

        if( Mouse.current.leftButton.isPressed && _clickedWithinArea && _birdOnSlingShot)
        {
            DrawSlingShot();
            PositionAndRotateAngieBird();
        }


        if (Mouse.current.leftButton.wasReleasedThisFrame && _birdOnSlingShot && _clickedWithinArea)
        {
            if( GameManager.instance.HasEnoughShots() )
            {
                _clickedWithinArea = false;
                _birdOnSlingShot = false;

                _spawnedAngieBird.LaunchBird(_direction, _shotForce);

                SoundManager.instance.PlayClip(_elasticReleaseClip, _audioSource);
                GameManager.instance.UseShot();
                AnimateSlingShot();

                if( GameManager.instance.HasEnoughShots() )
                {
                    StartCoroutine(nameof(SpawnAngieBirdAfterTime));
                }
            }
        }
    }

    #region Slingshot Methods
    private void DrawSlingShot()
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        _slingShotLinesPosition = _centerPosition.position + Vector3.ClampMagnitude(touchPosition - _centerPosition.position, _maxDistance);

        SetLines(_slingShotLinesPosition);

        _direction = (Vector2)_centerPosition.position - _slingShotLinesPosition;
        _directionNormalized = _direction.normalized;
    }

    private void SetLines(Vector2 position)
    {
        if (!_leftLineRender.enabled && !_rightLineRender.enabled)
        {
            _leftLineRender.enabled = true;
            _rightLineRender.enabled = true;
        }

        _leftLineRender.SetPosition(0, position);
        _leftLineRender.SetPosition(1, _leftStartPosition.position);

        _rightLineRender.SetPosition(0, position);
        _rightLineRender.SetPosition(1, _rightStartPosition.position);


    }

    #endregion

    #region Angie Bird Methods

    private void SpawnAngieBird()
    {
        _elasticTransform.DOComplete();
        SetLines(_idlePosition.position);

        Vector2 dir = (_centerPosition.position - _idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)_idlePosition.position + dir * _angieBirdPositionOffset;

        //_spawnedAngieBird = Instantiate(angieBirdPrefab, _idlePosition.position, Quaternion.identity);
        _spawnedAngieBird = Instantiate(blueBirdPrefab, _idlePosition.position, Quaternion.identity);
        _spawnedAngieBird.transform.right = dir;

        _birdOnSlingShot = true;
    }

    private void PositionAndRotateAngieBird()
    {
        _spawnedAngieBird.transform.position = _slingShotLinesPosition + _directionNormalized * _angieBirdPositionOffset;
        _spawnedAngieBird.transform.right = _directionNormalized;
    }

    private IEnumerator SpawnAngieBirdAfterTime()
    {
        yield return new WaitForSeconds(_timeBetweenBirdSpawns);

        SpawnAngieBird();

        _cameraManager.SwitchToIdleCam();
    }

    #endregion

    #region Animate SlingShot

    private void AnimateSlingShot()
    {
        _elasticTransform.position = _leftLineRender.GetPosition(0);

        float dist = Vector2.Distance(_elasticTransform.position, _centerPosition.position);

        float time = dist / _elasicDivider;

        _elasticTransform.DOMove(_centerPosition.position, time).SetEase(_elastiveCurve);
        StartCoroutine(AnimateSlingShotLines(_elasticTransform, time));

    }

    private IEnumerator AnimateSlingShotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;

        while( elapsedTime < time && elapsedTime < _maxAnimationTime)
        {
            elapsedTime += Time.deltaTime;

            SetLines(trans.position);

            yield return null;
        }
    }
    #endregion
}
