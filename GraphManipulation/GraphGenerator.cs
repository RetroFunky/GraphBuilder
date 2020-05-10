using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Security;
using Domain.Testing.Models;

namespace Domain.Testing
{
    public class GraphGenerator<NodeValue, EdgeValue>
    {
        private List<DomainSeed<NodeValue>> nodeValues;
        private List<DomainSeed<EdgeValue>> relationshipValues;
        private Random random;
        private string filePath;
        private List<string> randomNames;
        private bool logging;

        public GraphGenerator(List<DomainSeed<NodeValue>> NodeValues, List<DomainSeed<EdgeValue>> RelationshipValues, bool LogToFile = true)
        {
            this.nodeValues = NodeValues;
            this.relationshipValues = RelationshipValues;
            this.random = new Random();
            this.filePath = null;
            this.logging = LogToFile;
        }

        public GraphGenerator(List<DomainSeed<NodeValue>> NodeValues, List<DomainSeed<EdgeValue>> RelationshipValues, string FilePath) : this(NodeValues, RelationshipValues)
        {
            this.filePath = FilePath;
        }


        public Graph<NodeValue, EdgeValue> GenerateGraph(List<string> names, int n = 50, int m = 80, bool connected = true, bool directed = true)
        {
            this.randomNames = this.initializeRandomNames(names, n);
            var fileName = this.filePath == null ? "TestGraph" + randomNames[0] + randomNames[1] + randomNames[2] + ".json" : this.filePath;
            var edges = connected ? this.ConnectedGraph(n,m) : this.randomEdgeSet(n, m);
            var graph = this.edgeListToDomainGraph(edges);
            graph.directed = directed;

            if (this.logging)
            {
                File.WriteAllText(fileName, JsonConvert.SerializeObject(graph));
            }
            
            return graph;

        }

        /// <summary>
        /// Simple Random walk approach
        /// </summary>
        /// <param name="n">Number of nodes</param>
        /// <param name="m">Desired number of edges</param>
        /// <returns></returns>
        List<(int, int)> ConnectedGraph(int n, int m)
        {
            if(!CheckValidConnexGraph(n, m))
            {
                throw (new InvalidEnumArgumentException("Incorrect number of edges in relationship to nodes"));
            }

            var edges = new List<(int, int)>();
            var nodes = Enumerable.Range(1, n).ToList();
            var visitedNodes = new List<int>();
            Random random = new Random();

            var nodeIndex = random.Next(nodes.Count);
            var currentNode = nodes[nodeIndex];
            nodes.RemoveAt(nodeIndex);
            visitedNodes.Add(currentNode);

            while(nodes.Count > 0)
            {
                var neighbourIndex = random.Next(nodes.Count);
                var neighbourNode = nodes[neighbourIndex];
                nodes.RemoveAt(neighbourIndex);
                var edge = (currentNode, neighbourNode);
                edges.Add(edge);
                visitedNodes.Add(neighbourNode);
                currentNode = neighbourNode;

            }

            nodes = visitedNodes;

            while(edges.Count < m)
            {
                // Make a random edge
                bool success = false;
                (int, int) edge = (0,0);
                while (!success)
                {
                    var node1 = random.Next(nodes.Count());
                    var node2 = random.Next(nodes.Count());

                    if(node1 != node2 && !edges.Contains((node1, node2))) {
                        edge = (node1, node2);
                        success = true;
                    }
                }

                edges.Add(edge);
            }

            return edges;



        }

        bool CheckValidConnexGraph(int nodes, int edges)
        {
            if(edges < nodes - 1)
            {
                return false;
            }

            if(edges > nodes * (nodes - 1))
            {
                return false;
            }

            return true;
        }

