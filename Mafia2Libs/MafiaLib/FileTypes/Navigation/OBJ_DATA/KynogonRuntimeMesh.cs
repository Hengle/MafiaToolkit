﻿using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.SharpDXExtensions;
using Utils.StringHelpers;

namespace ResourceTypes.Navigation
{
    public struct Unk10DataSet
    {
        public BoundingBox B1;
        public int UnkOffset;
        public int Unk20;
    }

    public struct Unk12DataSet
    {
        public BoundingBox B1;
        public int Unk01; //-1?
        public int Unk02; //-1?
        public float Unk03;
        public float Unk04;
        public float Unk05;
    }

    public struct Unk14DataSet
    {
        public int Offset;
        public Vector3[] Points;
    }

    public struct Unk18DataSet
    {
        public float Unk0;
        public float Unk1;
        public float Unk2;
        public int Offset;
        public Vector3[] Points;
    }

    public struct UnkSet0
    {
        public float X;
        public float Y;
        public int Offset;

        //This data is after each segment is listed.
        public int cellUnk0;
        public int cellUnk1;
        public int cellUnk2;
        public int cellUnk3;
        public float cellUnk4;
        public float cellUnk5;
        public float cellUnk6;
        public float cellUnk7;
        public int cellUnk8;
        public int cellUnk9;
        public int cellUnk10;
        public int cellUnk11;
        public int cellUnk12;
        public int cellUnk13;
        public int cellUnk14;
        public int cellUnk15;
        public int NumEdges;
        public int cellUnk17;
        public int cellUnk18;
        public int cellUnk19;
        public Unk10DataSet[] unk10Boxes;
        public Unk12DataSet[] unk12Boxes;
        public int[] unk14Offsets;
        public int unk14End;
        public Unk14DataSet[] unk14Boxes;
        public byte[] unk14Data;
        public int[] unk16Offsets;
        public int EdgesDataEndOffset;
        public BoundingBox[] EdgeBoxes;
        public Unk18DataSet[] unk18Set;
        public int unk18End;

    }

    public class KynogonRuntimeMesh
    {
        public class Cell
        {
            public UnkSet0[] Sets;
            public int Offset;

            public override string ToString()
            {
                return string.Format("Offset: {0}, Set Count {1}", Offset, Sets.Length);
            }
        }

        public int Unk0;
        public int Unk1;
        public BoundingBox BoundingBox;
        public int CellSizeX;
        public int CellSizeY;
        public float Radius;
        public int Unk2;
        public int Height;
        public int Offset;
        public int[] Grid;
        public Cell[] Cells;

