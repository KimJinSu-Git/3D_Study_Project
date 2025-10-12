using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Addressable Settings")]
    [SerializeField] private string mMonsterAddress = "Monster/Runner"; // 로드할 몬스터의 Addressable 주소
    [SerializeField] private int mSpawnCount = 5;
    [SerializeField] private float mSpawnRadius = 5f;

    private void Start()
    {
        SpawnMonsters();
    }

    private void SpawnMonsters()
    {
        for (int i = 0; i < mSpawnCount; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * mSpawnRadius;
            spawnPosition.y = transform.position.y;
            
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(
                mMonsterAddress, 
                spawnPosition, 
                Quaternion.identity, 
                transform // 스포너의 자식으로 생성
            );

            handle.Completed += OnMonsterSpawned;
        }
    }

    private void OnMonsterSpawned(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject monster = handle.Result;
            Debug.Log($"Addressable 로드 성공 및 생성: {monster.name}");
            
            // TODO: 몬스터 초기화 로직 (예: 스폰 이펙트, AI 활성화)
        }
        else
        {
            Debug.LogError($"Addressable 로드 실패: {handle.OperationException}");
        }
    }
    
    // TODO :: Addressables를 사용한 오브젝트 제거 시 메모리 해제 필수
    public void ReleaseMonster(GameObject monster)
    {
        Addressables.ReleaseInstance(monster); // 메모리 해제
    }
}
