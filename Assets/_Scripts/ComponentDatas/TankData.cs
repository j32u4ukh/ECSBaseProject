using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace udemy
{
    // IComponentData: 定義自己的 Component，在 ECS 當中的作用可以類似於 tag
    // [GenerateAuthoringComponent]: 使得此腳本可以被掛載在物件上
    [GenerateAuthoringComponent]
    public struct TankData : IComponentData
    {

    }
}
