using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber =  1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public AudioSource m_ShootingAudio;  
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 50f; 
    public float m_MaxLaunchForce = 100f; 
    public float m_MaxChargeTime = 0.75f;

    
    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;        
    private bool m_Fired;                


    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire1"; //+ m_PlayerNumber;
    }
    

    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
        //m_AimSlider.value = m_MinLaunchForce;

        if (Input.GetButtonDown(m_FireButton))
        {
            // have we pressed for the first time
            m_Fired = false;
            Fire();
        }
    }


    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }
}