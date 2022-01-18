using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class CubeDesignSystem : SystemBase
    {
        EndSimulationEntityCommandBufferSystem endSimulationECBsystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            endSimulationECBsystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        protected override void OnUpdate()
        {
            
        }
        //==========================================================================
        // Job
        //==========================================================================
        struct spawnCursorJob : IJob
        {
            public void Execute()
            {
            }
        }
        struct moveCursorJob : IJob
        {
            public void Execute()
            {
                
            }
        }
        struct putBrickJob : IJob
        {
            public EntityCommandBuffer commandBuffer;
            public void Execute()
            {

            }
        }
        struct delBrickJob : IJob
        {
            public void Execute()
            {
            }
        }
    }

}
