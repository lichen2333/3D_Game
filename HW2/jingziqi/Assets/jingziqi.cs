
using UnityEngine;
using System.Collections;
 
public class jingziqi : MonoBehaviour {
 
	private int turn = 1;	
	private int[,] state = new int[3,3];	
	void Start() {
		init();
	}
 
	void OnGUI() {
		GUIStyle play_button = new GUIStyle{
			fontSize = 30,
			alignment = TextAnchor.MiddleCenter
		};  
		GUIStyle text = new GUIStyle{
			fontSize = 30
		};  
		if (GUI.Button(new Rect(305,450,150,50)," "))
			init();
		GUI.Button(new Rect(330,450,100,50),"play again",style:play_button);
		int result = check();
		if (result==1) {
			GUI.Label(new Rect(330,410,100,50),"O wins!",style:text);
		}
		else if (result==2) {
			GUI.Label(new Rect(330,410,100,50),"X wins!",style:text);
		}
		else if (result==3) {
			GUI.Label(new Rect(330,410,100,50),"Draw!",style:text);
		}
		for (int i=0; i<3; ++i) {
			for (int j=0; j<3; ++j) {
				if (state[i,j]==1)
					GUI.Button(new Rect(300 + i*50,250+j*50,50,50),"O",style:play_button);
				if (state[i,j]==2)
					GUI.Button(new Rect(300 + i*50,250+j*50,50,50),"X",style:play_button);
				if(GUI.Button(new Rect(300 + i*50,250+j*50,50,50),"")) {
					if (result==0) {
						if (turn == 1)
							state[i,j] = 1;
						else
							state[i,j] = 2;
						turn = -turn;
					}
				}
			}
		}
	}
	void init() {
		turn = 1;
		for (int i=0; i<3; ++i) {
			for (int j=0; j<3; ++j) {
				state[i,j] = 0;
			}
		}
	}
	int check() {
		for (int i=0; i<3; ++i) {
			if (state[i,0]!=0 && state[i,0]==state[i,1] && state[i,1]==state[i,2]) {
				return state[i,0];
			}
		}
		for (int j=0; j<3; ++j) {
			if (state[0,j]!=0 && state[0,j]==state[1,j] && state[1,j]==state[2,j]) {
				return state[0,j];
			}
		}
		if (state[1,1]!=0 &&
		    state[0,0]==state[1,1] && state[1,1]==state[2,2] ||
		    state[0,2]==state[1,1] && state[1,1]==state[2,0]) {
			return state[1,1];
		}
		int step = 0;
		for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (state[i, j] != 0)
                    step++;
            }
        }
		if(step==9) return 3;
		return 0;
	}
}
