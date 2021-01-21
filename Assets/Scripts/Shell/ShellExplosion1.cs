using UnityEngine;

public class ShellExplosion1 : MonoBehaviour
{
    public LayerMask m_TankMask; //controls layers effected
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_damage = 1f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = .1f;              


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
        Debug.Log("Start");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("EnterOnTrigger");
        // Find all the tanks in an area around the shell and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for(int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidBody = colliders[i].GetComponent<Rigidbody>();

            if(!targetRigidBody)
                continue;
            Debug.Log("RigidBody");
            targetRigidBody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth targetHealth = targetRigidBody.GetComponent<TankHealth>();

            if(!targetHealth)
                continue;
            
            //float damage = 1f; //CalculateDamage(targetRigidBody.position);
            if(other.tag != "Player")
                targetHealth.TakeDamage(m_damage);
            Debug.Log("Hit");
            
            
            m_ExplosionParticles.transform.parent = null;

            m_ExplosionParticles.Play();

            m_ExplosionAudio.Play();
            
            Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
            Destroy(gameObject);
        }
    }

    /*
    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        float damage = relativeDistance * m_MaxDamage;

        damage = Mathf.Max(0f, damage);

        return damage;
    }
    */
}