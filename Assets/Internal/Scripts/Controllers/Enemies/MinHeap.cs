// Убедитесь, что пространство имен такое же, как в PathfindingService.cs
namespace Internal.Scripts.Pathfinding 
{
    using System;
    using System.Collections.Generic;
    using UnityEngine; // Для Debug.Log

    // Простая реализация Min-Heap для пары (TElement, TPriority)
    public class MinHeap<TElement, TPriority> where TPriority : IComparable<TPriority>
    {
        private List<(TElement element, TPriority priority)> _heap = new List<(TElement, TPriority)>();

        public int Count => _heap.Count;

        public void Enqueue(TElement element, TPriority priority)
        {
            // // Debug.Log($"MinHeap: Enqueuing element {element}, priority {priority}, before count: {_heap.Count}");
            _heap.Add((element, priority));
            SiftUp(_heap.Count - 1);
            // // Debug.Log($"MinHeap: After enqueue, heap count: {_heap.Count}");
        }

        public (TElement element, TPriority priority) Dequeue()
        {
            if (Count == 0)
                throw new InvalidOperationException("Heap is empty");

            // // Debug.Log($"MinHeap: Dequeuing, before count: {_heap.Count}");
            var root = _heap[0];
            var lastElement = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);

            if (_heap.Count > 0)
            {
                _heap[0] = lastElement;
                SiftDown(0);
            }
            // // Debug.Log($"MinHeap: After dequeue, heap count: {_heap.Count}, dequeued element: {root.element}, priority: {root.priority}");
            return root;
        }

        private void SiftUp(int index)
        {
            // // Debug.Log($"MinHeap: SiftUp called for index {index}");
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (_heap[index].priority.CompareTo(_heap[parentIndex].priority) >= 0)
                {
                    // // Debug.Log($"MinHeap: SiftUp stopping at index {index}, parent priority is lower or equal.");
                    break;
                }

                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void SiftDown(int index)
        {
            // // Debug.Log($"MinHeap: SiftDown called for index {index}");
            while (true)
            {
                int smallest = index;
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;

                if (leftChild < _heap.Count && _heap[leftChild].priority.CompareTo(_heap[smallest].priority) < 0)
                {
                    smallest = leftChild;
                    // // Debug.Log($"MinHeap: Left child {leftChild} has smaller priority, considering it as smallest.");
                }
                if (rightChild < _heap.Count && _heap[rightChild].priority.CompareTo(_heap[smallest].priority) < 0)
                {
                    smallest = rightChild;
                    // // Debug.Log($"MinHeap: Right child {rightChild} has smaller priority, considering it as smallest.");
                }

                if (smallest == index)
                {
                    // // Debug.Log($"MinHeap: SiftDown stopping at index {index}, it's the smallest.");
                    break;
                }

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int i, int j)
        {
            var temp = _heap[i];
            _heap[i] = _heap[j];
            _heap[j] = temp;
            // // Debug.Log($"MinHeap: Swapped indices {i} and {j}");
        }
    }
}