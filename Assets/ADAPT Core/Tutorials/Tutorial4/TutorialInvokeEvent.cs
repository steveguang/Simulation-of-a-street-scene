using UnityEngine;
using TreeSharpPlus;
using System.Collections;

public class TutorialInvokeEvent : MonoBehaviour
{
	public Behavior Wanderer;
	public Behavior Friend;

	protected Node Converse()
	{
		return new Sequence(
			Wanderer.Node_Gesture("acknowledging"),
			Friend.Node_Gesture("dismissing_gesture"),
			Wanderer.Node_Gesture("being_cocky"),
			Friend.Node_Gesture("lenghty_head_nod"));
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.R) == true)
			BehaviorEvent.Run(
				this.ConversationTree(), Wanderer, Friend);
	}
		
	public Node ConversationTree()
	{
		Val<Vector3> WandererPos = Val.Val(() => Wanderer.transform.position);
		Val<Vector3> FriendPos = Val.Val(() => Friend.transform.position);
		return new Sequence(
			this.ApproachAndOrient(WandererPos, FriendPos),
			this.EyeContactAndConverse(WandererPos, FriendPos));
	}

	protected Node ApproachAndOrient(
		Val<Vector3> WandererPos, Val<Vector3> FriendPos)
	{
		return new Sequence(
			// Approach at distance 1.0f
			Friend.Node_GoTo(WandererPos, 1.0f),
			new SequenceParallel(
			Friend.Node_OrientTowards(WandererPos),
			Wanderer.Node_OrientTowards(FriendPos)));
	}

	protected Node EyeContactAndConverse(
		Val<Vector3> WandererPos, Val<Vector3> FriendPos)
	{
		return new Race(
			this.EyeContact(WandererPos, FriendPos),
			this.Converse());
	}

	protected Node EyeContact(
		Val<Vector3> WandererPos, Val<Vector3> FriendPos)
	{
		// Estimate the head position based on height
		Vector3 height = new Vector3(0.0f, 1.85f, 0.0f);
		Val<Vector3> WandererHead = Val.Val(() => WandererPos.Value + height);
		Val<Vector3> FriendHead = Val.Val(() => FriendPos.Value + height);
		return new SequenceParallel(
			Friend.Node_HeadLook(WandererHead),
			Wanderer.Node_HeadLook(FriendHead));
	}
}
