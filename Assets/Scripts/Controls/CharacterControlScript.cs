using UnityEngine;
using System.Collections;
//using RootMotion.FinalIK;

// NOTE: Many codes were borrowed from the Official Unity Mecanim tutorial on: http://www.youtube.com/watch?v=Xx21y9eJq1U
public class CharacterControlScript : MonoBehaviour {

	private Animator anim;
	private AnimatorStateInfo currentBaseState;	// a reference to the current state of the animator, used for base layer
	private CapsuleCollider col;				// a reference to the capsule collider of the character

	public float animSpeed = 1.5f;	// a public setting for overall animator animation speed

	// these integers are references to our animator's states
	static int idleState = Animator.StringToHash("Base Layer.Idle");	
	static int walksState = Animator.StringToHash("Base Layer.Walks");	
	static int locomotionState = Animator.StringToHash("Base Layer.Locomotion");
	static int jumpState = Animator.StringToHash("Base Layer.Jump");

	NavMeshAgent agent;
	GameObject objToPickUp;
	Vector3 destination;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		col = GetComponent<CapsuleCollider>();	

		// We need a navigation
		agent = GetComponent<NavMeshAgent> ();
		agent.stoppingDistance = 0.9f;

		// At first, don't move from the current position
		destination = transform.position;
	}

	void Update() {
		if (anim.GetBool("IsApproaching")) {
			agent.SetDestination(destination);
			anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
			anim.speed = animSpeed;

			// Check if we've reached the destination
			float dist = agent.remainingDistance;
			if (agent.hasPath)
			{
				if (dist <= agent.stoppingDistance &&
				    agent.velocity.sqrMagnitude == 0f)
				{
					// Done
					agent.ResetPath();
					anim.SetBool ("IsApproaching", false);
					// first, face it...
					transform.LookAt (
						new Vector3(objToPickUp.transform.position.x,
					            transform.position.y,
					            objToPickUp.transform.position.z));
					// release this object since we don't need it anymore...
					objToPickUp = null;
					// try to pick up
					anim.SetBool("IsPickingUp", true);
				}
			}
		}
	}
	
	void FixedUpdate () {
		if (!anim.GetBool ("IsApproaching"))
		{
			float h = Input.GetAxis ("Horizontal");				// setup h variable as our horizontal input axis
			float v = Input.GetAxis ("Vertical");				// setup v variables as our vertical input axis
			anim.SetFloat ("Speed", v);							// set our animator's float parameter 'Speed' equal to the vertical input axis				
			anim.SetFloat ("Direction", h); 						// set our animator's float parameter 'Direction' equal to the horizontal input axis
			anim.SetBool ("IsRunning", Input.GetKey (KeyCode.LeftShift));
		}

		anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation

		// WALKS
		if (currentBaseState.nameHash == walksState) 
		{
			if (Input.GetButtonDown("Jump"))
			{
				anim.SetBool("Jump", true);
			}
		}

		// LOCOMOTION
		if (currentBaseState.nameHash == locomotionState) {
			if (Input.GetButtonDown ("Jump")) {
				anim.SetBool ("Jump", true);
			}
		}

		// JUMP
		else if (currentBaseState.nameHash == jumpState) {
			//  ..and not still in transition..
			if (!anim.IsInTransition (0)) {
				// ..set the collider height to a float curve in the clip called ColliderHeight
				col.height = anim.GetFloat ("ColliderHeight");
				
				// reset the Jump bool so we can jump again, and so that the state does not loop 
				anim.SetBool ("Jump", false);
			}
			
			// Raycast down from the center of the character.. 
			Ray ray = new Ray (transform.position + Vector3.up, -Vector3.up);
			RaycastHit hitInfo = new RaycastHit ();
			
			if (Physics.Raycast (ray, out hitInfo)) {
				// ..if distance to the ground is more than 1.75, use Match Target
				if (hitInfo.distance > 1.75f) {
					// MatchTarget allows us to take over animation and smoothly transition our character towards a location - the hit point from the ray.
					// Here we're telling the Root of the character to only be influenced on the Y axis (MatchTargetWeightMask) and only occur between 0.35 and 0.5
					// of the timeline of our animation clip
					anim.MatchTarget (hitInfo.point, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask (new Vector3 (0, 1, 0), 0), 0.35f, 0.5f);
				}
			}
		}
		
		// IDLE
		else if (currentBaseState.nameHash == idleState) {
			if (Input.GetKeyDown (KeyCode.P)) {
				// check the hand is empty
				// find the closest pickable object and navigate to it
				objToPickUp = FindClosestPickable ();
				destination = objToPickUp.transform.position;
				transform.LookAt (
					new Vector3(destination.x,
				            transform.position.y,
				            destination.z));
				anim.SetBool ("IsApproaching", true);
			}
		}	
	}

	private GameObject FindClosestPickable() {
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag("Pickable");
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject go in gos) {
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = go;
				distance = curDistance;
			}
		}
		return closest;
	}
}
