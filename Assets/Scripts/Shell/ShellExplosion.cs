using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask; //controls layers effected
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_damage = 1f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;

    private bool isHit = false;


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            Debug.Log("Trigger Collision");
            return;
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemey Detected");
            TankHealth targetHealth = other.GetComponent<TankHealth>();
            targetHealth.TakeDamage(m_damage);

            m_ExplosionParticles.transform.parent = null;

            m_ExplosionParticles.Play();

            m_ExplosionAudio.Play();

            Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
            Destroy(gameObject);
        }



        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (other.gameObject.CompareTag("Player") && !isHit)
            {
                Rigidbody targetRigidBody = colliders[i].GetComponent<Rigidbody>();

                if (!targetRigidBody)
                    continue;

                TankHealth targetHealth = targetRigidBody.GetComponent<TankHealth>();

                if (!targetHealth)
                    continue;

                targetHealth.TakeDamage(m_damage);
                isHit = true;
                Debug.Log("Player Got Hit => Health: " + targetHealth + " (-" + m_damage + ")");

            }

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