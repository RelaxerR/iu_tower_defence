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
                return;
            }

            // В 3D вектор направления вычисляется как обычно
            Vector3 directionToTarget = nearestTarget.position - transform.position;

            // Для поворота вокруг Y (ось вверх), игнорируем X и Z, если нужно поворот только по горизонтали
            // Но обычно в 3D поворот в сторону цели делается по всем осям, поэтому оставим как есть
            // Если нужно, чтобы башня поворачивалась только по Y (например, в плоскости Y), то:
            directionToTarget.y = 0f; // <-- Закомментируйте, если нужно 3D поворот во всех осях

            if (directionToTarget.sqrMagnitude == 0f)
            {
                Debug.LogWarning($"[Building] {gameObject.name}: Target is exactly on top of the building, cannot rotate.", this);
                return;
            }

            // Вычисляем направление взгляда (forward) в сторону цели
            // Для этого используем LookRotation
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

            // Плавный поворот
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Логируем поворот
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
            if (angleDifference > 1f)
            {
                // Debug.Log($"[Building] {gameObject.name}: Rotating towards {nearestTarget.name}. Angle difference: {angleDifference:F2} degrees", this);
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