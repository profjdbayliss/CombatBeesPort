using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Tiny.Rendering;
using Unity.Collections;

namespace CombatBees
{
    ///<summary>
    /// Instantiate all main items
    ///</summary>
    public class SpawnAll : ComponentSystem
    {
        Random m_Random;
        private static int init = 0;
        private static float oneThirdFieldX;
        private static float oneThirdFieldZ;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<ModelSpawnerComponent>();
            m_Random = new Random(314159);

            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {

            if (init == 0)
            {
                var settings = GetSingleton<SettingsSingleton>();
                var game = GetSingleton<Game>();
                var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
                init = 1;
                int sizeX = settings.fieldX;
                int sizeZ = settings.fieldZ;
                var settingsEntity = GetSingletonEntity<ModelSpawnerComponent>();
                var allPrefabs = EntityManager.GetBuffer<ModelComponent>(settingsEntity);
                // Check if the list of prefab is empty
                if (allPrefabs.Length == 0)
                    throw new System.Exception("No prefabs to instantiate");

                if (settings.initialResourceCount > 0)
                {

                    Entity prefab;
                    Entity prefabEntityType = allPrefabs[0].Model;
                    oneThirdFieldX = settings.fieldX * 2 / 3.0f;
                    oneThirdFieldZ = settings.fieldZ * 2 / 3.0f;

                    for (int i = 0; i < settings.initialResourceCount; i++)
                    {

                        // Instantiate next in the list hierarchy
                        prefab = cmdBuffer.Instantiate(prefabEntityType);

                        int posX = (int)(m_Random.NextFloat(oneThirdFieldX) - sizeX / 3.0f);
                        int posZ = (int)(m_Random.NextFloat(oneThirdFieldZ) - sizeZ / 3.0f);
                        cmdBuffer.SetComponent(prefab, new Translation()
                        {
                            Value = new float3(posX - 0.5f, 1, posZ - 0.5f),

                        });
                        cmdBuffer.AddComponent<ResourceComponent>(prefab);


                    }
                }

                if (settings.initialBeeCount > 0)
                {
                    Entity bee;
                    Entity beeEntityType = allPrefabs[1].Model;
                    float spawnSpeed = settings.maxSpawnSpeed;

                    for (int i = 0; i < settings.initialBeeCount; i++)
                    {

                        // Instantiate next in the list hierarchy
                        bee = cmdBuffer.Instantiate(beeEntityType);

                        int posX = (int)(m_Random.NextFloat(sizeX * 2) - sizeX);
                        int posY = (int)(m_Random.NextFloat(5) + 1);
                        int posZ = (int)(m_Random.NextFloat(sizeZ * 2) - sizeZ);
                        cmdBuffer.SetComponent(bee, new Translation()
                        {
                            Value = new float3(posX - 0.5f, posY, posZ - 0.5f),

                        });
                        float3 vel = m_Random.NextFloat3Direction() * spawnSpeed;
                        cmdBuffer.AddComponent<BeeComponent>(bee, new BeeComponent { velocity = vel });

                    }
                }

                cmdBuffer.Playback(EntityManager);
                cmdBuffer.Dispose();
            }


        }
    }
}