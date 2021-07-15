using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace CombatBees
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class BeeAttractRepel : SystemBase
    {
        Random m_Random;
        EntityQuery m_TeamOneBees;
        EntityQuery m_TeamTwoBees;
        EntityQuery m_Group;

        protected override void OnCreate()
        {
            m_Random = new Random(314159);
            m_TeamOneBees = GetEntityQuery(ComponentType.ReadOnly<TeamOneTag>(), ComponentType.ReadOnly<Translation>());
            m_TeamTwoBees = GetEntityQuery(ComponentType.ReadOnly<TeamTwoTag>(), ComponentType.ReadOnly<Translation>());
            m_Group = GetEntityQuery(typeof(AttractRepelComponent), ComponentType.ReadOnly<BeeMoveComponent>());
        }

        protected override void OnDestroy()
        {           
        }

        struct UpdateJob : IJobEntityBatch
        {
            [NativeDisableContainerSafetyRestrictionAttribute][ReadOnly] public NativeArray<RandomElement>.ReadOnly randArray;
            [NativeDisableContainerSafetyRestrictionAttribute][ReadOnly] public NativeArray<TeamOneTranslationElement>.ReadOnly teamOne;
            [NativeDisableContainerSafetyRestrictionAttribute][ReadOnly] public NativeArray<TeamTwoTranslationElement>.ReadOnly teamTwo;
            [ReadOnly] public int randLength;
            [ReadOnly] public int teamOneLength;
            [ReadOnly] public int teamTwoLength;
            [ReadOnly] public int startIndex;
            [ReadOnly] public ComponentTypeHandle<BeeMoveComponent> beeMoveHandle;
            public ComponentTypeHandle<AttractRepelComponent> beeAttractRepelHandle;

            public void Execute(ArchetypeChunk chunk, int chunkIndex)
            {
                NativeArray<BeeMoveComponent> beesMove = chunk.GetNativeArray(beeMoveHandle);
                NativeArray<AttractRepelComponent> beeAttract = chunk.GetNativeArray(beeAttractRepelHandle);
                int count = chunk.Count;
                for (int i=0; i< count; i++)
                {
                    int nextRandIndex = (startIndex + i) % randLength;
                    if (beesMove[i].team == 1)
                    {
                        float3 attractPos = teamOne[(int)math.abs(math.abs(randArray[nextRandIndex].Value) * teamOneLength)].Value;
                        nextRandIndex = (nextRandIndex + 1) % randLength;
                        float3 repelPos = teamOne[(int)math.abs(math.abs(randArray[nextRandIndex].Value) * teamOneLength)].Value;
                        beeAttract[i] = new AttractRepelComponent
                        {
                            attractivePos = attractPos,
                            repellantPos = repelPos,
                        };
                                           }
                    else if (beesMove[i].team == 2)
                    {
                        float3 attractPos = teamTwo[(int)math.abs(math.abs(randArray[nextRandIndex].Value) * teamTwoLength)].Value;
                        nextRandIndex = (nextRandIndex + 1) % randLength;
                        float3 repelPos = teamTwo[(int)math.abs(math.abs(randArray[nextRandIndex].Value) * teamTwoLength)].Value;
                        beeAttract[i] = new AttractRepelComponent
                        {
                            attractivePos = attractPos,
                            repellantPos = repelPos,
                        };

                    }
                }
                 
                
            }
        }

        protected override void OnUpdate()
        {
            var beeMovementHandle = GetComponentTypeHandle<BeeMoveComponent>(true);
            var beeAttractRepelHandle = GetComponentTypeHandle<AttractRepelComponent>();

            var teamOneEntity = GetSingletonEntity<TeamOneTranslationArray>();
            NativeArray<TeamOneTranslationElement>.ReadOnly teamOne = EntityManager.GetBuffer<TeamOneTranslationElement>(teamOneEntity).AsNativeArray().AsReadOnly();
            int teamOneLength = SpawnAll.teamOneLength;

            var teamTwoEntity = GetSingletonEntity<TeamTwoTranslationArray>();
            var teamTwo = EntityManager.GetBuffer<TeamTwoTranslationElement>(teamTwoEntity).AsNativeArray().AsReadOnly();
            int teamTwoLength = SpawnAll.teamTwoLength;

            var randomEntity = GetSingletonEntity<RandomArray>();
            NativeArray<RandomElement>.ReadOnly randomArray = EntityManager.GetBuffer<RandomElement>(randomEntity).AsNativeArray().AsReadOnly();
            int maxRandomIndex = EntityManager.GetBuffer<RandomElement>(randomEntity).Length;
            int startingIndex = m_Random.NextInt(0, maxRandomIndex);


            var job = new UpdateJob()
            {
                randArray = randomArray,
                teamOne = teamOne,
                teamTwo = teamTwo,
                randLength = maxRandomIndex,
                teamOneLength = teamOneLength,
                teamTwoLength = teamTwoLength,
                startIndex = startingIndex,
                beeMoveHandle = beeMovementHandle,
                beeAttractRepelHandle = beeAttractRepelHandle,
            };

            Dependency = job.ScheduleParallel(m_Group, 1, Dependency);

        }
    }
}