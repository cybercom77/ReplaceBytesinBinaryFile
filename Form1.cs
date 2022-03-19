using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReplaceBytesinBinaryFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        public static string ConvertHex(String hexString)
        {
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    String hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    uint decval = System.Convert.ToUInt32(hs, 16);
                    char character = System.Convert.ToChar(decval);
                    ascii += character;

                }

                return ascii;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return string.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string hexstringsource= ConvertHex("6D00730067003A002F");
            string hexstringtarget= ConvertHex("6D007300670078002F");
            string temppatchfile = textBox2.Text + ".temp";
            listBox1.Items.Add("Phase 1 - find 'm.s.g.:./'");
            ReplaceTextInBinaryFile(textBox1.Text, temppatchfile, hexstringsource, hexstringtarget);
            listBox1.Items.Add("Phase 1 FINISHED");

            listBox1.Items.Add("Phase 2 - find 'm.s.g.b.n.d.......f.o.n.t.:./'");
            string hexstringsource2 = ConvertHex("6D007300670062006E00640000000000000066006F006E0074003A002F");
            string hexstringtarget2 = ConvertHex("6D007300670062006E00640000000000000066006F006E00740078002F");
            ReplaceTextInBinaryFile(temppatchfile, textBox2.Text, hexstringsource2, hexstringtarget2);
            listBox1.Items.Add("Phase 2 FINISHED");
            File.Delete(temppatchfile);
            button1.Enabled = false;
            button3.Enabled = false;



        }

        private void ReplaceTextInBinaryFile(string originalFile, string outputFile, string searchTerm, string replaceTerm)
        {
            byte b;
            //UpperCase bytes to search
            byte[] searchBytes = Encoding.UTF8.GetBytes(searchTerm.ToUpper());
            //LowerCase bytes to search
            byte[] searchBytesLower = Encoding.UTF8.GetBytes(searchTerm.ToLower());
            //Temporary bytes during found loop
            byte[] bytesToAdd = new byte[searchBytes.Length];
            //Search length
            int searchBytesLength = searchBytes.Length;
            //First Upper char
            byte searchByte0 = searchBytes[0];
            //First Lower char
            byte searchByte0Lower = searchBytesLower[0];
            //Replace with bytes
            byte[] replaceBytes = Encoding.UTF8.GetBytes(replaceTerm);
            int counter = 0;
            using (FileStream inputStream = File.OpenRead(originalFile))
            {
                //input length
                long srcLength = inputStream.Length;
                using (BinaryReader inputReader = new BinaryReader(inputStream))
                {
                    using (FileStream outputStream = File.OpenWrite(outputFile))
                    {
                        using (BinaryWriter outputWriter = new BinaryWriter(outputStream))
                        {
                            for (int nSrc = 0; nSrc < srcLength; ++nSrc)
                                //first byte
                                if ((b = inputReader.ReadByte()) == searchByte0
                                    || b == searchByte0Lower)
                                {
                                    
                                    bytesToAdd[0] = b;
                                    int nSearch = 1;
                                    //next bytes
                                    for (; nSearch < searchBytesLength; ++nSearch)
                                        //get byte, save it and test
                                        if ((b = bytesToAdd[nSearch] = inputReader.ReadByte()) != searchBytes[nSearch]
                                            && b != searchBytesLower[nSearch])
                                        {
                                            break;//fail
                                        }
                                    //Avoid overflow. No need, in my case, because no chance to see searchTerm at the end.
                                    //else if (nSrc + nSearch >= srcLength)
                                    //    break;
                                    
                                    if (nSearch == searchBytesLength)
                                    {
                                        //success
                                        ++counter;
                                        outputWriter.Write(replaceBytes);
                                        nSrc += nSearch - 1;
                                    
                                        
                                    }
                                    else
                                    {
                                        //failed, add saved bytes
                                        outputWriter.Write(bytesToAdd, 0, nSearch + 1);
                                        nSrc += nSearch;
                                    }
                                }
                                else
                                    outputWriter.Write(b);
                        }
                    }
                }
            }
            //Console.WriteLine("ReplaceTextInBinaryFile.counter = " + counter);
            listBox1.Items.Add("Number of matches found: " + counter);
            //MessageBox.Show("FINISHED");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            
            textBox1.Text = openFileDialog1.FileName;
            textBox2.Text = openFileDialog1.FileName+".patched";



        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

            textBox2.Text = openFileDialog1.FileName;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
        
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