        public void ReadFromFile(BinaryReader reader, StreamWriter writer)
        {
            //KynogonRuntimeMesh
            long mesh_pos = reader.BaseStream.Position;
            string magicName = new string(reader.ReadChars(18));
            if (magicName != "KynogonRuntimeMesh")
            {
                throw new FormatException("Did not find KynogonRuntimeMesh");
            }

            short mesh_unk0 = reader.ReadInt16();
            int magicVersion = reader.ReadInt32();

            if (magicVersion != 2)
            {
                throw new FormatException("Version did not equal 2");
            }

            Unk0 = reader.ReadInt32();
            Unk1 = reader.ReadInt32();
            BoundingBox = BoundingBoxExtenders.ReadFromFile(reader);
            CellSizeX = reader.ReadInt32();
            CellSizeY = reader.ReadInt32();
            Radius = reader.ReadSingle();
            Unk2 = reader.ReadInt32();
            Height = reader.ReadInt32();
            Offset = reader.ReadInt32(); //this is a potential offset;
            Grid = new int[(CellSizeX * CellSizeY)];
            Cells = new Cell[Grid.Length];

            for (int i = 0; i < Grid.Length; i++)
            {
                Grid[i] = reader.ReadInt32();
            }

            int end = reader.ReadInt32();

            for (int i = 0; i < Cells.Length; i++)
            {
                //if (i + 1 >= Cells.Length)
                //    break;

                Cell cell = new Cell();
                int numSet0 = reader.ReadInt32();
                cell.Sets = new UnkSet0[numSet0];
                Cells[i] = cell;

                writer.WriteLine("-----------------------");
                writer.WriteLine(string.Format("{0} {1}", i, numSet0));
                writer.WriteLine("");

                if (numSet0 == 0)
                    continue;

                //NOTE: EVERY OFFSET IN UNKSET0 BEGINS FROM MESH_POS HIGHER IN THE CODE FILE.
                cell.Offset = reader.ReadInt32();

                for (int x = 0; x < numSet0; x++)
                {
                    UnkSet0 set = new UnkSet0();
                    set.X = reader.ReadSingle();
                    set.Y = reader.ReadSingle();
                    set.Offset = reader.ReadInt32();
                    cell.Sets[x] = set;
                }

                //NOTE: EVERY BLOCK OF DATA SEEMS TO START WITH 96, THIS IS HOWEVER UNCONFIRMED.
                for (int z = 0; z < numSet0; z++)
                {
                    UnkSet0 set = cell.Sets[z];
                    //NOTE: MOST OF THE OFFSETS BEGIN HERE, JUST AFTER THE SETS HAVE BEEN DEFINED ABOVE. 
                    set.cellUnk0 = reader.ReadInt32();

                    if (set.cellUnk0 != Unk1)
                        throw new FormatException();
                    writer.WriteLine("");
                    set.cellUnk1 = reader.ReadInt32();
                    set.cellUnk2 = reader.ReadInt32();
                    set.cellUnk3 = reader.ReadInt32();
                    set.cellUnk4 = reader.ReadSingle();
                    set.cellUnk5 = reader.ReadSingle();
                    set.cellUnk6 = reader.ReadSingle();
                    set.cellUnk7 = reader.ReadSingle();
                    set.cellUnk8 = reader.ReadInt32();
                    set.cellUnk9 = reader.ReadInt32(); //-1?
                    set.cellUnk10 = reader.ReadInt32(); //1;
                    set.cellUnk11 = reader.ReadInt32();
                    set.cellUnk12 = reader.ReadInt32(); //0
                    set.cellUnk13 = reader.ReadInt32(); //-1;
                    set.cellUnk14 = reader.ReadInt32(); //0
                    set.cellUnk15 = reader.ReadInt32(); //-1;
                    set.NumEdges = reader.ReadInt32(); //8;
                    set.cellUnk17 = reader.ReadInt32(); //112;
                    set.cellUnk18 = reader.ReadInt32(); //0;
                    set.cellUnk19 = reader.ReadInt32(); //-1;
                    writer.WriteLine(string.Format("Unk1: {0}", set.cellUnk1));
                    writer.WriteLine(string.Format("Unk2: {0}", set.cellUnk2));
                    writer.WriteLine(string.Format("Unk3: {0}", set.cellUnk3));
                    writer.WriteLine(string.Format("Unk4: {0}", set.cellUnk4));
                    writer.WriteLine(string.Format("Unk5: {0}", set.cellUnk5));
                    writer.WriteLine(string.Format("Unk6: {0}", set.cellUnk6));
                    writer.WriteLine(string.Format("Unk7: {0}", set.cellUnk7));
                    writer.WriteLine(string.Format("Unk8: {0}", set.cellUnk8));
                    writer.WriteLine(string.Format("Unk9: {0}", set.cellUnk9));
                    writer.WriteLine(string.Format("Unk10: {0}", set.cellUnk10));
                    writer.WriteLine(string.Format("Unk11: {0}", set.cellUnk11));
                    writer.WriteLine(string.Format("Unk12: {0}", set.cellUnk12));
                    writer.WriteLine(string.Format("Unk13: {0}", set.cellUnk13));
                    writer.WriteLine(string.Format("Unk14: {0}", set.cellUnk14));
                    writer.WriteLine(string.Format("Unk15: {0}", set.cellUnk15));
                    writer.WriteLine(string.Format("Unk16: {0}", set.NumEdges));
                    writer.WriteLine(string.Format("Unk17: {0}", set.cellUnk17));
                    writer.WriteLine(string.Format("Unk18: {0}", set.cellUnk18));
                    writer.WriteLine(string.Format("Unk19: {0}", set.cellUnk19));
                    writer.WriteLine("");
                    //THIS BIT IS UNKNOWN, UPTO CELLUNK20
                    set.unk10Boxes = new Unk10DataSet[set.cellUnk10];
                    writer.WriteLine("Unk10 Boxes");
                    for (int x = 0; x < set.cellUnk10; x++)
                    {
                        Unk10DataSet unk10Set = new Unk10DataSet();
                        unk10Set.B1 = BoundingBoxExtenders.ReadFromFile(reader);
                        unk10Set.UnkOffset = reader.ReadInt32();
                        unk10Set.Unk20 = reader.ReadInt32();
                        set.unk10Boxes[x] = unk10Set;
                        writer.WriteLine(string.Format("Minimum: {0} ", unk10Set.B1.Minimum.ToString()));
                        writer.WriteLine(string.Format("Maximum: {0} ", unk10Set.B1.Maximum.ToString()));
                        writer.WriteLine(string.Format("UnkOffset: {0} ", unk10Set.UnkOffset));
                        writer.WriteLine(string.Format("Unk20: {0} ", unk10Set.Unk20));
                        writer.WriteLine("");
                    }
                    //END OF CONFUSING BIT.


                    //THIS BIT IS UNKNOWN, BUT IS CELLUNK12
                    set.unk12Boxes = new Unk12DataSet[set.cellUnk12];
                    writer.WriteLine("Unk12 Boxes");
                    for (int x = 0; x < set.cellUnk12; x++)
                    {
                        Unk12DataSet unk12Set = new Unk12DataSet();
                        unk12Set.B1 = BoundingBoxExtenders.ReadFromFile(reader);
                        unk12Set.Unk01 = reader.ReadInt32();
                        unk12Set.Unk02 = reader.ReadInt32();
                        unk12Set.Unk03 = reader.ReadSingle();
                        unk12Set.Unk04 = reader.ReadSingle();
                        unk12Set.Unk05 = reader.ReadSingle();
                        set.unk12Boxes[x] = unk12Set;
                        writer.WriteLine(string.Format("Minimum: {0} ", unk12Set.B1.Minimum.ToString()));
                        writer.WriteLine(string.Format("Maximum: {0} ", unk12Set.B1.Maximum.ToString()));
                        writer.WriteLine(string.Format("Unk01: {0} ", unk12Set.Unk01));
                        writer.WriteLine(string.Format("Unk02: {0} ", unk12Set.Unk02));
                        writer.WriteLine(string.Format("Unk03: {0} ", unk12Set.Unk03));
                        writer.WriteLine(string.Format("Unk04: {0} ", unk12Set.Unk04));
                        writer.WriteLine(string.Format("Unk05: {0} ", unk12Set.Unk05));
                        writer.WriteLine("");
                    }

                    //END OF CONFUSING BIT.

                    //THIS LOOPS THROUGH OFFSETS TO BBOX'S
                    writer.WriteLine("Unk14 Offsets");
                    set.unk14Boxes = new Unk14DataSet[set.cellUnk14];
                    for (int x = 0; x < set.cellUnk14; x++)
                    {
                        var dataSet = new Unk14DataSet();
                        dataSet.Offset = reader.ReadInt32();
                        set.unk14Boxes[x] = dataSet;
                        writer.WriteLine(string.Format("{0} ", dataSet.Offset));
                    }

                    //ALWAYS A 4-BYTE INTEGER WHICH DENOTES THE END OF THE BATCH
                    if (set.cellUnk14 > 0)
                    {
                        set.unk14End = reader.ReadInt32();
                        var currentOffset = set.unk14Boxes[0].Offset;
                        for (int x = 0; x < set.cellUnk14; x++)
                        {
                            var dataSet = set.unk14Boxes[x];
                            var offset = (x + 1 < set.cellUnk14 ? set.unk14Boxes[x + 1].Offset : set.unk14End);
                            var size = offset - currentOffset;
                            currentOffset = offset;
                            var count = size / 12;
                            dataSet.Points = new Vector3[count];
                            writer.WriteLine(string.Format("Unk14: {0} {1}", x, count));
                            for (int f = 0; f < dataSet.Points.Length; f++)
                            {
                                dataSet.Points[f] = Vector3Extenders.ReadFromFile(reader);
                                writer.WriteLine("{0}: {1}", f, dataSet.Points[f]);
                            }
                            set.unk14Boxes[x] = dataSet;
                        }
                    }
                    writer.WriteLine("");

                    //CONTINUE ONTO THE NEXT BATCH
                    set.unk16Offsets = new int[set.NumEdges];
                    writer.WriteLine("Unk16 Offsets");
                    for (int x = 0; x < set.NumEdges; x++)
                    {
                        set.unk16Offsets[x] = reader.ReadInt32();
                        writer.WriteLine(string.Format("{0} ", set.unk16Offsets[x]));
                    }
                    writer.WriteLine("");
                    //ALWAYS A 4-BYTE INTEGER WHICH DENOTES THE END OF THE BATCH
                    if (set.NumEdges > 0)
                        set.EdgesDataEndOffset = reader.ReadInt32();

                    set.EdgeBoxes = new BoundingBox[set.NumEdges];
                    writer.WriteLine("Unk16 Boxes");
                    for (int x = 0; x < set.NumEdges; x++)
                    {
                        set.EdgeBoxes[x] = BoundingBoxExtenders.ReadFromFile(reader);
                        writer.WriteLine(string.Format("{0} ", set.EdgeBoxes[x]));
                    }

                    set.unk18Set = new Unk18DataSet[set.cellUnk18];
                    writer.WriteLine("");
                    if (set.cellUnk18 > 0)
                    {
                        set.unk18End = reader.ReadInt32();                    
                        writer.WriteLine("Unk18 Boxes:");
                        for (int x = 0; x < set.cellUnk18; x++)
                        {
                            //THIS COULD BE AN OFFSET LIST WITH SOMEKIND OF FLOAT/ROTATION DATA
                            Unk18DataSet dataSet = new Unk18DataSet();
                            dataSet.Unk0 = reader.ReadSingle();
                            dataSet.Unk1 = reader.ReadSingle();
                            dataSet.Unk2 = reader.ReadSingle();
                            dataSet.Offset = reader.ReadInt32();
                            writer.WriteLine(string.Format("{0} {1} {2} {3}", dataSet.Unk0, dataSet.Unk1, dataSet.Unk2, dataSet.Offset));
                            set.unk18Set[x] = dataSet;
                        }

                        byte[] unknown_data = reader.ReadBytes(12); //usually padding

                        var currentOffset = set.unk18End;
                        for (int x = 0; x < set.cellUnk18; x++)
                        {
                            var dataSet = set.unk18Set[x];
                            var size = dataSet.Offset - currentOffset;
                            var count = size / 12;
                            dataSet.Points = new Vector3[count];

                            for(int f = 0; f < dataSet.Points.Length; f++)
                            {
                                dataSet.Points[f] = Vector3Extenders.ReadFromFile(reader);
                                writer.WriteLine("{0}: {1}", f, dataSet.Points[f]);
                            }
                            currentOffset = dataSet.Offset;
                            set.unk18Set[x] = dataSet;
                        }

                    }
                    writer.WriteLine("");
                    cell.Sets[z] = set;
                }
                Console.WriteLine("Completed: " + i);
                //byte[] data = reader.ReadBytes(size);
                //File.WriteAllBytes("grid_" + i + ".bin", data);
            }
            writer.Close();

            //File.WriteAllLines("model.obj", data.ToArray());

        }

