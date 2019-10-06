using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriestsAndDevils;


	public class Moveable: MonoBehaviour {

		readonly float move_speed = 20;
		int moving_status;	
		Vector3 dest;
		Vector3 middle;

		void Update() {
			if (moving_status == 1) {  
				transform.position = Vector3.MoveTowards (transform.position, middle, move_speed * Time.deltaTime);
				if (transform.position == middle) {
					moving_status = 2;
				}
			} else if (moving_status == 2) {
				transform.position = Vector3.MoveTowards (transform.position, dest, move_speed * Time.deltaTime);
				if (transform.position == dest) {
					moving_status = 0;
				}
			}
		}
		public void setDestination(Vector3 _dest) {    
			dest = _dest;
			middle = _dest;         
			if (_dest.y == transform.position.y) {	
				moving_status = 2;
			}
			else if (_dest.y < transform.position.y) {	
				middle.y = transform.position.y;
			} else {								
				middle.x = transform.position.x;
			}
			moving_status = 1;
		}

		public void reset() {
			moving_status = 0;
		}
	}


	public class MyCharacterController {
		readonly GameObject character;
		readonly Moveable moveableScript;
		readonly ClickGUI clickGUI;
		readonly int characterType;	
		bool _isOnBoat;
		bankController bankController;


		public MyCharacterController(string which_character) {

			if (which_character == "priest") {
				character = Object.Instantiate (Resources.Load ("Priest", typeof(GameObject)), new Vector3(0,0,0), Quaternion.identity, null) as GameObject;
				characterType = 0;
			} else {
				character = Object.Instantiate (Resources.Load ("Devil", typeof(GameObject)),  new Vector3(0,0,0), Quaternion.identity, null) as GameObject;
				characterType = 1;
			}
			moveableScript = character.AddComponent (typeof(Moveable)) as Moveable;

			clickGUI = character.AddComponent (typeof(ClickGUI)) as ClickGUI;
			clickGUI.setController (this);
		}

		public void setName(string name) {
			character.name = name;
		}

		public void setPosition(Vector3 pos) {
			character.transform.position = pos;
		}

		public void moveToPosition(Vector3 destination) {
			moveableScript.setDestination(destination);
		}

		public int getType() {	
			return characterType;
		}

		public string getName() {
			return character.name;
		}

		public void getOnBoat(BoatController boatCtrl) {
			bankController = null;
			character.transform.parent = boatCtrl.getGameobj().transform;
			_isOnBoat = true;
		}

		public void getOnbank(bankController bankCtrl) {
			bankController = bankCtrl;
			character.transform.parent = null;
			_isOnBoat = false;
		}

		public bool isOnBoat() {
			return _isOnBoat;
		}

		public bankController getbankController() {
			return bankController;
		}
		public GameObject getGameobj() {
			return character;
		}

		public void reset() {
			moveableScript.reset ();
			bankController = (Director.getInstance ().currentSceneController as FirstController).frombank;
			getOnbank (bankController);
			setPosition (bankController.getEmptyPosition ());
			bankController.getOnbank (this);
		}
	}
	public class bankController {
		readonly GameObject bank;
		readonly Vector3 from_pos = new Vector3(9,1,0);
		readonly Vector3 to_pos = new Vector3(-9,1,0);
		readonly Vector3[] positions;
		readonly int to_or_from;	
		MyCharacterController[] passengerPlaner;

		public bankController(string _to_or_from) {
			positions = new Vector3[] {new Vector3(6.5F,4.0f,0), new Vector3(7.5F,4.0f,0), new Vector3(8.5F,4.0f,0), 
				new Vector3(9.5F,4.0f,0), new Vector3(10.5F,4.0f,0), new Vector3(11.5F,4.0f,0)};

			passengerPlaner = new MyCharacterController[6];

			if (_to_or_from == "from") {
				bank = Object.Instantiate (Resources.Load ("Land", typeof(GameObject)), from_pos, Quaternion.identity, null) as GameObject;
				bank.name = "from";
				to_or_from = 1;
			} else {
				bank = Object.Instantiate (Resources.Load ("Land", typeof(GameObject)), to_pos, Quaternion.identity, null) as GameObject;
				bank.name = "to";
				to_or_from = -1;
			}
		}

		public int getEmptyIndex() {
			for (int i = 0; i < passengerPlaner.Length; i++) {
				if (passengerPlaner [i] == null) {
					return i;
				}
			}
			return -1;
		}

		public Vector3 getEmptyPosition() {
			Vector3 pos = positions [getEmptyIndex ()];
			pos.x *= to_or_from;
			return pos;
		}

		public void getOnbank(MyCharacterController characterCtrl) {
			int index = getEmptyIndex ();
			passengerPlaner [index] = characterCtrl;
		}

		public MyCharacterController getOffbank(string passenger_name) {	
			for (int i = 0; i < passengerPlaner.Length; i++) {
				if (passengerPlaner [i] != null && passengerPlaner [i].getName () == passenger_name) {
					MyCharacterController charactorCtrl = passengerPlaner [i];
					passengerPlaner [i] = null;
					return charactorCtrl;
				}
			}
			Debug.Log ("cant find passenger on bank: " + passenger_name);
			return null;
		}

		public int get_to_or_from() {
			return to_or_from;
		}

		public int[] getCharacterNum() {
			int[] count = {0, 0};
			for (int i = 0; i < passengerPlaner.Length; i++) {
				if (passengerPlaner [i] == null)
					continue;
				if (passengerPlaner [i].getType () == 0) {	
					count[0]++;
				} else {
					count[1]++;
				}
			}
			return count;
		}

		public void reset() {
			passengerPlaner = new MyCharacterController[6];
		}
	}

	public class BoatController {
		readonly GameObject boat;
		readonly Moveable moveableScript;
		readonly Vector3 fromPosition = new Vector3 (3.0f, 1.5f, 0.0f);
		readonly Vector3 toPosition = new Vector3 (-3.0f, 1.5f, 0.0f);
		readonly Vector3[] from_positions;
		readonly Vector3[] to_positions;

		int to_or_from; 
		MyCharacterController[] passenger = new MyCharacterController[2];

		public BoatController() {
			to_or_from = 1;

			from_positions = new Vector3[] { new Vector3 (4.5F, 3.0F, 0), new Vector3 (5.5F, 3.0F, 0) };
			to_positions = new Vector3[] { new Vector3 (-5.5F, 3.0F, 0), new Vector3 (-4.5F, 3.0F, 0) };

			boat = Object.Instantiate (Resources.Load ("Boat", typeof(GameObject)), fromPosition, Quaternion.identity, null) as GameObject;
			boat.name = "boat";

			moveableScript = boat.AddComponent (typeof(Moveable)) as Moveable;
			boat.AddComponent (typeof(ClickGUI));
		}


		public void Move() {   
			if (to_or_from == -1) {
				moveableScript.setDestination(fromPosition);
				to_or_from = 1;
			} else {
				moveableScript.setDestination(toPosition);
				to_or_from = -1;
			}
		}

		public Vector3 getBoatDest(){
			Vector3 pos;
			if (to_or_from == -1) {
				pos = fromPosition;
			} else {
				pos = toPosition;
			}
			return pos;
		}
		public void pos_change(){
			to_or_from = 0 - to_or_from;
		}
		public int getEmptyIndex() {
			for (int i = 0; i < passenger.Length; i++) {
				if (passenger [i] == null) {
					return i;
				}
			}
			return -1;
		}

		public bool isEmpty() {
			for (int i = 0; i < passenger.Length; i++) {
				if (passenger [i] != null) {
					return false;
				}
			}
			return true;
		}

		public Vector3 getEmptyPosition() {
			Vector3 pos;
			int emptyIndex = getEmptyIndex ();
			if (to_or_from == -1) {
				pos = to_positions[emptyIndex];
			} else {
				pos = from_positions[emptyIndex];
			}
			return pos;
		}
		
		public void GetOnBoat(MyCharacterController characterCtrl) {
			int index = getEmptyIndex ();
			passenger [index] = characterCtrl;
		}

		public MyCharacterController GetOffBoat(string passenger_name) {
			for (int i = 0; i < passenger.Length; i++) {
				if (passenger [i] != null && passenger [i].getName () == passenger_name) {
					MyCharacterController charactorCtrl = passenger [i];
					passenger [i] = null;
					return charactorCtrl;
				}
			}
			Debug.Log ("Cant find passenger in boat: " + passenger_name);
			return null;
		}

		public GameObject getGameobj() {
			return boat;
		}

		public int get_to_or_from() { 
			return to_or_from;
		}

		public int[] getCharacterNum() {   
			int[] count = {0, 0};
			for (int i = 0; i < passenger.Length; i++) {
				if (passenger [i] == null)
					continue;
				if (passenger [i].getType () == 0) {	
					count[0]++;
				} else {
					count[1]++;
				}
			}
			return count;
		}

		public void reset() {
			if (to_or_from == -1) {
				boat.transform.position = fromPosition;
            	to_or_from = 1;
			}
			moveableScript.reset ();   
			passenger = new MyCharacterController[2];
		}
	}