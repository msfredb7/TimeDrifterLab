// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class NetMessageRegistry
{
    public static readonly ulong crc = 17330123413781677869;

    public static readonly Type[] types = new Type[]
    {
        typeof(NetMessageChatMessage)
        ,
        typeof(NetMessageChatMessageSubmission)
        ,
        typeof(NetMessageClientHello)
        ,
        typeof(NetMessageExample)
        ,
        typeof(NetMessagePlayerIdAssignment)
        ,
        typeof(NetMessagePlayerJoined)
        ,
        typeof(NetMessagePlayerLeft)
        ,
        typeof(NetMessagePlayerRepertoireSync)
        ,
        typeof(PlayerId)
        ,
        typeof(PlayerInfo)
    };

    public static readonly Dictionary<UInt16, Func<object, int>> netBitSizeMap = new Dictionary<UInt16, Func<object, int>>()
    {
        [0] = (obj) =>
        {
            NetMessageChatMessage castedObj = (NetMessageChatMessage)obj;
            return NetSerializer_NetMessageChatMessage.GetNetBitSize(ref castedObj);
        }
        ,
        [1] = (obj) =>
        {
            NetMessageChatMessageSubmission castedObj = (NetMessageChatMessageSubmission)obj;
            return NetSerializer_NetMessageChatMessageSubmission.GetNetBitSize(ref castedObj);
        }
        ,
        [2] = (obj) =>
        {
            NetMessageClientHello castedObj = (NetMessageClientHello)obj;
            return NetSerializer_NetMessageClientHello.GetNetBitSize(ref castedObj);
        }
        ,
        [3] = (obj) =>
        {
            NetMessageExample castedObj = (NetMessageExample)obj;
            return NetSerializer_NetMessageExample.GetNetBitSize(ref castedObj);
        }
        ,
        [4] = (obj) =>
        {
            NetMessagePlayerIdAssignment castedObj = (NetMessagePlayerIdAssignment)obj;
            return NetSerializer_NetMessagePlayerIdAssignment.GetNetBitSize(ref castedObj);
        }
        ,
        [5] = (obj) =>
        {
            NetMessagePlayerJoined castedObj = (NetMessagePlayerJoined)obj;
            return NetSerializer_NetMessagePlayerJoined.GetNetBitSize(ref castedObj);
        }
        ,
        [6] = (obj) =>
        {
            NetMessagePlayerLeft castedObj = (NetMessagePlayerLeft)obj;
            return NetSerializer_NetMessagePlayerLeft.GetNetBitSize(ref castedObj);
        }
        ,
        [7] = (obj) =>
        {
            NetMessagePlayerRepertoireSync castedObj = (NetMessagePlayerRepertoireSync)obj;
            return NetSerializer_NetMessagePlayerRepertoireSync.GetNetBitSize(ref castedObj);
        }
        ,
        [8] = (obj) =>
        {
            PlayerId castedObj = (PlayerId)obj;
            return NetSerializer_PlayerId.GetNetBitSize(ref castedObj);
        }
        ,
        [9] = (obj) =>
        {
            PlayerInfo castedObj = (PlayerInfo)obj;
            return NetSerializer_PlayerInfo.GetNetBitSize(ref castedObj);
        }
    };

    public static readonly Dictionary<UInt16, Action<object, BitStreamWriter>> serializationMap = new Dictionary<UInt16, Action<object, BitStreamWriter>>()
    {
        [0] = (obj, writer) =>
        {
            NetMessageChatMessage castedObj = (NetMessageChatMessage)obj;
            NetSerializer_NetMessageChatMessage.NetSerialize(ref castedObj, writer);
        }
        ,
        [1] = (obj, writer) =>
        {
            NetMessageChatMessageSubmission castedObj = (NetMessageChatMessageSubmission)obj;
            NetSerializer_NetMessageChatMessageSubmission.NetSerialize(ref castedObj, writer);
        }
        ,
        [2] = (obj, writer) =>
        {
            NetMessageClientHello castedObj = (NetMessageClientHello)obj;
            NetSerializer_NetMessageClientHello.NetSerialize(ref castedObj, writer);
        }
        ,
        [3] = (obj, writer) =>
        {
            NetMessageExample castedObj = (NetMessageExample)obj;
            NetSerializer_NetMessageExample.NetSerialize(ref castedObj, writer);
        }
        ,
        [4] = (obj, writer) =>
        {
            NetMessagePlayerIdAssignment castedObj = (NetMessagePlayerIdAssignment)obj;
            NetSerializer_NetMessagePlayerIdAssignment.NetSerialize(ref castedObj, writer);
        }
        ,
        [5] = (obj, writer) =>
        {
            NetMessagePlayerJoined castedObj = (NetMessagePlayerJoined)obj;
            NetSerializer_NetMessagePlayerJoined.NetSerialize(ref castedObj, writer);
        }
        ,
        [6] = (obj, writer) =>
        {
            NetMessagePlayerLeft castedObj = (NetMessagePlayerLeft)obj;
            NetSerializer_NetMessagePlayerLeft.NetSerialize(ref castedObj, writer);
        }
        ,
        [7] = (obj, writer) =>
        {
            NetMessagePlayerRepertoireSync castedObj = (NetMessagePlayerRepertoireSync)obj;
            NetSerializer_NetMessagePlayerRepertoireSync.NetSerialize(ref castedObj, writer);
        }
        ,
        [8] = (obj, writer) =>
        {
            PlayerId castedObj = (PlayerId)obj;
            NetSerializer_PlayerId.NetSerialize(ref castedObj, writer);
        }
        ,
        [9] = (obj, writer) =>
        {
            PlayerInfo castedObj = (PlayerInfo)obj;
            NetSerializer_PlayerInfo.NetSerialize(ref castedObj, writer);
        }
    };

    public static readonly Dictionary<UInt16, Func<BitStreamReader, object>> deserializationMap = new Dictionary<UInt16, Func<BitStreamReader, object>>()
    {
        [0] = (reader) =>
        {
            NetMessageChatMessage obj = default;
            NetSerializer_NetMessageChatMessage.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [1] = (reader) =>
        {
            NetMessageChatMessageSubmission obj = default;
            NetSerializer_NetMessageChatMessageSubmission.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [2] = (reader) =>
        {
            NetMessageClientHello obj = default;
            NetSerializer_NetMessageClientHello.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [3] = (reader) =>
        {
            NetMessageExample obj = default;
            NetSerializer_NetMessageExample.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [4] = (reader) =>
        {
            NetMessagePlayerIdAssignment obj = default;
            NetSerializer_NetMessagePlayerIdAssignment.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [5] = (reader) =>
        {
            NetMessagePlayerJoined obj = default;
            NetSerializer_NetMessagePlayerJoined.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [6] = (reader) =>
        {
            NetMessagePlayerLeft obj = default;
            NetSerializer_NetMessagePlayerLeft.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [7] = (reader) =>
        {
            NetMessagePlayerRepertoireSync obj = default;
            NetSerializer_NetMessagePlayerRepertoireSync.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [8] = (reader) =>
        {
            PlayerId obj = default;
            NetSerializer_PlayerId.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [9] = (reader) =>
        {
            PlayerInfo obj = default;
            NetSerializer_PlayerInfo.NetDeserialize(ref obj, reader);
            return obj;
        }
    };
}
