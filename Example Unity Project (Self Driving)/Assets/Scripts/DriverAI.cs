using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscreteSignals.GrayMatter;

public class DriverAI : MonoBehaviour
{
    public Brain brain;
    public Sense sight;
    public List<Receptor> eyes;
    public Reaction motor;
    public List<Muscle> gears;
    public CarController car;

    public int trackPosition = 0;

    public float fitness;
    public bool halted = true;

    public void Initialize()
    {
        foreach(Receptor eye in eyes)
        {
            eye.SetSensoryFunction(() => 
            {
                RaycastHit light;
                if (Physics.Raycast (eye.transform.position, eye.transform.forward, out light, eye.range))
                {
                    return 1.0f;
                }
                else 
                {
                    return -1.0f;
                }
            });
        }
        System.Func<float, float> ActivationFunction = (float value) => 
        {
            //SIGMOID
            return 1.0f / (1.0f + Mathf.Pow(System.MathF.E, -1.0f * value));
        };
        foreach(Muscle gear in gears)
        {
            gear.SetControlFunction((float value) =>
            {
                car.controls[gear.ID] = value;//ActivationFunction(value);
                //gear.bone.GetComponent<Rigidbody>().AddForceAtPosition(gear.transform.forward * value, gear.transform.position);
                //Debug.Log("Flexing Muscle: " + action.gameObject.name + " with force of: " + value.ToString());
            });
        }
        sight = new Sense(eyes, ActivationFunction);
        motor = new Reaction(gears, ActivationFunction);
        brain = new Brain(sight, motor);
        fitness = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldDrive())
        {
            brain.Think();
            car.Act();
        }
    }

    bool ShouldDrive()
    {
        if (transform.position.y <= -0.2) halted = true;
        return !halted;
    }

    private void OnTriggerStay(Collider other)
    {
        int newTrackID = other.gameObject.GetComponent<Road>().ID;
        int delta = newTrackID - trackPosition;
        bool checkpoint = delta == 1 || delta == -61 ? true : false;
        if (checkpoint) 
        {
            fitness += 100.0f - Mathf.Log(gameObject.GetComponent<Rigidbody>().velocity.magnitude + 1.0f);
            trackPosition = newTrackID;
        }
    }

}
