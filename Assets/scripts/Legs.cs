using UnityEngine;
using System.Collections;

public class Legs : MonoBehaviour {

    public bool IsGrounded
    {
        get { return _isGrounded; }
    }

	// Use this for initialization
	void Start () {
        _animator = GetComponentInParent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
	    _rigidBody = GetComponentInParent<Rigidbody2D>(); // For velocity of legs.

	}

    // Update is called once per frame
    void Update () {
        _animator.SetBool(Bird.AnimParams.Grounded, _isGrounded);
        _animator.SetFloat(Bird.AnimParams.HorzSpeed, Mathf.Abs(_rigidBody.velocity.x)); // Only works when animator is enabled.

	    _renderer.enabled = _isGrounded;
	}


    private bool _isGrounded = false;
    private Animator _animator;
    private SpriteRenderer _renderer;
    private Rigidbody2D _rigidBody;

    bool IsCloseTo(float y1, float y2, float epsilon)
    {
        return Mathf.Abs(y1 - y2) < epsilon;
    }


    //make sure u replace "floor" with your gameobject name.on which player is standing
    void OnCollisionEnter2D(Collision2D theCollision)
    {
        if (theCollision.gameObject.name.Contains("Platform"))
        {
            var pointOfContact = theCollision.contacts[0].normal; //Grab the normal of the contact point we touched

            //Detect which side of the collider we touched
#if OLD_WAY
            if (pointOfContact == new Vector2(-1, 0))
            {
                Debug.Log("We touched the left side of the platform!");
            }

            if (pointOfContact == new Vector2(1, 0))
            {
                Debug.Log("We touched the right side of the platform!");
            }

            if (pointOfContact == new Vector2(0, -1))
            {
                Debug.Log("We touched the platforms's bottom!");
            }
#endif
            if ( IsCloseTo(pointOfContact.x,0.0f,.01f) && IsCloseTo(pointOfContact.y, 1.0f,.01f))
            {
                //Debug.Log("We touched the top of the platform!");
                _isGrounded = true;
            }

        }
    }

    //consider when character is jumping .. it will exit collision.
    void OnCollisionExit2D(Collision2D theCollision)
    {
        if (theCollision.gameObject.name.Contains("Platform"))
        {
            _isGrounded = false;
        }
    }

}
