using System;
using UnityEngine;
using TreeSharpPlus;
using System.Collections;

public class AvatarBehavior : NewBehavior
{

	void Start()
	{
		this.StartTree(this.BuildTreeRoot());
	}
	

	protected Node BuildTreeRoot()
	{
		return
			new DecoratorLoop(
				new LeafWait(5000)
				);
	}
}
