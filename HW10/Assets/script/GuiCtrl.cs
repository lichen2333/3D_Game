using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using baseCode ;

public class FirstSceneGuiCtrl : MonoBehaviour {
	firstScenceUserAction action ;

	// Use this for initialization
	void Start () {
		action = Director.getInstance().currentSceneController as firstScenceUserAction;
	}
	
	// Update is called once per frame
	void OnGUI () {
		firstScenceUserAction action = Director.getInstance().currentSceneController as firstScenceUserAction;
		string status = action.getStatus ();




		if (status == "playing") {
			if (GUI.Button (new Rect(130 , 10 , 100, 50), "restart")) {
				action.reset ();
			}
		}
		else {
			string showMsg;
			if (status == "lost") {
				showMsg = "you lost!!";
			}
			else {
				showMsg = "you win!!";
			}
			if (GUI.Button (new Rect(Screen.width/2-50, Screen.height/2-25, 100, 50), showMsg) ) {
				action.reset ();
			}
		}

		if (GUI.Button (new Rect(250 , 10 , 100, 50), "tips")) {
			action.nextStep ();
		}
	}
}
