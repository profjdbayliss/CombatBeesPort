using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

namespace CombatBees
{
    public class ModelSpawner : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public List<GameObject> allPrefabs;
        public short initialResourceCount;
        public short initialBeeCount;
        public short fieldX;
        public short fieldZ;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            Unity.Mathematics.Random m_Random = new Unity.Mathematics.Random(314159);

            dstManager.AddComponentData(entity, new ModelSpawnerComponent()
            {
            });

            // TODO temporarily placing this here
            dstManager.AddComponentData(entity, new Game
            {
                gameState = GameState.Initialization,
                score = 0,
                currentLevel = 0
            });

            // create the converted prefabs for ecs
            var entityPrefabs = dstManager.AddBuffer<ModelComponent>(entity);

            foreach (var onePrefab in allPrefabs)
            {
                entityPrefabs.Add(new ModelComponent
                {
                    Model = conversionSystem.GetPrimaryEntity(onePrefab)
                });
            }

            // do the initial item spawns
            ////////////////////////////////////
            //int sizeX = fieldX;
            //int sizeZ = fieldZ;
            //Entity prefabEntityType = entityPrefabs[0].Model;
            //Entity beeEntityType = entityPrefabs[1].Model;

            ////Check if the list of prefab is empty
            //if (entityPrefabs.Length == 0)
            //    throw new System.Exception("No prefabs to instantiate");

            ////spawn all initial resources
            //if (initialResourceCount > 0)
            //{
            //    Entity prefab;

            //    float oneThirdFieldX = fieldX * 2 / 3.0f;
            //    float oneThirdFieldZ = fieldZ * 2 / 3.0f;

            //    for (int i = 0; i < initialResourceCount; i++)
            //    {

            //        // Instantiate next in the list hierarchy
            //        prefab = dstManager.Instantiate(prefabEntityType);

            //        int posX = (int)(m_Random.NextFloat(oneThirdFieldX) - sizeX / 3.0f);
            //        int posZ = (int)(m_Random.NextFloat(oneThirdFieldZ) - sizeZ / 3.0f);
            //        dstManager.SetComponentData(prefab, new Unity.Transforms.Translation()
            //        {
            //            Value = new float3(posX - 0.5f, 1, posZ - 0.5f),

            //        });


            //    }
            //}

            //// spawn all initial bees
            //if (initialBeeCount > 0)
            //{
            //    Entity bee;

            //    for (int i = 0; i < initialBeeCount; i++)
            //    {

            //        // Instantiate next in the list hierarchy
            //        bee = dstManager.Instantiate(beeEntityType);

            //        int posX = (int)(m_Random.NextFloat(sizeX * 2) - sizeX);
            //        int posY = (int)(m_Random.NextFloat(5) + 1);
            //        int posZ = (int)(m_Random.NextFloat(sizeZ * 2) - sizeZ);
            //        dstManager.SetComponentData(bee, new Unity.Transforms.Translation()
            //        {
            //            Value = new float3(posX - 0.5f, posY, posZ - 0.5f),

            //        });

            //    }
            //}
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.AddRange(allPrefabs);
        }
    }
}