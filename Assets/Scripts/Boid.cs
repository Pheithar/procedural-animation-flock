using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    // Serialized Fields
    // -----------------
    [SerializeField] private float _avoidFactor = 1f;
    public float AvoidFactor {
        get { return _avoidFactor; }
        set { _avoidFactor = value; }
    }

    [SerializeField] private float _matchingFactor = 1f;
    public float MatchingFactor {
        get { return _matchingFactor; }
        set { _matchingFactor = value; }
    }

    [SerializeField] private float _centeringFactor = 1f;
    public float CenteringFactor {
        get { return _centeringFactor; }
        set { _centeringFactor = value; }
    }

    [SerializeField] private float _maxSpeed = 5f;
    public float MaxSpeed {
        get { return _maxSpeed; }
        set { _maxSpeed = value; }
    }

    [SerializeField] private float _protectedRange;
    public float ProtectedRange {
        get { return _protectedRange; }
        set { _protectedRange = value; }
    }

    [SerializeField] private float _visibleRange;
    public float VisibleRange {
        get { return _visibleRange; }
        set { _visibleRange = value; }
    }

    [SerializeField] private Rigidbody _rb;
    public Rigidbody Rb {
        get { return _rb; }
        set { _rb = value; }
    }

    [SerializeField] private bool _drawGizmos = true;
    public bool DrawGizmos {
        get { return _drawGizmos; }
        set { _drawGizmos = value; }
    }
    
    // Variables
    // ---------
    private List<Boid> _boidsInProtectedRange = new List<Boid>();
    private List<Boid> BoidsInProtectedRange {
        get { return _boidsInProtectedRange; }
        set { _boidsInProtectedRange = value; }
    }

    private List<Boid> _boidsInVisibleRange = new List<Boid>();
    private List<Boid> BoidsInVisibleRange {
        get { return _boidsInVisibleRange; }
        set { _boidsInVisibleRange = value; }
    }

    // Public Functions
    // ----------------

    // Private Functions
    // -----------------

    /*
    Updates the list of visible and protected boids.

    Args:
    -----

    Returns:
    --------
        void
    */
    private void DetectBoids() {
        BoidsInProtectedRange.Clear();
        BoidsInVisibleRange.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, VisibleRange);
        foreach (Collider collider in colliders) {
            Boid boid = collider.GetComponent<Boid>();
            if (boid != null && boid != this) {
                float distance = Vector3.Distance(transform.position, boid.transform.position);
                if (distance < ProtectedRange) {
                    BoidsInProtectedRange.Add(boid);
                } else if (distance < VisibleRange) {
                    BoidsInVisibleRange.Add(boid);
                }
            }
        }
    }

    /*
    Steers the boid toward the other boids that are inside the VisibleRange.

    Args:
    -----

    Returns:
    --------
        Vector3: The velocity vector of the boid
    */
    private Vector3 Cohesion() {
        Vector3 centerMass = Vector3.zero;
        foreach (Boid boid in BoidsInVisibleRange) {
            centerMass += boid.transform.position;
        }

        if (BoidsInVisibleRange.Count == 0) {
            return Vector3.zero;
        }

        centerMass /= BoidsInVisibleRange.Count;
        return centerMass - transform.position;
    }

    /* 
    Aligns the boids to have similar velocities.

    Args:
    -----

    Returns:
    --------
        Vector3: The velocity vector of the boid
    */
    private Vector3 Alignment() {
        Vector3 velocity = Vector3.zero;
        foreach (Boid boid in BoidsInVisibleRange) {
            velocity += boid.Rb.velocity;
        }

        if (BoidsInVisibleRange.Count == 0) {
            return Vector3.zero;
        }

        velocity /= BoidsInVisibleRange.Count;
        return velocity - Rb.velocity;
    }

    /* 
    Separates the boids that are too close to each other.

    Args:
    -----

    Returns:
    --------
        Vector3: The velocity vector of the boid
    */
    private Vector3 Separation() {
        Vector3 avoidVector = Vector3.zero;
        foreach (Boid boid in BoidsInProtectedRange) {
            avoidVector += transform.position - boid.transform.position;
        }
        return avoidVector;
    }

    /*
    Fixed update function that updates the boid velocity.
    The object will always face the movement direction.

    Args:
    -----

    Returns:
    --------
        void
    */
    private void FixedUpdate() {
        DetectBoids();

        Vector3 cohesionVector = Cohesion() * CenteringFactor;
        Vector3 alignmentVector = Alignment() * MatchingFactor;
        Vector3 avoidVector = Separation() * AvoidFactor;

        Vector3 velocity = cohesionVector + alignmentVector + avoidVector;

        float speed = 

        Rb.velocity = new Vector3(velocity.x, Rb.velocity.y, velocity.z);

        if (Mathf.Abs(Rb.velocity.x) > 0.1f || Mathf.Abs(Rb.velocity.z) > 0.1f) {
            Vector3 lookDirection = new Vector3(Rb.velocity.x, 0, Rb.velocity.z);
            transform.rotation = Quaternion.LookRotation(lookDirection, transform.forward);
        }

        // // Apply additional gravity
        // Rb.AddForce(Physics.gravity*AdditionalGravity, ForceMode.Acceleration);
        
    }
    
    /*
    Function to draw gizmos in the scene.
    Args:
    -----

    Returns:
    --------
        void
    */
    private void OnDrawGizmos() {
        if (DrawGizmos) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ProtectedRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, VisibleRange);
        }

    }
}
