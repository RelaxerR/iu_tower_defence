// Пространство имен должно совпадать с MinHeap.cs
namespace Internal.Scripts.Pathfinding
{
  using System.Collections.Generic;
  using Internal.Scripts.Models; // Предполагается, что Tile находится здесь
  using Internal.Scripts.Bootstrap; // Предполагается, что TileManager находится здесь
  using UnityEngine;

  /// <summary>
  /// Статический класс для поиска пути с использованием алгоритма A*
  /// </summary>
  public static class PathfindingService
  {
    #region Поля и константы

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

    #endregion

    #region Публичные методы

    /// <summary>
    /// Находит путь между двумя точками на карте с использованием алгоритма A*
    /// </summary>
    /// <param name="startX">Координата X начальной точки</param>
    /// <param name="startZ">Координата Z начальной точки</param>
    /// <param name="endX">Координата X конечной точки</param>
    /// <param name="endZ">Координата Z конечной точки</param>
    /// <returns>Список координат пути или null, если путь не найден</returns>
    public static List<(int x, int z)> FindPath(int startX, int startZ, int endX, int endZ)
    {
      var tileManager = TileManager.GetInstance();
      if (tileManager?.Tiles == null)
      {
        Debug.LogError("PathfindingService: Экземпляр TileManager или словарь Tiles равен null.");
        return null;
      }

      // Проверяем, являются ли начальный и конечный тайлы проходимыми
      if (!IsWalkable(startX, startZ))
      {
        Debug.LogWarning($"PathfindingService: Начальный тайл ({startX},{startZ}) непроходим.");
        return null; // Невозможно найти путь
      }
      if (!IsWalkable(endX, endZ))
      {
        Debug.LogWarning($"PathfindingService: Конечный тайл ({endX},{endZ}) непроходим.");
        return null; // Невозможно найти путь
      }

      // --- Реализация A* с использованием MinHeap ---
      var openSet = new MinHeap<(int x, int z), float>(); // (координаты, fScore)
      var closedSet = new HashSet<(int x, int z)>();

      var cameFrom = new Dictionary<(int x, int z), (int x, int z)>();
      var gScore = new Dictionary<(int x, int z), float> { { (startX, startZ), 0 } };
      var fScore = new Dictionary<(int x, int z), float> { { (startX, startZ), Heuristic(startX, startZ, endX, endZ) } };

      openSet.Enqueue((startX, startZ), fScore[(startX, startZ)]);
      var loopCounter = 0; // Счетчик итераций для предотвращения зависания (на всякий случай)

      while (openSet.Count > 0)
      {
        loopCounter++;
        if(loopCounter > 10000) // Ограничение на случай ошибки
        {
          Debug.LogError("PathfindingService: Цикл поиска превысил 10000 итераций, прерывание.");
          return null;
        }

        var (current, _) = openSet.Dequeue(); // Извлекаем только элемент, игнорируя fScore

        if (current.x == endX && current.z == endZ)
        {
          // Путь найден! Восстановим его.
          var resultPath = ReconstructPath(cameFrom, current);
          return resultPath;
        }

        closedSet.Add(current);

        // Проверяем соседей (вверх, вниз, влево, вправо)
        foreach (var neighbor in GetNeighbors(current.x, current.z))
        {
          if (closedSet.Contains(neighbor))
          {
            continue;
          }

          if (!IsWalkable(neighbor.x, neighbor.z))
          {
            continue; // Пропускаем непроходимые тайлы
          }

          var tentativeGScore = gScore[current] + 1; // Простое расстояние между соседями = 1
          var newPathFound = false;

          if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
          {
            gScore[neighbor] = tentativeGScore;
            newPathFound = true;
          }

          if (!newPathFound)
            continue;
          // Это лучший путь к этому соседу
          cameFrom[neighbor] = current;
          fScore[neighbor] = tentativeGScore + Heuristic(neighbor.x, neighbor.z, endX, endZ);
          openSet.Enqueue(neighbor, fScore[neighbor]); // Добавляем в очередь
        }
      }

      // Путь не найден
      Debug.LogWarning($"PathfindingService: Не удалось найти путь от ({startX},{startZ}) до ({endX},{endZ}). OpenSet был исчерпан после {loopCounter} итераций.");
      return null;
    }

    #endregion

    #region Внутренние методы

    /// <summary>
    /// Проверяет, является ли тайл проходимым
    /// </summary>
    /// <param name="x">Координата X тайла</param>
    /// <param name="z">Координата Z тайла</param>
    /// <returns>true если тайл проходим, иначе false</returns>
    private static bool IsWalkable(int x, int z)
    {
      var tile = TileManager.GetInstance().Tiles.GetValueOrDefault((x, z));
      var walkable = tile != null && WalkableTypes.Contains(tile.Type);
      return walkable;
    }

    /// <summary>
    /// Возвращает список соседних тайлов
    /// </summary>
    /// <param name="x">Координата X текущего тайла</param>
    /// <param name="z">Координата Z текущего тайла</param>
    /// <returns>Список координат соседних тайлов</returns>
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

    /// <summary>
    /// Вычисляет эвристическое расстояние между двумя точками (Манхэттенское расстояние)
    /// </summary>
    /// <param name="x1">Координата X первой точки</param>
    /// <param name="z1">Координата Z первой точки</param>
    /// <param name="x2">Координата X второй точки</param>
    /// <param name="z2">Координата Z второй точки</param>
    /// <returns>Эвристическое расстояние между точками</returns>
    private static float Heuristic(int x1, int z1, int x2, int z2)
    {
      // Манхэттенское расстояние
      return Mathf.Abs(x1 - x2) + Mathf.Abs(z1 - z2);
    }

    /// <summary>
    /// Восстанавливает путь из словаря cameFrom
    /// </summary>
    /// <param name="cameFrom">Словарь, содержащий информацию о предыдущих узлах</param>
    /// <param name="current">Текущая конечная точка</param>
    /// <returns>Список координат пути</returns>
    private static List<(int x, int z)> ReconstructPath(Dictionary<(int x, int z), (int x, int z)> cameFrom, (int x, int z) current)
    {
      var path = new List<(int x, int z)> { current };
      var count = 0; // Счетчик для безопасности
      while (cameFrom.ContainsKey(current) && count++ < 1000) // Защита от зацикливания
      {
        current = cameFrom[current];
        path.Add(current);
      }
      path.Reverse(); // Путь был построен от цели к началу
      return path;
    }

    #endregion
  }
}