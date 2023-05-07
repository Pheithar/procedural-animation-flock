using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualMovement : MonoBehaviour
{
    // Serialized Fields
    // -----------------
    [SerializeField] private float _speed = 5f;
    public float Speed {
        get { return _speed; }
        set { _speed = value; }
    }

    [SerializeField] private float _additionalGravity = 0f;
    public float AdditionalGravity {
        get { return _additionalGravity; }
        set { _additionalGravity = value; }
    }

    [SerializeField] private bool _drawGizmos = true;
    public bool DrawGizmos {
        get { return _drawGizmos; }
        set { _drawGizmos = value; }
    }

    [SerializeField] private Rigidbody _rb;
    public Rigidbody Rb {
        get { return _rb; }
        set { _rb = value; }
    }

    // Variables
    // ---------

    // Public Functions
    // ----------------

    // Private Functions
    // -----------------

    /*
    Fixed update function that controls user input. The user can move the object around by changing the velocity of the rigidbody.
    The object will always face the movement direction.

    Args:
    -----

    Returns:
    --------
        void
    */
    private void FixedUpdate() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Rb.velocity = new Vector3(horizontalInput * Speed, Rb.velocity.y, verticalInput * Speed);
        if (Mathf.Abs(Rb.velocity.x) > 0.1f || Mathf.Abs(Rb.velocity.z) > 0.1f) {
            Vector3 lookDirection = -1 * new Vector3(Rb.velocity.x, 0, Rb.velocity.z);
            transform.rotation = Quaternion.LookRotation(lookDirection, transform.up);
        }

        // Apply additional gravity
        Rb.AddForce(Physics.gravity*AdditionalGravity, ForceMode.Acceleration);
        
    }

    /*
    Function to draw gizmos in the scene. Set the vector to be drawn to the forward vector of the object

    Args:
    -----

    Returns:
    --------
        void
    */
    private void OnDrawGizmos() {
        if (DrawGizmos) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * 3);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * 3);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 3);
        }

    }
}