        Graph<NodeValue, EdgeValue> edgeListToDomainGraph(List<(int, int)> edges) {

            Dictionary<int, Node<NodeValue>> userDictionary = new Dictionary<int, Node<NodeValue>>();
            List<Edge<EdgeValue>> domainEdges = new List<Edge<EdgeValue>>();

            for(int i = 0; i < edges.Count(); ++i)
            {
                var edge = edges[i];

                if (!userDictionary.ContainsKey(edge.Item1))
                {
                    var user = this.GenerateRandomUser(edge.Item1);

                    userDictionary.Add(edge.Item1, user);
                }

                domainEdges.Add(this.GetDomainEdge(edge));
                //userDictionary[edge.Item1].OutgoingResponses.Add(relationship);
            }

            var users = new List<Node<NodeValue>>(userDictionary.Values);

            var graph = new Graph<NodeValue, EdgeValue>();

            graph.edges = domainEdges;
            graph.nodes = users;

            return graph;
        }

        private Node<NodeValue> GenerateRandomUser(int id)
        {
            Node<NodeValue> user = new Node<NodeValue>();
            user.Id = id;
            user.Name = this.GetRandomName();
            user.Role = getRandomDomainValue(this.nodeValues);
            return user;
        }

        private T getRandomDomainValue<T>(List<DomainSeed<T>> values)
        {
            // super simple randomness bias
            int sum = 0;
            for(int i = 0; i < values.Count; ++i)
            {
                sum += values[i].bias;
            }

            var rawValue = random.Next(1, sum);
            for(int i = 0; i < values.Count; ++i)
            {
                if(rawValue < values[i].bias)
                {
                    return values[i].value;
                }
                rawValue -= values[i].bias;
            }

            // this should really never happen.
            throw (new NotImplementedException());
            
        }


        private Edge<EdgeValue> GetDomainEdge((int, int) edge)
        {
            var relationship = new Edge<EdgeValue>();

            relationship.edge = edge;
            relationship.type = getRandomDomainValue(this.relationshipValues);

            return relationship;
        }


        /// <summary>
        /// Creates a Graph representing as an edgelist of numerical ids
        /// </summary>
        /// <param name="n">
        /// Number of nodes
        /// </param>
        /// <param name="m">
        /// Number of edges
        /// </param>
        /// <param name="fileName"></param>
        List<(int,int)> randomEdgeSet(int n, int m)
        {
            var nodes = Enumerable.Range(1, n).ToList();
            var edgeList = this.randomChoose(this.unorderedPairs(nodes), m);

            return edgeList;

        }

        List<(T, T)> unorderedPairs<T>(List<T> s)
        {
            int j;
            var a = new List<(T, T)>();
            int n = s.Count();
            for (int i = 0; i < n; ++i)
            {
                j = i;
                while (++j < n)
                {
                    a.Add((s[i], s[j]));
                }
            }
            return a;
        }

        /// <summary>
        /// Returns a randomly selected subset of the provided list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s">The list from which the subset is selected</param>
        /// <param name="k">The length of the subset</param>
        List<T> randomChoose<T>(IEnumerable<T> s, int k)
        {
            var a = new List<T>();
            var listCopy = new List<T>(s);
            int j;
            for (int i = 0; i < k; ++i)
            {
                j = random.Next(listCopy.Count());
                a.Add(listCopy[j]);
                listCopy.RemoveAt(j);
            };
            return a;
        }

        private List<string> initializeRandomNames(List<string> names, int n)
        {
            var usableNames = new List<string>(names);
            var subSet = new List<string>();
            for (int i = 0; i < n; ++i)
            {
                int index = random.Next(usableNames.Count());
                subSet.Add(usableNames[index]);
                usableNames.RemoveAt(index);
            }

            return subSet;
        }

        private string GetRandomName()
        {
            // strong feeling that removing last should be best
            if(this.randomNames.Count > 0)
            {
                var name = this.randomNames[randomNames.Count - 1];
                randomNames.RemoveAt(randomNames.Count - 1);
                return name;
            } else
            {
                /// This is pretty bad tbh
                return null;
            }
            


        }
    }
}

