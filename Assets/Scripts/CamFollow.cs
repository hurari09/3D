using UnityEngine;

public class CamFollow : MonoBehaviour
{
    // 트랜스폼 컴포넌트 변수 설정
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        // 카메라의 위치를 target의 위치에 일치
        transform.position = target.position;
    }
}
