using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;

    [SerializeField] float speed;
    [SerializeField] float sprintSpeed;

    static float sensivity = 2;
    [SerializeField] AudioClip[] audioList;
    private Vector3 moveDirection;
    private float mouseUpDown;

    float footSteptmp;
    [SerializeField] float footStepSize;
    bool leftStep;

    AudioSource audioSource;

    [SerializeField] GameObject Map;
    Vector3 mapRotation;

    Transform myCamera;
    [SerializeField] InputField sensivityText;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //Cursor settings
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        characterController = GetComponent<CharacterController>();
        myCamera = transform.GetChild(0);
    }

    private void Update()
    {
        MouseMovement();
        Movement();
        TakeMap();
    }

    void MouseMovement()
    {
        float mouseLeftRight = Input.GetAxis("Mouse X") * sensivity * Time.deltaTime * 10;
        transform.Rotate(0, mouseLeftRight, 0);

        mouseUpDown -= Input.GetAxis("Mouse Y") * sensivity * Time.deltaTime * 10;

        mouseUpDown = Mathf.Clamp(mouseUpDown, -90, 78);
        myCamera.localRotation = Quaternion.Euler(mouseUpDown, 0, 0);
    }

    void Movement()
    {
        WalkSound();
        //Sprint();

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        moveDirection *= speed;
        moveDirection = transform.TransformDirection(moveDirection);

        if (moveDirection.magnitude > 7)
            moveDirection /= 1.3f;

        characterController.SimpleMove(moveDirection);
    }

    void TakeMap()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            Map.SetActive(!Map.activeInHierarchy);
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            //mapRotation.z += 90;
            //Map.transform.GetChild(0).GetComponent<RectTransform>().localEulerAngles = mapRotation;
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            sensivityText.transform.parent.gameObject.SetActive(!sensivityText.transform.parent.gameObject.activeInHierarchy);

            Cursor.visible = !Cursor.visible;

            if(sensivityText.transform.parent.gameObject.activeInHierarchy) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;

            if (sensivityText.text != "")
            sensivity = int.Parse(sensivityText.text);
        }
    }

    void Sprint()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed += sprintSpeed;
            footStepSize /= 2;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed -= sprintSpeed;
            footStepSize *= 2;
        }
    }

    void WalkSound()
    {
        if (moveDirection.magnitude > 1 && characterController.velocity.magnitude > 0.8f)
        {
            footSteptmp += Time.deltaTime;

            if (footSteptmp >= footStepSize)
            {
                FootStepSound();
            }
        }
    }

    void FootStepSound()
    {
        leftStep = !leftStep;
        footSteptmp = 0;

        if(leftStep)
            audioSource.PlayOneShot(audioList[0]);
        else
            audioSource.PlayOneShot(audioList[1]);

    }
}

