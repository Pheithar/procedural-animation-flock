using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimator : MonoBehaviour
{
    // Serialized Fields
    // -----------------
    // Target, defined per rows 
    [SerializeField] private LimbRow[] _limbs;
    public LimbRow[] Limbs {
        get { return _limbs; }
        set { _limbs = value; }
    }

    [SerializeField] private LayerMask _terrainLayerMask;
    public LayerMask TerrainLayerMask {
        get { return _terrainLayerMask; }
        set { _terrainLayerMask = value; }
    }

    [SerializeField] private float _stepDistance = 0.5f;
    public float StepDistance {
        get { return _stepDistance; }
        set { _stepDistance = value; }
    }

    // Variables
    // ---------

    // Public Functions
    // ----------------

    // Private Functions
    // -----------------
    /*
    Start function to define the starting target positions.

    Args:
    -----

    Returns:
    --------
        void
    */
    private void Start() {
        for (int i = 0; i < Limbs.Length; i++) {
            // Set position
            Limbs[i].SetPositions();
            // Set offset
            Limbs[i].SetOffsets();
        }
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
        for (int i = 0; i < Limbs.Length; i++) {
            // Position on the ground
            RaycastHit hit;
            Ray ray = new Ray(Limbs[i].Body.position + Limbs[i].RightOffset + Vector3.up * 3f, Vector3.down);

            if (Physics.Raycast(ray, out hit, 6f, TerrainLayerMask)) {
                Limbs[i].RightNewPosition = hit.point;
            }

            ray = new Ray(Limbs[i].Body.position + Limbs[i].LeftOffset + Vector3.up * 3f, Vector3.down);
            
            if (Physics.Raycast(ray, out hit, 6f, TerrainLayerMask)) {
                Limbs[i].LeftNewPosition = hit.point;
            }

            // // Move the feet if the target position is too far away
            Limbs[i].SetTargetPositions();
        }


    }

    private void OnDrawGizmos() {
        for (int i = 0; i < Limbs.Length; i++) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Limbs[i].LeftNewPosition, 0.1f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(Limbs[i].RightNewPosition, 0.1f);

            // offsets
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Limbs[i].Body.position, Limbs[i].Body.position + Limbs[i].LeftOffset);
            Gizmos.DrawLine(Limbs[i].Body.position, Limbs[i].Body.position + Limbs[i].RightOffset);
        }
    }
}
