using System;
using System.Collections;
using System.Collections.Generic;
using DoublyLinkedList;

namespace MyCollections.CircularDoublyLinkedList
{
    public class CircularDoublyLinkedList<T> : IEnumerable<T>
    {
        public CircularDoublyLinkedListNode<T>? First { get; private set; }
        public CircularDoublyLinkedListNode<T>? Last => First?.Previous;
        public int Length { get; private set; }

        public CircularDoublyLinkedList() { }

        public CircularDoublyLinkedList(IEnumerable<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            foreach (T item in collection)
            {
                AddLast(item);
            }
        }

        public CircularDoublyLinkedListNode<T> AddLast(T value)
        {
            var newNode = new CircularDoublyLinkedListNode<T>(this, value);

            if (First == null)
            {
                AddToEmptyListInternal(newNode);
            }
            else
            {
                AddAfterInternal(Last!, newNode);
            }

            return newNode;
        }

        public CircularDoublyLinkedListNode<T> AddLast(CircularDoublyLinkedListNode<T>? newNode)
        {
            ValidateNewNode(newNode);

            if (First == null)
            {
                AddToEmptyListInternal(newNode!);
            }
            else
            {
                AddAfterInternal(Last!, newNode!);
            }

            newNode!.List = this;
            return newNode;
        }

        public CircularDoublyLinkedListNode<T> AddFirst(T? value)
        {
            var newNode = new CircularDoublyLinkedListNode<T>(this, value);

            if (First == null)
            {
                AddToEmptyListInternal(newNode);
            }
            else
            {
                AddAfterInternal(Last!, newNode);
                First = newNode;
            }

            return newNode;
        }

        public CircularDoublyLinkedListNode<T> AddFirst(CircularDoublyLinkedListNode<T>? newNode)
        {
            ValidateNewNode(newNode);

            if (First == null)
            {
                AddToEmptyListInternal(newNode!);
            }
            else
            {
                AddAfterInternal(Last!, newNode!);
                First = newNode;
            }

            newNode!.List = this;
            return newNode;
        }

        public CircularDoublyLinkedListNode<T> AddAfter(CircularDoublyLinkedListNode<T>? node, T? value)
        {
            ValidateNode(node);
            var newNode = new CircularDoublyLinkedListNode<T>(this, value);

            AddAfterInternal(node!, newNode);
            return newNode;
        }

        public CircularDoublyLinkedListNode<T> AddAfter(CircularDoublyLinkedListNode<T>? node, CircularDoublyLinkedListNode<T>? newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            AddAfterInternal(node!, newNode!);
            newNode!.List = this;
            return newNode;
        }

        public CircularDoublyLinkedListNode<T> AddBefore(CircularDoublyLinkedListNode<T>? node, T? value)
        {
            ValidateNode(node);
            var newNode = new CircularDoublyLinkedListNode<T>(this, value);

            AddAfterInternal(node!.Previous!, newNode);
            if (node == First)
            {
                First = newNode;
            }
            return newNode;
        }

        public CircularDoublyLinkedListNode<T> AddBefore(CircularDoublyLinkedListNode<T>? node, CircularDoublyLinkedListNode<T>? newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            AddAfterInternal(node!.Previous!, newNode);
            newNode.List = this;
            if (node == First)
            {
                First = newNode;
            }
            return newNode;
        }

        public bool Remove(T? value)
        {
            var nodeToRemove = Find(value);
            if(nodeToRemove != null)
            {
                RemoveNodeInternal(nodeToRemove);
                return true;
            }

            return false;
        }

        public void Remove(CircularDoublyLinkedListNode<T>? nodeToRemove)
        {
            ValidateNode(nodeToRemove);
            RemoveNodeInternal(nodeToRemove);
        }

        public void RemoveFirst()
        {
            if (First == null)
                throw new InvalidOperationException("The DoublyLinkedList is empty");

            RemoveNodeInternal(First);
        }

        public void RemoveLast()
        {
            if (First == null)
                throw new InvalidOperationException("The DoublyLinkedList is empty");

            RemoveNodeInternal(Last);
        }

