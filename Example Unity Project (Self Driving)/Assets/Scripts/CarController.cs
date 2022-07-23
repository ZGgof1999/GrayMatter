using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float maxV;
    public float maxA;
    public float turnPower;
    public float breakPower;
    public float gas;
    public float steerLeft;
    public float steerRight;
    //public float reverse;
    public float breaks;
    public List<float> controls;

    public bool manualOverride = false;
    
    public void Act()
    {
        if (manualOverride) OverrideInput();
        else GetInputFromBrain();
        Drive();
    }

    void OverrideInput()
    {
        gas = Input.GetKey("w") ? 1.0f : 0.0f;
        steerLeft = Input.GetKey("a") ? 1.0f : 0.0f;
        steerRight = Input.GetKey("d") ? 1.0f : 0.0f;
        //reverse = Input.GetKey("s") ? 1.0f : 0.0f;
        breaks = Input.GetKey("b") ? 1.0f : 0.0f;
    }

    void GetInputFromBrain()
    {
        gas = controls[0];
        steerLeft = controls[1];
        steerRight = controls[2];
        //reverse = controls[0];
        breaks = controls[3];
    }

    void Drive()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        Vector3 localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        //transform.TransformDirection(locVel);
        
        rigidbody.AddForce((Mathf.Max(0.0f, maxV - localVelocity.z) / maxV) * gas * maxA * transform.forward);
        rigidbody.AddRelativeTorque(0, steerLeft * -turnPower, 0);
        rigidbody.AddRelativeTorque(0, steerRight * turnPower, 0);
        rigidbody.AddRelativeForce((Mathf.Max(0.0f, localVelocity.magnitude) / maxV) * breaks * breakPower * -1.0f * localVelocity.normalized);

        //rigidbody.AddRelativeForce((localVelocity.x / maxV) * -0.999f * transform.right);
        float sidewaysVelocity = localVelocity.magnitude * Mathf.Cos(Vector3.Angle(transform.right, localVelocity));
        rigidbody.AddForce(localVelocity.x * -0.999f * transform.right);
        //Debug.Log(localVelocity.x);
    }
}
