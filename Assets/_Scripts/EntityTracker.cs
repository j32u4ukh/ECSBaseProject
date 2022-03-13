using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class EntityTracker : MonoBehaviour
{
    private Entity tracking_target = Entity.Null;

    void LateUpdate()
    {
        if(tracking_target != Entity.Null)
        {
            try
            {
                var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                transform.position = manager.GetComponentData<Translation>(tracking_target).Value;
                transform.rotation = manager.GetComponentData<Rotation>(tracking_target).Value;
            }
            catch
            {
                tracking_target = Entity.Null;
            }
        }
    }

    public void setReceivedEntity(Entity tracking_target)
    {
        this.tracking_target = tracking_target;
    }
}
