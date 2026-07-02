using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFSM : MonoBehaviour
{
    // 적 상태 상수
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }

    // 적 상태 변수 사용 변수
    EnemyState m_State;

    // 플레이어 발견 범위
    public float findDistance = 8f;
    Transform player;

    // 이동 속도
    public float moveSpeed = 4f;

    // 공격 가능 범위
    public float attackDistance = 2f;

    CharacterController cc;

    // 누적 시간
    float currentTime = 0;
    // 공격 딜레이 시간
    float attackDelay = 2f;
    // 적의 공격력
    public int attackPower = 3;

    // 초기 위치와 회전 저장용 변수
    Vector3 originPos;
    Quaternion originRot;
    // 이동 가능 범위
    public float moveDistance = 20f;

    // 체력 설정
    public int hp;
    int maxHp = 15;

    // 적 hp slider 변수
    public Slider hpSlider;

    // 애니메이터 변수
    Animator anim;

    public GameObject enemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 적 상태의 초기 값은 Idle로 설정
        m_State = EnemyState.Idle;
        // 플레이어의 트랜스폼 컴포넌트를 받아옴
        player = GameObject.Find("Player").transform;

        cc = GetComponent<CharacterController>();

        // 적의 초기 위치와 회전 값을 저장
        originPos = transform.position;
        originRot = transform.rotation;

        hp = maxHp;

        // 자식 오브젝트의 애니메이터 컴포넌트를 받아옴
        anim = transform.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 현재 상태를 체크해 상태별로 정해진 기능 수행
        switch (m_State)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:
                // Damaged();
                break;
            case EnemyState.Die:
                // Die();
                break;
        }
        Debug.Log("hp: " + hp);

        // 현재 체력의 비율을 슬라이더의 값에 반영
        hpSlider.value = (float)hp / (float)maxHp;
    }

    void Idle()
    {
        // 만일 플레이어와의 거리가 findDistance 이내라면 Move 상태로 전환함
        if (Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_State = EnemyState.Move;
            print("상태 전환: Idle -> Move");

            // 이동 애니메이션으로 전환
            anim.SetTrigger("IdleToMove");
        }
    }

    void Move()
    {
        // 만약 현재 위치가 초기 위치에서 moveDistance 값을 넘어간다면 Return 상태로 전환
        if (Vector3.Distance(transform.position, originPos) > moveDistance)
        {
            m_State = EnemyState.Return;
            print("상태 전환: Move -> Return");
        }
        // 만약 플레이어와의 거리가 공격 범위 밖이라면 플레이어를 향해 이동
        else if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            // 이동 방향 설정
            Vector3 dir = (player.position - transform.position).normalized;
            // 플레이어를 향해 이동
            cc.Move(dir * moveSpeed * Time.deltaTime);
            // 플레이어를 향해 방향 전환
            transform.forward = dir;
        }
        // 현재 상태를 공격 상태로 전환
        else
        {
            m_State = EnemyState.Attack;
            print("상태 전환: Move -> Attack");
            // 누적 시간을 공격 딜레이 시간만큼 설정
            currentTime = attackDelay;

            // 공격 대기 애니메이션 실행
            anim.SetTrigger("MoveToAttackDelay");
        }
    }

    void Attack()
    {
        // 만일 플레이어가 공격 범위 안에 있다면 플레이어를 공격
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            // 일정한 시간마다 플레이어를 공격
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                // PlayerMove 스크립트의 DamageAction 함수를 실행
                player.GetComponent<PlayerMove>().DamageAction(attackPower);
                print("공격");
                currentTime = 0;

                // 공격 애니메이션 플레이
                anim.SetTrigger("StartAttack");
            }
        }
        // 현재 상태를 이동으로 전환 (재추격)
        else
        {
            m_State = EnemyState.Move;
            print("상태 전환: Attack -> Move");
            currentTime = 0;
        }
    }

    void Return()
    {
        // 만일 초기 위치에서의 거리가 0.1f 이상이라면 초기 위치 쪽으로 이동
        if (Vector3.Distance(transform.position, originPos) > 0.1f)
        {
            Vector3 dir = (originPos - transform.position).normalized;
            cc.Move(dir * moveSpeed * Time.deltaTime);
            // 원래 위치로 방향을 전환
            transform.forward = dir;
        }
        // 적의 위치를 초기 위치와 회전 값으로 조정하고 현재 상태를 대기로 전환
        else
        {
            transform.position = originPos;
            transform.rotation = originRot;
            hp = maxHp;

            m_State = EnemyState.Idle;
            print("상태 전환: Return -> Idle");

            // 대기 애니메이션으로 전환하는 트랜지션 호출
            anim.SetTrigger("MoveToIdle");
        }
    }

    void Damaged()
    {
        StartCoroutine(DamageProcess());
    }

    // 데미지 코루틴 함수
    IEnumerator DamageProcess()
    {
        // 공격 애니메이션 시간만큼 대기
        yield return new WaitForSeconds(0.5f);
        // 현재 상태를 Move로 전환
        m_State = EnemyState.Move;
        print("상태 전환: Damaged -> Move");
    }

    // 데미지 실행 함수
    public void HitEnemy(int hitPower)
    {
        // 만일 Damaged, Die, Return일 경우 아무런 처리를 하지 않고 함수 종료
        if (m_State == EnemyState.Damaged || m_State == EnemyState.Die || m_State == EnemyState.Return)
        {
            return;
        }
        // 플레이어의 공격력만큼 적의 체력이 감소
        hp -= hitPower;
        // 적의 체력이 0보다 크면 Damage 상태로 전환
        if (hp > 0)
        {
            m_State = EnemyState.Damaged;
            print("상태 전환: Any State -> Damaged");
            Damaged();
        }
        // 적의 체력이 0보다 작거나 같다면 Die 상태로 전환
        else
        {
            m_State = EnemyState.Die;
            print("상태 전환: Any State -> Die");
            Die();
        }
    }

    void Die()
    {
        // 진행 중인 코루틴을 중지
        StopAllCoroutines();
        // 사망 상태 코루틴 실행
        StartCoroutine(DieProcess());

        anim.SetTrigger("IsDie");
    }

    // 사망 상태를 처리하기 위한 코루틴
    IEnumerator DieProcess()
    {
        // 캐릭터 컨트롤러 컴포넌트를 비활성화
        cc.enabled = false;
        // 일정 시간 대기 후 자기 자신을 제거
        yield return new WaitForSeconds(2f);
        print("적 소멸");
        Destroy(gameObject);

        if (enemy != null)
        {
            Vector2 random = Random.insideUnitCircle.normalized * 10;
            Vector3 spawnPos = player.position + new Vector3(random.x, 0, random.y);

            GameObject obj = Instantiate(enemy, spawnPos, Quaternion.identity);
            EnemyFSM fsm = obj.GetComponent<EnemyFSM>();
            if (fsm != null)
            {
                fsm.enemy = this.enemy;
            }
        }
    }

    public void Knockback(Vector3 dir, float force, float duration)
    {
        if (m_State != EnemyState.Die)
        {
            StopCoroutine("KnockbackProcess");
            dir.y = 0;
            dir = dir.normalized;
            StartCoroutine(KnockbackProcess(dir, force, duration));
        }
    }

    IEnumerator KnockbackProcess(Vector3 dir, float force, float duration)
    {
        float time = 0f;
        float decayRate = force / duration;

        time += Time.deltaTime;

        while (time < duration)
        {
            cc.Move(dir * force * Time.deltaTime);
            force = Mathf.Max(0, force - decayRate * Time.deltaTime);

            yield return null;
        }
    }
}
