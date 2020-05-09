﻿using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class FixTransformAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [System.Serializable]
    public struct SerializedData
    {
        public fix3 LocalPosition;
        public fixQuaternion LocalRotation;
        public fix3 LocalScale;
        public FixTransformAuth Parent;
        public int SiblingIndex;
    }

    private Transform _tr;

    public bool HasSetDataFromUnityTransform { get; private set; }
    public fix3 LocalScale { get => _data.LocalScale; set { _data.LocalScale = value; } }
    public fix3 LocalPosition { get => _data.LocalPosition; set { _data.LocalPosition = value; } }
    public fixQuaternion LocalRotation { get => _data.LocalRotation; set { _data.LocalRotation = value; } }

    [UnityEngine.SerializeField]
    [CCC.InspectorDisplay.AlwaysExpand]
    public SerializedData _data = new SerializedData() // needs to be public for Editor access
    {
        LocalScale = new fix3(1, 1, 1)
    };

    public void SetDataFromUnityTransform()
    {
        if (!_tr)
            _tr = transform;

        // we must use "local space data" instead of "world space data" because using world space would
        //  mean using unity's matrix calculations, which is non-deterministic (using floats)
        LocalPosition = ToFix(_tr.localPosition);
        LocalRotation = ToFix(_tr.localRotation);
        LocalScale = ToFix(_tr.localScale);

        HasSetDataFromUnityTransform = true;
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (!HasSetDataFromUnityTransform)
        {
            SetDataFromUnityTransform();
        }

        dstManager.AddComponentData(entity, new FixTranslation() { Value = LocalPosition });
        dstManager.AddComponentData(entity, new FixRotation() { Value = LocalRotation });
        dstManager.AddComponent<RemoveTransformInConversionTag>(entity);

        dstManager.RemoveComponent<Translation>(entity);
        dstManager.RemoveComponent<Rotation>(entity);
        // we don't do anything for the scale at the moment
    }


    private static fixQuaternion ToFix(in Quaternion fixQuat)
    {
        return new fixQuaternion((fix)fixQuat.x, (fix)fixQuat.y, (fix)fixQuat.z, (fix)fixQuat.w);
    }
    private static fix3 ToFix(in Vector3 vec)
    {
        return new fix3((fix)vec.x, (fix)vec.y, (fix)vec.z);
    }
}