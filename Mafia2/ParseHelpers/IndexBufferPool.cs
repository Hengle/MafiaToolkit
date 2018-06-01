﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Mafia2
{
    public class IndexBufferPool
    {
        //MAX BUFFER SIZE IS 128
        private BufferType version;
        private int numBuffers;
        private int size;
        private IndexBuffer[] buffers;

        private List<IndexBuffer[]> prebuffers = new List<IndexBuffer[]>();

        public IndexBuffer[] Buffers {
            get { return buffers; }
            set { buffers = value; }
        }

        public IndexBufferPool(List<FileInfo> files)
        {
            foreach (FileInfo file in files)
            {
                using (BinaryReader reader = new BinaryReader(File.Open(file.Name, FileMode.Open)))
                    ReadFromFile(reader);
            }
            BuildBuffer();
        }

        public void ReadFromFile(BinaryReader reader)
        {
            version = (BufferType)reader.ReadByte();
            numBuffers = reader.ReadInt32();
            size = reader.ReadInt32();

            IndexBuffer[] buffer = new IndexBuffer[numBuffers];

            for (int i = 0; i != numBuffers; i++)
            {
                buffer[i] = new IndexBuffer(reader);
            }
            prebuffers.Add(buffer);
        }

        public void BuildBuffer()
        {
            int totalsize = 0;

            for (int i = 0; i != prebuffers.Count; i++)
                totalsize += prebuffers[i].Length;

            List<IndexBuffer> listBuffer = new List<IndexBuffer>();

            for (int i = 0; i != prebuffers.Count; i++)
            {
                for (int x = 0; x != prebuffers[i].Length; x++)
                {
                    listBuffer.Add(prebuffers[i][x]);
                }
            }

            Buffers = listBuffer.ToArray();
        }
    }

    public class IndexBuffer
    {
        private ulong hash;
        private int u;
        private int len;
        private ushort[] data;

        public ulong Hash {
            get { return hash; }
            set { hash = value; }
        }
        public ushort[] Data {
            get { return data; }
            set { data = value; }
        }

        public IndexBuffer(BinaryReader reader)
        {
            ReadFromFile(reader);
        }

        public void ReadFromFile(BinaryReader reader)
        {
            hash = reader.ReadUInt64();
            u = reader.ReadInt32();
            len = reader.ReadInt32();
            data = new ushort[len / 2];

            int num = 0;
            int index = 0;

            while (num < len)
            {
                data[index] = reader.ReadUInt16();
                num += 2;
                ++index;
            }

        }
    }
}
