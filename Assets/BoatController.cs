using UnityEngine;

public class BoatController : MonoBehaviour
{

    public static BoatController Instance;

    private float initialYPosition;
    
    [SerializeField] private Rigidbody boatRigidBody;
    [SerializeField] private Transform turnAxis, SteerTransform, ThrustTransform;
    [SerializeField] private float buoyancyBobRange;
    [SerializeField] private float buoyancyBobSpeed;
    [SerializeField] private GameObject _rightPaddle;
    [SerializeField] private GameObject _leftPaddle;
    [SerializeField] private float _rotationModifier = 20f;
	[SerializeField] private float fuelEfficiency = .5f;

	[Range(0f, 1f)]
    [SerializeField] public float thrust;
    [Range(-1f, 1f)] 
    [SerializeField] private float steering;

	private AudioSource engineAudio;
	private AudioSource paddleAudio;

	private void Start()
	{
		var aSrcs = GetComponents<AudioSource>();

		if (aSrcs.Length != 2)
			Debug.LogError("Missing needed audio sources on boat.", this);

		engineAudio = aSrcs[1];
		paddleAudio = aSrcs[0];
	}

	private void Awake()
    {
        boatRigidBody = this.GetComponent<Rigidbody>();
        initialYPosition = this.transform.localPosition.y;

        //singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void FixedUpdate()
    {
        float speed = boatRigidBody.velocity.magnitude;

        //Debug.Log(speed);

        SpinThePaddle();

		AdjustEngineSounds();

        buoyancyBobRange = 0.1f + (speed / 4) * 0.3f;
        
        float y = (initialYPosition + (Mathf.Sin(Time.time * buoyancyBobSpeed)) * buoyancyBobRange);
        transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);

        if (GameManager.Instance.fuelLevel > 0)
        {
			//Get rotation values from the throttle
            float thrustRotation;

            if (ThrustTransform.localRotation.eulerAngles.x > 180f)
                thrustRotation = ThrustTransform.localRotation.eulerAngles.x - 360f;
            else
            {
                thrustRotation = ThrustTransform.localRotation.eulerAngles.x;
            }

            thrust = ((80f - (thrustRotation)) / 80f);
            
			//Get rotation values from the steering wheel
            float steeringRotation = 0f;

            if (SteerTransform.rotation.eulerAngles.x > 180f)
                steeringRotation = SteerTransform.rotation.eulerAngles.x - 360f;
            else
                steeringRotation = SteerTransform.rotation.eulerAngles.x;
            
            steering = (Mathf.PI / 180) * (steeringRotation * -1);

			//Apply forward force
            boatRigidBody.AddForceAtPosition(transform.forward * (thrust * GameManager.Instance.speedMultiplier * Mathf.Cos(steering)  * -1f), turnAxis.position);

			//Apply turning force
			//TODO: make this apply torque force instead of side force.
            boatRigidBody.AddForceAtPosition(turnAxis.right * (-1f * thrust * GameManager.Instance.speedMultiplier * Mathf.Sin(steering)), turnAxis.position);

            if (thrust != 0 && !IsInvoking(nameof(ReduceFuel)))
            {
                InvokeRepeating(nameof(ReduceFuel), 0f, 3f);
            }
        }
        else
        {
            CancelInvoke(nameof(ReduceFuel));
			GameManager.Instance.fuelLevel = 0f;
        }
    }

    private void ReduceFuel()
    {
        GameManager.Instance.useFuel(fuelEfficiency * thrust);
    }

    private void SpinThePaddle()
    {
		if (GameManager.Instance.fuelLevel > 0)
		{
			Vector3 _rotation = new Vector3(0, 0, 0);
			float RotationSpeed = _rotationModifier * thrust * Time.deltaTime;
			_rotation.x = RotationSpeed;
			_rightPaddle.transform.Rotate(_rotation);
			_leftPaddle.transform.Rotate(_rotation);
		}
	}

	private void AdjustEngineSounds()
	{
		float fullSoundThreshold = 1f; //how far do you have to push the lever to fully hear the paddle sounds.
		float pitchAdjustAmount = .5f; //use a fraction between 0 and 1.

		if (GameManager.Instance.fuelLevel > 0)
		{
			//ajdust engine
			engineAudio.pitch = 1 + (thrust * pitchAdjustAmount);

			//adjust paddle
			paddleAudio.volume = Mathf.Clamp(thrust / fullSoundThreshold, 0, 1);
		}
		else if (GameManager.Instance.fuelLevel == 0 && engineAudio.pitch > 0)
		{
			//ajdust engine
			engineAudio.pitch -= .01f;

			//adjust paddle
			paddleAudio.volume = 0;
		}
		else if (GameManager.Instance.fuelLevel == 0 && engineAudio.pitch <= 0)
		{
			//quiet down the engine
			engineAudio.volume = 0;
		}
	}
}