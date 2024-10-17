using UnityEngine;

public class Fish : MonoBehaviour
{
    public float fishSpeed=5;
    private float randomizedSpeed = 0f;
    private float nextActionTime = -1f;
    private Vector3 targetPosition;

    private void FixedUpdate()
    {
        if (Time.fixedTime >= nextActionTime) // 랜덤 움직임 결정
        {
            randomizedSpeed = fishSpeed * UnityEngine.Random.Range(.5f, 1.5f); // 기본 속도의 50%~150% 범위

            // 랜덤 타겟 설정 후 타겟 방향으로 회전
            targetPosition = PenguinArea.ChooseRandomPosition(transform.parent.position, 100f, 260f, 2f, 13f);
            transform.rotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);

            // 다음 행동 시간 계산
            float timeToGetThere = Vector3.Distance(transform.position, targetPosition) / randomizedSpeed;
            nextActionTime = Time.fixedTime + timeToGetThere;
        }
        else // 목표를 향한 이동
        {
            Vector3 moveVector = randomizedSpeed * transform.forward * Time.fixedDeltaTime;
            if (moveVector.magnitude <= Vector3.Distance(transform.position, targetPosition))
            {
                transform.position += moveVector;
            }
            else // 도착
            {
                transform.position = targetPosition;
                nextActionTime = Time.fixedTime;
            }
        }
    }    
}
