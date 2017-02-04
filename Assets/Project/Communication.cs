using UnityEngine;
using TreeSharpPlus;
using System.Collections;

public class Communication : MonoBehaviour
{
	public NewBehavior GuyOne;
	public NewBehavior GuyTwo;
	public NewBehavior GuyThree;
	public NewBehavior GuyFour;
	
	void Start ()
	{
		
	}
	
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.S) == true) {
			
			// now it's safe to run this event
			BehaviorEvent.Run (
				this.ConversationTree (), GuyOne, GuyTwo, GuyThree, GuyFour);
		}
	}
	
	public Node ConversationTree()
	{
		Val<Vector3> GuyOnePos = Val.Val(() => GuyOne.transform.position);
		Val<Vector3> GuyTwoPos = Val.Val(() => GuyTwo.transform.position);
		Val<Vector3> GuyThreePos = Val.Val(() => GuyThree.transform.position);
		Val<Vector3> GuyFourPos = Val.Val(() => GuyFour.transform.position);
		return new Sequence(
			this.ApproachAndOrient(GuyOnePos, GuyTwoPos, GuyOne, GuyTwo),
			this.EyeContactAndConverse(GuyOnePos, GuyTwoPos, GuyOne, GuyTwo),
			new Race(this.ApproachAndOrient(GuyOnePos, GuyThreePos, GuyOne, GuyThree),
		         this.PatOrientTowards(GuyOnePos, GuyThreePos)),
			new SequenceParallel(this.Converse(GuyOne, GuyThree), new Sequence(this.ApproachAndOrient(GuyTwoPos, 
		                                                                                          GuyFourPos, GuyTwo, GuyFour),
		                                                                   this.Converse(GuyTwo, GuyFour)))
			//this.EyeContactAndConverse(GuyOnePos, GuyThreePos, GuyOne, GuyThree)
			/*new SequenceParallel(this.EyeContactAndConverse(GuyOnePos, GuyThreePos, GuyOne, GuyThree), 
		                     new Sequence(GuyFour.Node_GoTo(GuyTwoPos), this.Converse(GuyThree, GuyFour)))*/
			);
	}
	
	protected Node ApproachAndOrient(
		Val<Vector3> GuyOnePos, Val<Vector3> GuyTwoPos, NewBehavior guy1, NewBehavior guy2)
	{
		return new Sequence(
			// stop current movement
			new SequenceParallel(
			guy1.Nav_Stop(),
			guy2.Nav_Stop()
			),
			// approach
			guy2.Node_GoTo(GuyOnePos, 0.0f),
			// face to face
			new SequenceParallel(
			guy2.Node_OrientTowards(GuyOnePos),
			guy1.Node_OrientTowards(GuyTwoPos)
			)
			);
	}
	
	protected Node EyeContactAndConverse(Val<Vector3> GuyOnePos, Val<Vector3> GuyTwoPos, NewBehavior guy1, NewBehavior guy2)
	{
		return new Race(
			this.EyeContact(GuyOnePos, GuyTwoPos),
			this.Converse(guy1, guy2));
	}
	
	protected Node PatOrientTowards(Val<Vector3> GuyOnePos, Val<Vector3> GuyThreePos)
	{
		Vector3 height = new Vector3(0.0f, 1.25f, 0.0f);
		//print ("haha");
		Val<Vector3> GuyOneHead = Val.Val(() => GuyOnePos.Value + 1.5f * height);
		return new Sequence( 
		                    GuyThree.Node_Reach(GuyOneHead) 
		                    );
	}
	
	protected Node EyeContact(Val<Vector3> GuyOnePos, Val<Vector3> GuyTwoPos)
	{
		Vector3 height = new Vector3(0.0f, 1.25f, 0.0f);
		
		Val<Vector3> GuyOneHead = Val.Val(() => GuyOnePos.Value + 1.5f * height);
		Val<Vector3> GuyTwoHead = Val.Val(() => GuyTwoPos.Value + height);
		
		return new SequenceParallel(
			GuyTwo.Node_HeadLook(GuyOneHead),
			GuyOne.Node_HeadLook(GuyTwoHead));
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
