using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    private float m_HorizantalInputMove;
    private float m_VerticalInputMove;
    private float m_HorizantalInputTurret;
    private float m_VerticalInputTurret;
    private float m_OriginalPitch;
    private const float smooth = 0.85f;

    private Rigidbody tankR;

    public Transform frontLeftT, frontRightT, backLeftT, backRightT;
    public Transform mount, cannon;
    public GameObject  tank;
    public float maxSteerAngle = 45f;
    public float maxCannonAngle = 90f;
    public float maxMountRotation = 90f;
    public float maxSpeed = 100f;
    public float turnSpeed = 180f;
    public float speed = 12f;
    public float frontWheelRadius = 170f;
    public float backWheelRadius = 245f;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;
    public float m_PitchRange = 0.2f;


    private void Awake()
    {
        tankR = tank.GetComponent<Rigidbody>();
        m_OriginalPitch = m_MovementAudio.pitch;
    }

    public void GetInput()
    {                                          
        m_HorizantalInputMove = Input.GetAxis("Horizontal1");
        m_VerticalInputMove = Input.GetAxis("Vertical1");
        m_HorizantalInputTurret = Input.GetAxis("Horizontal2");
        m_VerticalInputTurret = Input.GetAxis("Vertical2");
    }
    
    private void EngineAudio()
    {
        if (Mathf.Abs(m_VerticalInputMove) < 0.1f)
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            Debug.Log("Else");
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                Debug.Log("Not Idle");
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }
    
    private void RotateCannon()
    {
        if (m_VerticalInputTurret <= 0)
            cannon.localRotation = Quaternion.Euler(cannon.rotation.x + maxCannonAngle * m_VerticalInputTurret, 0f, 0f);

        mount.localRotation = Quaternion.Euler(0f, mount.rotation.y + maxMountRotation * m_HorizantalInputTurret, 0f);   
    }

    private void Steer()
    {
        float turn = m_HorizantalInputMove * turnSpeed * Time.deltaTime;

        if(frontLeftT.parent.localEulerAngles.y < 43f)
        {
            frontLeftT.parent.Rotate(0f, turn, 0f);
            frontRightT.parent.Rotate(0f, turn, 0f);
        }
        else if(frontLeftT.parent.localEulerAngles.y > 317f)
        {
            frontLeftT.parent.Rotate(0f, turn, 0f);
            frontRightT.parent.Rotate(0f, turn, 0f);
        }
        else if (frontLeftT.parent.localEulerAngles.y < 45f && m_HorizantalInputMove < 0)
        {
            frontLeftT.parent.Rotate(0f, turn, 0f);
            frontRightT.parent.Rotate(0f, turn, 0f);
        }
        else if (frontLeftT.parent.localEulerAngles.y > 315 && m_HorizantalInputMove > 0)
        {
            frontLeftT.parent.Rotate(0f, turn, 0f);
            frontRightT.parent.Rotate(0f, turn, 0f);
        }

        if(m_HorizantalInputMove == 0 && m_VerticalInputMove != 0 && frontLeftT.parent.localEulerAngles.y > 0.1f)
        {
            frontLeftT.parent.localRotation = Quaternion.Lerp(frontLeftT.parent.localRotation, Quaternion.identity, Time.deltaTime * smooth);
            frontRightT.parent.localRotation = Quaternion.Lerp(frontRightT.parent.localRotation, Quaternion.identity, Time.deltaTime * smooth);

        }
    }

    private void Accelerate()
    {
        Vector3 move = transform.forward * m_VerticalInputMove * speed * Time.deltaTime;
        
        tankR.MovePosition(tankR.position + move);
        
        Quaternion turnRotation;
        float Yvalue = frontLeftT.parent.localEulerAngles.y;

        if (Yvalue < 45)
            turnRotation = Quaternion.Euler(0f, Yvalue * m_VerticalInputMove * Time.deltaTime, 0f);
        else
            turnRotation = Quaternion.Euler(0f, (Yvalue-360f) * m_VerticalInputMove * Time.deltaTime, 0f);

        
        if (m_VerticalInputMove != 0)
            tankR.MoveRotation(tankR.rotation * turnRotation);

        float distanceTraveled = m_VerticalInputMove * 1000f * Time.deltaTime;
        float rotationInRadians = distanceTraveled / frontWheelRadius;
        float rotationInDegrees = rotationInRadians * Mathf.Rad2Deg;

        frontLeftT.Rotate(rotationInDegrees, 0f, 0f);
        frontRightT.Rotate(rotationInDegrees, 0f, 0f);

        rotationInRadians = distanceTraveled / backWheelRadius;
        rotationInDegrees = rotationInRadians * Mathf.Rad2Deg;

        backLeftT.Rotate(rotationInDegrees, 0f, 0f);
        backRightT.Rotate(rotationInDegrees, 0f, 0f);

    }

    private void FixedUpdate()
    {
        GetInput();
        EngineAudio();
        Steer();
        Accelerate();
        RotateCannon();
     }


}
