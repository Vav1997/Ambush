using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FirstPersonController : MonoBehaviour
{
    public static FirstPersonController instance;
    public enum ControllerType {FirstPerson, ThirdPerson, NoMove}
    public enum AllowedMoveDirection {Forward, Left, NoMove}
    [SerializeField] AudioListener myListener;

    private AllowedMoveDirection allowedMoveDirection;
    public ControllerType controllerType;
    // References
    [SerializeField] private Transform MainCameraTransform;
    

    // Player settings
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveInputDeadZone;

    [SerializeField] private float minYRotation;
    [SerializeField] private float maxYRotation;
    [SerializeField] private float minXRotation;
    [SerializeField] private float maxXRotation;



    // Touch detection
    private int leftFingerId, rightFingerId;
    private float halfScreenWidth;
    private float halfScreenHeight;

    // Camera control
    private Vector2 lookInput;
    private float cameraPitch;
    private float cameraPitchx;

    // Player movement
    private Vector2 moveTouchStartPosition;
    private Vector2 moveInput;

    [SerializeField] private Transform thirdPersonCamera;
    [SerializeField] private Transform thirdPersonVisual;

    //Swipe 

    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    [SerializeField]
    private bool detectSwipeOnlyAfterRelease = false;

    [SerializeField]
    private float minDistanceForSwipe = 40f;


    [SerializeField] private Animator playerAnim;

    [SerializeField] private bool edgeCollisionNotified;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float proneTime;
    [SerializeField] private float proneSpeed;
    [SerializeField] private float sideProneSpeed;

    public event EventHandler OnProneForward;
    public event EventHandler OnChangedToFirstPerson;


    public event EventHandler<OnScopeSideCollidedEventArgs> OnScopeSideCollided;
    public class OnScopeSideCollidedEventArgs : EventArgs
    {
        public CollidedScopeSide collidedSide;
    }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource scopeAudioSource;
    [SerializeField] private AudioClip MovingSound;
    [SerializeField] private AudioClip MovingSoundShort;
    [SerializeField] private AudioClip scopeMovementSound;
    [SerializeField] private  float scopeAudioFadeDuration = 1.0f;
    public CheckPointer checkPointer;

    private Coroutine scopeSoundFadeInCor;
    private Coroutine scopeSoundFadeOutCor;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        // only calculate once
        halfScreenWidth = Screen.width / 2;
        halfScreenHeight = Screen.height / 2;

        cameraPitch = (minYRotation + maxYRotation) / 2;
        cameraPitchx = (minXRotation + maxXRotation) / 2;;
    }

    private void Start()
    {
        controllerType = ControllerType.FirstPerson;
        // id = -1 means the finger is not being tracked
        leftFingerId = -1;
        rightFingerId = -1;


        // calculate the movement input dead zone
        moveInputDeadZone = Mathf.Pow(Screen.height / moveInputDeadZone, 2);
    }

    // Update is called once per frame
    private void Update()
    {
        if(controllerType != ControllerType.NoMove)
        {
            switch(controllerType)
            {
            case ControllerType.FirstPerson:
                // Handles input
                GetTouchInput();

                if (rightFingerId != -1) {
                    // Ony look around if the right finger is being tracked
                    
                    LookAround();
                    
                }

                if (leftFingerId != -1)
                {
                    // Ony move if the left finger is being tracked
                    
                    // Move();
                }
                break;
            case ControllerType.ThirdPerson:
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        fingerUpPosition = touch.position;
                        fingerDownPosition = touch.position;
                    }

                    if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
                    {
                        

                        fingerDownPosition = touch.position;
                        DetectSwipe();
                    }

                    if (touch.phase == TouchPhase.Ended)
                    {
                        
                        fingerDownPosition = touch.position;
                        DetectSwipe();
                    }
                }
                break;
            case ControllerType.NoMove:
                break;
            }
        }


       
    }

    void GetTouchInput() {
        // Iterate through all the detected touches
        for (int i = 0; i < Input.touchCount; i++)
        {

            Touch t = Input.GetTouch(i);

            // Check each touch's phase
            switch (t.phase)
            {
                case TouchPhase.Began:

                    if (t.position.x < halfScreenWidth && leftFingerId == -1)
                    {
                       
                        checkPointer.Shoot();

                        // Start tracking the left finger if it was not previously being tracked
                        leftFingerId = t.fingerId;

                        // Set the start position for the movement control finger
                        moveTouchStartPosition = t.position;
                    }
                    else if (t.position.x > halfScreenWidth && rightFingerId == -1)
                    {
                        // Start tracking the rightfinger if it was not previously being tracked
                        rightFingerId = t.fingerId;


                        scopeAudioSource.time = UnityEngine.Random.Range(0, scopeAudioSource.clip.length * 0.7f);
                        
                        PlayAudio();
                        //scopeAudioSource.Play();
                    
                    }

                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:

                    if (t.fingerId == leftFingerId)
                    {
                        // Stop tracking the left finger
                        leftFingerId = -1;
                        // Debug.Log("Stopped tracking left finger");
                    }
                    else if (t.fingerId == rightFingerId)
                    {
                        // Stop tracking the right finger
                        rightFingerId = -1;

                        StopAudio();
                        //scopeAudioSource.Stop();
                        // Debug.Log("Stopped tracking right finger");
                    }

                    break;
                case TouchPhase.Moved:

                    // Get input for looking around
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                    }
                    else if (t.fingerId == leftFingerId) {

                        // calculating the position delta from the start position
                        moveInput = t.position - moveTouchStartPosition;
                    }

                    break;
                case TouchPhase.Stationary:
                    // Set the look input to zero if the finger is still
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = Vector2.zero;
                    }
                    break;
            }
        }
    }



    IEnumerator FadeIn()
    {
        // if(scopeSoundFadeOutCor != null)
        // {
        //     StopCoroutine(scopeSoundFadeOutCor);
        // }
        float t = 0.0f;
        float currentVolume = scopeAudioSource.volume;
        while (t < scopeAudioFadeDuration)
        {
            t += Time.deltaTime;
            scopeAudioSource.volume = Mathf.Lerp(currentVolume, 1.0f, t / scopeAudioFadeDuration);
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        
        // if(scopeSoundFadeInCor != null)
        // {
        //     StopCoroutine(scopeSoundFadeInCor);
        // }

        float t = 0.0f;
        float currentVolume = scopeAudioSource.volume;

        while (t < scopeAudioFadeDuration)
        {
            t += Time.deltaTime;
            scopeAudioSource.volume = Mathf.Lerp(currentVolume, 0.0f, t / scopeAudioFadeDuration);
            yield return null;
        }
        audioSource.Stop();
    }

    public void PlayAudio()
    {
        
        scopeAudioSource.Play();
        scopeSoundFadeInCor =  StartCoroutine(FadeIn());
        StopCoroutine(FadeOut());
    }

    public void StopAudio()
    {
        scopeSoundFadeOutCor = StartCoroutine(FadeOut());
        StopCoroutine(FadeIn());
    }



    void LookAround() {

        // vertical (pitch) rotation
        
        cameraPitch = cameraPitch - lookInput.y;

        if(cameraPitch <= minYRotation)
        {
            //reached top
            if(!edgeCollisionNotified)
            {
                edgeCollisionNotified = true;
                
                OnScopeSideCollided?.Invoke(this, new OnScopeSideCollidedEventArgs()
                {
                    collidedSide = CollidedScopeSide.Top
                });
                
            }
            
            
        }
        else if(cameraPitch >= maxYRotation)
        {
            //reached down
            if(!edgeCollisionNotified)
            {
                edgeCollisionNotified = true;
                
                OnScopeSideCollided?.Invoke(this, new OnScopeSideCollidedEventArgs()
                {
                    collidedSide = CollidedScopeSide.Bot
                });
                
            }
        }

        else if(cameraPitchx <= minXRotation)
        {
            if(!edgeCollisionNotified)
            {
                edgeCollisionNotified = true;
                
                OnScopeSideCollided?.Invoke(this, new OnScopeSideCollidedEventArgs()
                {
                    collidedSide = CollidedScopeSide.Right
                });
                
            }
        }

        else if(cameraPitchx >= maxXRotation)
        {
            if(!edgeCollisionNotified)
            {
                edgeCollisionNotified = true;
                
                OnScopeSideCollided?.Invoke(this, new OnScopeSideCollidedEventArgs()
                {
                    collidedSide = CollidedScopeSide.Left
                });
                
            }
        }

        cameraPitch = Mathf.Clamp(cameraPitch, minYRotation, maxYRotation);
        MainCameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        // horizontal (yaw) rotation
        cameraPitchx = Mathf.Clamp(cameraPitchx - lookInput.x, minXRotation, maxXRotation);
        transform.rotation = Quaternion.Euler(0, -cameraPitchx, 0);
        
        if(cameraPitch > minYRotation && cameraPitch < maxYRotation)
        {
            edgeCollisionNotified = false;
        }
    }

    // void Move() {

    //     // Don't move if the touch delta is shorter than the designated dead zone
    //     if (moveInput.sqrMagnitude <= moveInputDeadZone) return;

    //     // Multiply the normalized direction by the speed
    //     Vector2 movementDirection = moveInput.normalized * moveSpeed * Time.deltaTime;
    //     // Move relatively to the local transform's direction
    //     characterController.Move(transform.right * movementDirection.x + transform.forward * movementDirection.y);
    // }

    public void Move()
    {
        StartCoroutine(ProneForward());
    }


    public IEnumerator ProneForward()
    {
        playerAnim.SetBool("Move", false);
        float proneTimer = proneTime;
        OnProneForward?.Invoke(this, EventArgs.Empty);
        while(proneTimer >= 0)
        {
            characterController.Move(transform.forward * proneSpeed * Time.deltaTime);
            proneTimer -= Time.deltaTime;
            yield return null;
        }
        
    }

    public IEnumerator ProneLeft()
    {
        float proneTimer = proneTime;
        playerAnim.SetBool("MoveLeft", true);
        audioSource.PlayOneShot(MovingSoundShort);    
        while(proneTimer >= 0)
        {
            characterController.Move(-transform.right * sideProneSpeed * Time.deltaTime);
            proneTimer -= Time.deltaTime;
            yield return null;
        }
        playerAnim.SetBool("MoveLeft", false);
    }

    public void ChangeToFirstPerson()
    {
        MainCameraTransform.gameObject.SetActive(true);
        thirdPersonCamera.gameObject.SetActive(false);
        thirdPersonVisual.gameObject.SetActive(false);
        controllerType = ControllerType.FirstPerson;
        Listener.instance.TurOnListener();
        myListener.enabled = false;
        OnChangedToFirstPerson?.Invoke(this, EventArgs.Empty);
    }

    public void ChangeToThirdPerson(AllowedMoveDirection allowedMoveDir)
    {
        allowedMoveDirection = allowedMoveDir;
        MainCameraTransform.gameObject.SetActive(false);
        thirdPersonCamera.gameObject.SetActive(true);
        thirdPersonVisual.gameObject.SetActive(true);
        controllerType = ControllerType.ThirdPerson;
        Listener.instance.TurOffListener();
        myListener.enabled = true;
    }

    public void ChangeToNoMovement()
    {
        controllerType = ControllerType.NoMove;
    }


    // Swipe

    private void DetectSwipe()
    {
        if (SwipeDistanceCheckMet())
        {
            if (IsVerticalSwipe())
            {
                if(fingerDownPosition.y - fingerUpPosition.y > 0 && allowedMoveDirection == AllowedMoveDirection.Forward)
                {
                    if(!playerAnim.GetBool("Move"))
                    {
                        audioSource.PlayOneShot(MovingSound);
                        playerAnim.SetBool("Move", true);
                    } 
                }
            }
            else
            {
                if( fingerDownPosition.x - fingerUpPosition.x < 0 && allowedMoveDirection == AllowedMoveDirection.Left)
                {
                    //swiped left
                    if(!playerAnim.GetBool("MoveLeft"))
                    {
                        StartCoroutine(ProneLeft());
                    }
                }
            }
            fingerUpPosition = fingerDownPosition;
        }
    }

    private bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() > HorizontalMovementDistance();
    }

    private bool SwipeDistanceCheckMet()
    {
        return VerticalMovementDistance() > minDistanceForSwipe || HorizontalMovementDistance() > minDistanceForSwipe;
    }

    private float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
    }

    public void ChangeScopeScrollValues(float minXValue, float maxXValue, float minYValue, float maxYValue)
    {
        minXRotation = minXValue;
        maxXRotation = maxXValue;
        minYRotation = minYValue;
        maxYRotation = maxYValue;

        cameraPitch = (minYRotation + maxYRotation) / 2;
        cameraPitchx = (minXRotation + maxXRotation) / 2;;
    }
}