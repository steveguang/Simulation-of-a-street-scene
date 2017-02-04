using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class NewBehavior : MonoBehaviour, IBehavior
	//where T : MonoBehaviour, ICharacter
{
	[HideInInspector]
	public NewCharacter Characters = null;

    [HideInInspector]
    public BehaviorAgent Agent { get; private set; }

    void Awake() { this.Initialize(); }

    protected void Initialize() 
    {
		this.Characters = this.GetComponent<NewCharacter>(); 
    }

    protected void StartTree(
        Node root, 
        BehaviorAgent.OnStatusChanged statusChanged = null)
    {
        this.Agent = new BehaviorAgent(root, statusChanged);
        this.Agent.BehaviorStart();
    }
	

	public Node Node_Gesture(Val<string> gestureName)
	{
		return new LeafInvoke (
			() => this.Characters.Gesture(gestureName)
			);
	}

	public Node Node_GoTo(Val<Vector3> targ)
	{
		return new LeafInvoke(
			() => this.Characters.NavGoTo(targ), // Approach the target
			() => this.Characters.NavStop());    // Stop if we're terminated
	}
	
	/// <summary>
	/// Approaches a target at a given radius
	/// </summary>
	public Node Node_GoTo(Val<Vector3> targ, Val<float> dist)
	{
		//print ("haha");
		Func<Vector3> position =
			delegate()
		{
			Vector3 targPos = targ.Value;
			Vector3 direction =
			(targPos - transform.position).normalized;
			return targPos - (direction * dist.Value);
		};
		//print ("haha2");
		return new LeafInvoke(
			() => this.Characters.NavGoTo(position),
			() => this.Characters.NavStop());
	}

	public Node Node_Reach(Val<Vector3> targ)
	{
		return new LeafInvoke(
			() => this.Characters.ReachFor(targ),
			() => this.Characters.ReachStop());
	}
	public Node Node_Stop()
	{
		return new LeafInvoke(
			() => this.Characters.ReachStop());
	}

	public Node Nav_Stop()
	{
		return new LeafInvoke (
			() => this.Characters.NavStop()
		);
	}

	public Node Node_HeadLook(Val<Vector3> targ)
	{
		return new LeafInvoke(
			() => this.Characters.HeadLook(targ),
			() => this.Characters.HeadLookStop());
	}

	public Node Node_OrientTowards(Val<Vector3>  targ)
	{
		return new LeafInvoke (
			() => this.Characters.NavOrient(targ)
		);
	}


}

//public class Behavior : Behavior<Character> { }