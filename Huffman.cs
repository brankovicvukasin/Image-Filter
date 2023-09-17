using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Projekat16011
{
    public class HuffmanNode : IComparable<HuffmanNode>
    {
        public byte Value;
        public int Frequency;
        public HuffmanNode Left;
        public HuffmanNode Right;

        public int CompareTo(HuffmanNode other)
        {
            return Frequency.CompareTo(other.Frequency);
        }
    }
    public static class HuffmanEncoder
    {
        public static void CompressBitmap(Bitmap bitmap, string outputPath)
        {
            byte[] bitmapBytes = BitmapToByteArray(bitmap);
            Dictionary<byte, int> frequencyTable = BuildFrequencyTable(bitmapBytes);
            HuffmanNode huffmanTree = BuildHuffmanTree(frequencyTable);
            Dictionary<byte, string> huffmanCodes = GenerateHuffmanCodes(huffmanTree);
            byte[] compressedData = Encode(bitmapBytes, huffmanCodes);

            using (FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                WriteHuffmanTree(fs, huffmanTree);
                fs.Write(compressedData, 0, compressedData.Length);
            }
        }

        private static void WriteHuffmanTree(FileStream fs, HuffmanNode node)
        {
            if (node == null)
            {
                return;
            }

            if (node.Left == null && node.Right == null)
            {
                fs.WriteByte((byte)1);
                fs.WriteByte(node.Value);
            }
            else
            {
                fs.WriteByte((byte)0);
                WriteHuffmanTree(fs, node.Left);
                WriteHuffmanTree(fs, node.Right);
            }
        }

        private static byte[] Encode(byte[] data, Dictionary<byte, string> huffmanCodes)
        {
            StringBuilder encodedDataBuilder = new StringBuilder();
            foreach (byte b in data)
            {
                encodedDataBuilder.Append(huffmanCodes[b]);
            }

            string encodedDataStr = encodedDataBuilder.ToString();
            int paddedLength = (8 - (encodedDataStr.Length % 8)) % 8;
            encodedDataStr = encodedDataStr.PadRight(encodedDataStr.Length + paddedLength, '0');

            List<byte> encodedData = new List<byte>();
            for (int i = 0; i < encodedDataStr.Length; i += 8)
            {
                byte encodedByte = Convert.ToByte(encodedDataStr.Substring(i, 8), 2);
                encodedData.Add(encodedByte);
            }

            return encodedData.ToArray();
        }

        private static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        private static Dictionary<byte, int> BuildFrequencyTable(byte[] data)
        {
            return data.GroupBy(b => b).ToDictionary(g => g.Key, g => g.Count());
        }

        private static HuffmanNode BuildHuffmanTree(Dictionary<byte, int> frequencyTable)
        {
            List<HuffmanNode> nodes = frequencyTable.Select(kvp => new HuffmanNode { Value = kvp.Key, Frequency = kvp.Value }).ToList();

            while (nodes.Count > 1)
            {
                nodes.Sort();
                HuffmanNode left = nodes[0];
                HuffmanNode right = nodes[1];
                nodes.RemoveRange(0, 2);

                HuffmanNode newNode = new HuffmanNode { Frequency = left.Frequency + right.Frequency, Left = left, Right = right };
                nodes.Add(newNode);
            }

            return nodes[0];
        }

        private static Dictionary<byte, string> GenerateHuffmanCodes(HuffmanNode rootNode)
        {
            Dictionary<byte, string> huffmanCodes = new Dictionary<byte, string>();
            GenerateHuffmanCodesRecursive(rootNode, string.Empty, huffmanCodes);
            return huffmanCodes;
        }

        private static void GenerateHuffmanCodesRecursive(HuffmanNode node, string code, Dictionary<byte, string> huffmanCodes)
        {
            if (node == null)
            {
                return;
            }

            
            if (node.Left == null && node.Right == null)
            {
                huffmanCodes.Add(node.Value, code);
            }
            else
            {
               
                GenerateHuffmanCodesRecursive(node.Left, code + "0", huffmanCodes);

  
                GenerateHuffmanCodesRecursive(node.Right, code + "1", huffmanCodes);
            }
        }


 }  }
