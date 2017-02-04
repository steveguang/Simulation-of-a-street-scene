using UnityEngine;
using System.Collections;

// NOTE: Many codes were borrowed from the Official Unity Mecanim tutorial on: http://www.youtube.com/watch?v=Xx21y9eJq1U
public class CharacterControl : MonoBehaviour {

	[HideInInspector]
	public Animator anim;
	public AnimatorStateInfo currentBaseState;	// a reference to the current state of the animator, used for base layer
	//public CapsuleCollider col;				// a reference to the capsule collider of the character

	[HideInInspector]
	public float animSpeed = 1.5f;	// a public setting for overall animator animation speed

	// these integers are references to our animator's states
	[HideInInspector]
	static int jumpState = Animator.StringToHash("Base Layer.Jump");

	[HideInInspector]
	public static int stateLaugh = Animator.StringToHash("Motion.Happy");
	public static int stateTalkNormal = Animator.StringToHash("Motion.Talk");
	public static int stateTalkAngry = Animator.StringToHash("Motion.Dismiss");
	public static int stateWave = Animator.StringToHash("Motion.Cocky");
	public static int stateIdle = Animator.StringToHash("Motion.Idle");

	[HideInInspector]
	public NavMeshAgent agent;
	//public GameObject objToPickUp;
	public Vector3 destination;
	public Vector3 reachPosition;
	public Vector3 lookPosition;

	public bool ikReachActive;
	public bool ikLookActive;

	[HideInInspector]
	public Vector3 pos;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent> ();
		destination = transform.position;
		pos = transform.position;
		//reachPosition = transform.position;
		//print ("haha");
	}

	void OnTriggerEnter(Collider other) {
		/*pos.y = transform.rotation.y + 90;
		pos.z = transform.rotation.z;
		pos.x = transform.rotation.z;
		transform.rotation = pos;*/
		pos.y = transform.position.y + 10;
		pos.z = transform.position.z;
		pos.x = transform.position.x;
		transform.position = pos;
		print ("haha");
		print (transform.position);
	}

	public void SetTargetToGo(Vector3 targetPos) {
		destination = targetPos;
		anim.SetBool ("Approaching", true);
	}
	
	public void ResetTargetToGo() {
		//agent.ResetPath();
		//print ("stopped");
		agent.Stop ();
		anim.SetBool ("Approaching", false);
	}

	public bool IsNavStopped() {
		if (agent.velocity.sqrMagnitude == 0f)
			return true;
		else
			return false;
		//return anim.GetBool ("Approaching");
	}

	public bool IsNavAtTarget() {
		float dist = agent.remainingDistance;
		if (dist <= agent.stoppingDistance)
			return true;
		else
			return false;
	}

	public bool IsNavAtTarget(Vector3 target) {
		float dist = (this.transform.position - target).magnitude;
		if (dist <= agent.stoppingDistance)
			return true;
		else
			return false;
	}

	public bool IsNavArrived() {
		//print ("test");
		//print (IsNavStopped ());
		//print (IsNavAtTarget ());
		if (IsNavStopped () && IsNavAtTarget ()) {
			return true;
		} else {
			return false;
		}
	}

	public bool NavCanReach(Vector3 target)
	{
		NavMeshPath path = new NavMeshPath();
		//print ("nav can reach");
		//print (path.status);
		//print (NavMeshPathStatus.PathComplete);
		this.agent.CalculatePath(target, path);
		return (path.status == NavMeshPathStatus.PathComplete);
	}

	public bool IsNavArrived(Vector3 target) {
		if (IsNavStopped () && IsNavAtTarget (target)) {
			return true;
		} else {
			return false;
		}
	}

	public void ReachFor(Vector3 target)
	{
		ikReachActive = true;
		reachPosition = target;
	}

	public void ReachStop()
	{
		ikReachActive = false;
	}

	public void HeadLookAt(Vector3 target)
	{
		ikLookActive = true;
		lookPosition = target;
	}

	public void HeadLookStop()
	{
		ikLookActive = false;
	}

	void OnAnimatorIK()
	{
		if(anim) {
			//if the IK is active, set the position and rotation directly to the goal. 
			if(ikReachActive) 
			{
					anim.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
					anim.SetIKRotationWeight(AvatarIKGoal.RightHand,0);  
					anim.SetIKPosition(AvatarIKGoal.RightHand,reachPosition);
					//anim.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);
			}
			
			//if the IK is not active, set the position and rotation of the hand and head back to the original position
			if(!ikReachActive) 
			{          
				anim.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
				anim.SetIKRotationWeight(AvatarIKGoal.RightHand,0); 
				//print ("haha");
				//anim.SetLookAtWeight(0);
			}

			if(ikLookActive)
			{
				anim.SetLookAtWeight(1);
				anim.SetLookAtPosition(lookPosition);
			}

			if(!ikLookActive)
			{
				anim.SetLookAtWeight(0);
			}
		}
	}    

	public void Happy() {
		//print ("happy");
		anim.SetBool ("Happy", true);
	}

	public void Talk() {
		//print ("talk");
		anim.SetBool ("Talk", true);
	}

	public void Dismiss() {
		//print ("dismiss");
		anim.SetBool ("Dismiss", true);
	}

	public void Cocky() {
		//print ("cocky");
		anim.SetBool ("Cocky", true);
	}

	public void OrientToward(Vector3 target) {
		Vector3 relativePos = target - transform.position;
		Quaternion rotation = Quaternion.identity;
		rotation = Quaternion.LookRotation (relativePos);
		transform.rotation = rotation;
	}

	public bool IsGestureDone(string gestureName) {
		if (anim.GetCurrentAnimatorStateInfo (1).nameHash == stateIdle) {
			//print ("test happy");
			//print (!anim.GetBool ("Happy"));
			if (gestureName.Equals ("Happy"))
				return !anim.GetBool ("Happy");
			if (gestureName.Equals ("Talk"))
				return !anim.GetBool ("Talk");
			if (gestureName.Equals ("Dismiss"))
				return !anim.GetBool ("Dismiss");
			if (gestureName.Equals ("Cocky"))
				return !anim.GetBool ("Cocky");
			// unknown gesture name -- finish automatically
			return true;
		} else {
			// animation is playing
			return false;
		}
	}

	void FixedUpdate () {
		anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'
		currentBaseState = anim.GetCurrentAnimatorStateInfo (0);	// set our currentState variable to the current state of the Base Layer (0) of animation

		// WALK, LOCOMOTION
		if (anim.GetBool("Approaching")) {
			agent.SetDestination(destination);

			anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
			anim.SetBool ("Running", Input.GetKey (KeyCode.R));
			anim.speed = animSpeed;
			// Check if we've reached the destination
			if (agent.hasPath && IsNavArrived()) {
				// re-initialize navagent
				ResetTargetToGo();
				//print ("process");
				// Rotate
				OrientToward(destination);
			}
		}

		// GESTURES

		AnimatorStateInfo currentState = this.anim.GetCurrentAnimatorStateInfo (1);
		if (currentState.nameHash != stateIdle && currentState.normalizedTime > 0.8f) {
			if (anim.GetBool("Happy"))
				anim.SetBool ("Happy", false);
			if (anim.GetBool("Talk"))
				anim.SetBool ("Talk", false);
			if (anim.GetBool ("Dismiss"))
				anim.SetBool ("Dismiss", false);
			if (anim.GetBool ("Cocky"))
				anim.SetBool ("Cocky", false);
			}
	}
		
}
