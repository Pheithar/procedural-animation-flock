using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLeg : MonoBehaviour
{
    // Serialized Fields
    // -----------------
    [SerializeField] private float _stepDistance = 0.75f;
    public float StepDistance {
        get { return _stepDistance; }
        set { _stepDistance = value; }
    }
    [SerializeField] private float _stepHeight = 0.3f;
    public float StepHeight {
        get { return _stepHeight; }
        set { _stepHeight = value; }
    }
    [SerializeField] private float _stepSpeed = 5f;
    public float StepSpeed {
        get { return _stepSpeed; }
        set { _stepSpeed = value; }
    }

    [SerializeField] private ProceduralAnimator _body;
    public ProceduralAnimator Body {
        get { return _body; }
        set { _body = value; }
    }

    [SerializeField] private LayerMask _terrainLayerMask;
    public LayerMask TerrainLayerMask {
        get { return _terrainLayerMask; }
        set { _terrainLayerMask = value; }
    }

    // Variables
    // ---------
    private Vector3 _defaultPosition;
    public Vector3 DefaultPosition {
        get { return _defaultPosition; }
        set { _defaultPosition = value; }
    }

    private Vector3 _currentPosition;
    public Vector3 CurrentPosition {
        get { return _currentPosition; }
        set { _currentPosition = value; }
    }

    private Vector3 _newPosition;
    public Vector3 NewPosition {
        get { return _newPosition; }
        set { _newPosition = value; }
    }

    private float _lerp = 1f;
    public float Lerp {
        get { return _lerp; }
        set { _lerp = value; }
    }

    private float _distanceToObjective;
    public float DistanceToObjective {
        get { return _distanceToObjective; }
        set { _distanceToObjective = value; }
    }

    private bool _canMove;
    public bool CanMove {
        get { return _canMove; }
        set { _canMove = value; }
    }

    private bool _isMoving;
    public bool IsMoving {
        get { return _isMoving; }
        set { _isMoving = value; }
    }

    // Public Functions
    // ----------------

    // Private Functions
    // -----------------
    /*
    Start function

    Args:
    -----

    Returns:
    --------
        void
    */
    private void Start() {
        // Set default position
        DefaultPosition = transform.localPosition;
        // Positions
        CurrentPosition = transform.position;
        NewPosition = transform.position;
    }

    /*
    Update function that places the foot in the correct position.

    Args:
    -----

    Returns:
    --------
        void
    */
    private void Update() {
        transform.position = CurrentPosition;

        // Position on the ground
        Vector3 desiredPosition = Body.transform.position + Body.transform.TransformDirection(DefaultPosition);
        Vector3 BodyVelocity = Body.Velocity;
        RaycastHit hit;
        Ray ray = new Ray(desiredPosition + BodyVelocity + Vector3.up * 3f, Vector3.down);

        if (Physics.Raycast(ray, out hit, 6f, TerrainLayerMask)) {
            DistanceToObjective = Vector3.Distance(NewPosition, hit.point);
            if (DistanceToObjective > StepDistance && CanMove && !IsMoving) {
                Lerp = 0f;
                NewPosition = hit.point;
            }
        }

        if (Lerp < 1f) {
            IsMoving = Lerp < 0.3f;
            Vector3 updatedPosition = Vector3.Lerp(CurrentPosition, NewPosition, Lerp);
            updatedPosition.y += Mathf.Sin(Lerp * Mathf.PI) * StepHeight;
            CurrentPosition = updatedPosition;
            Lerp += Time.deltaTime * StepSpeed;
        }
        else {
            IsMoving = false;
            CurrentPosition = NewPosition;
        }
    }

    private void OnDrawGizmos() {
        // offset
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Body.transform.position, Body.transform.position + Body.transform.TransformDirection(DefaultPosition));
        // current position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(CurrentPosition, 0.1f);
        // new position
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(NewPosition, 0.1f);
    }
}
