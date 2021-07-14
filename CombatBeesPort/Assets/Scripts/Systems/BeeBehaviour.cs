﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace CombatBees
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class BeeBehaviour : SystemBase
    {
        Random m_Random;
        EntityQuery m_TeamOneBees;
        EntityQuery m_TeamTwoBees;

        protected override void OnCreate()
        {
            m_Random = new Random(314159);
            m_TeamOneBees = GetEntityQuery(ComponentType.ReadOnly<TeamOneTag>(), ComponentType.ReadOnly<Translation>());
            m_TeamTwoBees = GetEntityQuery(ComponentType.ReadOnly<TeamTwoTag>(), ComponentType.ReadOnly<Translation>());
            base.OnCreate();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            //var teamOne = m_TeamOneBees.ToEntityArray(Allocator.TempJob);
            //var teamTwo = m_TeamTwoBees.ToEntityArray(Allocator.TempJob);
            //var teamOneLength = m_TeamOneBees.CalculateEntityCount();
            //var teamTwoLength = m_TeamTwoBees.CalculateEntityCount();

            var teamOneEntity = GetSingletonEntity<TeamOneTranslationArray>();
            var teamOne = EntityManager.GetBuffer<TeamOneTranslationElement>(teamOneEntity).AsNativeArray().AsReadOnly();
            int teamOneLength = SpawnAll.teamOneLength;

            var teamTwoEntity = GetSingletonEntity<TeamTwoTranslationArray>();
            var teamTwo = EntityManager.GetBuffer<TeamTwoTranslationElement>(teamTwoEntity).AsNativeArray().AsReadOnly();
            int teamTwoLength = SpawnAll.teamTwoLength;

            var randomEntity = GetSingletonEntity<RandomArray>();
            var randArray = EntityManager.GetBuffer<RandomElement>(randomEntity).AsNativeArray().AsReadOnly();
            int maxRandomIndex = EntityManager.GetBuffer<RandomElement>(randomEntity).Length;
            int startingIndex = m_Random.NextInt(0, maxRandomIndex);

            var settings = GetSingleton<SettingsSingleton>();
            var aggression = settings.aggression;
            var attackDistance = settings.attackDistance;
            var chaseForce = settings.chaseForce;
            var attackForce = settings.attackForce;
            var hitDistance = settings.hitDistance;

            var job = Entities.ForEach((int entityInQueryIndex, ref BeeBehaviourComponent beeBehaviour, in BeeMoveComponent beeMove, in Translation trans) =>
            {
                if (beeBehaviour.enemyTarget == Entity.Null && beeBehaviour.resourceTarget == Entity.Null)
                {
                    //int nextRandIndex = (startingIndex + entityInQueryIndex) % maxRandomIndex;
                    //var nextRand = randArray[nextRandIndex].Value;
                    //nextRandIndex = (startingIndex + entityInQueryIndex) % maxRandomIndex;
                    //var tmp1 = teamTwo[0];
                   // var tmp2 = teamOne[0];
            //        //if (math.abs(nextRand) < aggression)
            //        //{
            //        //    if (beeMove.team == 1)
            //        //    {
            //        //        if (teamTwoLength > 0)
            //        //        {
            //        //            beeBehaviour.enemyTarget = teamTwo[(int)math.abs(randArray[nextRandIndex].Value * teamTwoLength)];
            //        //            nextRandIndex = (startingIndex + entityInQueryIndex) % maxRandomIndex;
            //        //        }
            //        //        else
            //        //        {
            //        //            beeBehaviour.enemyTarget = teamOne[(int)math.abs(randArray[nextRandIndex].Value * teamOneLength)];
            //        //            nextRandIndex = (startingIndex + entityInQueryIndex) % maxRandomIndex;
            //        //        }
            //        //        // old code:
            //        //        //    List<Bee> enemyTeam = teamsOfBees[1 - bee.team];
            //        //        //    if (enemyTeam.Count > 0)
            //        //        //    {
            //        //        //        bee.enemyTarget = enemyTeam[Random.Range(0, enemyTeam.Count)];
            //        //        //    }
            //        //    }
            //        //    else
            //        //    {
            //        //        // WORK: still needs to be done
            //        //        //    bee.resourceTarget = ResourceManager.TryGetRandomResource();
            //        //    }
            //        //}
            //    }
            //    //    else if (beeBehaviour.enemyTarget != Entity.Null)
            //    //    {
            //    //        //if (HasComponent<BeeBehaviourComponent>(beeBehaviour.enemyTarget))
            //    //        //{
            //    //        BeeMoveComponent enemyBehaviour = GetComponent<BeeMoveComponent>(beeBehaviour.enemyTarget);
            //    //        Translation enemyPosition = GetComponent<Translation>(beeBehaviour.enemyTarget);
            //    //        if (enemyBehaviour.dead)
            //    //        {
            //    //            beeBehaviour.enemyTarget = Entity.Null;
            //    //            beeBehaviour.chaseVelocity = 0;
            //    //        }
            //    //        else
            //    //        {

            //    //            float3 delta = enemyPosition.Value - trans.Value;
            //    //            float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
            //    //            if (sqrDist > attackDistance * attackDistance)
            //    //            {
            //    //                beeBehaviour.chaseVelocity += delta * (chaseForce * deltaTime / math.sqrt(sqrDist));
            //    //            }
            //    //            else
            //    //            {
            //    //                beeBehaviour.isAttacking = true;
            //    //                beeBehaviour.chaseVelocity += delta * (attackForce * deltaTime / math.sqrt(sqrDist));
            //    //                if (sqrDist < hitDistance * hitDistance)
            //    //                {
            //    //                    // WORK: need proper death and burial here
            //    //                    //                ParticleManager.SpawnParticle(bee.enemyTarget.position, ParticleType.Blood, bee.velocity * .35f, 2f, 6);
            //    //                    //                bee.enemyTarget.dead = true;
            //    //                    //                bee.enemyTarget.velocity *= .5f;
            //    //                    beeBehaviour.enemyTarget = Entity.Null;
            //    //                    //AddComponent<Dead>(beeBehaviour.enemyTarget);
            //    //                }
            //    //            }

            //    //        }
            //    //        //}



                }
            });

            job.Schedule();
        }
    }

}