using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AngieBird : MonoBehaviour
{
    [SerializeField] private AudioClip _hitClip;
    [SerializeField] private float _timeToDelete = 5f;

    protected Rigidbody2D _rb;
    protected CircleCollider2D _circleCollider;

    protected bool _hasBeenLaunched;
    protected bool _shouldFaceVelocityDirection;

    private AudioSource _audioSource;
    protected bool _deleting;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    protected void Start()
    {
        _rb.isKinematic = true;
        _circleCollider.enabled = false;
    }

    private void FixedUpdate()
    {
        if( _hasBeenLaunched && _shouldFaceVelocityDirection )
        {
           transform.right = _rb.velocity;
        }
    }

    public void LaunchBird(Vector2 direction, float force)
    {
        _rb.isKinematic = false;
        _circleCollider.enabled = true;

        _rb.AddForce(direction * force, ForceMode2D.Impulse);

        _hasBeenLaunched = true;
        _shouldFaceVelocityDirection = true;
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        _shouldFaceVelocityDirection = false;
        SoundManager.instance.PlayClip(_hitClip, _audioSource);
        if( !_deleting )
        {
            StartCoroutine(_DestroyAtTime(_timeToDelete));
            _deleting = true;
        }
    }

    private IEnumerator _DestroyAtTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }

    protected void Update()
    {
        
    }
}
