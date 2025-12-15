using System;
using UnityEngine;
using System.Collections.Generic;
using Internal.Scripts.Controllers.Enemies;
using Unity.VisualScripting;

namespace Internal.Scripts.Controllers.Buildings
{
    public abstract class Building : MonoBehaviour
    {
        [Header("Building Configuration")]
        [SerializeField] protected float detectionRadius = 5f;
        [SerializeField] protected float rotationSpeed = 90f;
        [SerializeField] protected LayerMask enemyLayerMask = -1;

        // Используем обычный Collider для 3D
        protected Collider[] detectedEnemiesBuffer = new Collider[10];
        protected Transform nearestTarget = null;

        private Transform lastKnownTarget = null;

        protected bool CanShootByRotation;

        protected virtual void Start()
        {
            // Debug.Log($"[Building] {gameObject.name} initialized. Detection Radius: {detectionRadius}, Rotation Speed: {rotationSpeed}", this);
        }

        protected virtual void Update()
        {
            // Debug.Log($"[Building] {gameObject.name} Update called.", this);
            FindNearestEnemy();
            RotateTowardsTarget();
        }

        protected virtual void FindNearestEnemy()
        {
            // Используем 3D физику: Physics.OverlapSphereNonAlloc
            int count = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, detectedEnemiesBuffer, enemyLayerMask);

            if (count > 0)
            {
                // Debug.Log($"[Building] {gameObject.name}: Found {count} colliders in radius.", this);
            }

            if (count == 0)
            {
                if (nearestTarget != null)
                {
                    // Debug.Log($"[Building] {gameObject.name}: No enemies detected, clearing target.", this);
                }
                nearestTarget = null;
                return;
            }

            float closestDistanceSqr = Mathf.Infinity;
            Transform closestEnemy = null;

            for (int i = 0; i < count; i++)
            {
                if (detectedEnemiesBuffer[i] == null) continue; // Защита от null

                Transform potentialTarget = detectedEnemiesBuffer[i].transform;

                if (potentialTarget.GetComponent<IEnemy>() == null)
                {
                    // Debug.Log($"[Building] {gameObject.name}: Skipping collider {potentialTarget.name}, no IEnemy component.", this);
                    continue;
                }

                float distanceSqr = (potentialTarget.position - transform.position).sqrMagnitude;
                // Debug.Log($"[Building] {gameObject.name}: Checking enemy '{potentialTarget.name}' at distance {Mathf.Sqrt(distanceSqr):F2}", this);

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestEnemy = potentialTarget;
                }
            }

            if (closestEnemy != null && closestEnemy != lastKnownTarget)
            {
                // Debug.Log($"[Building] {gameObject.name}: New nearest target selected: {closestEnemy.name}", this);
                lastKnownTarget = closestEnemy;
            }
            else if (closestEnemy == null && nearestTarget != null)
            {
                // Debug.Log($"[Building] {gameObject.name}: Cleared target after losing sight.", this);
            }

            nearestTarget = closestEnemy;
        }
        
        protected virtual void RotateTowardsTarget()
        {
            if (nearestTarget == null)
            {
                CanShootByRotation = false;
                return;
            }

            Vector3 directionToTarget = nearestTarget.position - transform.position;
            directionToTarget.y = 0f; // Только горизонтальный поворот

            if (directionToTarget.sqrMagnitude == 0f)
            {
                Debug.LogWarning($"[Building] {gameObject.name}: Target is exactly on top of the building, cannot rotate.", this);
                CanShootByRotation = false;
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

            // Плавный поворот
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Вычисляем угол между текущим и целевым поворотом
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            // Используем Mathf.Abs (хотя угол всегда положителен, но на всякий случай)
            float absAngleDifference = Mathf.Abs(angleDifference);

            // Проверяем, находится ли поворот в нужном диапазоне (например, 1 градус)
            if (absAngleDifference <= 1f) // Можно заменить 1f на переменную, например maxAimAngle
            {
                CanShootByRotation = true;
                // Debug.Log($"[Building] {gameObject.name}: Accurate aim achieved, can shoot!", this);
            }
            else
            {
                CanShootByRotation = false;
                // Debug.Log($"[Building] {gameObject.name}: Aiming... Angle diff: {absAngleDifference:F2}°", this);
            }
        }

        // Отладочный метод для отображения радиуса обнаружения в 3D
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}