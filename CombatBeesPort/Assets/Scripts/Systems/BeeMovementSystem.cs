using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace CombatBees
{
    public class BeeMovementSystem : SystemBase
    {
        Random m_Random;
        EntityQuery m_AllBees;

        protected override void OnCreate()
        {
            m_Random = new Random(314159);    
            m_AllBees = GetEntityQuery(typeof(BeeComponent), typeof(Translation));

            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            var settings = GetSingleton<SettingsSingleton>();
            int sizeX = settings.fieldX;
            int sizeZ = settings.fieldZ;
            int sizeY = 30;
            int flightJitter = settings.jitter;
            float damping = settings.damping;
            float aggression = settings.aggression;
            int teamAttraction = settings.teamAttraction;
            int teamRepulsion = settings.teamRepulsion;

            int randMaxCount = 10;
            //NativeArray<float> randomArray = new NativeArray<float>(randMaxCount, Allocator.Temp);
            float rand1 = m_Random.NextFloat(-1, 1);
            float rand2 = m_Random.NextFloat(-1, 1);
            float rand3 = m_Random.NextFloat(-1, 1);
             
            //for (int i=0; i<randMaxCount; i++)
            //{
            //    randomArray[i] = m_Random.NextFloat(-1, 1);
            //}

            EntityManager.GetAllEntities();
            var bees = m_AllBees.ToComponentDataArray<Translation>(Allocator.Temp);

            Entities.ForEach((ref Translation translation, ref BeeComponent bee, in Rotation rotation) =>
            {
                // test code:
                //translation.Value += math.mul(rotation.Value, new float3(0, 1, 0)) * deltaTime;                
                float3 direction = new float3(rand1, rand2, rand3);
                
                bee.velocity += direction * (flightJitter * deltaTime);
                bee.velocity *= (1f - damping);

                Translation friendPos = bees[(int)math.abs(rand3*bees.Length)];
           
                float3 delta = friendPos.Value - translation.Value;
                float dist = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                if (dist > 0f)
                {
                    bee.velocity += delta * (teamAttraction * deltaTime / dist);
                }

                Translation repellentFriendPos = bees[(int)math.abs(rand1 * bees.Length)];
                
                delta = repellentFriendPos.Value - translation.Value;
                dist = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                if (dist > 0f)
                {
                    bee.velocity -= delta * (teamRepulsion * deltaTime / dist);
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


                float3 movement = deltaTime * bee.velocity;
				translation.Value = movement + translation.Value;

                if (math.abs(translation.Value.x) > sizeX )
                {
                    translation.Value.x = sizeX  * math.sign(translation.Value.x);
                    bee.velocity.x *= -.5f;
                    bee.velocity.y *= .8f;
                    bee.velocity.z *= .8f;
                }
                if (System.Math.Abs(translation.Value.z) > sizeZ )
                {
                    translation.Value.z = sizeZ * math.sign(translation.Value.z);
                    bee.velocity.z *= -.5f;
                    bee.velocity.x *= .8f;
                    bee.velocity.y *= .8f;
                }

                // the following keeps bees carrying resources from slamming them outside the playfield
                // WORK: need to add this back in
                //float resourceModifier = 0f;
                //if (bee.isHoldingResource)
                //{
                //	resourceModifier = ResourceManager.instance.resourceSize;
                //}
                if (translation.Value.y < 1 )
                {
                    translation.Value.y = 1;
                    bee.velocity.y *= -.5f;
                    bee.velocity.z *= .8f;
                    bee.velocity.x *= .8f;
                } else if (translation.Value.y > sizeY)
                {
                    translation.Value.y = sizeY * math.sign(translation.Value.y);
                    bee.velocity.y *= -.5f;
                    bee.velocity.z *= .8f;
                    bee.velocity.x *= .8f;
                }
            }).Schedule();

            //randomArray.Dispose();
        }
    }

}