
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    Rigidbody rigidBody;
    AudioSource audioSource;

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

    bool isTransitioning = false;
    bool collisonDisabled = false;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

    }

  
    // Update is called once per frame
    void Update () {
        if (!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }

    }

    void RespondToDebugKeys() {
        if (Input.GetKey(KeyCode.L)){
            LoadNextLevel();

        }
        else if (Input.GetKey(KeyCode.C)) {
            collisonDisabled = !collisonDisabled;
        }

    }
 

    void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || collisonDisabled)
        {

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

    }

    private void StartFinishSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(levelStart);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }


    private void StartDeathSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }


    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
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
            stopApplyingThrust();
        }   
    }

    private void stopApplyingThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
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
        rigidBody.angularVelocity = Vector3.zero;

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

  
    }

  
}