        public void WriteToFile(BinaryWriter writer)
        {
            StringHelpers.WriteString(writer, "KynogonRuntimeMesh", false);
            writer.Write((ushort)0); //magic is name with two extra 00
            writer.Write(2); //version

            writer.Write(Unk0);
            writer.Write(Unk1);
            BoundingBoxExtenders.WriteToFile(BoundingBox, writer);
            writer.Write(CellSizeX);
            writer.Write(CellSizeY);
            writer.Write(Radius);
            writer.Write(Unk2);
            writer.Write(Height);
            writer.Write(Offset);

            for (int i = 0; i < Grid.Length; i++)
            {
                writer.Write(Grid[i]);
            }

            for (int i = 0; i < Grid.Length; i++)
            {
                Cell cell = Cells[i];
                writer.Write(cell.Sets.Length);

                if(cell.Sets.Length == 0)
                {
                    continue;
                }

                writer.Write(cell.Offset);

                for(int x = 0; x < cell.Sets.Length; x++)
                {
                    var set = cell.Sets[x];
                    writer.Write(set.X);
                    writer.Write(set.Y);
                    writer.Write(set.Offset);
                }

                for(int x = 0; x < cell.Sets.Length; x++)
                {
                    var set = cell.Sets[x];
                    writer.Write(set.cellUnk0);
                    writer.Write(set.cellUnk1);
                    writer.Write(set.cellUnk2);
                    writer.Write(set.cellUnk3);
                    writer.Write(set.cellUnk4);
                    writer.Write(set.cellUnk5);
                    writer.Write(set.cellUnk6);
                    writer.Write(set.cellUnk7);
                    writer.Write(set.cellUnk8);
                    writer.Write(set.cellUnk9);
                    writer.Write(set.cellUnk10);
                    writer.Write(set.cellUnk11);
                    writer.Write(set.cellUnk12);
                    writer.Write(set.cellUnk13);
                    writer.Write(set.cellUnk14);
                    writer.Write(set.cellUnk15);
                    writer.Write(set.NumEdges);
                    writer.Write(set.cellUnk17);
                    writer.Write(set.cellUnk18);
                    writer.Write(set.cellUnk19);

                    foreach(var dataSet in set.unk10Boxes)
                    {
                        BoundingBoxExtenders.WriteToFile(dataSet.B1, writer);
                        writer.Write(dataSet.UnkOffset);
                        writer.Write(dataSet.Unk20);
                    }

                    foreach(var dataSet in set.unk12Boxes)
                    {
                        BoundingBoxExtenders.WriteToFile(dataSet.B1, writer);
                        writer.Write(dataSet.Unk01);
                        writer.Write(dataSet.Unk02);
                        writer.Write(dataSet.Unk03);
                        writer.Write(dataSet.Unk04);
                        writer.Write(dataSet.Unk05);
                    }

                    foreach(var offset in set.unk14Offsets)
                    {
                        writer.Write(offset);
                    }

                    if(set.cellUnk14 > 0)
                    {
                        writer.Write(set.unk14End);
                        foreach(var dataSet in set.unk14Boxes)
                        {
                            foreach(var point in dataSet.Points)
                            {
                                Vector3Extenders.WriteToFile(point, writer);
                            }
                        }
                    }

                    foreach (var offset in set.unk16Offsets)
                    {
                        writer.Write(offset);
                    }

                    if (set.NumEdges > 0)
                    {
                        writer.Write(set.EdgesDataEndOffset);
                        
                        foreach(var dataSet in set.EdgeBoxes)
                        {
                            dataSet.WriteToFile(writer);
                        }
                    }

                    if(set.cellUnk18 > 0)
                    {
                        writer.Write(set.unk18End);

                        foreach(var dataSet in set.unk18Set)
                        {
                            writer.Write(dataSet.Unk0);
                            writer.Write(dataSet.Unk1);
                            writer.Write(dataSet.Unk2);
                            writer.Write(dataSet.Offset);
                        }

                        writer.Write(new byte[12]);

                        foreach(var dataSet in set.unk18Set)
                        {
                            foreach (var point in dataSet.Points)
                            {
                                Vector3Extenders.WriteToFile(point, writer);
                            }
                        }
                    }
                }
            }
        }
    }
}