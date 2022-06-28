using ProtoBuf;
using System;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.MountAndBlade.SynchedMissionObject;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class SynchedMissionObjectSerializer
    {
        private static Type SynchStateEnum = typeof(SynchedMissionObject).GetField("_synchState", BindingFlags.Instance | BindingFlags.NonPublic).GetType();
        [ProtoMember(1)]
        public MissionObjectSerializer SynchedMissionObjectRef { get; set; }
        [ProtoMember(2)]
        public bool VisibilityExcludeParents { get; set; }
        [ProtoMember(3)]
        public bool HasSynchTransformFlag { get; set; }
        [ProtoMember(4)]
        public MatrixFrameSerializer GameEntityFrame { get; set; }
        [ProtoMember(5)]
        public bool SynchStateIsSynchronizeFrameOverTime { get; set; }
        [ProtoMember(6)]
        public MatrixFrameSerializer LastSynchedFrame { get; set; }
        [ProtoMember(7)]
        public float DeltaTime { get; set; }
        [ProtoMember(8)]
        public bool HasSynchAnimationFlag { get; set; }
        [ProtoMember(9)]
        public int AnimationIndexAtChannel { get; set; }
        [ProtoMember(10)]
        public float AnimationSpeedAtChannel { get; set; }
        [ProtoMember(11)]
        public float AnimationParameterAtChannel { get; set; }
        [ProtoMember(12)]
        public bool IsSkeletonAnimationPaused { get; set; }
        [ProtoMember(13)]
        public bool HasSyncColorsFlag { get; set; }
        [ProtoMember(14)]
        public uint Color { get; set; }
        [ProtoMember(15)]
        public uint Color2 { get; set; }
        [ProtoMember(16)]
        public bool IsDisabled { get; set; }

        public SynchedMissionObjectSerializer() { }
        public SynchedMissionObjectSerializer(SynchedMissionObject synchedMissionObject) 
        {
            SynchedMissionObjectRef = synchedMissionObject;
            if (synchedMissionObject != null)
            {
                SynchFlags _initialSynchFlags = (SynchFlags)synchedMissionObject.GetType().GetField("_initialSynchFlags", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(synchedMissionObject); ;
                VisibilityExcludeParents = synchedMissionObject.GameEntity.GetVisibilityExcludeParents();
                HasSynchTransformFlag = _initialSynchFlags.HasAnyFlag(SynchFlags.SynchTransform);
                if (HasSynchTransformFlag)
                {
                    GameEntityFrame = synchedMissionObject.GameEntity.GetFrame();
                    object _synchState = synchedMissionObject.GetType().GetField("_synchState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(synchedMissionObject);
                    SynchStateIsSynchronizeFrameOverTime = _synchState == Enum.ToObject(SynchStateEnum,3);
                    if (SynchStateIsSynchronizeFrameOverTime)
                    {
                        LastSynchedFrame = (MatrixFrame)synchedMissionObject.GetType().GetField("_lastSynchedFrame", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(synchedMissionObject);
                        DeltaTime = (float)synchedMissionObject.GetType().GetField("_duration", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(synchedMissionObject) - (float)synchedMissionObject.GetType().GetField("_timer", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(synchedMissionObject);
                    }
                }
                if (synchedMissionObject.GameEntity.Skeleton != null)
                {
                    int animationIndexAtChannel = synchedMissionObject.GameEntity.Skeleton.GetAnimationIndexAtChannel(0);
                    bool flag = animationIndexAtChannel >= 0;
                    HasSynchAnimationFlag = flag && _initialSynchFlags.HasAnyFlag(SynchFlags.SynchAnimation);
                    if (HasSynchAnimationFlag)
                    {
                        float animationSpeedAtChannel = synchedMissionObject.GameEntity.Skeleton.GetAnimationSpeedAtChannel(0);
                        AnimationIndexAtChannel = animationIndexAtChannel;
                        AnimationSpeedAtChannel = animationSpeedAtChannel;
                        AnimationParameterAtChannel = synchedMissionObject.GameEntity.Skeleton.GetAnimationParameterAtChannel(0);
                        IsSkeletonAnimationPaused = synchedMissionObject.GameEntity.IsSkeletonAnimationPaused();
                    }
                }
                HasSyncColorsFlag = _initialSynchFlags.HasAnyFlag(SynchFlags.SyncColors);
                if (HasSyncColorsFlag)
                {
                    Color = synchedMissionObject.Color;
                    Color2 = synchedMissionObject.Color2;
                }
                IsDisabled = synchedMissionObject.IsDisabled;
            }

        }

        public static implicit operator SynchedMissionObjectSerializer(SynchedMissionObject synchedMissionObject)
        {
            return new SynchedMissionObjectSerializer(synchedMissionObject);
        }

        public static implicit operator SynchedMissionObject(SynchedMissionObjectSerializer serializer)
        {
            MissionObject missionObject = serializer.SynchedMissionObjectRef;
            if (missionObject != null)
            {
                SynchedMissionObject synchedMissionObject = (SynchedMissionObject)missionObject;
                synchedMissionObject.GameEntity.SetVisibilityExcludeParents(serializer.VisibilityExcludeParents);
                if (serializer.HasSynchTransformFlag)
                {
                    MatrixFrame matrixFrame = serializer.GameEntityFrame;
                    synchedMissionObject.GameEntity.SetFrame(ref matrixFrame);
                    if(serializer.SynchStateIsSynchronizeFrameOverTime)
                    {
                        synchedMissionObject.GetType().GetField("_firstFrame",BindingFlags.Instance | BindingFlags.NonPublic).SetValue(synchedMissionObject,synchedMissionObject.GameEntity.GetFrame());
                        synchedMissionObject.GetType().GetField("_lastSynchedFrame",BindingFlags.Instance | BindingFlags.NonPublic).SetValue(synchedMissionObject,serializer.LastSynchedFrame);
                        synchedMissionObject.GetType().GetMethod("SetSynchState",BindingFlags.Instance | BindingFlags.NonPublic).Invoke(synchedMissionObject,new object[] { Enum.ToObject(SynchStateEnum, 3) });
                        float _duration = serializer.DeltaTime;
                        synchedMissionObject.GetType().GetField("_timer",BindingFlags.Instance | BindingFlags.NonPublic).SetValue(synchedMissionObject, 0f);
                        if (_duration.ApproximatelyEqualsTo(0f, 1E-05f))
                        {
                            _duration = 0.1f;
                        }
                        synchedMissionObject.GetType().GetField("_duration", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(synchedMissionObject, _duration);
                    }
                }
                if (synchedMissionObject.GameEntity.Skeleton != null && serializer.HasSynchAnimationFlag)
                {
                    synchedMissionObject.GameEntity.Skeleton.SetAnimationAtChannel(serializer.AnimationIndexAtChannel, 0, serializer.AnimationSpeedAtChannel, 0f, 0f);
                    synchedMissionObject.GameEntity.Skeleton.SetAnimationParameterAtChannel(0, serializer.AnimationParameterAtChannel);
                    if (serializer.IsSkeletonAnimationPaused)
                    {
                        synchedMissionObject.GameEntity.Skeleton.TickAnimationsAndForceUpdate(0.001f, synchedMissionObject.GameEntity.GetGlobalFrame(), true);
                        synchedMissionObject.GameEntity.PauseSkeletonAnimation();
                    }
                    else
                    {
                        synchedMissionObject.GameEntity.ResumeSkeletonAnimation();
                    }
                }
                if (serializer.HasSyncColorsFlag)
                {
                    synchedMissionObject.GameEntity.SetColor(serializer.Color, serializer.Color2, "use_team_color");
                }
                if (serializer.IsDisabled)
                {
                    synchedMissionObject.SetDisabledAndMakeInvisible(false);
                }
                return synchedMissionObject;
            }
            return null;
        }
    }
}
