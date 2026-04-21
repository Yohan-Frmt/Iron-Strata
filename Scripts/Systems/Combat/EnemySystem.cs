using System;
using System.Collections.Generic;
using Godot;
using IronStrata.Scripts.Components.Character;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Render;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;
using IronStrata.Scripts.Registry;

namespace IronStrata.Scripts.Systems.Combat;

/// <summary>
/// System responsible for enemy AI behavior, including movement, targeting, and combat.
/// It also handles the spawning of enemy hordes when in certain zones.
/// </summary>
public class EnemySystem(Node3D trainRoot) : ISystem {
    private float _hordeTimer;
    private const float _hordeSpawnInterval = 1f;

    /// <summary>
    /// Updates the state of the enemy system on each frame, executing enemy behavior,
    /// managing horde spawn timers, and resolving interactions between enemies and their targets.
    /// </summary>
    /// <param name="world">The global game world that manages entities and components.</param>
    /// <param name="delta">The time elapsed since the last update, used to calculate time-dependent changes.</param>
    public void Update(World world, double delta) {
        Option<Entity> locationEntityOption = world.QueryFirst<LocationComponent>();
        bool isInCity = false;
        if (locationEntityOption.IsSome) { isInCity = world.Get<LocationComponent>(locationEntityOption.Unwrap()).IsInCityZone; }

        if (isInCity) {
            _hordeTimer += (float)delta;
            if (_hordeTimer >= _hordeSpawnInterval) {
                _hordeTimer = 0f;
                foreach (HordeSpawnRule rule in EnemyRegistry.SpawnRules) {
                    if (GD.Randf() <= rule.Chance) { SpawnHorde(world, rule.Count, rule.Type); }
                }
            }
        }
        else { _hordeTimer = 0f; }

        List<Entity> wagons = [.. world.Query<WagonSlotComponent, WagonTypeComponent, HealthComponent>()];
        if (wagons.Count == 0) { return; }

        List<Entity> allEnemies = [.. world.Query<EnemyComponent, PositionComponent>()];

        foreach (Entity entity in world.Query<EnemyComponent, PositionComponent, MovementComponent>()) {
            ref EnemyComponent enemy = ref world.Get<EnemyComponent>(entity);
            ref PositionComponent positionComponent = ref world.Get<PositionComponent>(entity);
            ref MovementComponent movementComponent = ref world.Get<MovementComponent>(entity);
            bool needsNewTarget = enemy.CurrentTarget.Match(target => !world.IsAlive(target), () => true);
            if (needsNewTarget) { enemy.CurrentTarget = FindBestTarget(enemy.Type, wagons, world); }

            if (enemy.CurrentTarget.IsNone) { continue; }

            Entity target = enemy.CurrentTarget.Unwrap();
            ref WagonSlotComponent wagonSlotComponent = ref world.Get<WagonSlotComponent>(target);
            const float wagonSize = 5f;
            const float wagonPhysicalRadius = 4.0f;
            Vector3 targetGlobalPosition = trainRoot.GlobalPosition + new Vector3(-wagonSlotComponent.SlotIndex * wagonSize, 2.0f, 0);
            float distanceToTargetCenter = positionComponent.Value.DistanceTo(targetGlobalPosition);
            if (distanceToTargetCenter <= enemy.AttackRange + wagonPhysicalRadius) {
                enemy.AttackTimer -= (float)delta;
                if (!(enemy.AttackTimer <= 0)) { continue; }

                enemy.AttackTimer = 1f / enemy.AttackSpeed;
                ref HealthComponent health = ref world.Get<HealthComponent>(target);
                health.Current -= enemy.Damage;
                if (health.Current <= 0) { HandleWagonDestruction(world, target); }
            }
            else {
                Vector3 moveDirection = (targetGlobalPosition - positionComponent.Value).Normalized();
                Vector3 repulsionForce = Vector3.Zero;
                int neighbors = 0;
                foreach (Entity otherEntity in allEnemies) {
                    if (entity.Equals(otherEntity)) { continue; }

                    Vector3 otherPosition = world.Get<PositionComponent>(otherEntity).Value;
                    float distanceToOtherEnemy = positionComponent.Value.DistanceTo(otherPosition);
                    const float separationRadius = 10.0f;
                    if (!(distanceToOtherEnemy < separationRadius) || !(distanceToOtherEnemy > 0.01f)) { continue; }

                    Vector3 pushDirection = (positionComponent.Value - otherPosition).Normalized();
                    repulsionForce += pushDirection * (separationRadius - distanceToOtherEnemy);
                    neighbors++;
                }

                foreach (Entity wagon in wagons) {
                    ref WagonSlotComponent wagonSlot = ref world.Get<WagonSlotComponent>(wagon);
                    Vector3 wagonPosition = trainRoot.GlobalPosition + new Vector3(-wagonSlot.SlotIndex * wagonSize, 2.0f, 0);
                    float distanceToWagon = positionComponent.Value.DistanceTo(wagonPosition);
                    if (!(distanceToWagon < wagonPhysicalRadius + 0.5f)) { continue; }

                    Vector3 pushDirection = (positionComponent.Value - wagonPosition).Normalized();
                    repulsionForce += pushDirection * (wagonPhysicalRadius + 0.5f - distanceToWagon) * 5.0f;
                    neighbors++;
                }

                if (neighbors > 0) {
                    repulsionForce /= neighbors;
                    moveDirection = (moveDirection + repulsionForce * 1.5f).Normalized();
                }

                positionComponent.Value += moveDirection * movementComponent.Speed * (float)delta;
                if (movementComponent.IsFlying) { continue; }

                positionComponent.Value.Y -= 9.81f * (float)delta;
                if (positionComponent.Value.Y < 0) {
                    positionComponent.Value.Y = 0;
                }
            }
        }
    }

