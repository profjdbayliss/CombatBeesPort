using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace CombatBees
{
    [UpdateInGroup(typeof(TransformSystemGroup))]
    public class BeeMovementSystem : SystemBase
    {
        Random m_Random;
        EntityQuery m_Group;

        protected override void OnCreate()
        {
            m_Random = new Random(314159);
            m_Group = this.GetEntityQuery(typeof(Translation), typeof(BeeMoveComponent), typeof(Rotation), ComponentType.ReadOnly<AttractRepelComponent>(), ComponentType.ReadOnly<BeeBehaviourComponent>());
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        struct UpdateJob : IJobEntityBatch
        {
            [ReadOnly] public NativeArray<RandomElement> randArray;
            [ReadOnly] public float deltaTime;
            [ReadOnly] public float damping;
            [ReadOnly] public float aggression;
            [ReadOnly] public int randLength;
            [ReadOnly] public int startIndex;
            [ReadOnly] public int sizeX;
            [ReadOnly] public int sizeZ;
            [ReadOnly] public int sizeY;
            [ReadOnly] public int rotationStiffness;
            [ReadOnly] public int flightJitter;
            [ReadOnly] public int teamAttraction;
            [ReadOnly] public int teamRepulsion;
            [ReadOnly] public ComponentTypeHandle<BeeBehaviourComponent> beeBehaviourHandle;
            [ReadOnly] public ComponentTypeHandle<AttractRepelComponent> beeAttractRepelHandle;
            public ComponentTypeHandle<BeeMoveComponent> beeMoveHandle;
            public ComponentTypeHandle<Rotation> rotationHandle;
            public ComponentTypeHandle<Translation> translationHandle;

            public void Execute(ArchetypeChunk chunk, int chunkIndex)
            {
                NativeArray<BeeMoveComponent> bee = chunk.GetNativeArray(beeMoveHandle);
                NativeArray<AttractRepelComponent> attractRepel = chunk.GetNativeArray(beeAttractRepelHandle);
                NativeArray<Translation> translation = chunk.GetNativeArray(translationHandle);
                NativeArray<Rotation> rotation = chunk.GetNativeArray(rotationHandle);
                NativeArray<BeeBehaviourComponent> beeBehaviour = chunk.GetNativeArray(beeBehaviourHandle);
                int count = chunk.Count;

                for (int i = 0; i < count; i++)
                {
                    float3 vel = bee[i].velocity;
                    int nextRandIndex = (startIndex + i) % randLength;
                    float randX = randArray[nextRandIndex].Value;
                    nextRandIndex = (nextRandIndex + 1) % randLength;
                    float randY = randArray[nextRandIndex].Value;
                    nextRandIndex = (nextRandIndex + 1) % randLength;
                    float randZ = randArray[nextRandIndex].Value;
                    float3 direction = new float3(randX, randY, randZ);

                    vel += direction * (flightJitter * deltaTime);
                    vel *= (1f - damping);

                    nextRandIndex = (nextRandIndex + 1) % randLength;
                    //Translation friendPos = bees[(int)math.abs(math.abs(randArray[nextRandIndex]) * bees.Length)];
                    //nextRandIndex = (nextRandIndex + 1) % maxRandomIndex;


                    // calculate attract and repel for bees
                    //if (bee.team == 1)
                    //{
                    //    attractRepel.attractivePos = teamOne[(int)math.abs(math.abs(randArray[nextRandIndex].elem) * teamOneLength)].Value;
                    //    nextRandIndex = (nextRandIndex + 1) % maxRandomIndex;
                    //    attractRepel.repellantPos = teamOne[(int)math.abs(math.abs(randArray[nextRandIndex].elem) * teamOneLength)].Value;
                    //    //attractRepel.repellantPos = attractRepel.attractivePos;
                    //}
                    //else if (bee.team == 2)
                    //{
                    //    attractRepel.attractivePos = teamTwo[(int)math.abs(math.abs(randArray[nextRandIndex].elem) * teamTwoLength)].Value;
                    //    nextRandIndex = (nextRandIndex + 1) % maxRandomIndex;
                    //    attractRepel.repellantPos = teamTwo[(int)math.abs(math.abs(randArray[nextRandIndex].elem) * teamTwoLength)].Value;
                    //    //attractRepel.repellantPos = attractRepel.attractivePos;
                    //}


                    float3 delta = attractRepel[i].attractivePos - translation[i].Value;
                    float dist = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                    if (dist > 0f)
                    {
                        vel += delta * (teamAttraction * deltaTime / dist);
                    }

                    //Translation repellentFriendPos = bees[(int)math.abs(math.abs(randArray[nextRandIndex]) * bees.Length)];
                    //nextRandIndex = (nextRandIndex + 1) % maxRandomIndex;

                    delta = attractRepel[i].repellantPos - translation[i].Value;
                    dist = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                    if (dist > 0f)
                    {
                        vel -= delta * (teamRepulsion * deltaTime / dist);
                    }




                    //if (bee.enemyTarget == null && bee.resourceTarget == null)
                    //{
                    //if (m_Random.NextFloat() < aggression)
                    //{
                    //    List<Bee> enemyTeam = teamsOfBees[1 - bee.team];
                    //    if (enemyTeam.Count > 0)
                    //    {
                    //        bee.enemyTarget = enemyTeam[Random.Range(0, enemyTeam.Count)];
                    //    }
                    //}
                    //else
                    //{
                    //    bee.resourceTarget = ResourceManager.TryGetRandomResource();
                    //}
                    //}

                    //else if (bee.enemyTarget != null)
                    //{
                    //    if (bee.enemyTarget.dead)
                    //    {
                    //        bee.enemyTarget = null;
                    //    }
                    //    else
                    //    {
                    //        delta = bee.enemyTarget.position - bee.position;
                    //        float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
                    //        if (sqrDist > attackDistance * attackDistance)
                    //        {
                    //            bee.velocity += delta * (chaseForce * deltaTime / Mathf.Sqrt(sqrDist));
                    //        }
                    //        else
                    //        {
                    //            bee.isAttacking = true;
                    //            bee.velocity += delta * (attackForce * deltaTime / Mathf.Sqrt(sqrDist));
                    //            if (sqrDist < hitDistance * hitDistance)
                    //            {
                    //                ParticleManager.SpawnParticle(bee.enemyTarget.position, ParticleType.Blood, bee.velocity * .35f, 2f, 6);
                    //                bee.enemyTarget.dead = true;
                    //                bee.enemyTarget.velocity *= .5f;
                    //                bee.enemyTarget = null;
                    //            }
                    //        }
                    //    }
                    //}

                    // set up bee direction and make sure it's not off the playfield
                    float3 movement = deltaTime * vel;

                    float3 newTrans = movement + translation[i].Value;

                    if (math.abs(newTrans.x) > sizeX)
                    {
                        newTrans.x = sizeX * math.sign(newTrans.x);
                        vel.x *= -.5f;
                        vel.y *= .8f;
                        vel.z *= .8f;
                    }
                    if (System.Math.Abs(newTrans.z) > sizeZ)
                    {
                        newTrans.z = sizeZ * math.sign(newTrans.z);
                        vel.z *= -.5f;
                        vel.x *= .8f;
                        vel.y *= .8f;
                    }

                    // the following keeps bees carrying resources from slamming them outside the playfield
                    // WORK: need to add this back in
                    //float resourceModifier = 0f;
                    //if (bee.isHoldingResource)
                    //{
                    //	resourceModifier = ResourceManager.instance.resourceSize;
                    //}
                    if (newTrans.y < 1)
                    {
                        newTrans.y = 1;
                        vel.y *= -.5f;
                        vel.z *= .8f;
                        vel.x *= .8f;
                    }
                    else if (newTrans.y > sizeY)
                    {
                        newTrans.y = sizeY * math.sign(newTrans.y);
                        vel.y *= -.5f;
                        vel.z *= .8f;
                        vel.x *= .8f;
                    }

                    translation[i] = new Translation { Value = newTrans };
                    // WORK: make the following assignment able to be fully parallelized
                    bool beeDead = bee[i].dead;
                    short beeTeam = bee[i].team;

                    float3 oldSmoothPos = bee[i].smoothPosition;

                    // do smoothing for rotation

                    ////if (bee.isAttacking == false)
                    ////{
                    //bee[i].smoothPosition = math.lerp(bee[i].smoothPosition, translation[i].Value, deltaTime * rotationStiffness);
                    bee[i] = new BeeMoveComponent
                    {
                        velocity = vel + beeBehaviour[i].chaseVelocity,
                        dead = beeDead,
                        team = beeTeam,
                        smoothPosition = math.lerp(bee[i].smoothPosition, translation[i].Value, deltaTime * rotationStiffness),
                    };

                    ////}
                    ////else
                    ////{
                    ////    bee.smoothPosition = bee.position;
                    ////}
                    float3 smoothDirection = bee[i].smoothPosition - oldSmoothPos;
                    if (math.all(smoothDirection != float3.zero))
                    {
                        rotation[i] = new Rotation { Value = quaternion.LookRotation(smoothDirection, new float3(0, 1, 0)) };
                    }

                }


            }
        }

        protected override void OnUpdate()
        {
            var translationHandle = GetComponentTypeHandle<Translation>();
            var beeMovementHandle = GetComponentTypeHandle<BeeMoveComponent>();
            var rotationHandle = GetComponentTypeHandle<Rotation>();
            var beeBehaviourHandle = GetComponentTypeHandle<BeeBehaviourComponent>(true);
            var beeAttractRepelHandle = GetComponentTypeHandle<AttractRepelComponent>(true);

            var randomEntity = this.GetSingletonEntity<RandomArray>();
            NativeArray<RandomElement> randomArray = EntityManager.GetBuffer<RandomElement>(randomEntity).AsNativeArray();
            int maxRandomIndex = EntityManager.GetBuffer<RandomElement>(randomEntity).Length;
            int startingIndex = m_Random.NextInt(0, maxRandomIndex);

            float deltaTime = Time.DeltaTime;
            var settings = GetSingleton<SettingsSingleton>();
            var settingsEntity = GetSingletonEntity<ModelSpawnerComponent>();

            int sizeX = settings.fieldX;
            int sizeZ = settings.fieldZ;
            int sizeY = 30;
            int rotationStiffness = settings.rotationStiffness;
            int flightJitter = settings.jitter;

            float damping = settings.damping;
            float aggression = settings.aggression;
            int teamAttraction = settings.teamAttraction;
            int teamRepulsion = settings.teamRepulsion;

            var job = new UpdateJob()
            {
                randArray = randomArray,
                randLength = maxRandomIndex,
                startIndex = startingIndex,
                beeMoveHandle = beeMovementHandle,
                beeAttractRepelHandle = beeAttractRepelHandle,
                translationHandle = translationHandle,
                rotationHandle = rotationHandle,
                beeBehaviourHandle = beeBehaviourHandle,
                deltaTime = deltaTime,
                sizeX = sizeX,
                sizeY = sizeY,
                sizeZ = sizeZ,
                rotationStiffness = rotationStiffness,
                flightJitter = flightJitter,
                damping = damping,
                aggression = aggression,
                teamAttraction = teamAttraction,
                teamRepulsion = teamRepulsion,
            };

            this.Dependency = job.ScheduleParallel(m_Group, 1, Dependency);

        }
    }

}