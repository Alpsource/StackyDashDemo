using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaptoStartBehaviour : MonoBehaviour
{
    public bool oneTime = true;
    private Vector3 firstPos;
    private void Start()
    {
        firstPos = transform.position;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && oneTime)
        {
            PlayerController.Instance.isMoving = false;
            oneTime = false;
            MyAudioManager.Instance.Play("taptostart");
            StartCoroutine(LeaveScreen());
        }
    }
    private IEnumerator LeaveScreen()
    {
        while (transform.position.y > -2000)
        {
            transform.position -= Vector3.up*50;
            yield return new WaitForSeconds(0.001f);
        }
        transform.position = firstPos;
        gameObject.SetActive(false);
    }
}
