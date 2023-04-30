using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LimbRow
{
    // Serialized Fields
    // -----------------
    [SerializeField] private Transform _rightTarget;
    public Transform RightTarget {
        get { return _rightTarget; }
        set { _rightTarget = value; }
    }

    [SerializeField] private Transform _leftTarget;
    public Transform LeftTarget {
        get { return _leftTarget; }
        set { _leftTarget = value; }
    }

    [SerializeField] private Transform _body;
    public Transform Body {
        get { return _body; }
        set { _body = value; }
    }

    [SerializeField] private float _stepDistance = 1f;

    // Variables
    // ---------
    // These variables are used to save the target positions.
    private Vector3 _rightCurrentPosition;
    private Vector3 _leftCurrentPosition;

    // These variables are used to save the new positions, to detect if the target positions are too far away and update them if necessary.
    private Vector3 _rightNewPosition;
    public Vector3 RightNewPosition {
        get { return _rightNewPosition; }
        set { _rightNewPosition = value; }
    }
    private Vector3 _leftNewPosition;
    public Vector3 LeftNewPosition {
        get { return _leftNewPosition; }
        set { _leftNewPosition = value; }
    }

    // Offset vector to the body
    private Vector3 _rightOffset;
    public Vector3 RightOffset {
        get { return _rightOffset; }
        set { _rightOffset = value; }
    }
    private Vector3 _leftOffset;
    public Vector3 LeftOffset {
        get { return _leftOffset; }
        set { _leftOffset = value; }
    }


    // Public Functions
    // ----------------
    /*
    Function that sets the initial target positions.

    Args:
    -----

    Returns:
    --------
        void
    */
    public void SetPositions() {
        _rightCurrentPosition = RightTarget.position;
        _leftCurrentPosition = LeftTarget.position;

        _rightNewPosition = RightTarget.position;
        _leftNewPosition = LeftTarget.position;
    }

    /*
    Function that sets the offset vector to the body.

    Args:
    -----
    
    Returns:
    --------
        void
    */
    public void SetOffsets() {
        _rightOffset = RightTarget.position - Body.position;
        _leftOffset = LeftTarget.position - Body.position;
    }


    /*
    Set the target positions to the saved target positions.

    Args:
    -----

    Returns:
    --------
        void
    */
    public void SetTargetPositions() {
        float rightDistance = Vector3.Distance(_rightCurrentPosition, _rightNewPosition);
        float leftDistance = Vector3.Distance(_leftCurrentPosition, _leftNewPosition);
        // Debug.Log(rightDistance);

        if (rightDistance > _stepDistance) {
            _rightCurrentPosition = _rightNewPosition;
        }

        if (leftDistance > _stepDistance) {
            _leftCurrentPosition = _leftNewPosition;
        }

        RightTarget.position = _rightCurrentPosition;
        LeftTarget.position = _leftCurrentPosition;
    }

    // Private Functions
    // -----------------
}
