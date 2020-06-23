using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public float ShakeMagnitude;
    public float collectibleHeight;
    /// for swipe control
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;
    private float minDistanceForSwipe = 20f;

    private Rigidbody myRigid;
    private RaycastHit myHit;
    private Vector3 targetPos;
    public bool isMoving = false;
    private bool oneTime = false;
    private bool climbNow = false;
    private bool fallNow = false;
    private string direction;
    private Collider myCollider;
    public static PlayerController Instance;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        myRigid = transform.GetComponent<Rigidbody>();
        targetPos = transform.position;
        myCollider = transform.GetComponent<BoxCollider>();
        isMoving = true;
    }

    // Update is called once per frame
    void Update()
    {
        checkInputs();
        //movePlayer(SwipeDirection.None);//for debug in editor
        //drawRays();//for debug in editor

    }
    void FixedUpdate()
    {
        if (Mathf.Abs(myRigid.position.x - targetPos.x) >0.01f  || Mathf.Abs(myRigid.position.z - targetPos.z) > 0.01f)
        {
            Vector3 newPosition = Vector3.MoveTowards(myRigid.position, new Vector3(targetPos.x,transform.position.y,targetPos.z), movementSpeed * Time.deltaTime);
            myRigid.MovePosition(newPosition);
        }
        else
        {
            if (oneTime)
            {
                StartCoroutine(Shake());
                oneTime = false;
            }
            if (climbNow)
            {   
                StartCoroutine(Climb());
                climbNow = false;
            }
            if (fallNow)
            {
                StartCoroutine(Fall());
                fallNow = false;
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 13)
        {
            if (transform.childCount == 1)
            {
                Transform mychild = transform.GetChild(transform.childCount - 1);
                mychild.GetComponent<CollectibleBehaviour>().Deattaching(collision.transform);
                mychild.parent = null;
                StartCoroutine(DeattachSpace());
                collision.transform.GetComponent<BoxCollider>().isTrigger = true;
                collision.transform.GetComponent<BoxCollider>().enabled = true;
                nextLevel();
            }
            else
            {
                Debug.Log("Space");
                Transform mychild = transform.GetChild(transform.childCount - 1);
                mychild.GetComponent<CollectibleBehaviour>().Deattaching(collision.transform);
                mychild.parent = null;
                StartCoroutine(DeattachSpace());
                MyAudioManager.Instance.Play("gather");
                collision.transform.GetComponent<BoxCollider>().isTrigger = true;
            }
            
        }
        
    }
    public void movePlayer(SwipeDirection swipe)
    {
        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.W) || swipe == SwipeDirection.Up)
            {
                if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.forward, out myHit, 100, 1 << 9))
                {
                    Debug.Log(myHit.distance);
                    targetPos = transform.position + new Vector3(0, 0, myHit.distance - 0.5f);
                    targetPos.z = Mathf.Round(targetPos.z);
                    Debug.Log(targetPos);
                    oneTime = true;
                    direction = "forward";
                    Debug.Log("forward");
                    isMoving = true;
                }
                else if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.forward, out myHit, 100, 1 << 11))
                {
                    Debug.Log("climb area detected");
                    targetPos = transform.position + new Vector3(0, 0, myHit.distance - 1f);
                    targetPos.z = Mathf.Round(targetPos.z);
                    Debug.Log(targetPos);
                    StopAllCoroutines();
                    direction = "forward";
                    climbNow = true;
                    isMoving = true;
                    oneTime = false;
                }
            }
            else if (Input.GetKeyDown(KeyCode.A) || swipe == SwipeDirection.Left)
            {
                if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.left, out myHit, 100, 1 << 9))
                {
                    Debug.Log(myHit.distance);
                    targetPos = transform.position - new Vector3(myHit.distance - 0.5f, 0, 0);
                    targetPos.z = Mathf.Round(targetPos.z);
                    Debug.Log(targetPos);
                    oneTime = true;
                    direction = "left";
                    isMoving = true;
                }
                else if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.left, out myHit, 100, 1 << 14))
                {
                    Debug.Log(myHit.distance);
                    targetPos = transform.position - new Vector3(myHit.distance - 0.5f, 0, 0);
                    targetPos.z = Mathf.Round(targetPos.z);
                    Debug.Log(targetPos);
                    fallNow = true;
                    direction = "left";
                    isMoving = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.S) || swipe == SwipeDirection.Down)
            {
                if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.back, out myHit, 100, 1 << 9))
                {
                    Debug.Log(myHit.distance);
                    targetPos = transform.position - new Vector3(0, 0, myHit.distance - 0.5f);
                    targetPos.z = Mathf.Round(targetPos.z);
                    Debug.Log(targetPos);
                    oneTime = true;
                    direction = "back";
                    isMoving = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.D) || swipe == SwipeDirection.Right)
            {
                if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.right, out myHit, 100, 1 << 9))
                {
                    Debug.Log(myHit.distance);
                    targetPos = transform.position + new Vector3(myHit.distance - 0.5f, 0, 0);
                    targetPos.z = Mathf.Round(targetPos.z);
                    Debug.Log(targetPos);
                    oneTime = true;
                    direction = "right";
                    isMoving = true;
                }
                else if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.right, out myHit, 100, 1 << 14))
                {
                    Debug.Log(myHit.distance);
                    targetPos = transform.position + new Vector3(myHit.distance - 0.5f, 0, 0);
                    targetPos.z = Mathf.Round(targetPos.z);
                    Debug.Log(targetPos);
                    fallNow = true;
                    direction = "right";
                    isMoving = true;
                }
            } 
        }
    }
    private void drawRays()
    {
        Debug.DrawRay(transform.position + new Vector3(0, 0.1f, 0), transform.TransformDirection(Vector3.forward), Color.red);
        Debug.DrawRay(transform.position + new Vector3(0, 0.1f, 0), transform.TransformDirection(Vector3.left), Color.red);
        Debug.DrawRay(transform.position + new Vector3(0, 0.1f, 0), transform.TransformDirection(Vector3.right), Color.red);
        Debug.DrawRay(transform.position + new Vector3(0, 0.1f, 0), transform.TransformDirection(Vector3.back), Color.red);
    }
    private IEnumerator Shake()
    {
        Vector3 newRot;
        newRot = new Vector3(0, 0, 0);
        myRigid.constraints = RigidbodyConstraints.FreezePosition;
        myCollider.isTrigger = true;
        if(direction=="forward" || direction == "back")
        {
            float targetAngle = direction == "forward" ? ShakeMagnitude + 5 : -ShakeMagnitude - 5;

            
            int count = 0;
            while (count<10)
            {
                float angle = Mathf.LerpAngle(transform.eulerAngles.x, targetAngle, Time.deltaTime * 20);
                newRot.x = angle;
                transform.eulerAngles = newRot;
                yield return new WaitForSeconds(0.001f);
                count++;
            }
            MyAudioManager.Instance.SetPitch("wood", Random.Range(0.5f, 1.5f));
            MyAudioManager.Instance.Play("wood");
            count = 0;
            while (count<10)
            {
                float angle = Mathf.LerpAngle(transform.eulerAngles.x, 0, Time.deltaTime * 20);
                newRot.x = angle;
                transform.eulerAngles = newRot;
                yield return new WaitForSeconds(0.001f);
                count++;
            } 
            
            
        }
        if (direction == "left" || direction == "right")
        {
            float targetAngle = direction == "left" ? ShakeMagnitude + 5 : -ShakeMagnitude - 5;
            
            int count = 0;

            while (count<10)
            {
                float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * 20);
                newRot.z = angle;
                transform.eulerAngles = newRot;
                yield return new WaitForSeconds(0.001f);
                count++;
            }
            MyAudioManager.Instance.SetPitch("wood", Random.Range(0.5f, 1.5f));
            MyAudioManager.Instance.Play("wood");
            count = 0;
            while (count<10)
            {
                float angle = Mathf.LerpAngle(transform.eulerAngles.z, 0, Time.deltaTime * 20);
                newRot.z = angle;
                transform.eulerAngles = newRot;
                yield return new WaitForSeconds(0.001f);
                count++;
            }
            
            
            
        }

        transform.eulerAngles = Vector3.zero;
        yield return new WaitForSeconds(0.1f);
        myRigid.constraints = RigidbodyConstraints.None;
        myCollider.isTrigger = false;
        isMoving = false;
    }
    
    public IEnumerator Attach(Transform attachment)
    {
        int i = 0;
        Vector3 vel = Vector3.zero;
        Debug.Log("attaching");
        attachment.localEulerAngles = Vector3.zero;
        attachment.parent = transform;
        while (i < 5)
        {
            attachment.localPosition = Vector3.SmoothDamp(attachment.localPosition, new Vector3(0, 0.45f, 0), ref vel, 0.01f);
            int childNumber = transform.childCount;
            for(int z=0;z<childNumber;z++)
            {
                Vector3 addheight = new Vector3(0, collectibleHeight / 5f, 0);
                transform.GetChild(z).localPosition += addheight;
            }
            i++;
            yield return new WaitForSeconds(0.001f);
        }
        attachment.localEulerAngles = Vector3.zero;
        attachment.localPosition = new Vector3(0, collectibleHeight / 2, 0);

    }
    public IEnumerator DeattachSpace()
    {
        int i = 0;
        Vector3 heightDecrease = new Vector3(0, collectibleHeight, 0);
        while (i < 10)
        {
            foreach(Transform element in transform)
            {
                element.localPosition -= heightDecrease/10f;
            }
            yield return new WaitForSeconds(0.001f);
            i++;
        }
    }
    private IEnumerator Climb()
    {
        yield return new WaitForSeconds(0.1f);
        bool canClimb = false;
        int index = 0;
        int i = 0;
        foreach(Transform element in transform)
        {
            if (Physics.Raycast(element.position + new Vector3(0, 0.1f, 0), Vector3.forward, out myHit, 100, 1 << 12))
            {
                canClimb = true;
                index = i;
            }
            i++;
        }
        Debug.Log("It can climb : " + canClimb);
        if (canClimb)
        {
            myRigid.useGravity = false;
            myRigid.isKinematic = true;
            myCollider.isTrigger = true;
            float targetHeight = (transform.childCount - (index + 1)) * collectibleHeight;
            
            Vector3 newPos = transform.position + new Vector3(0, targetHeight, 0);
            float counter = 0f;
            newPos = transform.position + new Vector3(0, 0.1f, 1.5f);
            for (int a = transform.childCount - 1; a > index; a--)
            {

                transform.GetChild(a).GetComponent<Rigidbody>().useGravity = true;
                transform.GetChild(a).GetComponent<BoxCollider>().isTrigger = false;
                transform.GetChild(a).GetComponent<Rigidbody>().isKinematic = false;
                transform.GetChild(a).parent = null;
            }
            transform.position = transform.position + new Vector3(0, targetHeight, 0);
            for (int a = 0; a <= index; a++)
            {
                transform.GetChild(a).localPosition -= new Vector3(0, targetHeight, 0);
            }
            MyAudioManager.Instance.Play("climb");
            while (counter <= 1f)
            {
                transform.position = Vector3.Lerp(transform.position, newPos + new Vector3(0, targetHeight+0.1f, 0), counter);
                counter += 0.1f;
                yield return new WaitForSeconds(0.001f);
            }
            counter = 0;
            targetPos = transform.position;
            myCollider.isTrigger = false;
            myRigid.isKinematic = false;
            myRigid.useGravity = true;
            isMoving = false;
            
        }
        else
        {
            StartCoroutine(Shake());
        }
        
    }
    private IEnumerator Fall()
    {
        float targetAngle = direction == "left" ? 30 : -30;
        Vector3 newRot;
        newRot = new Vector3(0, 0, 0);
        int count = 0;
        myRigid.constraints = RigidbodyConstraints.FreezePosition;
        myCollider.isTrigger = true;
        myRigid.AddTorque(new Vector3(0, 0, targetAngle * 0.5f));
        yield return new WaitForSeconds(1);
        //while (count < 20)
        //{
        //    float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * 10);
        //    newRot.z = angle;
        //    transform.eulerAngles = newRot;
        //    yield return new WaitForSeconds(0.001f);
        //    count++;
        //}
        for (int a = transform.childCount-1; a >=0; a--)
        {
            Rigidbody cubeRigid = transform.GetChild(a).GetComponent<Rigidbody>();
            cubeRigid.useGravity = true;
            cubeRigid.isKinematic = false;
            cubeRigid.constraints = RigidbodyConstraints.None;
            myRigid.velocity = Vector3.zero;
            myRigid.angularVelocity = Vector3.zero;
            transform.GetChild(a).GetComponent<BoxCollider>().isTrigger = false;
            transform.GetChild(a).parent = null;
        }
        yield return new WaitForSeconds(2);
        Debug.Log("Restart Level");
        MyGameManager.Instance.resetLevel();
    }
    public void nextLevel()
    {
        StartCoroutine(nextLevelDelay());
    }
    private IEnumerator nextLevelDelay()
    {
        Debug.Log("Level ended");
        MyAudioManager.Instance.Play("taptostart");
        yield return new WaitForSeconds(1.5f);
        MyGameManager.Instance.changeLevel();
    }
    //Swipe Controls
    public void checkInputs()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }
        }
    }
    private void DetectSwipe()
    {
        if (SwipeDistanceCheckMet())
        {
            if (IsVerticalSwipe())
            {
                var direction = fingerDownPosition.y - fingerUpPosition.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                movePlayer(direction);
            }
            else
            {
                var direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                movePlayer(direction);
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
    public enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right,
        None
    }
}
