using UnityEngine;
using TreeSharpPlus;
using System.Collections;

public class NewCharacter : MonoBehaviour//, ICharacter
{
	/// <summary>
	/// The Body interface for this character. Sits below this level in the
	/// ADAPT character stack.
	/// </summary>
	//[HideInInspector]
	//public Body Body;

	[HideInInspector]
	public CharacterControl charactercontrollers = null;


	Vector3 newTarget;

	void Awake() { this.Initialize(); }
	
	public void Initialize() 
	{
		this.charactercontrollers = this.GetComponent<CharacterControl> ();
	}

	/// <summary>
	/// What gesture we're currently running, if any
	/// </summary>
	string currentGesture = null;

	public virtual RunStatus NavGoTo(Val<Vector3> target)
	{
		//print ("im here " );
		//print (this.charactercontrollers.NavCanReach(target.Value));
		//if (this.charactercontrollers.NavCanReach(target.Value) == false)
			//return RunStatus.Failure;
		if (charactercontrollers.anim.GetBool ("Approaching")) {
			//print ("nav running");
			return RunStatus.Running;
		} 
			if (charactercontrollers.IsNavArrived(target.Value)) {
				//print ("nav just arrived");
				return RunStatus.Success;
			} else {
				//print ("nav start");
				charactercontrollers.SetTargetToGo (target.Value);
				return RunStatus.Running;
			}

	}

	public virtual RunStatus ReachFor(Val<Vector3> target)
	{
		// TODO: Heuristic check here - AS
		this.charactercontrollers.ReachFor(target.Value);
		// TODO: Currently, this blocks indefinitely. - AS
		//if (this.Body.ReachHasReached() == true)
		//    return RunStatus.Success;
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus ReachStop()
	{
		this.charactercontrollers.ReachStop();
		if (!this.charactercontrollers.ikReachActive)
			return RunStatus.Success;
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	/// <summary>
	/// Starts the HeadLook controller. Blocks until it's fully faded in.
	/// </summary>
	public virtual RunStatus HeadLook(Val<Vector3> target)
	{
		this.charactercontrollers.HeadLookAt(target.Value);
		// TODO: Maybe actually check the alignment here? - AS
		// TODO: Currently, this blocks indefinitely. - AS
		//if (this.Body.Coordinator.hWeight.IsMax == true)
		//    return RunStatus.Success;
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	/// <summary>
	/// Stops the HeadLook controller. Blocks until it's fully faded out.
	/// </summary>
	public virtual RunStatus HeadLookStop()
	{
		this.charactercontrollers.HeadLookStop();
		// TODO: Maybe actually check the alignment here? - AS
		if (!this.charactercontrollers.ikLookActive) {
			//print ("success");
			return RunStatus.Success;
		}
		//print ("running");
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	/// <summary>
	/// Stops the Navigation system. Blocks until the agent is stopped.
	/// </summary>
	public virtual RunStatus NavStop()
	{
		this.charactercontrollers.ResetTargetToGo ();
		if (charactercontrollers.IsNavStopped()) {
			//print ("nav stop");

			return RunStatus.Success;
		} else {
			return RunStatus.Running;
		}
	}

	public virtual RunStatus NavOrient(Val<Vector3> target)
	{
		//print ("nav orientation");
		charactercontrollers.OrientToward (target.Value);
		return RunStatus.Running;
	}

	public virtual RunStatus Gesture(Val<string> name)
	{
		string nameval = name.Value;

		if (currentGesture != null) {
			if (nameval.Equals (currentGesture)) {
				if (charactercontrollers.IsGestureDone(nameval)) {
					// the animator is not playing any animation
					//print (currentGesture);
					currentGesture = null;
					return RunStatus.Success;
				} else {
					//print ("running");
					return RunStatus.Running;
				}
			} else {
				//print ("blocked");
				return RunStatus.Failure;
			}
		} else {
			currentGesture = nameval;
			//print ("start : " + currentGesture);
		}

		if (nameval.Equals ("Happy")) {
			charactercontrollers.Happy ();
		} else if (nameval.Equals ("Talk")) {
			charactercontrollers.Talk ();
		} else if (nameval.Equals ("Dismiss")) {
			charactercontrollers.Dismiss ();
		} else if (nameval.Equals ("Cocky")) {
			charactercontrollers.Cocky ();
		} else {
			currentGesture = null;
			return RunStatus.Failure;
		}

		return RunStatus.Running;
	}
}
