using UnityEngine;

public class BoatController : MonoBehaviour
{
    private float initialYPosition;
    
    [SerializeField] private Rigidbody boatRigidBody;
    [SerializeField] private Transform turnAxis, SteerTransform, ThrustTransform;
    [SerializeField] private float buoyancyBobRange ;
    [SerializeField] private float buoyancyBobSpeed;
    
    [Range(0f, 1f)]
    [SerializeField] private float thrust;
    [Range(-1f, 1f)] 
    [SerializeField] private float steering;
    
    

    public float fuelCounter, maxFuel = 100f;
    public int trashCounter;
    
    private void Awake()
    {
        boatRigidBody = this.GetComponent<Rigidbody>();
        initialYPosition = this.transform.localPosition.y;
    }

    private void FixedUpdate()
    {
        float speed = boatRigidBody.velocity.magnitude;
        
        //Debug.Log(speed);

        buoyancyBobRange = 0.1f + (speed / 4) * 0.3f;
        
        float y = (initialYPosition + (Mathf.Sin(Time.time * buoyancyBobSpeed)) * buoyancyBobRange);
        transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);

        if (fuelCounter != 0)
        {
            float thrustRotation;

            
             if (ThrustTransform.localRotation.eulerAngles.x > 180f)
                thrustRotation = ThrustTransform.localRotation.eulerAngles.x - 360f;
            else
            {
                thrustRotation = ThrustTransform.localRotation.eulerAngles.x;
            }

            thrust = ((80f - (thrustRotation)) / 80f);
            
            float steeringRotation = 0f;

            
            if (SteerTransform.rotation.eulerAngles.x > 180f)
                steeringRotation = SteerTransform.rotation.eulerAngles.x - 360f;
            else
            {
                steeringRotation = SteerTransform.rotation.eulerAngles.x;
            }
            
            steering = (Mathf.PI / 180) * (steeringRotation * -1);

            boatRigidBody.AddForceAtPosition(transform.forward * (thrust * Mathf.Cos(steering)  * -1f), turnAxis.position);

            boatRigidBody.AddForceAtPosition(turnAxis.right * (-1f * thrust * Mathf.Sin(steering)), turnAxis.position);

            if (thrust != 0  && !IsInvoking(nameof(ReduceFuel)))
            {
                InvokeRepeating(nameof(ReduceFuel), 0f, 3f);
            }
        }
        else
        {
            CancelInvoke(nameof(ReduceFuel));
            fuelCounter = 0f;
        }
    }

    private void ReduceFuel()
    {
        fuelCounter -= 0.2f;
    }

    public void Refuel(int fuelAMount)
    {
        fuelCounter = fuelAMount;
    }

    public void AddTrashValue(int trashValue)
    {
        trashCounter += trashValue;
    }

}