using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiscreteSignals.GrayMatter
{
    public class Brain
    {
        public List<Layer> layers;
        public Sense sense;
        public Reaction reaction;

        public Brain(Sense _sense, Reaction _reaction, List<Layer> unconnectedHiddenLayers = null)
        {
            sense = _sense;
            reaction = _reaction;

            layers = new List<Layer>();
            layers.Add(sense.sensoryLayer);
            if (unconnectedHiddenLayers != null)
            {
                foreach (Layer layer in unconnectedHiddenLayers) layers.Add(layer);
            } 
            layers.Add(reaction.actionLayer);

            for (int layer = 1; layer < layers.Count; layer++)
            {
                layers[layer - 1].ConnectTo(layers[layer]);
            }
        }
        public Brain(Brain parent, Sense _sense, Reaction _reaction, float mutationRate = 0.1f, float maxMutation = 0.02f, List<Layer> unconnectedHiddenLayers = null)
        {
            sense = _sense;
            reaction = _reaction;

            layers = new List<Layer>();
            layers.Add(sense.sensoryLayer);
            if (unconnectedHiddenLayers != null)
            {
                foreach (Layer layer in unconnectedHiddenLayers) layers.Add(layer);
            } 
            layers.Add(reaction.actionLayer);

            for (int layer = 1; layer < layers.Count; layer++)
            {
                layers[layer - 1].ConnectTo(layers[layer]);
            }

            for (int layer = 0; layer < layers.Count; layer++)
            {
                for (int neuron = 0; neuron < layers[layer].neurons.Count; neuron++)
                {
                    for (int dendrite = 0; dendrite < layers[layer].neurons[neuron].dendrites.Count; dendrite++)
                    {
                        float weight = parent.layers[layer].neurons[neuron].dendrites[dendrite].weight;
                        layers[layer].neurons[neuron].dendrites[dendrite].weight = weight;
                        layers[layer].neurons[neuron].dendrites[dendrite].Mutate(mutationRate, maxMutation);
                    }
                }
            }
        }
        
        public void Mutate(float mutationRate = 0.1f, float maxMutation = 0.02f)
        {
            for (int layer = 0; layer < layers.Count - 1; layer++)
            {
                for (int neuron = 0; neuron < layers[layer].neurons.Count; neuron++)
                {
                    for (int dendrite = 0; dendrite < layers[layer].neurons[neuron].dendrites.Count; dendrite++)
                    {
                        layers[layer].neurons[neuron].dendrites[dendrite].Mutate(mutationRate, maxMutation);
                    }
                }
            }
        }
        

        public void Think()
        {
            sense.SenseData();
            foreach (Layer layer in layers)
            {
                layer.Fire();
            }
            reaction.React();
        }

        public void PrintState()
        {

        }
    }

    public class Sense
    {
        public List<Receptor> receptors;
        public Layer sensoryLayer;
        public Sense(List<Receptor> _receptors, System.Func<float, float> _ActivationFunction = null)
        {
            System.Func<float, float> ActivationFunction;
            if (_ActivationFunction == null) ActivationFunction = (float value) => { return value; };
            else ActivationFunction = _ActivationFunction;

            receptors = _receptors;
            sensoryLayer = new Layer(receptors.Count, ActivationFunction);
            for (int n = 0; n < receptors.Count; n++)
            {
                receptors[n].BindNeuron(sensoryLayer.neurons[n]);
            }
        }
        public void BindNewReceptors(List<Receptor> _receptors)
        {
            receptors = _receptors;
            for (int n = 0; n < receptors.Count; n++)
            {
                receptors[n].BindNeuron(sensoryLayer.neurons[n]);
            }
        }
        public void SenseData()
        {
            foreach (Receptor r in receptors)
            {
                r.SenseData();
            }
        }
    }

    public class Reaction
    {
        public List<Muscle> muscles;
        public Layer actionLayer;
        public Reaction(List<Muscle> _muscles, System.Func<float, float> _ActivationFunction = null)
        {
            System.Func<float, float> ActivationFunction;
            if (_ActivationFunction == null) ActivationFunction = (float value) => { return value; };
            else ActivationFunction = _ActivationFunction;

            muscles = _muscles;
            actionLayer = new Layer(muscles.Count, ActivationFunction);
            for (int n = 0; n < muscles.Count; n++)
            {
                actionLayer.neurons[n].ConnectTo(muscles[n]);
            }
        }
        public void BindNewMuscles(List<Muscle> _muscles)
        {
            muscles = _muscles;
            foreach (Neuron n in actionLayer.neurons)
            {
                for (int d = 0; d < n.dendrites.Count; d++)
                {
                    n.dendrites[d].M = muscles[d];
                }
            }
        }
        public void React()
        {
            foreach (Muscle muscle in muscles) 
            {
                muscle.Fire();
                muscle.Reset();
            }
        }
    }

    public class Layer
    {
        public Layer(int size, System.Func<float, float> _ActivationFunction = null)
        {
            System.Func<float, float> ActivationFunction;
            if (_ActivationFunction == null) ActivationFunction = (float value) => { return value; };
            else ActivationFunction = _ActivationFunction;

            neurons = new List<Neuron>();

            for (int n = 0; n < size; n++)
            {
                neurons.Add(new Neuron(ActivationFunction));
            }
        }
        public List<Neuron> neurons;

        public void ConnectTo(Layer B)
        {
            foreach (Neuron n1 in neurons)
            {
                foreach (Neuron n2 in B.neurons)
                {
                    n1.ConnectTo(n2);
                }
            }
        }

        public void Fire()
        {
            foreach (Neuron neuron in neurons)
            {
                neuron.Fire();
                neuron.Reset();
            }
        }
    }

    public class Neuron
    {
        public Neuron(System.Func<float, float> _ActivationFunction = null)
        {
            if (_ActivationFunction == null) ActivationFunction = (float value) => { return value; };
            else ActivationFunction = _ActivationFunction;

            value = 0;
            dendrites = new List<Dendrite>();
        }

        private System.Func<float, float> ActivationFunction;
        private float value;
        public List<Dendrite> dendrites;

        public void ConnectTo(Neuron B)
        {
            dendrites.Add(new Dendrite(this, B, ActivationFunction));
        }
        public void ConnectTo(Muscle M)
        {
            dendrites.Add(new Dendrite(this, M, ActivationFunction));
        }
        public void Prime(float v) { value = value + v; }

        public void Fire()
        {
            foreach (Dendrite dendrite in dendrites)
            {
                dendrite.Fire(value);
            }
        }
        public void Reset() { value = 0.0f; }
    }

    public class Dendrite
    {
        public float weight;
        public Neuron A;
        public Neuron B;
        public Muscle M;
        private System.Func<float, float> ActivationFunction;
        public Dendrite(Neuron _A, Neuron _B, System.Func<float, float> _ActivationFunction = null)
        {
            if (_ActivationFunction == null) ActivationFunction = (float value) => { return value; };
            else ActivationFunction = _ActivationFunction;

            weight = (2.0f * Random.value) - 1.0f;
            A = _A;
            B = _B;

            M = null;
        }
        public Dendrite(Neuron _A, Muscle _M, System.Func<float, float> _ActivationFunction = null)
        {
            if (_ActivationFunction == null) ActivationFunction = (float value) => { return value; };
            else ActivationFunction = _ActivationFunction;
            
            weight = (2.0f * Random.value) - 1.0f;
            A = _A;
            M = _M;

            B = null;
        }
        public void Fire(float value)
        {
            if (B != null)
            {
                B.Prime(ActivationFunction(value * weight));
            }
            if (M != null)
            {
                M.Prime(ActivationFunction(value * weight));
            }
        }
        public void Mutate(float mutationRate, float maxMutation)
        {
            if (Random.value <= mutationRate)
            {
                float delta = maxMutation * 2.0f * (Random.value - 0.5f);
                weight = weight + delta;//Mathf.Clamp(weight + delta, -10.0f, 10.0f));
            }
        }
    }
}