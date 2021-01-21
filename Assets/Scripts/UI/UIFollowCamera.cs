using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowCamera : MonoBehaviour
{
    public Camera playerCam;
    public Transform target;

    void Update()
    {
        if (playerCam == null)
        {
            playerCam = GameObject.FindGameObjectWithTag("TargetCamera").GetComponent<Camera>();
            target = playerCam.transform;
        }

        transform.eulerAngles = new Vector3(playerCam.transform.eulerAngles.x,
                                            playerCam.transform.parent.gameObject.transform.eulerAngles.y,
                                            transform.eulerAngles.z);
    }
}
