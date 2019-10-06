using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriestsAndDevils;

public class UserGUI : MonoBehaviour {
	public UserAction action;
	public int status = 0;
	GUIStyle style;
	GUIStyle buttonStyle;

	Judger judge;

	void Start() {
		action = Director.getInstance ().currentSceneController as UserAction;

		style = new GUIStyle();
		style.fontSize = 40;
		style.alignment = TextAnchor.MiddleCenter;

		buttonStyle = new GUIStyle("button");
		buttonStyle.fontSize = 30;

		judge = new Judger();
	}
	void OnGUI() {
		judge.judge(this,style,buttonStyle);
	}
}