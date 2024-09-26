using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class FoodAgent : Agent
{
    public bool flag = false; // ��ư�� ���� �� �ִ����� ��Ÿ���� ����

    public GameObject food = null;
    public GameObject button = null;

    //����, ����, �ൿ�� �����ϸ� �ȴ�.
    //�ൿ -> �յ� �翷���� �̵� (2����) continue
    //������ ���������� -0.001f
    private void Awake()
    {
        flag = true;
    }

    //��������
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
    //�׼��� �޴´�?
    public override void OnActionReceived(ActionBuffers actions)
    {
        float x = actions.ContinuousActions[0];
        float z = actions.ContinuousActions[1];
        transform.position += new Vector3(x, 0, z) * Time.deltaTime;
        AddReward(-0.001f);
    }
    //���Ǽҵ� ó������
    public override void OnEpisodeBegin()
    {
        //�÷��̾� ��ġ �ʱ�ȭ ��ư ��ġ �ʱ�ȭ
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
        if (collision.gameObject.tag == "button") // ��ư�� ������ +1 ����
        {
            if (flag == true)
            {
                food.SetActive(true);
                flag = false;
                AddReward(1.0f);

            }
        }

        if (collision.gameObject.tag == "food") // ������ ������ +1 ����� ���Ǽҵ� ����
        {
            food.SetActive(false);
            food.transform.localPosition = new Vector3(-4, 2, 0);
            flag = true;
            AddReward(1.0f);
            EndEpisode();
        }

        if (collision.gameObject.tag == "wall") // ���� �ε����� -1 ����� ���Ǽҵ� ����
        {
            AddReward(-1.0f);
            EndEpisode();
        }
    }
}
