using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;

namespace udemy
{
    public class SpawnParallel2 : MonoBehaviour
    {
        public GameObject sheepPrefab;
        Transform[] allSheepTransform;
        const int numSheep = 15000;

        MoveJob moveJob;
        TransformAccessArray transforms;
        JobHandle moveHandle;

        // Start is called before the first frame update
        void Start()
        {
            allSheepTransform = new Transform[numSheep];

            for (int i = 0; i < numSheep; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
                GameObject sheep = Instantiate(sheepPrefab, pos, Quaternion.identity);
                allSheepTransform[i] = sheep.transform;
            }

            transforms = new TransformAccessArray(allSheepTransform);
        }

        private void Update()
        {
            moveJob = new MoveJob { };
            moveHandle = moveJob.Schedule(transforms);            
        }

        private void LateUpdate()
        {
            moveHandle.Complete();
        }

        private void OnDestroy()
        {
            transforms.Dispose();
        }
    }

    struct MoveJob : IJobParallelForTransform
    {
        public void Execute(int index, TransformAccess transform)
        {
            // 0.1f * (transform.rotation * new Vector3(0, 0, 1) same as transform.Translate(0, 0, 0.1f)
            transform.position += 0.1f * (transform.rotation * new Vector3(0, 0, 1));

            if(transform.position.z > 50)
            {
                transform.position = new Vector3(transform.position.x, 0, -50);
            }
        }
    }
}
