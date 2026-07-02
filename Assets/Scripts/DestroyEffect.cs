using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    // 효과가 제거될 시간 변수
    public float destroyTime = 1.5f;
    // 경과 시간 측정 변수
    float currentTime = 0;

    // Update is called once per frame
    void Update()
    {
        if(currentTime > destroyTime)
        {
            Destroy(gameObject);
        }
        currentTime += Time.deltaTime;
    }
}
