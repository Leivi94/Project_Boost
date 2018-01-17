
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    Rigidbody rigidBody;
    AudioSource audioSource;
    enum State { Alive, Dying, Transcending }

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;

    //LOAD DELAY
    [SerializeField] float levelLoadDelay = 2f;

    //AUDIO
    [SerializeField] AudioClip levelStart;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;

    //PARTICLE
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    State state = State.Alive;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

    }

  
    // Update is called once per frame
    void Update () {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
            NextLevel();
        }

     
    }

    void NextLevel() { if (Input.GetKey(KeyCode.L)) { LoadNextLevel(); } }


    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive){ return; }

       switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("Do nothing");
                break;
            case "Finish":
                StartFinishSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }

    }

    private void StartFinishSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(levelStart);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }


    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }


    private void LoadNextLevel()
    {
  
        SceneManager.LoadScene(1);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }


    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }   
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }

        mainEngineParticles.Play();
        

    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // Manual rotation
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;   //Game Logic rotation
  
    }

  
}
