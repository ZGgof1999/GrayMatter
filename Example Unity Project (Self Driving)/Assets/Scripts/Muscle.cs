using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiscreteSignals.GrayMatter
{
    public class Muscle : MonoBehaviour
    {
        public System.Action<float> Activate;
        public GameObject bone;
        public int ID;
        private float value = 0.0f;

        public void SetControlFunction(System.Action<float> f)
        {
            Activate = f;
        }

        public void Prime(float _value)
        {
            value += _value;
        }
        public void Fire()
        {
            Activate(value);
        }
        public void Reset()
        {
            value = 0.0f;
        }
    }
}