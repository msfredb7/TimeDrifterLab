﻿using System;
using UnityEngine;

public abstract class BitStreamHead
{
    // Data is read from right to left like such:
    //    ...       3rd    2nd(bool)    ...       ...       ...    1st entry(int)
    // [,,,,,,,] [,,,,,,,] [,,,,,,,] [,,,,,,,] [,,,,,,,] [,,,,,,,] [,,,,,,,]
    //

    protected byte[] _buffer;
    protected int CurrentByteRemains => (8 - BitIndex);

    public int BitIndex { get; private set; }
    public int ByteIndex { get; private set; }
    public int TotalBitIndex => ByteIndex * 8 + BitIndex;
    public int RemainingBits => (_buffer.Length - ByteIndex) * 8 - BitIndex;

    public BitStreamHead(byte[] buffer)
    {
        _buffer = buffer;
        ByteIndex = 0;
        BitIndex = 0;
    }

    public void ResetBufferToZeros()
    {
        for (int i = 0; i < _buffer.Length; i++)
        {
            _buffer[i] = 0;
        }
    }

    public void ResetHead()
    {
        BitIndex = 0;
        ByteIndex = 0;
    }

    public void MoveHeadBackward(int bitCount)
    {
        BitIndex -= bitCount;
        while (BitIndex < 0)
        {
            BitIndex += 8;
            ByteIndex--;
        }

        if(ByteIndex < 0)
        {
            DebugService.LogError("BitStreamHead - Trying to move head below 0");
            ByteIndex = 0;
        }
    }

    public void MoveHeadForward(int bitCount)
    {
        BitIndex += bitCount;
        while (BitIndex >= 8)
        {
            BitIndex -= 8;
            ByteIndex++;
        }
    }
}