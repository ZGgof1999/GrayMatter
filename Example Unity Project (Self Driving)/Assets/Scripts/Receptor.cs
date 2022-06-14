using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiscreteSignals.GrayMatter
{
    public class Receptor : MonoBehaviour
    {
        public System.Func<float> GetData;
        public Neuron N = null;
        public float range;

        public void BindNeuron(Neuron _N)
        {
            N = _N;
        }
        public void SetSensoryFunction(System.Func<float> f)
        {
            GetData = f;
        }

        public void SenseData()
        {
            if (!N.Equals(null))
            {
                N.Prime(GetData());
            }
            else
            {
                Debug.Log("Neuron Not Bound: Cannot Sense!");
            }
        }
    }
}