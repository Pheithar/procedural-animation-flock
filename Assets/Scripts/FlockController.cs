using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

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

    [SerializeField] private string _path;
    public string Path {
        get { return _path; }
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
        // for (int i = 0; i < NumberBoids; i++) {
        //     Vector3 initPos = new Vector3(Random.Range(-40f, 40f), 5f, Random.Range(-40f, 40f));
        //     Instantiate(BoidPrefab, initPos, Quaternion.identity);
        // }
        // Load all not working, iterate through path and load all prefabs
        List<GameObject> boids = new List<GameObject>();

        string fullPath = "Assets/Resources/" + Path;

        foreach (string path in Directory.GetFiles(fullPath)) {
            if (path.EndsWith(".fbx")) {
                // Get the name of the file
                string fileName = path.Split('/').Last().Split('.')[0];
                GameObject boid = Resources.Load<GameObject>(Path + "/" + fileName);
                boids.Add(boid);
            }
        }

        GameObject test = Instantiate(boids[0], new Vector3(1, 1, 1), Quaternion.identity);
        AddRigidBodyBoxCollider(test);
        List<GameObject> legs = GetLegs(test);
        AddRig(test, legs);
        AddBoid(test);

    }

    /*
    Function to add a rigid body and a box collider to the boid.

    Args:
    -----
        GameObject boid: The boid to add the rigid body and box collider to.

    Returns:
    --------
        void
    */
    private void AddRigidBodyBoxCollider(GameObject boid) {
        // Add rigid body
        Rigidbody rb = boid.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.mass = 1f;
        rb.drag = 0.0f;
        rb.angularDrag = 0.05f;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        // Add box collider
        BoxCollider bc = boid.AddComponent<BoxCollider>();
        Bounds bounds = GetBounds(boid);
        bc.center = bounds.center - boid.transform.position;
        bc.size = bounds.size * 0.9f;
    }

    /* 
    Function to get the bounds of the boid.

    Args:
    -----
        GameObject boid: The boid to get the bounds of.

    Returns:
    --------
        Bounds: The bounds of the boid.
    */
    private Bounds GetBounds(GameObject boid) {
        Bounds bounds = new Bounds(boid.transform.position, Vector3.zero);
        foreach (Renderer renderer in boid.GetComponentsInChildren<Renderer>()) {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    /*
    Function to create an empty child object for the rig. Give it a 'Rig' script, and then add the 'Rig Builder' Script to the 
    original game object and assign the Rig to it.
    Also adds a IK constraint on each leg of the boid.

    Each IK constraint has a target and a hint that will be created.
    The position of the target needs to be the end of the leg.
    The position of the hint needs to be the elbow (mid).

    On the target, add the 'Procedural Leg' script

    The movement of the legs has to be proportional to the number of legs. The more legs, the faster the movement.

    Args:
    -----
        GameObject boid: The boid to add the rig to.
        List<GameObject> legs: The legs of the boid.

    Returns:
    --------
        void
    */
    private void AddRig(GameObject boid, List<GameObject> legs) {

        ProceduralAnimator proceduralAnimator = boid.AddComponent<ProceduralAnimator>();
        proceduralAnimator.Legs = new List<ProceduralLeg>();
        proceduralAnimator.TerrainLayerMask = LayerMask.GetMask("Terrain");

        GameObject rig = new GameObject("Rig");
        rig.transform.parent = boid.transform;
        rig.transform.localPosition = Vector3.zero;
        rig.transform.localRotation = Quaternion.identity;
        rig.transform.localScale = Vector3.one;
        rig.AddComponent<Rig>();

        // Add IK constraint to each leg
        foreach (GameObject leg in legs) {
            GameObject legIK = new GameObject(leg.name + "_IK");
            legIK.transform.parent = rig.transform;
            legIK.transform.localPosition = Vector3.zero;
            legIK.transform.localRotation = Quaternion.identity;
            legIK.transform.localScale = Vector3.one;

            GameObject root = leg.transform.GetChild(0).gameObject;
            GameObject mid = root.transform.GetChild(0).gameObject;
            GameObject end = mid.transform.GetChild(0).gameObject;

            GameObject legIKTarget = new GameObject(leg.name + "_IK_Target");
            legIKTarget.transform.parent = legIK.transform;
            legIKTarget.transform.position = end.transform.position;

            ProceduralLeg proceduralLeg = legIKTarget.AddComponent<ProceduralLeg>();
            proceduralLeg.Body = proceduralAnimator;
            proceduralLeg.TerrainLayerMask = LayerMask.GetMask("Terrain");
            proceduralLeg.StepDistance = 0.75f;
            proceduralLeg.StepHeight = 0.3f;
            proceduralLeg.StepSpeed = legs.Count * 2.5f;

            proceduralAnimator.Legs.Add(proceduralLeg);


            GameObject legIKHint = new GameObject(leg.name + "_IK_Hint");
            legIKHint.transform.parent = legIK.transform;
            legIKHint.transform.position = mid.transform.position;


            TwoBoneIKConstraint ikConstraint = legIK.AddComponent<TwoBoneIKConstraint>();
            ikConstraint.data.target = legIKTarget.transform;
            ikConstraint.data.hint = legIKHint.transform;
            ikConstraint.data.root = root.transform;
            ikConstraint.data.mid = mid.transform;
            ikConstraint.data.tip = end.transform;
            ikConstraint.data.targetPositionWeight = 1f;
            ikConstraint.data.targetRotationWeight = 1f;
            ikConstraint.data.hintWeight = 1f;

        }


        RigBuilder rigBuilder = boid.AddComponent<RigBuilder>();
        rigBuilder.layers.Clear();
        rigBuilder.layers.Add(new RigLayer(rig.GetComponent<Rig>(), true));
        rigBuilder.Build();
    }

    /*
    Function to get a list of game objects where each represents a leg of the boid.
    The structure of the boid follows this:
    Boid
        Armature
            HeadBone
                BodyBone0u
                    ShoulderBoneLeft_0 <- Store this
                        LegBoneAnteriorLeft_0
                            LegBonePosteriorLeft_0
                                LegBonePosteriorLeft_0_end
                    ShoulderBoneRight_0 <- Store this
                        ...
                    BodyBone1
                        ...
    
    Args:
    -----
        GameObject boid: The boid to get the legs of.
    
    Returns:
    --------
        List<GameObject>: A list of game objects where each represents a leg of the boid.
    */
    private List<GameObject> GetLegs(GameObject boid) {
        List<GameObject> legs = new List<GameObject>();

        // Get all childs in multiple levels
        Transform[] allChildren = boid.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren) {
            if (child.name.Contains("ShoulderBone")) {
                legs.Add(child.gameObject);
                Debug.Log(child.name);
            }
        }

        return legs;
    }

    /* 
    Function to add the 'Boid' script to the boid.

    Args:
    -----
        GameObject boid: The boid to add the 'Boid' script to.
    
    Returns:
    --------
        void
    */
    private void AddBoid(GameObject boid) {
        Boid boidScript = boid.AddComponent<Boid>();
        boidScript.Rb = boid.GetComponent<Rigidbody>();
    }
}
