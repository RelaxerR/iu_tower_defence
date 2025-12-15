using UnityEngine;
using Internal.Scripts.Controllers.Enemies;
using Internal.Scripts.Controllers.Projectiles;
using Unity.VisualScripting;

namespace Internal.Scripts.Controllers.Buildings
{
    public class BuildingCrossbow : Building
    {
        [Header("Shooting Configuration")]
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private GameObject projectilePrefab = null;
        [SerializeField] private Transform shootPoint = null;

        private float nextFireTime = 0f;

        protected override void Update()
        {
            // Debug.Log($"[BuildingCrossbow] {gameObject.name}: Update called. NearestTarget: {(nearestTarget != null ? nearestTarget.name : "null")}, Time: {Time.time}, NextFireTime: {nextFireTime}", this);

            base.Update(); // Вызываем родительскую логику (поиск цели, поворот)

            // Debug.Log($"[BuildingCrossbow] {gameObject.name}: After base.Update(), NearestTarget: {(nearestTarget != null ? nearestTarget.name : "null")}", this);

            if (nearestTarget != null)
            {
                // Debug.Log($"[BuildingCrossbow] {gameObject.name}: Has target {nearestTarget.name}, checking fire cooldown...", this);

                if (Time.time >= nextFireTime)
                {
                    // Debug.Log($"[BuildingCrossbow] {gameObject.name}: Fire cooldown ready ({Time.time} >= {nextFireTime}), attempting to shoot.", this);
                    if (CanShootByRotation) TryShoot();
                }
                else
                {
                    // Debug.Log($"[BuildingCrossbow] {gameObject.name}: Still cooling down. Next fire at {nextFireTime}, current time: {Time.time}", this);
                }
            }
            else
            {
                // Debug.Log($"[BuildingCrossbow] {gameObject.name}: No target to shoot at.", this);
            }
        }

        private void TryShoot()
        {
            // Debug.Log($"[BuildingCrossbow] {gameObject.name}: Trying to shoot at {nearestTarget?.name ?? "null"}", this);

            if (nearestTarget == null)
            {
                Debug.LogWarning("[BuildingCrossbow] Cannot shoot: no target.", this);
                return;
            }

            // Force reset cooldown for testing
            nextFireTime = Time.time - 1f; // This ensures we can shoot immediately
    
            float distanceToTarget = Vector3.Distance(transform.position, nearestTarget.position);
            // Debug.Log($"[BuildingCrossbow] {gameObject.name}: Distance to target {nearestTarget.name}: {distanceToTarget:F2}, Detection Radius: {detectionRadius}", this);

            if (distanceToTarget <= detectionRadius)
            {
                // Debug.Log($"[BuildingCrossbow] {gameObject.name}: Target {nearestTarget.name} is in range, shooting now!", this);
                Shoot(nearestTarget);
                nextFireTime = Time.time + (1f / fireRate);
                // Debug.Log($"[BuildingCrossbow] {gameObject.name}: Next shot scheduled at {nextFireTime}", this);
            }
            else
            {
                // Debug.Log($"[BuildingCrossbow] {gameObject.name}: Target {nearestTarget.name} is out of range ({distanceToTarget:F2} > {detectionRadius}).", this);
            }
        }

        private void Shoot(Transform target)
        {
            // Debug.Log($"[BuildingCrossbow] {gameObject.name}: Shooting at {target.name}", this);

            if (projectilePrefab == null)
            {
                Debug.LogError("[BuildingCrossbow] Projectile prefab is not assigned!", this);
                return;
            }

            Vector3 spawnPosition = shootPoint != null ? shootPoint.position : transform.position;
            // Debug.Log($"[BuildingCrossbow] {gameObject.name}: Spawning projectile at {spawnPosition}", this);

            GameObject newProjectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            // Debug.Log($"[BuildingCrossbow] {gameObject.name}: Projectile instantiated: {newProjectile.name}", this);

            var projectileScript = newProjectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetTarget(target);
                // Debug.Log($"[BuildingCrossbow] {gameObject.name}: Set target {target.name} on projectile.", this);
            }
            else
            {
                Debug.LogWarning("[BuildingCrossbow] Projectile prefab does not have a Projectile component!", this);
            }

            // Debug.Log($"[BuildingCrossbow] {gameObject.name}: Shot fired successfully at {target.name}", this);
        }
    }
}