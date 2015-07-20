using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IrcBot.Common.MarkovChains
{
    public class MarkovChainNode<T>
    {
        private readonly List<MarkovChainNode<T>> _links;
        private readonly ReadOnlyCollection<MarkovChainNode<T>> _linksReadOnly;

        public MarkovChainNode(T value)
            : this()
        {
            Value = value;
        }

        public MarkovChainNode()
        {
            _links = new List<MarkovChainNode<T>>();
            _linksReadOnly = new ReadOnlyCollection<MarkovChainNode<T>>(_links);
        }

        public T Value { get; set; }

        public ReadOnlyCollection<MarkovChainNode<T>> Links
        {
            get { return _linksReadOnly; }
        }

        public void AddLink(MarkovChainNode<T> toNode)
        {
            _links.Add(toNode);
        }
    }
}
