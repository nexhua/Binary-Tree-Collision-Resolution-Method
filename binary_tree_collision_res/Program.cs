using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace binary_tree_collision_resolution
{
    class Program
    {
        class Table
        {
            public List<Line> table { get; set; }
            public int tableSize { get; set; } = 0;
            public int numOfItems { get; set; } = 0;
            public int mod { get; set; } = 0;
            public int maxNum { get; set; } = 999;
            public int totalProbeCount { get; set; } = 0;
            public double packingFactor { get; set; } = 0.9;
            public List<Node> solutionList { get; set; } = new List<Node>();

            public Table(int tableSize, double packingFactor) //Could be created by giving a N size. It automatically fills the table with default packing factor being ~%90
            {
                table = Enumerable.Repeat(new Line(), tableSize).ToList();
                this.tableSize = tableSize;
                mod = tableSize;
                this.packingFactor = packingFactor;
                fillTable();
            }

            public Table(int[] numbers, int tableSize)//Could be created with user given array.
            {
                table = Enumerable.Repeat(new Line(), tableSize).ToList();
                this.tableSize = tableSize;
                mod = tableSize;
                fillTable(numbers);
            }

            public void fillTable()//Fills table with random values. Does not add the number if it already exist in the table.
            {
                Random rnd = new Random();
                BinaryTree Solver = new BinaryTree(solutionList);
                while (numOfItems < tableSize * packingFactor)
                {
                    bool added = placeNum(rnd.Next(maxNum), Solver);
                    numOfItems = added ? numOfItems + 1 : numOfItems;
                }
            }

            public void fillTable(int[] numbers)//Fills the table with user given array values.
            {
                BinaryTree Solver = new BinaryTree(solutionList);
                for (int i = 0; i < numbers.Length; i++)
                {
                    if (numOfItems < tableSize)
                    {
                        bool added = placeNum(numbers[i], Solver);
                        numOfItems = added ? numOfItems + 1 : numOfItems;
                    }
                }
            }

            public bool search(int key, int mode)//Searches for a value in the table. Return true if it is found.
            {
                int probeCount = 0;
                int homeAddress = key % mod;
                int incr = Convert.ToInt32(key / mod) % mod;
                int currentAddress = (homeAddress + incr) % mod;
                probeCount++;
                if (table[homeAddress].num == key)
                {
                    totalProbeCount += probeCount;
                    table[homeAddress].probeCount = probeCount;
                    if (mode == 1)
                        Console.WriteLine("Found {0} at {1}. Probe count is {2}", key, homeAddress, table[homeAddress].probeCount);
                    return true;
                }
                while (currentAddress != homeAddress)
                {
                    probeCount++;
                    if (table[currentAddress].num == key)
                    {
                        totalProbeCount += probeCount;
                        table[currentAddress].probeCount = probeCount;
                        if (mode == 1)
                            Console.WriteLine("Found {0} at {1}. Probe count is {2}", key, currentAddress, table[currentAddress].probeCount);
                        return true;
                    }
                    currentAddress = (currentAddress + incr) % mod;
                }
                if (mode == 1)
                    Console.WriteLine("Number {0} does not exist in the binary tree table.", key);
                return false;
            }

            public bool placeNum(int num, BinaryTree Solver)//Handles key placement for Binary Tree method.
            {
                int result = num % mod;
                if (table[result].init == true && numOfItems < table.Capacity)
                {
                    if (!search(num, 0))
                    {
                        Solver.root = new Node(table[result].num, result, null, num);
                        Solver.root.setMode(mod);
                        Solver.solutionList = solutionList;
                        Solver.Solve(table);
                        return true;
                    }
                }
                else
                {
                    table[result] = new Line(num);
                    return true;
                }
                return false;
            }

            public void updateProbe()//Resets and calculates total probe count.
            {
                totalProbeCount = 0;
                foreach (Line line in table)
                {
                    if (line.init == true)
                    {
                        search(line.num, 0);
                    }
                }
            }

            public void printSolutionList()
            {
                Console.WriteLine("\n{0} collisions has occured.\n", solutionList.Count);
                foreach (Node root in solutionList)
                {
                    Console.WriteLine("Collision between {0} and {1} at index {2} has occured.", root.carriedNum, root.num, root.address);
                    printTree(root, 0);
                    Console.WriteLine();
                }
            }
            public void printTab(int level)
            {
                for (int i = 0; i < level; i++)
                {
                    Console.Write("\t");
                }
            }

            public void printTree(Node root, int level)
            {
                if (root == null)
                    return;
                printTab(level);
                Console.WriteLine("{0}({1})", root.address, root.num);
                printTree(root.left, level + 1);
                printTree(root.right, level + 1);
            }

            public double getPackingFactor()
            {
                return Convert.ToDouble(numOfItems) / Convert.ToDouble(tableSize) * 100;
            }

            public double getAvProbe()
            {
                updateProbe();
                return Convert.ToDouble(totalProbeCount) / Convert.ToDouble(numOfItems);
            }

            public void printTable()
            {
                updateProbe();
                Console.WriteLine("{0}", "Binary Tree Method\n");
                for (int i = 0; i < table.Count; i++)
                {
                    Console.WriteLine("{0,3} {1,-25}", i, table[i].getString());
                }
                Console.WriteLine("\nTable Size          : {0,-10}", tableSize);
                Console.WriteLine("Table Mod           : {0,-10}", mod);
                Console.WriteLine("Number of Items     : {0,-10}", numOfItems);
                Console.WriteLine("Packing Factor      : %{0,-10:0.##}", getPackingFactor());
                Console.WriteLine("Avarage Probe Count : {0,-10:0.##}", getAvProbe());
            }
        }

        class Line//Represents each line in the table.
        {
            public int num { get; set; }
            public int link { get; set; }
            public int probeCount { get; set; }
            public bool init { get; set; } = false;
            public Line()
            {
                num = -1;
                link = -1;
                probeCount = -1;
                init = false;
            }
            public Line(int num)
            {
                this.num = num;
                link = -1;
                init = true;
            }
            public Line(int num, int link)
            {
                this.num = num;
                this.link = link;
                init = true;
            }
            public string getString()
            {
                string number = init == false ? "---" : num.ToString();
                string probeStr = probeCount == -1 ? string.Empty : "#Probe " + probeCount.ToString();
                return string.Format("{0,4} | {1}", number, probeStr);
            }
        }

        class Node
        {
            public int num { get; set; } = 0;
            public int address { get; set; } = 0;
            public int carriedNum { get; set; } = -1;
            public Node left { get; set; } = null;
            public Node right { get; set; } = null;
            public Node parent { get; set; } = null;
            public static int mod { get; set; } = -1;
            public Node(int num, int address, Node parent, int carriedNum)
            {
                this.num = num;
                left = null;
                right = null;
                this.parent = parent;
                this.address = address;
                this.carriedNum = carriedNum;
            }
            public bool continueLeft(List<Line> table, Node parent)//Adds a left node to current node.
            {
                int incr = Convert.ToInt32(parent.carriedNum / mod) % mod;
                int newAddress = (parent.address + incr) % mod;
                if (table[newAddress].init == false)
                {
                    parent.left = new Node(table[newAddress].num, newAddress, parent, parent.carriedNum);
                    return true;
                }
                else
                {
                    parent.left = new Node(table[newAddress].num, newAddress, parent, parent.carriedNum);
                    return false;
                }
            }
            public bool addRight(List<Line> table, Node parent)//Adds a right node to current node.
            {
                int incr = Convert.ToInt32(parent.num / mod) % mod;
                int newAddress = (parent.address + incr) % mod;
                if (table[newAddress].init == false)
                {
                    parent.right = new Node(table[newAddress].num, newAddress, parent, parent.num);
                    return true;
                }
                else
                {
                    parent.right = new Node(table[newAddress].num, newAddress, parent, parent.num);
                    return false;
                }
            }
            public void traceBack(Node current, List<Line> table)//Returns a list of nodes from found empyt node to root node.
            {
                List<Node> shiftList = new List<Node>();
                while (current != null)
                {
                    shiftList.Add(current);
                    current = current.parent;
                }
                shiftNodes(shiftList, table);
            }
            public void shiftNodes(List<Node> shiftList, List<Line> table)//Analyzes shift list and does the neccassary shift operations.
            {
                int currentCarry = shiftList[0].carriedNum;
                List<Node> orders = new List<Node>();
                orders.Add(shiftList[0]);//Order list contains which values will moved to which index.
                foreach (Node node in shiftList)
                {
                    if (node.carriedNum != currentCarry)
                    {
                        currentCarry = node.carriedNum;
                        orders.Add(node);
                    }
                }
                foreach (Node node in orders)//Executes the order list.
                {
                    table[node.address] = new Line(node.carriedNum);
                }
            }

            public void setMode(int num)
            {
                mod = num;
            }
        }
        class BinaryTree
        {
            public Node root { get; set; }
            public List<Node> solutionList { get; set; } = new List<Node>();

            public BinaryTree(int collisionAddress, int homeNum, int collidedNum, int mod, List<Node> solutionList)
            {
                root = new Node(homeNum, collisionAddress, null, collidedNum);
                root.setMode(mod);
                this.solutionList = solutionList;
            }

            public BinaryTree(List<Node> solutions)
            {
                solutionList = solutions;
            }

            public void Solve(List<Line> table)//Handles collisions for binary tree method.
            {
                List<Node> leaves = new List<Node>();
                getLeaves(root, leaves);
                bool left = false, right = false;
                while (left == false && right == false)
                {
                    foreach (Node leaf in leaves)
                    {
                        left = leaf.continueLeft(table, leaf);
                        if (left == true)
                        {
                            leaf.traceBack(leaf.left, table);
                            solutionList.Add(root);
                            break;
                        }
                        right = leaf.addRight(table, leaf);
                        if (right)
                        {
                            leaf.traceBack(leaf.right, table);
                            solutionList.Add(root);
                            break;
                        }
                    }
                    leaves = new List<Node>();
                    getLeaves(root, leaves);
                }
            }

            public void getLeaves(Node root, List<Node> leaves)
            {
                if (root != null)
                {
                    if (root.left == null && root.right == null)
                    {
                        leaves.Add(root);
                    }
                    if (root.left != null)
                    {
                        getLeaves(root.left, leaves);
                    }
                    if (root.right != null)
                    {
                        getLeaves(root.right, leaves);
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            int[] numbers = new int[] { 27, 18, 29, 28, 39, 13, 16, 41, 17, 19 };
            Table treeTable = new Table(numbers, 11);
            //Table treeTable2 = new Table(47 ,0.8); //Constructor with table size and packing factor. Values are created randomly.
            treeTable.printTable();
            treeTable.printSolutionList(); //Prints solutions trees for each collision.
        }
    }
}

