﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using DS4Control;
using System.Xml;

namespace ScpServer
{
    public partial class WinProgs : Form
    {
        ToolTip tp = new ToolTip();
        ComboBox[] cbs;
        public WinProgs(string[] oc)
        {
            InitializeComponent();
            cbs = new ComboBox[4] { comboBox1, comboBox2, comboBox3, comboBox4 };
            for (int i = 0; i < 4; i++)
            {
                cbs[i].Text = "(none)";
                cbs[i].Items.AddRange(oc);
            }
            try
            {
                string[] lods = Directory.GetDirectories(@"C:\Program Files (x86)\Steam\steamapps\common");
                foreach (string s in lods)
                    listBox1.Items.Add(Path.GetFileName(s));
            }
            catch { }
            if (!File.Exists(Global.appdatapath + @"\Auto Profiles.xml"))
                Create();
            //foreach (ListBox.ObjectCollection s in listBox1.Items)
              //  tp.SetToolTip((Control)s, @"C:\Program Files (x86)\Steam\SteamApps\common" + s.ToString());
        }

        
        private void button1_Click(object sender, EventArgs e)
        {

        }

        List<string> lods = new List<string>();
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = listBox1.SelectedItem.ToString();
            lods.Clear();
            lods.AddRange(Directory.GetFiles(@"C:\Program Files (x86)\Steam\SteamApps\common\" + s, "*.exe", SearchOption.AllDirectories));
            for (int i = lods.Count-1; i >= 0; i--)
            if (lods[i].Contains("etup") || lods[i].Contains("dotnet") || lods[i].Contains("SETUP") 
                || lods[i].Contains("edist") || lods[i].Contains("nstall"))
                lods.RemoveAt(i);

            listBox2.Items.Clear();
            foreach (string st in lods)
                //if (!st.Contains("etup") && !st.Contains("dotnet") && !st.Contains("SETUP") && !st.Contains("edist") && !st.Contains("nstall"))
                    listBox2.Items.Add(Path.GetFileNameWithoutExtension(st));
            if (listBox2.Items.Count == 1 && listBox2.SelectedIndex != 0)
                listBox2.SelectedIndex = 0;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
           if (listBox2.SelectedIndex >= 0)
            {
                label1.Text = lods[listBox2.SelectedIndex];
                LoadP();
            }
            else
                label1.Text = "";
        }

        protected String m_Profile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DS4Tool\\Auto Profiles.xml";
        protected XmlDocument m_Xdoc = new XmlDocument();

        public bool Create()
        {
            Boolean Saved = true;

            try
            {
                XmlNode Node;

                Node = m_Xdoc.CreateXmlDeclaration("1.0", "utf-8", String.Empty);
                m_Xdoc.AppendChild(Node);

                Node = m_Xdoc.CreateComment(String.Format(" Auto-Profile Configuration Data. {0} ", DateTime.Now));
                m_Xdoc.AppendChild(Node);

                Node = m_Xdoc.CreateWhitespace("\r\n");
                m_Xdoc.AppendChild(Node);

                Node = m_Xdoc.CreateNode(XmlNodeType.Element, "Programs", null);
                m_Xdoc.AppendChild(Node);
                m_Xdoc.Save(m_Profile);
            }
            catch { Saved = false; }

            return Saved;
        }
        public bool Save()
        {
            Boolean Saved = true;
            m_Xdoc.Load(m_Profile);
            //try
            {
                XmlNode Node;
                Node = m_Xdoc.SelectSingleNode("Programs");       

                string programname = listBox2.SelectedItem.ToString();
                //if (programname.Contains(" "))
                programname = programname.Replace(' ', '_');
                XmlNode xmlprogram = m_Xdoc.CreateNode(XmlNodeType.Element, programname, null);

                XmlNode xmlController1 = m_Xdoc.CreateNode(XmlNodeType.Element, "Controller1", null);
                xmlController1.InnerText = comboBox1.Text;
                xmlprogram.AppendChild(xmlController1);
                XmlNode xmlController2 = m_Xdoc.CreateNode(XmlNodeType.Element, "Controller2", null);
                xmlController2.InnerText = comboBox2.Text;
                xmlprogram.AppendChild(xmlController2);
                XmlNode xmlController3 = m_Xdoc.CreateNode(XmlNodeType.Element, "Controller3", null);
                xmlController3.InnerText = comboBox3.Text;
                xmlprogram.AppendChild(xmlController3);
                XmlNode xmlController4 = m_Xdoc.CreateNode(XmlNodeType.Element, "Controller4", null);
                xmlController4.InnerText = comboBox4.Text;
                xmlprogram.AppendChild(xmlController4);

                try 
                {
                    XmlNode oldxmlprocess = m_Xdoc.SelectSingleNode("/Programs/" + listBox2.SelectedItem.ToString());
                    Node.ReplaceChild(xmlprogram, oldxmlprocess); 
                }
                catch { Node.AppendChild(xmlprogram); }
                //Node.AppendChild(oldxmlprocess);
                m_Xdoc.AppendChild(Node);
                m_Xdoc.Save(m_Profile);
            }
            //catch { Saved = false; }

            return Saved;
        }

        public void LoadP()
        {
            //try
            {
                if (File.Exists(m_Profile))
                {
                    XmlNode Item;
                    m_Xdoc.Load(m_Profile);
                    string programname = listBox2.SelectedItem.ToString();
                    programname = programname.Replace(' ', '_');
                    string[] profiles = new string[4];
                    for (int i = 0; i < 4; i++)
                    {
                        try
                        {
                            Item = m_Xdoc.SelectSingleNode("/Programs/" + programname + "/Controller" + (i + 1));
                            profiles[i] = Item.InnerText;
                            for (int j = 0; j < cbs[i].Items.Count; j++)
                                if (cbs[i].Items[j].ToString() == profiles[i])
                                {
                                    cbs[i].SelectedIndex = j;
                                    break;
                                }
                            //else
                            //cbs[i].Text = "(none)";
                        }
                        catch { cbs[i].Text = "(none)"; } 
                    }
                }
            }
            //catch { missingSetting = true; }
            //return missingSetting;
            //if (missingSetting)
                //label1.Content = "Current version not found, please re-run DS4Tool";
        }

        private void CBProfile_IndexChanged(object sender, EventArgs e)
        {

        }

        private void bnSave_Click(object sender, EventArgs e)
        {
            Save();
        }
    }
}