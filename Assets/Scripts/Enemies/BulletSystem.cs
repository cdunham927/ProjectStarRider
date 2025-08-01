using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Pool;

public class BulletSystem : MonoBehaviour
{
	
	/*
	For the prefab, next points at the head of the freelist.
	For inactive instances, next points at the next inactive instance.
	For active instances, next points back at the source prefab.
	*/


	[System.NonSerialized] BulletSystem next;


	public static T Create<T>(T prefab, Vector3 pos, Quaternion rot) where T : BulletSystem
	{
		T result = null;
		if (prefab.next != null)
		{
			/*
			We have a free instance ready to recycle.
			*/
			result = (T)prefab.next;
			prefab.next = result.next;
			result.transform.SetPositionAndRotation(pos, rot);
		}
		else
		{
			/*
			There are no free instances, lets allocate a new one.
			*/
			result = Instantiate<T>(prefab, pos, rot);
			result.gameObject.hideFlags = HideFlags.HideInHierarchy;
		}
		result.next = prefab;
		result.gameObject.SetActive(true);
		return result;
	}

	public void Release()
	{
		if (next == null)
		{
			/*
			This instance wasn't made with Create(), so let's just destroy it.
			*/
			Destroy(gameObject);
		}
		else
		{
			/*
			Retrieve the prefab we were cloned from and add ourselves to its
			free list.
			*/
			var prefab = next;
			gameObject.SetActive(false);
			next = prefab.next;
			prefab.next = this;
		}
	}
}



struct BulletTransformJob : IJobParallelForTransform
{
	[ReadOnly]
	public NativeArray<Vector3> positions;

	public void Execute(int index, TransformAccess transform)
	{
		transform.localPosition = positions[index];
	}
};

class BulletSystems
{
	const int CAPACITY = 4096;
	
	Bullet[] bullets;
	int bulletCount = 0;

	TransformAccessArray transforms;
	NativeArray<Vector3> positionsToWrite;
	JobHandle txJob;

	NativeArray<RaycastCommand> commands;
	NativeArray<RaycastHit> results;
	JobHandle physJob;


	void Awake()
	{
        /*
		Allocate all the buffer memory we'll need up-front
		*/

        bullets = new Bullet[CAPACITY];
		transforms = new TransformAccessArray(CAPACITY);
		results = new NativeArray<RaycastHit>(CAPACITY, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		commands = new NativeArray<RaycastCommand>(CAPACITY, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		positionsToWrite = new NativeArray<Vector3>(CAPACITY, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
	}

	void Update()
	{

		if (bulletCount == 0)
		{
			return;
		}


		float dt = Time.deltaTime;
		
		/*
		Build job input buffers.
		*/

		for (int it = 0; it < bulletCount; ++it)
		{

			var bullet = bullets[it];
			positionsToWrite[it] = bullet.pos + (dt * bullet.speed) * bullet.dir;
			commands[it] = new RaycastCommand(bullet.pos, bullet.dir, bullet.speed * dt);
			
			//var distance = bullets[it].speed * dt;
			/*
			if (Physics.Raycast(bullets[it].pos, bullets[it].dir, out var hit, dist))
			{
					// hit something
					//bullet.Release();
					//HandleBulletImpact(hit);
			}
		
			else
			{
					// advancing
					bullets[it].pos += distance * bullets[it].dir;
					bullets[it].transform.position = bullets[it].pos;
			}*/
		}


		/*
		Schedule a batch transform update.
		*/
		BulletTransformJob job;
		job.positions = positionsToWrite;
		txJob = job.Schedule(transforms);

		/*
		Schedule a batch of physics queries.
		*/
		physJob = RaycastCommand.ScheduleBatch(commands.GetSubArray(0, bulletCount), results.GetSubArray(0, bulletCount), 1);
	}


	void LateUpdate()
	{
		if (bulletCount == 0)
		{
			return;
		}

		float dt = Time.deltaTime;

		/*
		Wait for both jobs to finish, if they're still going.
		*/
		txJob.Complete();
		physJob.Complete();

		/*
		Handle bullet impacts, swapping with the end of the array to remove
		*/
		for (int it = 0; it < bulletCount;)
		{
			var bullet = bullets[it];
			var hit = results[it];
			if (hit.collider == null)
			{
				bullet.pos += (dt * bullet.speed) * bullet.dir;
				++it;
				continue;
			}

			//bullet.Release();
			//HandleHit(hit);
			--bulletCount;
			transforms.RemoveAtSwapBack(it);
			if (it < bulletCount)
			{
				bullets[it] = bullets[bulletCount];
				results[it] = results[bulletCount];
			}
			bullets[bulletCount] = null;
		}

		
	}
		void OnDestroy()
		{
			/*
			Clean up after ourselves.
			*/
			transforms.Dispose();
			results.Dispose();
			commands.Dispose();
			positionsToWrite.Dispose();
		}


}


