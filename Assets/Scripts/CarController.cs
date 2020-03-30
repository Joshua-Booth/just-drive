using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LightingManager))]
public class CarController : MonoBehaviour
{
    // Game Managers
    public InputManager im;
    public LightingManager lm;
    public UIManager uim;

    // Vehicle
    public List<WheelCollider> throttleWheels;
    public List<WheelCollider> steeringWheels;
    public List<GameObject> meshes;
    public float strengthCoefficient = 20000f;
    public float maxTurn = 20f;
    public Transform cm;
    public Rigidbody rb;

    // Timing and Difficulty
    public int seconds = 0;
    public float interval = 1;
    public float speedDifficulty = 1;

    // Starting Engine Audio
    public AudioSource audioSource;
    public AudioClip startingAudioClip;

    // Engine Audio
    public AudioClip lowAccelClip;
    public AudioClip highAccelClip;
    public float pitchMultiplier = 1f;
    public float lowPitchMin = 1f;
    public float lowPitchMax = 4.8f;
    public float highPitchMultiplier = 0.2f;
    public float maxRolloffDistance = 500;  // The maximum distance where rolloff starts to take place

    private AudioSource lowAccelAudio;
    private AudioSource highAccelAudio;
    private bool startedSound;

    // Vehicle Explosion
    public GameObject explosionPrefab;
    private GameObject explosion;

    void Start()
    {
        im = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // Set vehicle center of mass
        if (cm)
        {
            rb.centerOfMass = cm.localPosition;
        }
    }

    // Used instead of Update() as there is more vehicle physics being calculated per frame
    void FixedUpdate()
    {
        // Apply speed to throttle wheels
        foreach (WheelCollider wheel in throttleWheels)
        {
            wheel.motorTorque = strengthCoefficient * Time.deltaTime * im.throttle * speedDifficulty;
        }

        // Set steering angle of steering wheels
        foreach (WheelCollider wheel in steeringWheels)
        {
            wheel.GetComponent<WheelCollider>().steerAngle = maxTurn * im.steer;
            wheel.transform.localEulerAngles = new Vector3(0f, im.steer * maxTurn, 0f);
        }

        // Make the wheels rotate
        foreach (GameObject mesh in meshes)
        {
            mesh.transform.Rotate(rb.velocity.magnitude * (transform.InverseTransformDirection(rb.velocity).z >= 0 ? 1 : -1 ) / (2 * Mathf.PI * 0.6f), 0f, 0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Play sound based on what oject was collided with
        if (collision.collider.tag == "Obstacle")
        {
            // Create an explosion
            if (!explosion)
            {
                explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            // Stop the car and it's sound
            StopSound();
            rb.constraints = RigidbodyConstraints.FreezePosition;

            // Game ends
            uim.GameOver();
        }
        else if (collision.collider.tag == "Border")
        {
            seconds = 0;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Turn the car around
            var rotation = transform.localRotation.y + 180;
            if (transform.localRotation.y < 0)
            {
                rotation = transform.localRotation.y / 360;
            }

            transform.rotation = Quaternion.Euler(0, rotation, 0);
            
            StartCoroutine(uim.FlashOutOfBounds());
        }
    }

    private void StartSound()
    {
        // setup the simple audio source
        highAccelAudio = SetUpEngineAudioSource(highAccelClip);
        
        // flag that we have started the sounds playing
        startedSound = true;
    }

    private void StopSound()
    {
        //Destroy all audio sources on this object:
        foreach (var source in GetComponents<AudioSource>())
        {
            Destroy(source);
        }

        startedSound = false;
    }

    public void increaseDifficulty()
    {
        seconds++;
        speedDifficulty = 1 + (seconds * 0.04f);
    }

    // Update is called once per frame
    private void Update()
    {
        Invoke("increaseDifficulty", interval);

        var speed = transform.InverseTransformVector(rb.velocity).z;

        // Keep the car moving
        if (rb.velocity.magnitude < 0.5f && im.throttle < 1)
        {
            rb.velocity = new Vector3(0, 0, (speed + 1) * speedDifficulty);
        }

        // Toggle car headlights
        if (im.lightsEnabled)
        {
            lm.ToggleHeadlights();
        }

        uim.changeSpeed(speed);

        // Get the distance to main camera
        float camDist = (Camera.main.transform.position - transform.position).sqrMagnitude;

        // Stop sound if the object is beyond the maximum roll off distance
        if (startedSound && camDist > maxRolloffDistance)
        {
            StopSound();
        }

        // Start the sound if it's not playing while the game is running
        // and it is nearer than the maximum distance
        if (!startedSound && camDist < maxRolloffDistance && !uim.Game.isGameOver)
        {
            StartSound();
        }

        // Stop the sound if the game ends
        if (uim.Game.isGameOver)
        {
            StopSound();
        }

        if (startedSound)
        {
            // Pitch is inserted in between the min and max values, according to the throttle
            float pitch = ULerp(lowPitchMin, lowPitchMax, im.throttle);

            // Clamp to minimum pitch (note, not clamped to max for high revs while burning out)
            pitch = Mathf.Min(lowPitchMax, pitch);
            
            highAccelAudio.pitch = pitch * pitchMultiplier * highPitchMultiplier;
            highAccelAudio.volume = 1;
        }
    }

    // Sets up and adds new audio source to the game object
    public AudioSource SetUpEngineAudioSource(AudioClip clip)
    {
        // Create the new audio source component on the game object and set up its properties
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 0;
        source.loop = true;

        // Start the clip from a random point
        source.time = Random.Range(0f, clip.length);
        source.Play();
        source.minDistance = 5;
        source.maxDistance = maxRolloffDistance;
        return source;
    }

    // Unclamped versions of Lerp and Inverse Lerp, to allow value to exceed the from-to range
    public virtual float ULerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }
}
