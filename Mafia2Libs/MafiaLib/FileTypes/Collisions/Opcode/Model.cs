﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ResourceTypes.Collisions.Opcode.SerializationUtils;

namespace ResourceTypes.Collisions.Opcode
{
    /// <summary>
    /// The base class for collision models
    /// </summary>
    abstract class ModelBase : IOpcodeSerializable
    {
        [Flags]
        enum ModelFlag
        {
            // ReSharper disable InconsistentNaming
            /// <summary>
            /// Compressed/uncompressed tree
            /// </summary>
            OPC_QUANTIZED = (1 << 0),
            /// <summary>
            /// Leaf/NoLeaf tree
            /// </summary>
            OPC_NO_LEAF = (1 << 1),
            /// <summary>
            /// Special case for 1-node models
            /// </summary>
            OPC_SINGLE_NODE = (1 << 2)
            // ReSharper restore InconsistentNaming
        };

        private const uint SUPPORTED_MODEL_VERSION = 1;

        private ModelFlag modelCode = ModelFlag.OPC_SINGLE_NODE;
        private AABBOptimizedTree tree = new DummyTree();

        public virtual void Load(BinaryReader reader)
        {
            char h0, h1, h2;
            bool littleEndian;

            ReadChunk(out h0, out h1, out h2, out littleEndian, reader);
            if ((h0 != 'O') && (h1 != 'P') && (h2 != 'C'))
            {
                throw new OpcodeException("Invalid 'OPC' header");
            }

            bool platformMismatch = !littleEndian;

            uint version = ReadDword(reader, platformMismatch);
            if (version != SUPPORTED_MODEL_VERSION)
            {
                throw new OpcodeException($"Unsupported model version {version}");
            }

            modelCode = (ModelFlag) ReadDword(reader, platformMismatch);

            CreateTree();
            tree.Load(reader, platformMismatch);
        }

        /// <summary>
        /// Creates an optimized tree according to the modelCode flags
        /// </summary>
        private void CreateTree()
        {
            if (modelCode.HasFlag(ModelFlag.OPC_SINGLE_NODE))
            {
                // special case: tree with one single node (not serialized)
                tree = new DummyTree();
            }
            else if (modelCode.HasFlag(ModelFlag.OPC_NO_LEAF))
            {
                if (modelCode.HasFlag(ModelFlag.OPC_QUANTIZED))
                {
                    tree = new AABBQuantizedNoLeafTree();
                }
                else
                {
                    //tree = new AABBNoLeafTree();
                    throw new NotImplementedException("AABBNoLeafTree is not implemented yet");
                }
            }
            else
            {
                if (modelCode.HasFlag(ModelFlag.OPC_QUANTIZED))
                {
                    //tree = new AABBQuantizedTree();
                    throw new NotImplementedException("AABBQuantizedTree is not implemented yet");
                }
                else
                {
                    //tree = new AABBCollisionTree();
                    throw new NotImplementedException("AABBCollisionTree is not implemented yet");
                }
            }
        }

        public virtual void Save(BinaryWriter writer, Endian endian = Endian.LITTLE)
        {
            // TODO: validation
            bool isLittleEndian = endian == Endian.LITTLE;
            bool platformMismatch = endian == Endian.BIG;

            WriteChunk('O', 'P', 'C', isLittleEndian, writer);
            WriteDword(SUPPORTED_MODEL_VERSION, writer, platformMismatch);

            WriteDword((uint) modelCode, writer, platformMismatch);

            tree.Save(writer, platformMismatch);
        }

        public virtual uint GetUsedBytes()
        {
            return 4 // magic + endian flag
                + 4 // version
                + 4 // modelCode
                + tree.GetUsedBytes();
        }
    }

    /// <summary>
    /// An hybrid collision model
    /// </summary>
    /// <remarks>
    /// See full description in the original sources to get a general idea (<c>OPC_HybridModel.cpp</c>)
    /// </remarks>
    class HybridModel : ModelBase
    {
        /// <summary>
        /// Leaf descriptor
        /// </summary>
        struct LeafTriangles
        {
            /// <summary>
            /// Packed data. Contains number of triangles in the leaf and triangle index for this leaf.  
            /// </summary>
            private uint data;

            public LeafTriangles(uint data)
            {
                this.data = data;
            }

            public static implicit operator uint(LeafTriangles lt) => lt.data;
            public static implicit operator LeafTriangles(uint val) => new LeafTriangles(val);
        }

        private const uint SUPPORTED_HYBRID_MODEL_VERSION = 0;

        private bool littleEndian = true;

        private uint numLeaves = 1;
        private IList<LeafTriangles> leafTriangles = new List<LeafTriangles>();
        private uint numPrimitives;
        private IList<uint> primitiveIndices = new List<uint>();

        public override void Load(BinaryReader reader)
        {
            base.Load(reader);

            char h0, h1, h2;
            ReadChunk(out h0, out h1, out h2, out littleEndian, reader);
            if ((h0 != 'H') && (h1 != 'B') && (h2 != 'M'))
            {
                throw new OpcodeException("Invalid 'HBM' header");
            }

            bool platformMismatch = !littleEndian;

            uint version = ReadDword(reader, platformMismatch);
            if (version != SUPPORTED_HYBRID_MODEL_VERSION)
            {
                throw new OpcodeException($"Unsupported hybrid model version {version}");
            }

            numLeaves = ReadDword(reader, platformMismatch);
            if (numLeaves > 1)
            {
                uint leafTrianglesMaxIndex = ReadDword(reader, platformMismatch);
                IList<uint> leafTrianglesAsUints = ReadIndices(leafTrianglesMaxIndex, numLeaves, reader, platformMismatch);
                leafTriangles = leafTrianglesAsUints.Select(elem => (LeafTriangles) elem).ToList();
            }

            numPrimitives = ReadDword(reader, platformMismatch);
            if (numPrimitives != 0)
            {
                // TODO: implement with ReadIndices (for now I didn't see any file that contains primitives)
                throw new NotImplementedException("Haha, here we have primitives in HBM!");
            }
        }

        public override void Save(BinaryWriter writer, Endian endian = Endian.LITTLE)
        {
            base.Save(writer, endian);
            // TODO: validation

            bool isLittleEndian = endian == Endian.LITTLE;
            bool platformMismatch = endian == Endian.BIG;

            WriteChunk('H', 'B', 'M', isLittleEndian, writer);
            WriteDword(SUPPORTED_HYBRID_MODEL_VERSION, writer, platformMismatch);

            WriteDword(numLeaves, writer, platformMismatch);
            if (numLeaves > 1)
            {
                WriteIndices(leafTriangles.Select(lt => (uint) lt).ToList(), writer, platformMismatch);
            }

            WriteDword(numPrimitives, writer, platformMismatch);
        }

        /// <summary>
        /// Gets the number of bytes used by the tree.
        /// </summary>
        /// <returns>Amount of bytes used</returns>
        public override uint GetUsedBytes()
        {
            return base.GetUsedBytes()
                + 4 // magic + endian flag
                + 4 // version
                + 4 // numLeaves
                + (numLeaves > 1 ? GetUsedBytesByIndices(leafTriangles.Select(lt => (uint)lt).ToList()) : 0) // leafTriangles
                + 4 // numPrimitives
                ; // TODO: primitives
        }
    }
}
