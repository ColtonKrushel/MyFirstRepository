using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class rocket : MonoBehaviour
{

    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 3f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;

    bool collisionsEnabled = true;
 
    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Debug.isDebugBuild)
        {
            debugKeys();
        }

        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
    }

    private void debugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsEnabled = !collisionsEnabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive || !collisionsEnabled) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                successStart();
                break;
            default:
                deathStart();
                break;
        }
    }

    private void successStart()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void deathStart()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }
    
    private void LoadNextScene()
    {
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true;
        
        float rotationSpeed = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationSpeed);
        }

        rigidBody.freezeRotation = false;
    }

    private void Thrust()
    {
        float thrustSpeed = mainThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust(thrustSpeed);
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
        
    }

    private void ApplyThrust(float thrustSpeed)
    {
        rigidBody.AddRelativeForce(Vector3.up * thrustSpeed);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }
}
