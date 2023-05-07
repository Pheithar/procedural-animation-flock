using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimator : MonoBehaviour
{
    // Serialized Fields
    // -----------------
    [Header("Legs")]
    [SerializeField] private ProceduralLeg[] _legs;

    [Header("Properties")]
    [SerializeField] private float _speed = 5f;
    public float Speed {
        get { return _speed; }
        set { _speed = value; }
    }

    [SerializeField] private float _rotationSpeed = 2.5f;
    public float RotationSpeed {
        get { return _rotationSpeed; }
        set { _rotationSpeed = value; }
    }

    [Header("Additional parameters and user interaction")]
    [SerializeField] private LayerMask _terrainLayerMask;
    public LayerMask TerrainLayerMask {
        get { return _terrainLayerMask; }
        set { _terrainLayerMask = value; }
    }

    [SerializeField] private bool _userControlled = false;
    public bool UserControlled {
        get { return _userControlled; }
        set { _userControlled = value; }
    }

    
    // Variables
    // ---------

    private Camera _camera;
    private Vector3 _targetPosition;

    private Vector3 _previousPosition;
    private Vector3 _velocity;
    public Vector3 Velocity {
        get { return _velocity; }
        set { _velocity = value; }
    }

    // Public Functions
    // ----------------

    // Private Functions
    // -----------------
    /* 
    Set the camera and target position.

    Args:
    -----

    Returns:
    --------
        void
    */
    private void Start() {
        _camera = Camera.main;
        _targetPosition = transform.position;

        _previousPosition = transform.position;
    }
    
    /*
    Update function to select which leg to move.

    Args:
    -----
        
    Returns:
    --------
        void
    */
    private void Update() {
        bool anyLegMoving = false;
        int legToMove = 0;
        float maxDistance = 0f;
        for (int i = 0; i < _legs.Length; i++) {
            if (_legs[i].IsMoving) {
                anyLegMoving = true;
                break;
            }
            else {
                if (_legs[i].DistanceToObjective > maxDistance) {
                    maxDistance = _legs[i].DistanceToObjective;
                    legToMove = i;
                }
            }
        }

        if (!anyLegMoving) {
            for (int i = 0; i < _legs.Length; i++) {
                _legs[i].CanMove = i == legToMove;
            }
        }

        Velocity = transform.position - _previousPosition;
        

        // if (UserControlled && Input.GetMouseButtonDown(0)) {
        //     Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //     if (Physics.Raycast(ray, out hit, Mathf.Infinity, TerrainLayerMask)) {
        //         _targetPosition = hit.point + Vector3.up * 1.5f;

        //         // Rotate the character to face the target position. Then rotate 180 degrees to face the opposite direction, because the character is facing the wrong way.
        //         // transform.rotation = Quaternion.LookRotation(_targetPosition - transform.position, Vector3.up) * Quaternion.Euler(0, 180, 0);
        //     }
        // }

        // // Move toward the target position until the target position is reached (margin of 0.1f)
        // if (Vector3.Distance(transform.position, _targetPosition) > 0.1f) {
        //     Vector3 newDirection = Vector3.RotateTowards(transform.forward, _targetPosition - transform.position, RotationSpeed * Time.deltaTime, 0.0f);
        //     transform.rotation = Quaternion.LookRotation(newDirection);

        //     // Raycast to the ground to check if the character is on the ground. If not, move it towards the ground.
        //     transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Speed * Time.deltaTime);
            
        //     RaycastHit hit;
        //     if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, TerrainLayerMask)) {
        //         transform.position = new Vector3(transform.position.x, hit.point.y + 1.5f, transform.position.z);
        //     }
        // }

        _previousPosition = transform.position;

    }
}
