/**************license begin*******************

Copyright (c) 2012 Yipeng Wang

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

***************license end*********************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        string dir_path="..\\";
        string[] filter_array;
        public Form1()
        {
            InitializeComponent();
            filter();
            show_file_tree();
        }

        /*button to add license*/
        private void button1_Click(object sender, EventArgs e)
        {
            CallNodesSelector();
            MessageBox.Show("Complete!");
            
        }
        //get the node that checked
        private void CallNodesSelector()
        {
            TreeNodeCollection nodes = this.treeView1.Nodes;
            foreach (TreeNode n in nodes)
            {
                GetNodeRecursive(n);
            }
        }

        private void GetNodeRecursive(TreeNode treeNode)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                GetNodeRecursive(tn);
            }
            if (treeNode.Checked == true)
            {
                find_and_replace(treeNode.ToolTipText, textBox2.Text, textBox3.Text, richTextBox1.Text);
            }
        }


        //refresh button
        private void button2_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            filter();
            show_file_tree();
        }


        //file tree code below is similar from MSDN walkthrough tutorial of treeview file explorer
        private void show_file_tree()
        {

                TreeNode rootN;
                DirectoryInfo info = new DirectoryInfo(@dir_path);

                if (info.Exists)
                {
                    rootN = new TreeNode(info.Name);
                    rootN.Tag = info;
                    rootN.ToolTipText = info.FullName;
                    GetDirectories(info.GetFiles(), info.GetDirectories(), rootN);
                    treeView1.Nodes.Add(rootN);

                }
        
        }

        private void GetDirectories(FileInfo[] subfiles,  DirectoryInfo[] subDirs, TreeNode node2add2)
        {

            TreeNode aNode;
            TreeNode filenode;
            DirectoryInfo[] subSubDirs=null;
            FileInfo[] subSubfiles = null;
            if (subDirs.Length != 0)
            {
                foreach (DirectoryInfo subDir in subDirs)
                {
                    aNode = new TreeNode(subDir.Name, 0, 0);
                    aNode.Tag = subDir;
                    aNode.ToolTipText = subDir.FullName;
                    aNode.ImageKey = "folder";
                    try
                    {
                        subSubDirs = subDir.GetDirectories();
                        subSubfiles = subDir.GetFiles();
                    }
                    catch { }

                    try
                    {

                        GetDirectories(subSubfiles, subSubDirs, aNode);

                    }
                    catch { }

                    node2add2.Nodes.Add(aNode);
                }
            }
            try
            {
                foreach (FileInfo file in subfiles)
                {
                    filenode = new TreeNode(file.Name, 0, 0);
                    foreach (string a in filter_array)
                    {
                        if (file.Extension == a)
                        {
                            filenode.ToolTipText = file.FullName;
                            node2add2.Nodes.Add(filenode);
                        }
                    }
                }
            }
            catch { }
        }

       /*create filter to filter files with certain extensions*/ 
        private void filter()
        {

          filter_array = Regex.Split(textBox1.Text, "\r\n");
        }

       
        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        /*check all child*/
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(e.Node, e.Node.Checked);
                }
            }

        }

        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        /*find start and end, replace the license between them*/
        private void find_and_replace(string file_path, string start, string end, string body )
        {
            if (File.Exists(file_path) != true) return;           //check if it is a file
            try
            {

                StreamReader rfile = new StreamReader(file_path);
                StreamWriter wfile = new StreamWriter("./CopyRight_adder_tmpfile");
                string line;
                bool find_start=false;
                bool find_end=false;


                while ((line = rfile.ReadLine()) != null)
                {
                    if (line == start)
                    {
                        find_start = true;
                        wfile.WriteLine(start);
                        while ((line = rfile.ReadLine()) != null)
                        {
                            if (line == end)
                            {
                                find_end = true;
                                break;
                            }
                        }

                        wfile.Write("\n"+body+"\n");
                    }
                    if(line!=null)
                        wfile.WriteLine(line);
                }
                rfile.Close();
                wfile.Close();

                if (find_start && find_end)
                {
                    File.Delete(file_path);
                    File.Move("./CopyRight_adder_tmpfile", file_path);

                }
                else if (find_start == false && find_end == false)
                {
                    StreamReader rfile1 = new StreamReader(file_path);
                    StreamWriter wfile1 = new StreamWriter("./CopyRight_adder_tmpfile");

                    wfile1.WriteLine(start);
                    wfile1.WriteLine("\n"+body+"\n");
                    wfile1.WriteLine(end);
                    wfile1.Write(rfile1.ReadToEnd());
                    rfile1.Close();
                    wfile1.Close();
                    File.Delete(file_path);
                    File.Move("./CopyRight_adder_tmpfile", file_path);
                }
                else
                {
                    MessageBox.Show(file_path + " format problem!");
                }

            }

            catch { }



        }

        private void button3_Click(object sender, EventArgs e)
        {
            folderBD.ShowDialog();
            dir_path=folderBD.SelectedPath;
            treeView1.Nodes.Clear();
            filter();
            show_file_tree();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
