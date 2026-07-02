using UnityEngine;

public class BombAction : MonoBehaviour
{
    public GameObject bombEffect;

    public float force = 30f;
    public float duration = 0.2f;

    // 충돌체 처리 함수 구현
    private void OnCollisionEnter(Collision collision)
    {
        // 이펙트 프리팹 생성
        GameObject eff = Instantiate(bombEffect);
        // 이펙트 프리팹 위치 설정
        eff.transform.position = transform.position;
        // 자기 오브젝트를 제거
        Destroy(gameObject);

        Collider[] colliders = Physics.OverlapSphere(transform.position, 5);
        foreach (Collider collider in colliders)
        {
            EnemyFSM fsm = collider.GetComponent<EnemyFSM>();
            if (fsm != null)
            {
                Vector3 dir = (collider.transform.position - transform.position).normalized;
                fsm.Knockback(dir, force, duration);
            }
        }
    }
}
