using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockController : MonoBehaviour
{
    // Serialized Fields
    // -----------------
    [SerializeField] private int _numberBoids;
    public int NumberBoids {
        get { return _numberBoids; }
        set { _numberBoids = value; }
    }
    
    [SerializeField] private int _seed;
    public int Seed {
        get { return _seed; }
        set { _seed = value; }
    }

    [SerializeField] private Boid _boidPrefab;
    public Boid BoidPrefab {
        get { return _boidPrefab; }
    } 
    
    // Variables
    // ---------

    // Public Functions
    // ----------------

    // Private Functions
    // -----------------

    /*
    Start function that initializes the flock controller. It creates a number of boids and sets their initial position and velocity.

    Args:
    -----

    Returns:
    --------
        void
    */
    private void Start() {
        Random.InitState(Seed);
        for (int i = 0; i < NumberBoids; i++) {
            Vector3 initPos = new Vector3(Random.Range(-40f, 40f), 5f, Random.Range(-40f, 40f));
            Instantiate(BoidPrefab, initPos, Quaternion.identity);
        }
    }
}