    /// <summary>
    /// Handles the destruction of a wagon in the train, resolving its removal, the detachment
    /// of connected wagons further back, and the abandonment of these detached wagons.
    /// Additionally, terminates the game if the locomotive (frontmost wagon) is destroyed.
    /// </summary>
    /// <param name="world">The global game world that manages entities and components.</param>
    /// <param name="target">The entity representing the wagon that was hit and triggered the destruction process.</param>
    private void HandleWagonDestruction(World world, Entity target) {
        ref WagonSlotComponent hitSlot = ref world.Get<WagonSlotComponent>(target);
        GD.Print($"Wagon at index {hitSlot.SlotIndex} (layer {hitSlot.Layer}) destroyed!");

        if (hitSlot.SlotIndex == 0) { GD.PrintErr("!!! GAME OVER - LOCOMOTIVE DESTROYED !!!"); }

        List<Entity> allWagonEntities = [.. world.Query<WagonSlotComponent>()];
        foreach (Entity wagonEntity in allWagonEntities) {
            if (!world.IsAlive(wagonEntity)) { continue; }

            ref WagonSlotComponent wagonSlot = ref world.Get<WagonSlotComponent>(wagonEntity);
            if (wagonSlot.SlotIndex == hitSlot.SlotIndex && wagonSlot.Layer >= hitSlot.Layer) { DestroyWagon(world, wagonEntity); }
            else if (wagonSlot.SlotIndex > hitSlot.SlotIndex) { AbandonWagon(world, wagonEntity, trainRoot); }
        }
    }

    /// <summary>
    /// Removes the specified wagon entity from the game world, releasing any associated resources
    /// and destroying the entity.
    /// </summary>
    /// <param name="world">The game world containing the entity and its associated components.</param>
    /// <param name="wagonEntity">The wagon entity to be destroyed.</param>
    private static void DestroyWagon(World world, Entity wagonEntity) {
        if (world.Has<RenderableComponent>(wagonEntity)) { world.Get<RenderableComponent>(wagonEntity).Node?.QueueFree(); }
        world.DestroyEntity(wagonEntity);
    }

    /// <summary>
    /// Detaches a specified entity's associated node from its parent, reattaches it to the main scene,
    /// and schedules it for cleanup after a delay. The method also ensures that the entity is properly
    /// removed from the world.
    /// </summary>
    /// <param name="world">The global game world that manages entities and their components.</param>
    /// <param name="entity">The entity to be detached and cleaned up.</param>
    /// <param name="nodeRoot">The root node of the train, used for reattaching the entity's node.</param>
    private static void AbandonWagon(World world, Entity entity, Node3D nodeRoot) {
        if (world.Has<RenderableComponent>(entity)) {
            Node3D node = world.Get<RenderableComponent>(entity).Node;
            if (node != null) {
                Transform3D globalTransform = node.GlobalTransform;
                node.GetParent()?.RemoveChild(node);
                nodeRoot.GetTree().CurrentScene.AddChild(node);
                node.GlobalTransform = globalTransform;
                SceneTreeTimer timer = node.GetTree().CreateTimer(6.0f);
                timer.Timeout += () => {
                    if (GodotObject.IsInstanceValid(node)) { node.QueueFree(); }
                };
            }
        }

        world.DestroyEntity(entity);
    }

