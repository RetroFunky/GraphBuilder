﻿using System;
using System.Collections.Generic;
using System.IO;
using Domain.Testing;
using Domain.Testing.Models;
using Newtonsoft.Json;

namespace DomainTests
{
    class SampleDomain
    {
        public List<DomainSeed<string>> nodeValues;
        public List<DomainSeed<string>> edgeValues;

        /// <summary>
        /// This domain attempts to simulate a relationship map generated by a role-playing matchmaking system
        /// </summary>
        public SampleDomain()
        {
            nodeValues = new List<DomainSeed<string>>();
            nodeValues.Add(new DomainSeed<string>("Player", 6));
            nodeValues.Add(new DomainSeed<string>("GM", 2));
            nodeValues.Add(new DomainSeed<string>("Flexible", 1));

            edgeValues = new List<DomainSeed<string>>();
            edgeValues.Add(new DomainSeed<string>("Yes", 3));
            edgeValues.Add(new DomainSeed<string>("Maybe", 10));
            edgeValues.Add(new DomainSeed<string>("No", 1));

        }

    }

    class Program
    {
        static void Main(string[] args)
        {

            // Totally not a debug path here, don't mind me
            var namesJson = File.ReadAllText(@"../../../Resources/Names.json");
            // This is like super overkill but it's ok.
            List<string> randomNames = JsonConvert.DeserializeObject<List<string>>(namesJson);

            var sampleDomain = new SampleDomain();

            var generator = new GraphGenerator<string, string>(sampleDomain.nodeValues, sampleDomain.edgeValues);
            int numberOfNodes = 50;
            int numberOfEdges = 80;
            bool directed = true;
            generator.GenerateGraph(randomNames, numberOfNodes, numberOfEdges, directed);
        }
    }
}
