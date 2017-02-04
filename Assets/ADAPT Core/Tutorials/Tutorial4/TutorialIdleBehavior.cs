using System;
using UnityEngine;
using TreeSharpPlus;
using System.Collections;
public class TutorialIdleBehavior : Behavior
{
	protected Node BuildTreeRoot()
	{
		return
			new DecoratorLoop(
				new Sequence(
					new LeafWait(6000),
					this.Node_Gesture("relieved_sigh")
				)
			);
	}

	void Start()
	{
		base.StartTree(this.BuildTreeRoot());
	}
}
