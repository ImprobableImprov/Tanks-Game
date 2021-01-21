using UnityEngine;


    public class TurretController : MonoBehaviour
    {
        [SerializeField] private TurretAim TurretAim = null;

        public Transform TargetPoint = null;

        private bool isIdle = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TurretAim.IsIdle = false;
            TargetPoint = other.transform;
        }
            
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TurretAim.IsIdle = true;
            TargetPoint = null;
        }
    }

        private void Update()
        {
            if (TurretAim == null)
                return;

            if (TargetPoint == null)
                TurretAim.IsIdle = TargetPoint == null;
            else
                TurretAim.AimPosition = TargetPoint.position;

            if (Input.GetMouseButtonDown(0))
                TurretAim.IsIdle = !TurretAim.IsIdle;
        }
    }
