﻿using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using Utils.Extensions;
using Utils.SharpDXExtensions;
//roadmap research
//https://media.discordapp.net/attachments/464158725079564303/468180499806945310/unknown.png?width=1202&height=676
//https://media.discordapp.net/attachments/464158725079564303/468180681646931969/unknown.png?width=1442&height=474

//green = main road
//blue = parking
//yellow = optional road, the AI knows its there, but not direct.



namespace ResourceTypes.Navigation
{
    public struct SplineDefintion
    {
        //12 bytes max!
        public int offset;
        public byte unk0; //knowing 2k, just 128.
        public short NumSplines1; //should be equal to unk2
        public short NumSplines2;
        public float unk3;

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4}", offset, unk0, NumSplines1, NumSplines2, unk3);
        }
    }

    public struct Spline
    {
        public Vector3[] points;
    }

    public struct SplineProperties
    {
        public ushort unk0;
        public ushort unk1;
        public int offset0;
        public ushort laneSize0;
        public ushort laneSize1;
        public int offset1;
        public ushort rangeSize0;
        public ushort rangeSize1;
        public int unk6;
        public unkStruct1Sect1[] lanes;
        public unkStruct1Sect2[] ranges;

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5}, {6}, {7}, {8}", unk0, unk1, offset0, laneSize0, laneSize1, offset1, rangeSize0, rangeSize1, unk6);
        }
    }

    public struct unkStruct1Sect1
    {
        //16 bytes
        public float unk01;
        public ushort unk02;
        public ushort unk03;
        public int unk04;
        public ushort unk05;
        public ushort unk06;
        public unkStruct1Sect1[] children; //SOMETIMES this is the case.

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5}", unk01, unk02, unk03, unk04, unk05, unk06);
        }
    }

    public struct unkStruct1Sect2
    {
        //16 bytes
        public float unk01;
        public float unk02;
        public short unk03;
        public short unk04;
        public float unk05;

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4}", unk01, unk02, unk03, unk04, unk05);
        }
    }

    public struct JunctionDefinition
    {
        public Vector3 position;
        public int offset0;
        public short junctionSize0;
        public short junctionSize1;
        public int offset1;
        public short boundarySize0;
        public short boundarySize1;
        public int junctionIdx;
        public int offset2;
        public short unk5;
        public short unk6;
        public Vector3[] boundaries; //lines, not a box.
        public JunctionSpline[] splines;
        public unkStruct2Sect2 dataSet2;

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}", offset0, junctionSize0, junctionSize1, offset1, boundarySize0, boundarySize1, junctionIdx, offset2, unk5, unk6);
        }
    }

    public struct JunctionSpline
    {
        public short unk0;
        public short unk1;
        public short unk2;
        public short unk3;
        public short unk4;
        public short unk5;
        public int unk6;
        public short pathSize0;
        public short pathSize1;
        public float catmullMod;
        public Vector3[] path;
        public unkStruct2Sect2 unk11;

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}", unk0, unk1, unk2, unk3, unk4, unk5, unk4, unk6, unk5, pathSize0, pathSize1, catmullMod);
        }
    }

    public struct unkStruct2Sect2
    {
        public int unk0;
        public int offset0;
        public short unk1;
        public short unk2;
        public int unk3;
        public int offset1;
        public short unk4;
        public short unk5;
        public short unk6;
        public short unk7;
        public short unk8;
        public short unk9;
        public byte[] unk3Bytes;
    }

    public struct unkStruct3
    {
        public ushort unk0;
        public ushort unk1;

        public override string ToString()
        {
            return string.Format("{0} {1}", unk0, unk1);
        }
    }

    public class Roadmap
    {
        public SplineDefintion[] data1;
        public Spline[] data2;
        public SplineProperties[] data3;
        public JunctionDefinition[] data4;
        public ushort[] unkSet3;
        public unkStruct3[] unkSet4;
        public ushort[] unkSet5;
        public ushort[] unkSet6;

        public Roadmap(FileInfo info)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(info.FullName, FileMode.Open)))
            {
                ReadFromFile(reader);
            }
        }

        public void ReadFromFile(BinaryReader reader)
        {
            int magic = reader.ReadInt32();
            short splineCount1 = reader.ReadInt16();
            short splineCount2 = reader.ReadInt16();
            int splineFinishOffset = reader.ReadInt24();
            reader.ReadByte();
            short splinePropertiesCount1 = reader.ReadInt16();
            short splinePropertiesCount2 = reader.ReadInt16();
            int splinePropertiesOffset = reader.ReadInt24();
            reader.ReadByte();
            short junctionPropertiesCount1 = reader.ReadInt16();
            short junctionPropertiesCount2 = reader.ReadInt16();
            int junctionPropertiesOffset = reader.ReadInt24();
            reader.ReadByte();
            short unkDataSet3Count1 = reader.ReadInt16();
            short unkDataSet3Count2 = reader.ReadInt16();
            int unkDataSet3Offset = reader.ReadInt24();
            reader.ReadByte();
            short unkDataSet4Count1 = reader.ReadInt16();
            short unkDataSet4Count2 = reader.ReadInt16();
            int unkDataSet4Offset = reader.ReadInt24();
            reader.ReadByte();
            short unkDataSet5Count1 = reader.ReadInt16();
            short unkDataSet5Count2 = reader.ReadInt16();
            int unkDataSet5Offset = reader.ReadInt24();
            reader.ReadByte();
            short unkDataSet6Count1 = reader.ReadInt16();
            short unkDataSet6Count2 = reader.ReadInt16();

            int count1 = reader.ReadInt32();
            data1 = new SplineDefintion[splineCount1];

            for (int i = 0; i != splineCount1; i++)
            {
                SplineDefintion data = new SplineDefintion();
                data.offset = reader.ReadInt24();
                data.unk0 = reader.ReadByte();
                data.NumSplines1 = reader.ReadInt16();
                data.NumSplines2 = reader.ReadInt16();
                data.unk3 = reader.ReadSingle();
                data1[i] = data;
            }

            data2 = new Spline[splineCount1];

            for (int i = 0; i != splineCount1; i++)
            {
                Spline splineData = new Spline();
                splineData.points = new Vector3[data1[i].NumSplines1];
                reader.BaseStream.Position = data1[i].offset - 4;

                for (int y = 0; y != data1[i].NumSplines1; y++)
                    splineData.points[y] = Vector3Extenders.ReadFromFile(reader);

                data2[i] = splineData;
            }

            data3 = new SplineProperties[splinePropertiesCount1];

            for (int i = 0; i != data3.Length; i++)
            {
                SplineProperties data = new SplineProperties();
                data.unk0 = reader.ReadUInt16();
                data.unk1 = reader.ReadUInt16();
                data.offset0 = reader.ReadInt24();
                reader.ReadByte();
                data.laneSize0 = reader.ReadUInt16();
                data.laneSize1 = reader.ReadUInt16();
                data.offset1 = reader.ReadInt24();
                reader.ReadByte();
                data.rangeSize0 = reader.ReadUInt16();
                data.rangeSize1 = reader.ReadUInt16();
                data.unk6 = reader.ReadInt32();
                data3[i] = data;
            }

            for (int i = 0; i != splinePropertiesCount1; i++)
            {
                if (i == 666)
                    Console.WriteLine("stop");

                SplineProperties data = data3[i];
                data.lanes = new unkStruct1Sect1[data.laneSize1];

                for (int y = 0; y != data.laneSize1; y++)
                {
                    unkStruct1Sect1 sect = new unkStruct1Sect1();
                    sect.unk01 = reader.ReadSingle();
                    sect.unk02 = reader.ReadUInt16();
                    sect.unk03 = reader.ReadUInt16();
                    sect.unk04 = reader.ReadInt24();
                    reader.ReadByte();
                    sect.unk05 = reader.ReadUInt16();
                    sect.unk06 = reader.ReadUInt16();

                    sect.children = new unkStruct1Sect1[sect.unk05];
                    for (int x = 0; x != sect.unk05; x++)
                    {
                        unkStruct1Sect1 child = new unkStruct1Sect1();
                        child.unk01 = reader.ReadSingle();
                        child.unk02 = reader.ReadUInt16();
                        child.unk03 = reader.ReadUInt16();
                        child.unk04 = reader.ReadInt24();
                        reader.ReadByte();
                        child.unk05 = reader.ReadUInt16();
                        child.unk06 = reader.ReadUInt16();

                        child.children = new unkStruct1Sect1[child.unk05];
                        for (int z = 0; z != child.unk05; z++)
                        {
                            unkStruct1Sect1 child2 = new unkStruct1Sect1();
                            child2.unk01 = reader.ReadSingle();
                            child2.unk02 = reader.ReadUInt16();
                            child2.unk03 = reader.ReadUInt16();
                            child2.unk04 = reader.ReadInt24();
                            reader.ReadByte();
                            child2.unk05 = reader.ReadUInt16();
                            child2.unk06 = reader.ReadUInt16();
                            child.children[z] = child2;
                        }
                        sect.children[x] = child;
                    }
                    data.lanes[y] = sect;
                }

                data.ranges = new unkStruct1Sect2[data.rangeSize0];

                for (int y = 0; y != data.rangeSize0; y++)
                {
                    unkStruct1Sect2 sect = new unkStruct1Sect2();
                    sect.unk01 = reader.ReadSingle();
                    sect.unk02 = reader.ReadSingle();
                    sect.unk03 = reader.ReadInt16();
                    sect.unk04 = reader.ReadInt16();
                    sect.unk05 = reader.ReadSingle();
                    data.ranges[y] = sect;
                }

                data3[i] = data;
            }

            data4 = new JunctionDefinition[junctionPropertiesCount1];
            for (int i = 0; i != junctionPropertiesCount1; i++)
            {
                JunctionDefinition data = new JunctionDefinition();
                data.position = Vector3Extenders.ReadFromFile(reader);
                data.offset0 = reader.ReadInt24();
                reader.ReadByte();
                data.junctionSize0 = reader.ReadInt16();
                data.junctionSize1 = reader.ReadInt16();
                data.offset1 = reader.ReadInt24();
                reader.ReadByte();
                data.boundarySize0 = reader.ReadInt16();
                data.boundarySize1 = reader.ReadInt16();
                data.junctionIdx = reader.ReadInt32();
                data.offset2 = reader.ReadInt24();
                reader.ReadByte();
                data.unk5 = reader.ReadInt16();
                data.unk6 = reader.ReadInt16();
                data4[i] = data;
            }

            for (int i = 0; i != junctionPropertiesCount1; i++)
            {
                if (i == 140)
                    Console.WriteLine("stop right thre!");

                data4[i].splines = new JunctionSpline[data4[i].junctionSize0];

                for (int y = 0; y != data4[i].junctionSize0; y++)
                {
                    JunctionSpline data4Sect = new JunctionSpline();
                    data4Sect.unk0 = reader.ReadInt16();
                    data4Sect.unk1 = reader.ReadInt16();
                    data4Sect.unk2 = reader.ReadInt16();
                    data4Sect.unk3 = reader.ReadInt16();
                    data4Sect.unk4 = reader.ReadInt16();
                    data4Sect.unk5 = reader.ReadInt16();
                    data4Sect.unk6 = reader.ReadInt24();
                    reader.ReadByte();
                    data4Sect.pathSize0 = reader.ReadInt16();
                    data4Sect.pathSize1 = reader.ReadInt16();
                    data4Sect.catmullMod = reader.ReadSingle();
                    Console.WriteLine(data4Sect.ToString());
                    data4[i].splines[y] = data4Sect;
                }
                for (int y = 0; y != data4[i].junctionSize0; y++)
                {
                    data4[i].splines[y].path = new Vector3[data4[i].splines[y].pathSize0];

                    for (int z = 0; z != data4[i].splines[y].pathSize1; z++)
                    {
                        data4[i].splines[y].path[z] = Vector3Extenders.ReadFromFile(reader);
                    }
                }

                data4[i].boundaries = new Vector3[data4[i].boundarySize0];
                for (int y = 0; y != data4[i].boundarySize0; y++)
                {
                    data4[i].boundaries[y] = Vector3Extenders.ReadFromFile(reader);
                }

                if(data4[i].unk5 >= 2)
                {
                    data4[i].dataSet2 = new unkStruct2Sect2();
                    data4[i].dataSet2.unk0 = reader.ReadInt32();
                    data4[i].dataSet2.offset0 = reader.ReadInt24();
                    reader.ReadByte();
                    data4[i].dataSet2.unk1 = reader.ReadInt16();
                    data4[i].dataSet2.unk2 = reader.ReadInt16();
                    data4[i].dataSet2.unk3 = reader.ReadInt32();
                    data4[i].dataSet2.offset1 = reader.ReadInt24();
                    reader.ReadByte();
                    data4[i].dataSet2.unk4 = reader.ReadInt16();
                    data4[i].dataSet2.unk5 = reader.ReadInt16();
                    data4[i].dataSet2.unk6 = reader.ReadInt16();
                    data4[i].dataSet2.unk7 = reader.ReadInt16();
                    data4[i].dataSet2.unk8 = reader.ReadInt16();
                    data4[i].dataSet2.unk9 = reader.ReadInt16();

                    if (data4[i].dataSet2.unk1 > 2 && data4[i].dataSet2.unk2 > 2)
                    {
                        if (data4[i].dataSet2.unk1 == 4)
                        {
                            reader.ReadInt32();
                        }
                        else
                        {
                            reader.ReadInt32();
                            reader.ReadInt32();
                        }
                    }


                    if (data4[i].unk5 == 3)
                        data4[i].dataSet2.unk3Bytes = reader.ReadBytes(16);
                }
                //6 5 2

                //if (reader.BaseStream.Position != data4[i + 1].offset0 - 4)
                //    break; //Console.WriteLine("POSSIBLE ERROR AT " + i);
            }

            unkSet3 = new ushort[unkDataSet3Count1];
            unkSet4 = new unkStruct3[unkDataSet4Count1];
            unkSet5 = new ushort[unkDataSet5Count1];
            unkSet6 = new ushort[unkDataSet6Count1];

            for(int i = 0; i < unkDataSet3Count1; i++)
                unkSet3[i] = reader.ReadUInt16();

            for (int i = 0; i < unkDataSet4Count1; i++)
            {
                unkSet4[i].unk0 = reader.ReadUInt16();
                unkSet4[i].unk1 = reader.ReadUInt16();
            }

            for (int i = 0; i < unkDataSet5Count1; i++)
                unkSet5[i] = reader.ReadUInt16();

            for (int i = 0; i < unkDataSet6Count1; i++)
                unkSet6[i] = reader.ReadUInt16();
        }
    }
}
