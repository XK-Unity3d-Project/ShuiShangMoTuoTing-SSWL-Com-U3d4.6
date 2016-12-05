using UnityEngine;
using System.Collections;

public class DHc : MonoBehaviour {

	public Animator animatorNPC;
	
	void OnTriggerEnter(Collider other)
	{
		animatorNPC.SetTrigger("DHSpecialAction");
	}

}