        public void Clear()
        {
            CircularDoublyLinkedListNode<T>? current = First;
            while (current != null)
            {
                CircularDoublyLinkedListNode<T> temp = current;
                current = current.Next;
                temp.Invalidate();
            }

            First = null;
            Length = 0;
        }

        public bool Contains(T? value) => Find(value) != null;

        public CircularDoublyLinkedListNode<T>? Find(T? value)
        {
            var node = First;
            var comparer = EqualityComparer<T>.Default;

            if (node != null)
            {
                if (value != null)
                {
                    CircularDoublyLinkedListNode<T>? nextNode;
                    do
                    {
                        if (comparer.Equals(node!.Value, value))
                        {
                            return node;
                        }
                        nextNode = node.Next;
                        node = nextNode;

                    } while (nextNode != First);
                }
                else
                {
                    do
                    {
                        if (node!.Value == null)
                        {
                            return node;
                        }
                        node = node.Next;

                    } while (node != First);
                }
            }
            return null;
        }

        public CircularDoublyLinkedListNode<T>? FindLast(T? value)
        {
            var node = First;
            var comparer = EqualityComparer<T>.Default;
            CircularDoublyLinkedListNode<T>? resultNode = null;

            if (node != null)
            {
                if (value != null)
                {
                    CircularDoublyLinkedListNode<T>? nextNode;
                    do
                    {
                        if (comparer.Equals(node!.Value, value))
                        {
                            resultNode = node;
                        }
                        nextNode = node.Next;
                        node = nextNode;

                    } while (nextNode != First);

                    return resultNode;
                }
                else
                {
                    do
                    {
                        if (node!.Value == null)
                        {
                            resultNode = node;
                        }
                        node = node.Next;

                    } while (node != First);

                    return resultNode;
                }
            }
            return null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new CircularDoublyLinkedListEnumerator<T>(this);
        }

        private void AddAfterInternal(CircularDoublyLinkedListNode<T> node, CircularDoublyLinkedListNode<T> newNode)
        {
            newNode.Next = node.Next;
            newNode.Previous = node;
            node.Next!.Previous = newNode;
            node.Next = newNode;
            Length++;
        }

        private void AddToEmptyListInternal(CircularDoublyLinkedListNode<T> newNode)
        {
            newNode.Next = newNode;
            newNode.Previous = newNode;
            First = newNode;
            Length++;
        }

        private void RemoveNodeInternal(CircularDoublyLinkedListNode<T>? nodeToRemove)
        {
            if (nodeToRemove!.Next == nodeToRemove)
            {
                First = null;
            }
            else
            {
                nodeToRemove.Next!.Previous = nodeToRemove.Previous;
                nodeToRemove.Previous!.Next = nodeToRemove.Next;

                if (First == nodeToRemove)
                {
                    First = nodeToRemove.Next;
                }
            }

            nodeToRemove.Invalidate();
            Length--;
        }

        private static void ValidateNewNode(CircularDoublyLinkedListNode<T>? node)
        {
            ArgumentNullException.ThrowIfNull(node);

            if (node.List != null)
            {
                throw new InvalidOperationException("Node is already part of a different DoublyLinkedList");
            }
        }

        private void ValidateNode(CircularDoublyLinkedListNode<T>? node)
        {
            ArgumentNullException.ThrowIfNull(node);

            if (node.List != this)
            {
                throw new InvalidOperationException("The node does not belong to the current DoublyLinkedList");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public sealed class CircularDoublyLinkedListNode<T>
    {
        public CircularDoublyLinkedList<T>? List { get; internal set; }
        public CircularDoublyLinkedListNode<T>? Next { get; internal set; }
        public CircularDoublyLinkedListNode<T>? Previous { get; internal set; }
        public T? Value { get; }

        internal CircularDoublyLinkedListNode(CircularDoublyLinkedList<T> list, T? value)
        {
            List = list;
            Value = value;
        }

        public CircularDoublyLinkedListNode(T? value)
        {
            Value = value;
        }

        public void Invalidate()
        {
            List = null;
            Next = null;
            Previous = null;
        }
    }
}