    /// <summary>
    /// Evaluates the available wagons and selects the most suitable target for the given enemy type.
    /// </summary>
    /// <param name="enemyType">The type of the enemy, used to determine targeting preferences and scoring.</param>
    /// <param name="wagons">A list of wagons available for target evaluation.</param>
    /// <param name="world">The global game world containing the entities and components necessary for processing wagons.</param>
    /// <returns>An optional entity representing the best target for the enemy, or none if no suitable target is found.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the enemy type is not recognized.</exception>
    private static Option<Entity> FindBestTarget(EnemyType enemyType, List<Entity> wagons, World world) {
        Option<Entity> bestTarget = Option<Entity>.None;
        float bestScore = -99999f;

        foreach (Entity wagon in wagons) {
            ref WagonTypeComponent wagonTypeComponent = ref world.Get<WagonTypeComponent>(wagon);
            ref WagonSlotComponent wagonSlotComponent = ref world.Get<WagonSlotComponent>(wagon);
            float score = GetDefaultTypePriority(wagonTypeComponent.Type) + wagonSlotComponent.Layer * 50;
            switch (enemyType) {
                case EnemyType.Safeguard:
                    score += wagonSlotComponent.Layer * 10000;
                    break;
                case EnemyType.Wasp:
                    switch (wagonTypeComponent.Type) {
                        case WagonType.Combat:
                            score += 5000;
                            break;
                        case WagonType.Locomotive:
                        case WagonType.Living:
                        case WagonType.Storage:
                        case WagonType.Research:
                        case WagonType.Medical:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(enemyType));
                    }

                    break;
                case EnemyType.Crawler:
                    switch (wagonTypeComponent.Type) {
                        case WagonType.Living:
                            score += 5000;
                            break;
                        case WagonType.Research:
                            score += 4000;
                            break;
                        case WagonType.Locomotive:
                        case WagonType.Combat:
                        case WagonType.Storage:
                        case WagonType.Medical:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(enemyType));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, null);
            }

            if (!(score > bestScore)) { continue; }

            bestScore = score;
            bestTarget = Option<Entity>.Some(wagon);
        }

        return bestTarget;
    }

    /// <summary>
    /// Determines the default priority score for a given wagon type, used in enemy targeting
    /// and decision-making processes.
    /// </summary>
    /// <param name="type">The type of the wagon whose default priority is being calculated.</param>
    /// <returns>A float representing the base priority score of the specified wagon type.</returns>
    private static float GetDefaultTypePriority(WagonType type) =>
        type switch {
            WagonType.Combat => 400,
            WagonType.Living => 300,
            WagonType.Research => 200,
            _ => 100
        };

    /// <summary>
    /// Spawns a horde of enemies in the game world, creating entities with appropriate components such as
    /// position, movement, health, and enemy-specific attributes.
    /// </summary>
    /// <param name="world">The global game world that manages entities and components.</param>
    /// <param name="count">The number of enemies to spawn in the horde.</param>
    /// <param name="type">The type of enemy to spawn, determining their behavior and attributes.</param>
    private void SpawnHorde(World world, int count, EnemyType type) {
        EnemyDefinition enemyDefinition = EnemyRegistry.EnemyDefs[type];
        float angle = GD.Randf() * Mathf.Tau;
        const float distance = 80f;
        Vector3 hordeEpicenter = trainRoot.GlobalPosition +
                                 new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);

        for (int enemyIndex = 0; enemyIndex < count; enemyIndex++) {
            Entity enemyEntity = world.CreateEntity();

            Vector3 randomOffset = new(
                (GD.Randf() - 0.5f) * enemyDefinition.DispersionRadius * 2f,
                enemyDefinition.IsFlying ? 5.0f : 0f,
                (GD.Randf() - 0.5f) * enemyDefinition.DispersionRadius * 2f
            );

            world.Add(enemyEntity, new PositionComponent { Value = hordeEpicenter + randomOffset });
            world.Add(enemyEntity, new MovementComponent { Speed = enemyDefinition.Speed + GD.Randf() * 2f, IsFlying = enemyDefinition.IsFlying });
            world.Add(enemyEntity, new HealthComponent { Max = enemyDefinition.Health, Current = enemyDefinition.Health });
            world.Add(
                enemyEntity,
                new EnemyComponent {
                    Type = enemyDefinition.Type,
                    Damage = enemyDefinition.Damage,
                    AttackRange = enemyDefinition.AttackRange,
                    AttackSpeed = enemyDefinition.AttackSpeed
                }
            );
        }
    }
}
