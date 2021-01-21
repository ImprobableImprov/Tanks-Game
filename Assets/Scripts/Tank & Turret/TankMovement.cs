using UnityEngine;

public class TankMovement : MonoBehaviour
{
    //public int m_PlayerNumber = 1; //Player 1, 2, 3, etc.        
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;
    public float m_MaxRay = 0.1f;
    public bool m_isGrounded = true;
    
    public GameObject m_Wheel_FL;
    public GameObject m_Wheel_FR;
    public GameObject m_Wheel_BL;
    public GameObject m_Wheel_BR;

    public WheelCollider m_W_FL;
    public WheelCollider m_W_FR;
    public WheelCollider m_W_BL;
    public WheelCollider m_W_BR;

    public float m_Torque = 1000f;
    public float m_MinWheelSpeed = 20f;
    public float m_MinWheelAngle = 40f;
    public float m_MaxWheelAngle = 20f;




    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;   // Move tank around      
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false; // lets forces be applied to it
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true; //stops forces applied
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical1"; // + m_PlayerNumber;
        m_TurnAxisName = "Horizontal1"; // + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Terrain")
        {
            m_isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Terrain")
        {
            m_isGrounded = false;
        }
    }

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis("Vertical1");
        m_TurnInputValue = Input.GetAxis("Horizontal1");

        EngineAudio();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.

        if(Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            if(m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }


    }

    private void FixedUpdate() //runs every physics step
    {
        
        // Move and turn the tank.
        Move();
       
        SteerWheels();
        RotateWheels();
        Turn();
    }


    private void Move()
    {
        
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime; //Time.deltaTime chanes per frame to per second

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        
        m_W_BL.motorTorque = m_Torque * Input.GetAxis("Vertical1");
        m_W_BR.motorTorque = m_Torque * Input.GetAxis("Vertical1");
        m_W_FL.motorTorque = m_Torque * Input.GetAxis("Vertical1");
        m_W_FR.motorTorque = m_Torque * Input.GetAxis("Vertical1");

        float speedFactor = GetComponent<Rigidbody>().velocity.magnitude / m_MinWheelSpeed;
        float currentWheelAngle = Mathf.Lerp(m_MinWheelAngle, m_MaxWheelAngle, speedFactor);

        currentWheelAngle *= Input.GetAxis("Horizontal1");

        m_W_FL.steerAngle = currentWheelAngle;
        m_W_FR.steerAngle = currentWheelAngle;
    }

    void RotateWheels()
    {
        /*
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        */
        
        m_Wheel_BL.gameObject.transform.Rotate(m_W_BL.rpm / 180 * Time.deltaTime, 0, 0);
        m_Wheel_BR.gameObject.transform.Rotate(m_W_BR.rpm / 180 * Time.deltaTime, 0, 0);
        m_Wheel_FR.gameObject.transform.Rotate(m_W_FR.rpm / 180 * Time.deltaTime, 0, 0);
        m_Wheel_FL.gameObject.transform.Rotate(m_W_FL.rpm / 180 * Time.deltaTime, 0, 0);
        /*
        
        //m_Wheel_FR.gameObject.transform.RotateAround(GetComponent<MeshRenderer>().bounds.center, m_W_FR.rpm / 180 * Time.deltaTime);
        //m_Wheel_FL.gameObject.transform.RotateAround(GetComponent<MeshRenderer>().bounds.center, m_W_FL.rpm / 180 * Time.deltaTime);
        */
    }

    void SteerWheels()
    {


        Vector3 temp;

        //front left
        temp = m_Wheel_FL.transform.localEulerAngles;
        temp.y = m_W_FL.steerAngle;
        m_Wheel_FL.transform.localEulerAngles = temp;
        
        //front right
        temp = m_Wheel_FR.transform.localEulerAngles;
        temp.y = m_W_FR.steerAngle;
        m_Wheel_FR.transform.localEulerAngles = temp;
    }


    private void Turn()
    {
        if (m_isGrounded)
        {
            // Adjust the rotation of the tank based on the player's input.
            float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        }
        else
        {
            float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
            
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX;

            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);

            m_Rigidbody.constraints = RigidbodyConstraints.None;
        }

    }
}