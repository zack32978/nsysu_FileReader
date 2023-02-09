using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;
namespace FileReader
{
    public partial class Huffman : Form
    {
        Bitmap image;
        public int[] dataGray = new int[256];
        public char[] arrayhuffman;
       
        public Huffman(Bitmap Image)
        {
            InitializeComponent();
            image = Image;
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            char[] arrayhuffman = new char[image.Width * image.Height];
            pictureBox1.Image = image;
            int index = 0;
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    dataGray[image.GetPixel(x, y).R]++;
                    arrayhuffman[index] = (char)image.GetPixel(x, y).R;
                    index++;
                }
            }
            #region 構建霍夫曼樹
            HuffmanTree huffmanTree = new HuffmanTree();
            huffmanTree.Build(arrayhuffman);
            #endregion

            #region 算好的東西輸出到datagriviews中
            Dictionary<char, int> symbols = huffmanTree.Frequencies;
            string new_input = "";
            foreach (char symbol in arrayhuffman)
            {
                if (!new_input.Contains(symbol))
                    new_input += symbol.ToString();
            }
            //foreach (char symbol in new_input)
            foreach (KeyValuePair<char, int> pair in symbols)
            {
                //dataGridView1
                DataGridViewRowCollection rows = dataGridView1.Rows;
                BitArray bits = huffmanTree.GetSymbolCode(pair.Key);
                string s = ToBitString(bits);
                rows.Add(new Object[] { (int)pair.Key, s, pair.Value });
            }
            //照次數排序
            dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
            #endregion  
        }
        public string ToBitString(BitArray bits)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < bits.Count; i++)
            {
                char c = bits[i] ? '1' : '0';
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
    class HuffmanTree
    {
        private List<Node> nodes = new List<Node>();
        public Node Root { get; set; }
        public Dictionary<char, int> Frequencies = new Dictionary<char, int>();

        public void Build(char[] source)
        {
            //算每個值出現次數
            for (int i = 0; i < source.Length; i++)
            {
                if (!Frequencies.ContainsKey(source[i]))
                {
                    Frequencies.Add(source[i], 0);
                }
                Frequencies[source[i]]++;
            }
            //登記節點
            foreach (KeyValuePair<char, int> symbol in Frequencies)
            {
                nodes.Add(new Node() { Symbol = symbol.Key, Frequency = symbol.Value });
            }
            
            while (nodes.Count > 1)
            {   //照頻率排結點
                List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList<Node>();
                //每次取最小的2個點組成一節點再加入list，建構樹的架構
                if (orderedNodes.Count >= 2)
                {
                    // Take first two items
                    List<Node> taken = orderedNodes.Take(2).ToList<Node>();

                    // Create a parent node by combining the frequencies
                    Node parent = new Node()
                    {
                        Symbol = '*',
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };
                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }
                this.Root = nodes.FirstOrDefault();
            }
        }

        public BitArray GetSymbolCode(char symbol)
        {
            List<bool> encodedSource = new List<bool>();
            List<bool> encodedSymbol = this.Root.Traverse(symbol, new List<bool>());
            encodedSource.AddRange(encodedSymbol);
            BitArray bits = new BitArray(encodedSource.ToArray());
            return bits;
        }

    }
    class Node
    {
        public char Symbol { get; set; }
        public int Frequency { get; set; }
        public List<bool> Code { get; set; }
        public Node Right { get; set; }
        public Node Left { get; set; }
        //從起點遍歷左右邊
        public List<bool> Traverse(char symbol, List<bool> data)
        {
            // Leaf
            if (Right == null && Left == null)
            {
                if (symbol.Equals(this.Symbol))
                {
                    Code = data;
                    return data;
                }
                else
                {return null;}
            }
            else
            {
                List<bool> left = null;
                List<bool> right = null;

                if (Left != null)
                {
                    List<bool> leftPath = new List<bool>();
                    leftPath.AddRange(data);
                    leftPath.Add(false);
                    left = Left.Traverse(symbol, leftPath);
                }

                if (Right != null)
                {
                    List<bool> rightPath = new List<bool>();
                    rightPath.AddRange(data);
                    rightPath.Add(true);
                    right = Right.Traverse(symbol, rightPath);
                }
                if (left != null)
                {return left;}
                else
                {return right;}
            }
        }
    }


}
