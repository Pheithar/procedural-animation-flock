using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimator : MonoBehaviour
{
    // Serialized Fields
    // -----------------
    [Header("Legs")]
    [SerializeField] private List<ProceduralLeg> _legs;
    public List<ProceduralLeg> Legs {
        get { return _legs; }
        set { _legs = value; }
    }

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

    [Header("Additional parameters")]
    [SerializeField] private LayerMask _terrainLayerMask;
    public LayerMask TerrainLayerMask {
        get { return _terrainLayerMask; }
        set { _terrainLayerMask = value; }
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
        for (int i = 0; i < Legs.Count; i++) {
            if (Legs[i].IsMoving) {
                anyLegMoving = true;
                break;
            }
            else {
                if (Legs[i].DistanceToObjective > maxDistance) {
                    maxDistance = Legs[i].DistanceToObjective;
                    legToMove = i;
                }
            }
        }

        if (!anyLegMoving) {
            for (int i = 0; i < Legs.Count; i++) {
                Legs[i].CanMove = i == legToMove;
            }
        }

        Velocity = transform.position - _previousPosition;
        
        _previousPosition = transform.position;

    }
}
