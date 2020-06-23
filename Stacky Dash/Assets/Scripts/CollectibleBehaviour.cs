using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleBehaviour : MonoBehaviour
{
    private bool oneTime = true;
    private Collider myCollider;
    private Rigidbody myRigid;
    void Start()
    {
        myCollider = transform.GetComponent<BoxCollider>();
        myRigid = transform.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (oneTime && collision.gameObject.tag=="Player")
        {
            myRigid.isKinematic = true;
            myRigid.useGravity = false;
            myCollider.isTrigger = true;
            myRigid.constraints = RigidbodyConstraints.FreezeRotation;
            StartCoroutine(PlayerController.Instance.Attach(transform));
            oneTime = false;
            Vibration.Vibrate(200);
            MyAudioManager.Instance.Play("gather");
            Debug.Log("collided");
        }
    }
    public void Deattaching(Transform targetObject)
    {
        StartCoroutine(Deattach(targetObject));
    }
    private IEnumerator Deattach(Transform targetObject)
    {
        yield return new WaitForSeconds(0.1f);
        
        myRigid.isKinematic = false;
        myRigid.constraints = RigidbodyConstraints.FreezeRotation;
        int count = 0;
        Vector3 vel = Vector3.zero;
        while (count < 100)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetObject.position, ref vel, 0.1f * Time.deltaTime);
            yield return new WaitForSeconds(0.001f);
            count++;
        }
        myCollider.isTrigger = false;
        myRigid.useGravity = true;
    }
}
