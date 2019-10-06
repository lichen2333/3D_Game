using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriestsAndDevils;

public class SSAction : ScriptableObject
{

    public bool enable = true;
    public bool destroy = false;

    public GameObject GameObject { get; set; }
    public Transform Transform { get; set; }
    public ISSActionCallback Callback { get; set; }

    protected SSAction() { }
    public virtual void Start()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Update()
    {
        throw new System.NotImplementedException();
    }
}

public class SSMoveToAction : SSAction{

    public Vector3 target;
    public float speed;
    public static SSMoveToAction GetSSMoveToAction(Vector3 target, float speed){
        SSMoveToAction action = ScriptableObject.CreateInstance<SSMoveToAction>();
        action.target = target;
        action.speed = speed;
        return action;
    }

    public override void Start(){

    }
    public override void Update(){
        this.Transform.position = Vector3.MoveTowards(this.Transform.position, target, speed * Time.deltaTime);
        if (this.Transform.position == target)
        {
            this.destroy = true;
            this.Callback.SSActionEvent(this);
        }
    }
}

public class SSSequenceAction : SSAction, ISSActionCallback{

    public List<SSAction> sequence; 
    public int repeat = -1;
    public int start = 0; 
    public static SSSequenceAction GetSSSequenceAction(int repeat, int start, List<SSAction> sequence){
        SSSequenceAction action = ScriptableObject.CreateInstance<SSSequenceAction>();
        action.repeat = repeat;
        action.sequence = sequence;
        action.start = start;
        return action;
    }
	
    public override void Update(){
        if (sequence.Count == 0) return;
        if (start < sequence.Count)
        {
            sequence[start].Update();
        }
    }

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Complete, 
    							int intParam = 0, string strParam = null, Object objectParam = null) {
        source.destroy = false;
        this.start++;
        if (this.start >= sequence.Count)
        {
            this.start = 0;
            if (repeat > 0) repeat--;
            if (repeat == 0)
            {
                this.destroy = true;
                this.Callback.SSActionEvent(this);
            }
        }
    }
    public override void Start(){
        foreach (SSAction action in sequence)
        {
            action.GameObject = this.GameObject;
            action.Transform = this.Transform;
            action.Callback = this;
            action.Start();
        }
    }
    void OnDestroy()
    {
        foreach (SSAction action in sequence)
        {
            DestroyObject(action);
        }
    }

    
}

public class SSActionManager : MonoBehaviour{
    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
    private List<SSAction> waitingAdd = new List<SSAction>();
    private List<int> waitingDelete = new List<int>();

    protected void Update(){
        foreach (SSAction action in waitingAdd){
            actions[action.GetInstanceID()] = action;
        }
        waitingAdd.Clear();

        foreach (KeyValuePair<int, SSAction> KeyValue in actions){
            SSAction action = KeyValue.Value;
            if (action.destroy){
                waitingDelete.Add(action.GetInstanceID()); 
            }
            else if (action.enable){
                action.Update(); 
            }
        }

        foreach (int key in waitingDelete){
            SSAction action = actions[key];
            actions.Remove(key);
            DestroyObject(action);
        }
        waitingDelete.Clear();
    }
    protected void Start(){ 

    }
    public void RunAction(GameObject gameObject, SSAction action, ISSActionCallback callback){
        action.GameObject = gameObject;
        action.Transform = gameObject.transform;
        action.Callback = callback;
        waitingAdd.Add(action);
        action.Start();
    }
    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Complete,
        int intParam = 0, string strParam = null, Object objectParam = null){
        
    }
}
public class ActionManager : SSActionManager, ISSActionCallback  
{

    private SSMoveToAction boat_move;     
    private SSSequenceAction char_move;     

    public FirstController sceneController;

    protected new void Start()
    {
        sceneController = (FirstController)Director.getInstance().currentSceneController;
        sceneController.actionManager = this;
    }
    public void moveBoat(BoatController boatCtrl, Vector3 dest, float speed)
    {
        boat_move = SSMoveToAction.GetSSMoveToAction(dest, speed);
        this.RunAction(boatCtrl.getGameobj(), boat_move, this);
    }

    public void moveChar(MyCharacterController charCtrl,Vector3 dest, float speed)
    {
        Vector3 middle = dest;
        Vector3 chara = charCtrl.getGameobj().transform.position;
        if (dest.y < chara.y) {	
			middle.y = chara.y;
		}else {	
			middle.x = chara.x;
		}

        SSAction action1 = SSMoveToAction.GetSSMoveToAction(middle, speed);
        SSAction action2 = SSMoveToAction.GetSSMoveToAction(dest, speed);
        char_move = SSSequenceAction.GetSSSequenceAction (1, 0, new List<SSAction> { action1, action2 });
        this.RunAction(charCtrl.getGameobj(), char_move, this);
    }
}

