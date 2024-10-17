using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PenguinAgent : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;
    public GameObject heartPrefab;
    public GameObject regurgitatedPrefab;
    private PenguinArea penguinArea;
    new private Rigidbody rigidbody;
    private GameObject baby;
    private bool isFull; // If true, penguin has a full stomach

    public override void Initialize()
    {
        base.Initialize();
        penguinArea = GetComponentInParent<PenguinArea>();
        baby = penguinArea.penguinBaby;
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        isFull = false;
        penguinArea.ResetArea();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // ...... 정보를 모음?
        //먹이를 먹었는지랑 방향정보만 담음
        sensor.AddObservation(isFull);
        sensor.AddObservation(transform.forward);

    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float forwardAmount = actionBuffers.DiscreteActions[0];
        float turnAmount = 0;
        //forwardAmount & turnAmount 정의 ......
        if (actionBuffers.DiscreteActions[1] == 1f)
        {
            turnAmount = -1f;
        }
        else if (actionBuffers.DiscreteActions[1] == 2f)
        {
            turnAmount += 1f;
        }

        // forwardAmount & turnAmount 적용
        rigidbody.MovePosition(transform.position + transform.forward * forwardAmount * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * turnSpeed * Time.fixedDeltaTime);

        // Apply a tiny negative reward every step to encourage action
        //......
        if (MaxStep > 0) AddReward(-1f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int forwardAction = 0;
        int turnAction = 0;
        // 위쪽 방향키: forwardAction=1, 왼쪽 방향키: turnAction=1, 오른쪽 방향키: turnAction=2 
        //......
        if (Input.GetKey(KeyCode.UpArrow))
        {
            forwardAction = 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            forwardAction = 1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            forwardAction = 2;
        }
        actionsOut.DiscreteActions.Array[0] = forwardAction;
        actionsOut.DiscreteActions.Array[1] = turnAction;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("fish"))
        {
            EatFish(collision.gameObject);
        }
        else if (collision.transform.CompareTag("baby"))
        {
            RegurgitateFish();
        }
    }

    // Penguin이 fish가 없는 상태로 fish를 잡으면 +1 보상
    private void EatFish(GameObject fishObject)
    {
        if (isFull) return;
        isFull = true;

        penguinArea.RemoveFish(fishObject);

        //......
        AddReward(1);
    }

    // Penguin이 fish를 가진 채로 baby에 도착하면 +1 보상
    private void RegurgitateFish()
    {
        if (!isFull) return;
        isFull = false;

        // Spawn regurgitated fish
        GameObject regurgitatedFish = Instantiate<GameObject>(regurgitatedPrefab);
        regurgitatedFish.transform.parent = transform.parent;
        regurgitatedFish.transform.position = baby.transform.position;
        Destroy(regurgitatedFish, 4f);

        // Spawn heart
        GameObject heart = Instantiate<GameObject>(heartPrefab);
        heart.transform.parent = transform.parent;
        heart.transform.position = baby.transform.position + Vector3.up;
        Destroy(heart, 4f);
        AddReward(+1);
        //......

        // 남은 fish가 없으면 에피소드 종료
        if (penguinArea.FishRemaining <= 0)
        {

            OnEpisodeBegin();//......
        }
    }
}
