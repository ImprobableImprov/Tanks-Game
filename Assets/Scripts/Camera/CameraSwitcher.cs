using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject overhead, follow;
    private AudioListener overheadListener, followListener;

    void Start()
    {
        overheadListener = overhead.GetComponent<AudioListener>();
        followListener = follow.GetComponent<AudioListener>();

        followListener.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            switchCamera();
    }

    private void switchCamera()
    {
        if (overhead.activeSelf)
        {
            follow.SetActive(true);
            followListener.enabled = true;

            overhead.SetActive(false);
            overheadListener.enabled = false;
        }
        else
        {
            overhead.SetActive(true);
            overheadListener.enabled = true;

            follow.SetActive(false);
            followListener.enabled = false;
        }
    }
}
