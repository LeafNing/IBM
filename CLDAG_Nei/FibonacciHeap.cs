using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LDAG
{
    class HeapNode
    {
        public int Key;
        public bool isMarked = false;
        public int numOfChild = 0;

        public HeapNode parent;
        public HeapNode left;
        public HeapNode right;
        public HeapNode child;
    }

    class FibonacciHeap
    {
        public HeapNode maxNode;
        public double[] data;
        HeapNode[] index;
        int n;

        HeapNode[] degree;
        public FibonacciHeap(int n)
        {
            this.n = n;
            data = new double[n];
            index = new HeapNode[n];
            degree = new HeapNode[(int)(Math.Log(n) / Math.Log((1 + Math.Sqrt(5)) / 2)) + 1];
        }

        public void insert(HeapNode node)
        {
            if (maxNode == null)
            {
                maxNode = node;
                node.left = node;
                node.right = node;
            }
            else
            {
                HeapNode left = maxNode.left;
                left.right = node;
                node.left = left;
                node.right = maxNode;
                maxNode.left = node;
            }
        }

        public void insert(int key, double value)
        {
            data[key] = value;
            HeapNode node = new HeapNode();
            node.Key = key;
            index[key] = node;

            insert(node);

            if (maxNode == null || value > data[maxNode.Key])
                maxNode = node;
        }

        public void deletaMax()
        {
            data[maxNode.Key] = 0;
            index[maxNode.Key] = null;

            HeapNode firstChild = maxNode.child;
            HeapNode maxleft = maxNode.left;
            HeapNode maxright = maxNode.right;

            if (maxleft == maxNode)
            {
                maxright = maxNode.child;
                if (maxNode.child != null)
                {
                    firstChild.left.right = null;
                    firstChild.left = null;
                }
            }
            else
            {
                maxright.left = null;

                maxleft.right = firstChild;
                if (firstChild != null)
                {
                    firstChild.left.right = null;
                    firstChild.left = maxleft;
                }
            }

            for (HeapNode i = maxNode.child; i != null && i.parent != null; i = i.right)
            {
                i.parent = null;
            }

            HeapNode last;
            HeapNode next;
            HeapNode current;
            HeapNode left;
            HeapNode right;
            HeapNode temp;

            for (current = maxright; current != null; current = next)
            {
                if (data[maxNode.Key] < data[current.Key])
                    maxNode = current;
                last = current;
                next = current.right;
                while (true)
                {
                    temp = degree[current.numOfChild];
                    if (temp == null)
                    {
                        degree[current.numOfChild] = current;
                        break;
                    }
                    else
                    {
                        if (data[temp.Key] > data[current.Key])
                        {
                            left = current.left;
                            right = current.right;
                            current.parent = temp;

                            if (current.numOfChild == 0)
                            {
                                current.left = current;
                                current.right = current;
                                temp.child = current;
                            }
                            else
                            {
                                current.right = temp.child;
                                current.left = temp.child.left;
                                temp.child.left.right = current;
                                temp.child.left = current;
                            }

                            if (left != null)
                                left.right = right;
                            if (right != null)
                                right.left = left;

                            current = temp;
                        }
                        else
                        {
                            left = temp.left;
                            right = temp.right;
                            temp.parent = current;

                            if (current.numOfChild == 0)
                            {
                                temp.left = temp;
                                temp.right = temp;
                            }
                            else
                            {
                                temp.right = current.child;
                                temp.left = current.child.left;
                                current.child.left.right = temp;
                                current.child.left = temp;
                            }

                            if (left != null)
                                left.right = right;
                            if (right != null)
                                right.left = left;
                        }
                        degree[current.numOfChild] = null;
                        current.numOfChild++;
                    }
                }
                last.right = maxright;
                maxright.left = last;
            }
        }

        public void update(int key, double value)
        {
            if (index[key] == null)
                insert(key, value);
            else
            {
                data[key] = value;
                if (data[maxNode.Key] < value)
                    maxNode = index[key];

                HeapNode node = index[key];
                HeapNode parent = node.parent;
                HeapNode leftnode;
                HeapNode rightnode;

                if (parent != null && data[parent.Key] < value)
                {
                    while (parent != null)
                    {
                        parent.numOfChild--;
                        if (parent.numOfChild == 0)
                            parent.child = null;
                        else if (parent.child == node)
                            parent.child = node.right;

                        leftnode = node.left;
                        rightnode = node.right;
                        leftnode.right = rightnode;
                        rightnode.left = leftnode;
                        insert(node);
                        node.parent = null;
                        if (parent.isMarked == false)
                        {
                            parent.isMarked = true;
                            break;
                        }
                        else
                            node = parent;
                    }
                }
            }
        }
    }
}
