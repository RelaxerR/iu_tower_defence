// Пространство имен должно совпадать с MinHeap.cs
namespace Internal.Scripts.Pathfinding
{
    using System.Collections.Generic;
    using Internal.Scripts.Models; // Предполагается, что Tile находится здесь
    using Internal.Scripts.Bootstrap; // Предполагается, что TileManager находится здесь
    using UnityEngine;

    public static class PathfindingService
    {
        // Определяем, какие типы тайлов считаются проходимыми для врагов
        private static readonly HashSet<Tile.TileType> WalkableTypes = new HashSet<Tile.TileType>
        {
            Tile.TileType.Road,
            Tile.TileType.RoadEnd,
            Tile.TileType.RoadCorner,
            Tile.TileType.Castle, // Добавляем Castle как проходимый для цели
            // Добавьте другие проходимые типы, если нужно (например, Ground)
            // Tile.TileType.Ground,
        };

        public static List<(int x, int z)> FindPath(int startX, int startZ, int endX, int endZ)
        {
            Debug.Log($"PathfindingService: Starting pathfinding from ({startX}, {startZ}) to ({endX}, {endZ}).");

            var tileManager = TileManager.GetInstance();
            if (tileManager?.Tiles == null)
            {
                Debug.LogError("PathfindingService: TileManager instance or Tiles dictionary is null.");
                return null;
            }
            Debug.Log($"PathfindingService: TileManager and Tiles dictionary are valid. Count: {tileManager.Tiles.Count}");

            // Проверяем, являются ли начальный и конечный тайлы проходимыми
            if (!IsWalkable(startX, startZ))
            {
                 Debug.LogWarning($"PathfindingService: Start tile ({startX},{startZ}) is not walkable.");
                 return null; // Невозможно найти путь
            }
             if (!IsWalkable(endX, endZ))
            {
                 Debug.LogWarning($"PathfindingService: End tile ({endX},{endZ}) is not walkable.");
                 return null; // Невозможно найти путь
            }

            Debug.Log("PathfindingService: Start and End tiles are walkable. Proceeding with A*.");

            // --- Реализация A* с использованием MinHeap ---
            var openSet = new MinHeap<(int x, int z), float>(); // (координаты, fScore)
            var closedSet = new HashSet<(int x, int z)>();

            var cameFrom = new Dictionary<(int x, int z), (int x, int z)>();
            var gScore = new Dictionary<(int x, int z), float> { { (startX, startZ), 0 } };
            var fScore = new Dictionary<(int x, int z), float> { { (startX, startZ), Heuristic(startX, startZ, endX, endZ) } };

            openSet.Enqueue((startX, startZ), fScore[(startX, startZ)]);
            int loopCounter = 0; // Счетчик итераций для предотвращения зависания (на всякий случай)

            while (openSet.Count > 0)
            {
                loopCounter++;
                if(loopCounter > 10000) // Ограничение на случай ошибки
                {
                     Debug.LogError("PathfindingService: Search loop exceeded 10000 iterations, aborting.");
                     return null;
                }

                var (current, _) = openSet.Dequeue(); // Извлекаем только элемент, игнорируя fScore
                Debug.Log($"PathfindingService: Dequeued node ({current.x}, {current.z}), gScore: {gScore[current]}, fScore: {fScore[current]}, loop #{loopCounter}");

                if (current.x == endX && current.z == endZ)
                {
                    Debug.Log($"PathfindingService: Goal ({endX}, {endZ}) reached after {loopCounter} iterations!");
                    // Путь найден! Восстановим его.
                    var resultPath = ReconstructPath(cameFrom, current);
                    Debug.Log($"PathfindingService: Path reconstructed, length: {resultPath.Count}");
                    return resultPath;
                }

                closedSet.Add(current);

                // Проверяем соседей (вверх, вниз, влево, вправо)
                foreach (var neighbor in GetNeighbors(current.x, current.z))
                {
                    Debug.Log($"PathfindingService: Checking neighbor ({neighbor.x}, {neighbor.z}) of ({current.x}, {current.z})");

                    if (closedSet.Contains(neighbor))
                    {
                         Debug.Log($"PathfindingService: Neighbor ({neighbor.x}, {neighbor.z}) is in closed set, skipping.");
                         continue;
                    }


                    if (!IsWalkable(neighbor.x, neighbor.z))
                    {
                         Debug.Log($"PathfindingService: Neighbor ({neighbor.x}, {neighbor.z}) is not walkable, skipping.");
                         continue; // Пропускаем непроходимые тайлы
                    }

                    float tentativeGScore = gScore[current] + 1; // Простое расстояние между соседями = 1
                    bool newPathFound = false;

                    if (!gScore.ContainsKey(neighbor))
                    {
                        gScore[neighbor] = tentativeGScore;
                        newPathFound = true;
                        Debug.Log($"PathfindingService: First time visiting ({neighbor.x}, {neighbor.z}), gScore: {tentativeGScore}");
                    }
                    else if (tentativeGScore < gScore[neighbor])
                    {
                        gScore[neighbor] = tentativeGScore;
                        newPathFound = true;
                        Debug.Log($"PathfindingService: Found better path to ({neighbor.x}, {neighbor.z}), updated gScore: {tentativeGScore}");
                    }

                    if(newPathFound)
                    {
                        // Это лучший путь к этому соседу
                        cameFrom[neighbor] = current;
                        fScore[neighbor] = tentativeGScore + Heuristic(neighbor.x, neighbor.z, endX, endZ);
                        openSet.Enqueue(neighbor, fScore[neighbor]); // Добавляем в очередь
                        Debug.Log($"PathfindingService: Added ({neighbor.x}, {neighbor.z}) to open set with fScore: {fScore[neighbor]}");
                    }
                    else
                    {
                        Debug.Log($"PathfindingService: Better path to ({neighbor.x}, {neighbor.z}) already exists, skipping.");
                    }
                }
            }

            // Путь не найден
            Debug.LogWarning($"PathfindingService: Could not find path from ({startX},{startZ}) to ({endX},{endZ}). OpenSet was exhausted after {loopCounter} iterations.");
            return null;
        }

        private static bool IsWalkable(int x, int z)
        {
            var tile = TileManager.GetInstance().Tiles.GetValueOrDefault((x, z));
            bool walkable = tile != null && WalkableTypes.Contains(tile.Type);
            // Debug.Log($"IsWalkable({x}, {z}): {(tile != null ? tile.Type.ToString() : "NULL_TILE")}, Walkable: {walkable}");
            return walkable;
        }

        private static List<(int x, int z)> GetNeighbors(int x, int z)
        {
            return new List<(int x, int z)>
            {
                (x + 1, z),
                (x - 1, z),
                (x, z + 1),
                (x, z - 1)
            };
        }

        private static float Heuristic(int x1, int z1, int x2, int z2)
        {
            // Манхэттенское расстояние
            return Mathf.Abs(x1 - x2) + Mathf.Abs(z1 - z2);
        }

        private static List<(int x, int z)> ReconstructPath(Dictionary<(int x, int z), (int x, int z)> cameFrom, (int x, int z) current)
        {
            Debug.Log("PathfindingService: Reconstructing path...");
            var path = new List<(int x, int z)> { current };
            int count = 0; // Счетчик для безопасности
            while (cameFrom.ContainsKey(current) && count++ < 1000) // Защита от зацикливания
            {
                current = cameFrom[current];
                path.Add(current);
            }
            path.Reverse(); // Путь был построен от цели к началу
            Debug.Log("PathfindingService: Path reconstruction complete.");
            return path;
        }
    }
}