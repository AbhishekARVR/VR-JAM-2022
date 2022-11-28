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
	[Tooltip("Speed modifier for the paddle spin.")]
    [SerializeField] private float _paddleRotationModifier = 20f;
	[Tooltip("The range of motion from neutral (dead center) to pegged.")]
	[SerializeField] private float rotationRange;
	[Tooltip("The offset in euler degrees from the starting rotation of the throttle to 0.")]
	[SerializeField] private float thrustRotationOffset;
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

        SpinThePaddle();

		AdjustEngineSounds();

		//HandleBuoyancy(speed);

        if (GameManager.Instance.fuelLevel > 0)
        {
			HandleThrustSteering();
			
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

	private void HandleBuoyancy(float speed)
	{
		buoyancyBobRange = 0.1f + (speed / 4) * 0.3f;

		float y = (initialYPosition + (Mathf.Sin(Time.time * buoyancyBobSpeed)) * buoyancyBobRange);
		transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
	}

	private void HandleThrustSteering()
	{
		//Get rotation values from the throttle
		float thrustRotation;

		//convert euler 0 ~ 360 to -180 ~ 180
		if (ThrustTransform.localEulerAngles.y > 180f)
			thrustRotation = ThrustTransform.localEulerAngles.y - 360f;
		else
			thrustRotation = ThrustTransform.localEulerAngles.y;

		//offset the rotation value so it's 0 ~ rotationRange. Don't think we'll need this any more.
		//thrustRotation += thrustRotationOffset;

		//get a normalized representation of thrust
		thrust = thrustRotation / rotationRange;

		//flip it, it's backwards :[
		thrust *= -1;

		//Get rotation values from the steering wheel
		float steeringRotation;

		//convert euler 0 ~ 360 to -180 ~ 180
		if (SteerTransform.localEulerAngles.y > 180f)
			steeringRotation = SteerTransform.localEulerAngles.y - 360f;
		else
			steeringRotation = SteerTransform.localEulerAngles.y;

		//flip it, it's also backwards.... :[[
		steeringRotation *= -1;

		steering = (Mathf.PI / 180) * (steeringRotation);

		//Apply forward force
		var thrustCos = thrust * GameManager.Instance.speedMultiplier * Mathf.Cos(steering);
		boatRigidBody.AddForceAtPosition(transform.forward * thrustCos, turnAxis.position);

		//Apply turning force
		boatRigidBody.AddTorque(-transform.up * (thrust * GameManager.Instance.speedMultiplier * Mathf.Sin(steering)));
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
			float RotationSpeed = _paddleRotationModifier * thrust * Time.deltaTime;
			_rotation.x = RotationSpeed;
			_rightPaddle.transform.Rotate(_rotation);
			_leftPaddle.transform.Rotate(_rotation);
		}
	}

	private void AdjustEngineSounds()
	{
		float fullSoundThreshold = 1f; //how far you have to push the lever to fully hear the paddle sounds.
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