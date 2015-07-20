using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IrcBot.Common.MarkovChains
{
    public class MarkovChain<T>
    {
        private static readonly IEqualityComparer<T> Comparer = EqualityComparer<T>.Default;

        private readonly Random _random = new Random();

        private readonly List<MarkovChainNode<T>> _nodes;
        private readonly ReadOnlyCollection<MarkovChainNode<T>> _nodesReadOnly;

        public MarkovChain()
        {
            _nodes = new List<MarkovChainNode<T>>();
            _nodesReadOnly = new ReadOnlyCollection<MarkovChainNode<T>>(_nodes);
        }

        public ReadOnlyCollection<MarkovChainNode<T>> Nodes
        {
            get { return _nodesReadOnly; }
        }

        public IEnumerable<T> GenerateSequence()
        {
            var curNode = GetNode(default(T));

            while (true)
            {
                if (curNode.Links.Count == 0)
                {
                    break;
                }

                curNode = curNode.Links[_random.Next(curNode.Links.Count)];

                if (curNode.Value == null)
                {
                    break;
                }

                yield return curNode.Value;
            }
        }

        public void Train(T fromValue, T toValue)
        {
            var fromNode = GetNode(fromValue);
            var toNode = GetNode(toValue);

            fromNode.AddLink(toNode);
        }

        private MarkovChainNode<T> GetNode(T value)
        {
            var node = _nodes.SingleOrDefault(n => Comparer.Equals(n.Value, value));

            if (node != null)
            {
                return node;
            }

            node = new MarkovChainNode<T>(value);
            _nodes.Add(node);

            return node;
        }
    }
}
