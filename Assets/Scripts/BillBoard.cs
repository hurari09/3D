using UnityEngine;

public class BillBoard : MonoBehaviour
{
    public Transform target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // 슬라이더의 방향을 카메라의 방향과 일치시킴
        transform.forward = target.forward;
    }
}
