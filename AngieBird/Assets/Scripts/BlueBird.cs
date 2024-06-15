using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlueBird : AngieBird
{
    private bool _originalInstance = true;
    [SerializeField] private BlueBird _blueBirdPrefab;

    new protected void Start()
    {
        base.Start();

        if( !_originalInstance )
        {
            _rb.isKinematic = false;
            _circleCollider.enabled = true;

            _hasBeenLaunched = true;
            _shouldFaceVelocityDirection = true;
        }
    }

    new protected void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && _originalInstance && _hasBeenLaunched)
        {
            _originalInstance = false;

            BlueBird blueBird_upper = Instantiate<BlueBird>(this.gameObject.GetComponent<BlueBird>());
            BlueBird blueBird_lower = Instantiate<BlueBird>(this.gameObject.GetComponent<BlueBird>());

            List<BlueBird> bird_list = new List<BlueBird>();
            bird_list.Add(blueBird_lower);
            bird_list.Add(blueBird_upper);

            int modifier = 0;
            foreach (BlueBird bird in bird_list)
            {
                bird._originalInstance = _originalInstance;
                bird._hasBeenLaunched = _hasBeenLaunched;
                bird._shouldFaceVelocityDirection = _shouldFaceVelocityDirection;
                bird._deleting = _deleting;

                Rigidbody2D rb = bird.GetComponent<Rigidbody2D>();
                Rigidbody2D rb_m = this.GetComponent<Rigidbody2D>();

                rb.velocity = rb_m.velocity;
                
                bird.transform.position = rb_m.position;
                bird.transform.rotation = rb_m.transform.rotation;


                float angleMod = 0f;
                if( modifier == 0 )
                {
                    angleMod = -10;
                }
                if( modifier == 1 )
                {
                    angleMod = 10;
                }

                bird._rb.isKinematic = false;
                bird._circleCollider.enabled = true;

                bird._hasBeenLaunched = true;
                bird._shouldFaceVelocityDirection = true;

                //rb.rotation = Quaternion.LookRotation(rigidbody.velocity, Vector3.up);
                //rb.velocity = Quaternion.Euler(0, transform.eulerAngles.y - angleMod, 0) * rb.velocity;
                rb.velocity = Quaternion.Euler(0f, 0f, angleMod) * rb.velocity;


                modifier++;
            }

        }       

        base.Update();
    }

    /*
    new protected void OnCollisionEnter2D(Collision2D collision)
    {
        if(Mouse.current.leftButton.wasPressedThisFrame && _originalInstance)
        {
            Debug.Log("Split");
        }

        base.OnCollisionEnter2D(collision);
    }
    */
}
