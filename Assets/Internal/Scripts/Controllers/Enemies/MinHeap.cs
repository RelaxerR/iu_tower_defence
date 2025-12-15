// Убедитесь, что пространство имен такое же, как в PathfindingService.cs
namespace Internal.Scripts.Pathfinding 
{
  using System;
  using System.Collections.Generic;
  using UnityEngine; // Для Debug.Log

  /// <summary>
  /// Простая реализация Min-Heap для пары (TElement, TPriority)
  /// </summary>
  /// <typeparam name="TElement">Тип элемента в куче</typeparam>
  /// <typeparam name="TPriority">Тип приоритета, реализующий IComparable</typeparam>
  public class MinHeap<TElement, TPriority> where TPriority : IComparable<TPriority>
  {
    #region Поля

    private readonly List<(TElement element, TPriority priority)> _heap = new();

    #endregion

    #region Свойства

    /// <summary>
    /// Возвращает количество элементов в куче
    /// </summary>
    public int Count
    {
      get => _heap.Count;
    }

    #endregion

    #region Методы

    /// <summary>
    /// Добавляет элемент в кучу с указанным приоритетом
    /// </summary>
    /// <param name="element">Элемент для добавления</param>
    /// <param name="priority">Приоритет элемента</param>
    public void Enqueue(TElement element, TPriority priority)
    {
      _heap.Add((element, priority));
      SiftUp(_heap.Count - 1);
    }

    /// <summary>
    /// Извлекает элемент с наименьшим приоритетом из кучи
    /// </summary>
    /// <returns>Кортеж с элементом и его приоритетом</returns>
    /// <exception cref="InvalidOperationException">Если куча пуста</exception>
    public (TElement element, TPriority priority) Dequeue()
    {
      if (Count == 0)
        throw new InvalidOperationException("Куча пуста");

      var root = _heap[0];
      var lastElement = _heap[^1];
      _heap.RemoveAt(_heap.Count - 1);

      if (_heap.Count <= 0)
        return root;
      _heap[0] = lastElement;
      SiftDown(0);
      return root;
    }

    /// <summary>
    /// Поднимает элемент вверх по куче до правильной позиции
    /// </summary>
    /// <param name="index">Индекс элемента для поднятия</param>
    private void SiftUp(int index)
    {
      while (index > 0)
      {
        var parentIndex = (index - 1) / 2;
        if (_heap[index].priority.CompareTo(_heap[parentIndex].priority) >= 0)
        {
          break;
        }

        Swap(index, parentIndex);
        index = parentIndex;
      }
    }

    /// <summary>
    /// Опускает элемент вниз по куче до правильной позиции
    /// </summary>
    /// <param name="index">Индекс элемента для опускания</param>
    private void SiftDown(int index)
    {
      while (true)
      {
        var smallest = index;
        var leftChild = 2 * index + 1;
        var rightChild = 2 * index + 2;

        if (leftChild < _heap.Count && _heap[leftChild].priority.CompareTo(_heap[smallest].priority) < 0)
        {
          smallest = leftChild;
        }
        if (rightChild < _heap.Count && _heap[rightChild].priority.CompareTo(_heap[smallest].priority) < 0)
        {
          smallest = rightChild;
        }

        if (smallest == index)
        {
          break;
        }

        Swap(index, smallest);
        index = smallest;
      }
    }

    /// <summary>
    /// Меняет местами два элемента в куче
    /// </summary>
    /// <param name="i">Индекс первого элемента</param>
    /// <param name="j">Индекс второго элемента</param>
    private void Swap(int i, int j)
    {
      (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
    }

    #endregion
  }
}