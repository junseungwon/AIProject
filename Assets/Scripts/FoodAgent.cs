using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class FoodAgent : Agent
{
    public bool flag = false; // 버튼을 누를 수 있는지를 나타내는 변수

    public GameObject food = null;
    public GameObject button = null;

    //관찰, 보상, 행동을 정의하면 된다.
    //행동 -> 앞뒤 양옆으로 이동 (2가지) continue
    //가만히 있을때마다 -0.001f
    private void Awake()
    {
        flag = true;
    }

    //정보수집
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(flag ? 1 : 0);
        if (flag == true)
        {
            Vector3 dirtofoodButton = (button.transform.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dirtofoodButton.x);
            sensor.AddObservation(dirtofoodButton.z);
        }
        else
        {
            Vector3 dirToFood = (food.transform.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dirToFood.x);
            sensor.AddObservation(dirToFood.z);
        }
    }
    //액션을 받는다?
    public override void OnActionReceived(ActionBuffers actions)
    {
        float x = actions.ContinuousActions[0];
        float z = actions.ContinuousActions[1];
        transform.position += new Vector3(x, 0, z) * Time.deltaTime;
        AddReward(-0.001f);
    }
    //에피소드 처음시작
    public override void OnEpisodeBegin()
    {
        //플레이어 위치 초기화 버튼 위치 초기화
        transform.localPosition = new Vector3(0, 0.5f, Random.Range(-2, 2));
        button.transform.localPosition = new Vector3(3.5f, 0f, Random.Range(-3, 3));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionOut = actionsOut.ContinuousActions;
        continuousActionOut[0] = Input.GetAxis("Horizontal");
        continuousActionOut[1] = Input.GetAxis("Vertical");

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "button") // 버튼을 누르면 +1 보상
        {
            if (flag == true)
            {
                food.SetActive(true);
                flag = false;
                AddReward(1.0f);

            }
        }

        if (collision.gameObject.tag == "food") // 음식을 먹으면 +1 보상과 에피소드 종료
        {
            food.SetActive(false);
            food.transform.localPosition = new Vector3(-4, 2, 0);
            flag = true;
            AddReward(1.0f);
            EndEpisode();
        }

        if (collision.gameObject.tag == "wall") // 벽에 부딪히면 -1 보상과 에피소드 종료
        {
            AddReward(-1.0f);
            EndEpisode();
        }
    }
}
