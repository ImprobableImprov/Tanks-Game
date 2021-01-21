using UnityEngine;
using System.Collections;


    public class TurretAim : MonoBehaviour
    {
        [Header("Rotations")]
        [SerializeField] private Transform turretBase = null;
        [SerializeField] private Transform barrels = null;

        [Header("Elevation")]
        public float ElevationSpeed = 30f;
        public float MaxElevation = 60f;
        public float MaxDepression = 5f;

        [Header("Traverse")]
        public float TraverseSpeed = 60f;
        [SerializeField] private bool hasLimitedTraverse = false;
        [Range(0, 179)] public float LeftLimit = 120f;
        [Range(0, 179)] public float RightLimit = 120f;

        [Header("Behavior")]
        public bool IsIdle = false;
        public Vector3 AimPosition = Vector3.zero;
        [SerializeField] private float aimedThreshold = 5f;
        private float limitedTraverseAngle = 0f;

        [Header("Debug")]
        public bool DrawDebugRay = true;
        public bool DrawDebugArcs = false;

        private float angleToTarget = 0f;
        private float elevation = 0f;

        private bool hasBarrels = false;
        private bool isAimed = false;
        private bool isBaseAtRest = false;
        private bool isBarrelAtRest = false;
        private bool shotReady = true;

        [SerializeField] private GameObject projectilePrefabR = null;
        [SerializeField] private GameObject projectilePrefabL = null;
        [SerializeField] private Transform spawnPointL = null;
        [SerializeField] private Transform spawnPointR = null;
        [SerializeField] private float delay = 0.1f;
        public float launchForce;

        /// <summary>
        /// True when the turret cannot rotate freely in the horizontal axis.
        /// </summary>
        public bool HasLimitedTraverse { get { return hasLimitedTraverse; } }

        /// <summary>
        /// True when the turret is idle and at its resting position.
        /// </summary>
        public bool IsTurretAtRest { get { return isBarrelAtRest && isBaseAtRest; } }

        /// <summary>
        /// True when the turret is aimed at the given <see cref="AimPosition"/>. When the turret
        /// is idle, this is never true.
        /// </summary>
        public bool IsAimed { get { return isAimed; } }

        /// <summary>
        /// Angle in degress to the given <see cref="AimPosition"/>. When the turret is idle,
        /// the angle reports 999.
        /// </summary>
        public float AngleToTarget { get { return IsIdle ? 999f : angleToTarget; } }

        private void Awake()
        {
            hasBarrels = barrels != null;
            if (turretBase == null)
                Debug.LogError(name + ": TurretAim requires an assigned TurretBase!");

            launchForce = GetComponent<SphereCollider>().radius * 600f;
        }

        private void Update()
        {
            if (IsIdle)
            {
                if (!IsTurretAtRest)
                    RotateTurretToIdle();
                isAimed = false;
            }
            else
            {
                RotateBaseToFaceTarget(AimPosition);

                if (hasBarrels)
                    RotateBarrelsToFaceTarget(AimPosition);

                // Turret is considered "aimed" when it's pointed at the target.
                angleToTarget = GetTurretAngleToTarget(AimPosition);

                // Turret is considered "aimed" when it's pointed at the target.
                isAimed = angleToTarget < aimedThreshold;

                if (isAimed && shotReady)
                {
                    Launch();
                    StartCoroutine(Delay());
                }

                isBarrelAtRest = false;
                isBaseAtRest = false;
            }
        }

       IEnumerator Delay()
        {
            yield return new WaitForSecondsRealtime(delay);
            shotReady = true;
        }

        public void Launch()
        {
            GameObject projectileInstanceL = Instantiate(projectilePrefabL, spawnPointL.position, Quaternion.LookRotation(barrels.forward));
            GameObject projectileInstanceR = Instantiate(projectilePrefabR, spawnPointR.position, Quaternion.LookRotation(barrels.forward));

            if (projectileInstanceL.TryGetComponent(out Rigidbody rbL))
                rbL.AddForce(barrels.forward * launchForce);

            if (projectileInstanceR.TryGetComponent(out Rigidbody rbR))
                rbR.AddForce(barrels.forward * launchForce);
            
            shotReady = false;

        }

        private float GetTurretAngleToTarget(Vector3 targetPosition)
        {
            float angle = 999f;

            if (hasBarrels)
            {
                angle = Vector3.Angle(targetPosition - barrels.position, barrels.forward);
            }
            else
            {
                Vector3 flattenedTarget = Vector3.ProjectOnPlane(
                    targetPosition - turretBase.position,
                    turretBase.up);

                angle = Vector3.Angle(
                    flattenedTarget - turretBase.position,
                    turretBase.forward);
            }

            return angle;
        }

        private void RotateTurretToIdle()
        {
            // Rotate the base to its default position.
            if (hasLimitedTraverse)
            {
                limitedTraverseAngle = Mathf.MoveTowards(
                    limitedTraverseAngle, 0f,
                    TraverseSpeed * Time.deltaTime);

                if (Mathf.Abs(limitedTraverseAngle) > Mathf.Epsilon)
                    turretBase.localEulerAngles = Vector3.up * limitedTraverseAngle;
                else
                    isBaseAtRest = true;
            }
            else
            {
                turretBase.rotation = Quaternion.RotateTowards(turretBase.rotation,
                                                               transform.rotation,
                                                               TraverseSpeed * Time.deltaTime);

                isBaseAtRest = Mathf.Abs(turretBase.localEulerAngles.y) < Mathf.Epsilon;
            }

            if (hasBarrels)
            {
                elevation = Mathf.MoveTowards(elevation, 0f, ElevationSpeed * Time.deltaTime);
                if (Mathf.Abs(elevation) > Mathf.Epsilon)
                    barrels.localEulerAngles = Vector3.right * -elevation;
                else
                    isBarrelAtRest = true;
            }
            else // Barrels automatically at rest if there are no barrels.
                isBarrelAtRest = true;
        }

        private void RotateBarrelsToFaceTarget(Vector3 targetPosition)
        {
            Vector3 localTargetPos = turretBase.InverseTransformDirection(targetPosition - barrels.position);
            Vector3 flattenedVecForBarrels = Vector3.ProjectOnPlane(localTargetPos, Vector3.up);

            float targetElevation = Vector3.Angle(flattenedVecForBarrels, localTargetPos);
            targetElevation *= Mathf.Sign(localTargetPos.y);

            targetElevation = Mathf.Clamp(targetElevation, -MaxDepression, MaxElevation);
            elevation = Mathf.MoveTowards(elevation, targetElevation, ElevationSpeed * Time.deltaTime);

            if (Mathf.Abs(elevation) > Mathf.Epsilon)
                barrels.localEulerAngles = Vector3.right * -elevation;

#if UNITY_EDITOR
            if (DrawDebugRay)
                Debug.DrawRay(barrels.position, barrels.forward * localTargetPos.magnitude, Color.red);
#endif
        }

        private void RotateBaseToFaceTarget(Vector3 targetPosition)
        {
            Vector3 turretUp = transform.up;

            Vector3 vecToTarget = targetPosition - turretBase.position;
            Vector3 flattenedVecForBase = Vector3.ProjectOnPlane(vecToTarget, turretUp);

            if (hasLimitedTraverse)
            {
                Vector3 turretForward = transform.forward;
                float targetTraverse = Vector3.SignedAngle(turretForward, flattenedVecForBase, turretUp);

                targetTraverse = Mathf.Clamp(targetTraverse, -LeftLimit, RightLimit);
                limitedTraverseAngle = Mathf.MoveTowards(
                    limitedTraverseAngle,
                    targetTraverse,
                    TraverseSpeed * Time.deltaTime);

                if (Mathf.Abs(limitedTraverseAngle) > Mathf.Epsilon)
                    turretBase.localEulerAngles = Vector3.up * limitedTraverseAngle;
            }
            else
            {
                turretBase.rotation = Quaternion.RotateTowards(
                    Quaternion.LookRotation(turretBase.forward, turretUp),
                    Quaternion.LookRotation(flattenedVecForBase, turretUp),
                    TraverseSpeed * Time.deltaTime);
            }

#if UNITY_EDITOR
            if (DrawDebugRay && !hasBarrels)
                Debug.DrawRay(turretBase.position,
                    turretBase.forward * flattenedVecForBase.magnitude,
                    Color.red);
#endif
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!DrawDebugArcs)
                return;

            if (turretBase != null)
            {
                const float kArcSize = 10f;
                Color colorTraverse = new Color(1f, .5f, .5f, .1f);
                Color colorElevation = new Color(.5f, 1f, .5f, .1f);
                Color colorDepression = new Color(.5f, .5f, 1f, .1f);

                Transform arcRoot = barrels != null ? barrels : turretBase;

                // Red traverse arc
                UnityEditor.Handles.color = colorTraverse;
                if (hasLimitedTraverse)
                {
                    UnityEditor.Handles.DrawSolidArc(
                        arcRoot.position, turretBase.up,
                        transform.forward, RightLimit,
                        kArcSize);
                    UnityEditor.Handles.DrawSolidArc(
                        arcRoot.position, turretBase.up,
                        transform.forward, -LeftLimit,
                        kArcSize);
                }
                else
                {
                    UnityEditor.Handles.DrawSolidArc(
                        arcRoot.position, turretBase.up,
                        transform.forward, 360f,
                        kArcSize);
                }

                if (barrels != null)
                {
                    // Green elevation arc
                    UnityEditor.Handles.color = colorElevation;
                    UnityEditor.Handles.DrawSolidArc(
                        barrels.position, barrels.right,
                        turretBase.forward, -MaxElevation,
                        kArcSize);

                    // Blue depression arc
                    UnityEditor.Handles.color = colorDepression;
                    UnityEditor.Handles.DrawSolidArc(
                        barrels.position, barrels.right,
                        turretBase.forward, MaxDepression,
                        kArcSize);
                }
            }
        }
#endif
    }

