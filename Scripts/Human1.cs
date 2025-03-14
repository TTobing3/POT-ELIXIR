using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Human1 : MonoBehaviour
{
    public Vector2 moveRange = new Vector2(5f, 0f); // 이동 범위
    public float moveDuration = 2f; // 이동 시간

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        MoveToRandomPosition();
    }

    void MoveToRandomPosition()
    {
        // 시작 위치를 기준으로 랜덤 위치 계산
        Vector3 targetPos = startPos + new Vector3(
            Random.Range(-moveRange.x, moveRange.x),
            Random.Range(-moveRange.y, moveRange.y),
            0f
        );

        moveDuration = Random.Range(15, 30);

        // DOTween으로 이동 후 완료되면 재귀 호출
        transform.DOMove(targetPos, moveDuration)
                 .SetEase(Ease.Linear)
                 .OnComplete(MoveToRandomPosition);
    }
}
