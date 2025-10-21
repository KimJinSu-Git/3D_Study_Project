using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Addressable Settings")]
    [SerializeField] private string mMonsterAddress = "Monster/Runner"; // 로드할 몬스터의 Addressable 주소
    [SerializeField] private string mHealthBarAddress = "UI/HealthBar";
    [SerializeField] private int mSpawnCount = 5;
    [SerializeField] private float mSpawnRadius = 5f;
    
    private List<GameObject> _spawnedMonsters = new List<GameObject>();
    
    private async void Start()
    {
        // 비동기 로드
        await SpawnMonstersAsync();
    }

    private async Task SpawnMonstersAsync()
        {
            for (int i = 0; i < mSpawnCount; i++)
            {
                Vector3 spawnPosition = transform.position + Random.insideUnitSphere * mSpawnRadius;
                spawnPosition.y = transform.position.y;
                
                // 몬스터 비동기 생성
                AsyncOperationHandle<GameObject> monsterHandle = Addressables.InstantiateAsync(
                    mMonsterAddress, 
                    spawnPosition, 
                    Quaternion.identity, 
                    transform
                );
                
                // 몬스터 생성 완료 대기 (await)
                await monsterHandle.Task; 
                
                if (monsterHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject monster = monsterHandle.Result;
                    _spawnedMonsters.Add(monster);
                    
                    // HP Bar UI 비동기 생성 및 부착
                    AsyncOperationHandle<GameObject> uiHandle = Addressables.InstantiateAsync(
                        mHealthBarAddress,
                        monster.transform.position + Vector3.up * 1.5f,
                        Quaternion.identity,
                        monster.transform
                    );
                    
                    await uiHandle.Task;

                    if (uiHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        GameObject healthBar = uiHandle.Result;
                        Debug.Log($"몬스터 {monster.name}와 HP바 생성 성공 (Addressables)");
                    }
                    else
                    {
                        Debug.LogError($"HP Bar UI 로드 실패: {uiHandle.OperationException}");
                    }
                }
                else
                {
                    Debug.LogError($"몬스터 로드 실패: {monsterHandle.OperationException}");
                }
            }
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
    
    private void OnDestroy()
    {
        foreach (GameObject monster in _spawnedMonsters)
        {
            if (monster != null)
            {
                Addressables.ReleaseInstance(monster);
            }
        }
        _spawnedMonsters.Clear();
    }
}
