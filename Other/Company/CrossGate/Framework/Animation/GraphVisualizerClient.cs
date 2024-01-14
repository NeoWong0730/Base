using System.Collections.Generic;
using UnityEngine.Playables;

namespace Framework
{
    public class GraphVisualizerClient
    {
        private static GraphVisualizerClient _instance;
        private Dictionary<PlayableGraph, string> graphs = new Dictionary<PlayableGraph, string>();

        public static GraphVisualizerClient Instnace
        {
            get
            {
                if (_instance == null)
                    _instance = new GraphVisualizerClient();
                return _instance;
            }
        }

        ~GraphVisualizerClient()
        {
            graphs.Clear();
        }

        public static void Show(PlayableGraph graph, string name)
        {
            if (!Instnace.graphs.ContainsKey(graph))
            {
                Instnace.graphs.Add(graph, name);
            }
        }

        public static void Hide(PlayableGraph graph)
        {
            if (Instnace.graphs.ContainsKey(graph))
            {
                Instnace.graphs.Remove(graph);
            }
        }

        public static IEnumerable<KeyValuePair<PlayableGraph, string>> GetGraphs()
        {
            return Instnace.graphs;
        }
    }
}
