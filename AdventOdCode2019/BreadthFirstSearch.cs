using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOdCode2019
{
    public class BreadthFirstSearch<TElement>
    {
        private readonly Func<TElement, IEnumerable<TElement>> _moveFunc;

        public BreadthFirstSearch(Func<TElement, IEnumerable<TElement>> moveFunc)
        {
            _moveFunc = moveFunc;
        }

        public int GetShortestPathLength(TElement initialElement, Func<TElement, bool> isFinishFunc)
        {
            var queue = new Queue<SearchElement<TElement>>();
            var visited = new List<TElement>();

            queue.Enqueue(new SearchElement<TElement>(initialElement, 0));

            while (queue.Any())
            {
                var current = queue.Dequeue();

                if (isFinishFunc(current.Element))
                    return current.Depth;

                visited.Add(current.Element);

                var newElements = _moveFunc(current.Element)
                    .Where(x => !visited.Contains(x))
                    .Select(x => new SearchElement<TElement>(x, current.Depth + 1));

                foreach (var searchElement in newElements)
                    queue.Enqueue(searchElement);
            }

            return -1;
        }

        public int GetShortestPathLength(TElement initialElement, Func<TElement, int, bool> isFinishFunc)
        {
            var queue = new Queue<SearchElement<TElement>>();
            var visited = new List<TElement>();

            queue.Enqueue(new SearchElement<TElement>(initialElement, 0));

            while (queue.Any())
            {
                var current = queue.Dequeue();

                if (isFinishFunc(current.Element, current.Depth))
                    return current.Depth;

                visited.Add(current.Element);

                var newElements = _moveFunc(current.Element)
                    .Where(x => !visited.Contains(x))
                    .Select(x => new SearchElement<TElement>(x, current.Depth + 1));

                foreach (var searchElement in newElements)
                    queue.Enqueue(searchElement);
            }

            return -1;
        }

        private class SearchElement<T>
        {
            public SearchElement(T element, int depth)
            {
                Element = element;
                Depth = depth;
            }

            public T Element { get; }

            public int Depth { get; }
        }
    }
}