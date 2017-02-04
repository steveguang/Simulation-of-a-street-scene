using UnityEngine;
using TreeSharpPlus;
using System.Collections;

public class BehaviourEvents1 : MonoBehaviour
{
	public NewBehavior FirstGuy;
	public NewBehavior SecondGuy;
	public NewBehavior ThirdGuy;
	public NewBehavior FourthGuy;
	public Transform obj;
	
	void Start ()
	{
		
	}
	
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.S) == true) {
		BehaviorEvent.Run (
			this.ConversationTree (), FirstGuy, SecondGuy, ThirdGuy, FourthGuy);
		}
	}
	
	public Node ConversationTree()
	{
		Val<Vector3> FirstGuyPos = Val.Val(() => FirstGuy.transform.position);
		Val<Vector3> SecondGuyPos = Val.Val(() => SecondGuy.transform.position);
		Val<Vector3> ThirdGuyPos = Val.Val(() => ThirdGuy.transform.position);
		Val<Vector3> FourthGuyPos = Val.Val(() => FourthGuy.transform.position);
		Val<Vector3> objPos = Val.Val(() => obj.transform.position);
		return new Sequence(
			this.ApproachAndOrient(FirstGuyPos, SecondGuyPos, FirstGuy, SecondGuy),
			this.EyeContactAndConverse(FirstGuyPos, SecondGuyPos, FirstGuy, SecondGuy),
			new Race(this.ApproachAndOrient(FirstGuyPos, ThirdGuyPos, FirstGuy, ThirdGuy), this.Greet(FirstGuyPos, ThirdGuyPos)),
			new SequenceParallel(this.Converse(FirstGuy, ThirdGuy), new Sequence(this.ApproachAndOrient(SecondGuyPos, 
		                         FourthGuyPos, SecondGuy, FourthGuy), this.Converse(SecondGuy, FourthGuy)))
			);
	}
	
	protected Node ApproachAndOrient(
		Val<Vector3> FirstGuyPos, Val<Vector3> SecondGuyPos, NewBehavior guy1, NewBehavior guy2)
	{
		return new Sequence(
			// stop current movement
			new SequenceParallel(
				guy1.Nav_Stop(),
				guy2.Nav_Stop()
			),
			// approach
			guy2.Node_GoTo(FirstGuyPos, 0.0f),
			// face to face
			new SequenceParallel(
				guy2.Node_OrientTowards(FirstGuyPos),
				guy1.Node_OrientTowards(SecondGuyPos)
			)
			);
	}
	
	protected Node EyeContactAndConverse(Val<Vector3> FirstGuyPos, Val<Vector3> SecondGuyPos, NewBehavior guy1, NewBehavior guy2)
	{
		return new Race(
			this.EyeContact(FirstGuyPos, SecondGuyPos),
			this.Converse(guy1, guy2));
	}
	
	protected Node Greet(Val<Vector3> FirstGuyPos, Val<Vector3> ThirdGuyPos)
	{
		Vector3 height = new Vector3(0.0f, 1.25f, 0.0f);
		//print ("haha");
		Val<Vector3> FirstGuyHead = Val.Val(() => FirstGuyPos.Value + 1.5f * height);
		return new Sequence( 
		                    ThirdGuy.Node_Reach(FirstGuyHead) 
		                    );
	}
	
	protected Node EyeContact(Val<Vector3> FirstGuyPos, Val<Vector3> SecondGuyPos)
	{
		Vector3 height = new Vector3(0.0f, 1.25f, 0.0f);
		
		Val<Vector3> FirstGuyHead = Val.Val(() => FirstGuyPos.Value + 1.5f * height);
		Val<Vector3> SecondGuyHead = Val.Val(() => SecondGuyPos.Value + height);
		
		return new SequenceParallel(
			SecondGuy.Node_HeadLook(FirstGuyHead),
			FirstGuy.Node_HeadLook(SecondGuyHead));
	}
	
	protected Node Converse(NewBehavior guy1, NewBehavior guy2)
	{
		//print ("lady");
		return new Sequence (
			guy1.Node_Gesture ("Happy"),
			guy2.Node_Gesture ("Talk"),
			guy1.Node_Gesture ("Dismiss"),
			guy2.Node_Gesture ("Cocky"));
	}
}
