using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace EagleEye
{
    public partial class EagleEyeForm : Form
    {
        string Url, sqlmapUrl1;
        string nmapCmdArguments, sqlmapCmdArguments;
        string sqlmapCmd;
        string currentPathTravFilename;

        List<string> scrapedUrls = new List<string>();
        List<string> UrlsWithParameters = new List<string>();

        public EagleEyeForm()
        {
            InitializeComponent();
            this.Text = "EagleEye";

        }

        private void EagleEyeForm_Load(object sender, EventArgs e)
        {
            portsComboBox.Enabled = false;
            scanTypeComboBox.Enabled = false;

            scanTypeComboBox.Items.Add("Intense Scan");
            scanTypeComboBox.Items.Add("Intense Scan (w/ UDP)");
            scanTypeComboBox.Items.Add("Intense Scan (All TCP ports)");
            scanTypeComboBox.Items.Add("Intense Scan (No Ping)");
            scanTypeComboBox.Items.Add("Ping Scan");
            scanTypeComboBox.Items.Add("Quick Scan");
            scanTypeComboBox.Items.Add("Quick Scan+");
            scanTypeComboBox.Items.Add("Traceroute");
            scanTypeComboBox.Items.Add("Regular Scan");
            scanTypeComboBox.Items.Add("Comprehensive Scan (SLOW!)");

            portsComboBox.Items.Add("Common Ports");
            portsComboBox.Items.Add("1-1024");
            portsComboBox.Items.Add("1-100");
            portsComboBox.Items.Add("1-80");
        }

        private void spiderButton_Click(object sender, EventArgs e)
        {
            Spider s = new Spider();
            HTTP http = new HTTP();

            scrapedUrls = s.spiderUrl(Url);

            if (scrapedUrls == null)
            {
                MessageBox.Show("Please set a URL in the Dashboard module.");
                return;
            }

            if (spiderDataGrid.Rows.Count > 0)
            {
                spiderDataGrid.Rows.Clear();
            }

            foreach (string url in scrapedUrls)
            {
                if (url.Contains("upload"))
                {
                    string warn = "Found an upload page: " + url;
                    spiderAlertBox.Items.Add(warn);
                    allAlertsListBox.Items.Add(warn);
                }

                if (url.Contains("login"))
                {
                    string warn = "Found an login page: " + url;
                    spiderAlertBox.Items.Add(warn);
                    allAlertsListBox.Items.Add(warn);
                }

                if (url.Contains("phpMyAdmin"))
                {
                    string warn = "Found phpMyAdmin panel: " + url;
                    spiderAlertBox.Items.Add(warn);
                    allAlertsListBox.Items.Add(warn);
                }

                spiderDataGrid.Rows.Add(url, url.Split("/")[3], http.GetHttpStatusCode(url));
            }
        }

        private void setUrlButton_Click(object sender, EventArgs e)
        {
            string strUrl;
            HTTP http = new HTTP();

            strUrl = urlTextBox.Text.ToString();

            if (strUrl == "")
            {
                MessageBox.Show("Please enter a URL.");
                return;
            }

            if (http.ValidateURL(strUrl) == false)
            {
                MessageBox.Show("Please enter a valid URL.");
                return;
            }

            else
            {
                Url = strUrl;
                Uri uri = new Uri(Url);
                var ip = Dns.GetHostAddresses(uri.Host)[0];
                scopeComboBox.Items.Add(ip);
                scanTypeComboBox.Enabled = true;
                portsComboBox.Enabled = true;
            }
        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void scopeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void portsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (portsComboBox.SelectedItem.ToString() == "Common Ports") { nmapCommandTextBox.Text += " -p21,22,25,53,80,110,123,143,443,465,631,993,995 ";  }
            if (portsComboBox.SelectedItem.ToString() == "1-1024") { nmapCommandTextBox.Text += " -p1-1024 "; }
            if (portsComboBox.SelectedItem.ToString() == "1-100") { nmapCommandTextBox.Text += " -p1-100 "; }
            if (portsComboBox.SelectedItem.ToString() == "1-80") { nmapCommandTextBox.Text += " -p1-80 "; }
        }

        private void nmapOutputListBox_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void WordlistLabel_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void scanButton_Click(object sender, EventArgs e)
        {
            if (nmapCmdArguments == null)
            {
                MessageBox.Show("Please select some options.");
                return;
            }
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "nmap";
            p.StartInfo.Arguments = nmapCmdArguments;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            nmapTextBox.Text = output;
            p.WaitForExit();
        }

        private void scanTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (scopeComboBox.SelectedItem.ToString() == null)
            {
                MessageBox.Show("Please provide scope.");
                return;
            }
            string ip = scopeComboBox.SelectedItem.ToString();


            if (scanTypeComboBox.SelectedItem.ToString() == "Intense Scan") { /*portsComboBox.Enabled = false;*/ nmapCmdArguments = "-T4 -A -v " + ip; nmapCommandTextBox.Text = "nmap" + " " + nmapCmdArguments; }
            if (scanTypeComboBox.SelectedItem.ToString() == "Intense Scan(w/ UDP)") { /*portsComboBox.Enabled = false;*/ nmapCmdArguments = "-sS -sU -T4 -A -v " + ip; nmapCommandTextBox.Text = "nmap" + " " + nmapCmdArguments; }
            if (scanTypeComboBox.SelectedItem.ToString() == "Intense Scan (All TCP ports)") { /*portsComboBox.Enabled = false;*/ nmapCmdArguments = "-p 1-65535 -T4 -A -v " + ip; nmapCommandTextBox.Text = "nmap" + " " + nmapCmdArguments; }
            if (scanTypeComboBox.SelectedItem.ToString() == "Intense Scan (No Ping)") { /*portsComboBox.Enabled = false;*/ nmapCmdArguments = "-T4 -A -v -Pn " + ip; nmapCommandTextBox.Text = "nmap" + " " + nmapCmdArguments; }
            if (scanTypeComboBox.SelectedItem.ToString() == "Ping Scan") { /*portsComboBox.Enabled = false;*/ nmapCmdArguments = "-sn " + ip; nmapCommandTextBox.Text = "nmap" + " " + nmapCmdArguments; }
            if (scanTypeComboBox.SelectedItem.ToString() == "Quick Scan") { /*portsComboBox.Enabled = false;*/ nmapCmdArguments = "-T4 -F " + ip; nmapCommandTextBox.Text = "nmap" + " " + nmapCmdArguments; }
            if (scanTypeComboBox.SelectedItem.ToString() == "Quick Scan+") { /*portsComboBox.Enabled = false;*/ nmapCmdArguments = "-sV -T4 -O -F --version-light " + ip; nmapCommandTextBox.Text = "nmap" + " " + nmapCmdArguments; }
            if (scanTypeComboBox.SelectedItem.ToString() == "Traceroute") { /*portsComboBox.Enabled = false;*/ nmapCmdArguments = "-sn --traceroute " + ip; nmapCommandTextBox.Text = "nmap" + " " + nmapCmdArguments; }
            if (scanTypeComboBox.SelectedItem.ToString() == "Regular Scan") { /*portsComboBox.Enabled = false;*/ nmapCmdArguments =  ip; nmapCommandTextBox.Text = "nmap" + " " + nmapCmdArguments; }
            if (scanTypeComboBox.SelectedItem.ToString() == "Comprehensive Scan (SLOW!)") { /*portsComboBox.Enabled = false;*/ nmapCmdArguments = "-sS -sU -T4 -A -v -PE -PP -PS80,443 -PA3389 -PU40125 -PY -g 53 --script \"default or (discovery and safe)\" " + ip; nmapCommandTextBox.Text = "nmap" + " " + nmapCmdArguments; }

        }

        private void SQLMapPage_Click(object sender, EventArgs e)
        {

        }

        private void NmapPage_Click(object sender, EventArgs e)
        {

        }

        private void nmapCommandTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void sqlmapButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(sqlmapCmdArguments);
            if (sqlmapCmdArguments == null)
            {
                MessageBox.Show("Please select some options.");
                return;
            }
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.FileName = "\"C:\\Users\\Max Gonzales\\AppData\\Local\\Programs\\Python\\Python311\\python.exe\"";
            p.StartInfo.Arguments = "\"C:\\Users\\Max Gonzales\\Downloads\\sqlmapproject-sqlmap-1.6.12-0-g33a6547\\sqlmapproject-sqlmap-33a6547\\sqlmap.py\"" + " " + sqlmapCmdArguments;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            sqlmapOutputTextBox.Text = output;
        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void sqlmapUrlTextBox_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void groupBox10_Enter_2(object sender, EventArgs e)
        {

        }

        private void comboBox15_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (predefinedWordlistComboBox.SelectedItem.ToString() == "Small") { currentPathTravFilename = "C:\\Users\\Max Gonzales\\Documents\\lfi-small.txt"; }
            if (predefinedWordlistComboBox.SelectedItem.ToString() == "Comprehensive") { currentPathTravFilename = "C:\\Users\\Max Gonzales\\Documents\\lfi-huge.txt"; }
        }

        private void pathTravButton_Click(object sender, EventArgs e)
        {
            HTTP http = new HTTP();
            NameValueCollection parameterCollection;
            string pathTravURL;

            if (pathTravURLTextBox.Text == "")
            {
                MessageBox.Show("Please provide a URL. This module does not use the global URL.");
                return;
            }

            if (http.ValidateURL(pathTravURLTextBox.Text) == false)
            {
                MessageBox.Show("Please provide a valid URL.");
            }    

            pathTravURL = pathTravURLTextBox.Text;

            if (predefinedWordlistComboBox.SelectedIndex == -1 && currentPathTravFilename == null)
            {
                MessageBox.Show("Please select or upload a file.");
                return;
            }

            else
            {
                try
                {
                    parameterCollection = http.GetUrlParameters(pathTravURL);

                    foreach (string line in System.IO.File.ReadLines(currentPathTravFilename))
                    {
                        if (parameterCollection == null)
                        {
                            return;
                        }
                        
                        // change parameters
                        foreach (string key in parameterCollection.AllKeys)
                        {
                            parameterCollection[key] = line;
                        }
                        string tempUrl = http.GetAbsoluteUrlStringNoParam(pathTravURL, pathTravURL);
                        var builder = new UriBuilder();
                        builder.Query = string.Join("&", parameterCollection.AllKeys.Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(parameterCollection[key]))));
                        string baseUrl = tempUrl;
                        tempUrl += builder.Query;

                        if (http.getWebpage(tempUrl).Contains("root:x:0:0:root"))
                        {
                            var warn = "Contents spilled for /etc/passwd: " + tempUrl;
                            allAlertsListBox.Items.Add(warn);
                            summaryListBox.Items.Add(warn);
                            PathTravWarnList.Items.Add(warn);
                        }

                        if (http.getWebpage(tempUrl).Contains("The MySQL"))
                        {
                            var warn = "Contents spilled for MySQL configuration: " + tempUrl;
                            allAlertsListBox.Items.Add(warn);
                            summaryListBox.Items.Add(warn);
                            PathTravWarnList.Items.Add(warn);
                        }

                        pathTravDataGridView.Rows.Add(baseUrl, builder.Query, http.GetHttpStatusCode(tempUrl));
                    }
                }
                
                catch
                {
                    MessageBox.Show("File is empty.");
                }
            }
        }
        private void filePickerButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentPathTravFilename = openFileDialog1.FileName;
                PathTravWarnList.Items.Add("Using \"" + openFileDialog1.FileName + "\"");
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void spiderPage_Click(object sender, EventArgs e)
        {

        }

        private void pathTravURLTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void databaseTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableTextBox_TextChanged(object sender, EventArgs e)
        {


        }

        private void columnTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void urlTextBox_TextChanged_1(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (sqlmapUrl.Text == String.Empty)
            {
                MessageBox.Show("Please enter a valid URL");
            }

            if (wafDetectionCheckBox.Checked)
            {
                sqlmapCmdArguments += "--identify-waf ";
                sqlmapCommandTextBox.AppendText("--identify-waf ");
            }

            if (dumpAllCheckBox.Checked)
            {
                sqlmapCmdArguments += "--dump-all ";
                sqlmapCommandTextBox.AppendText("--dump-all ");
            }

            if (TorCheckBox.Checked)
            {
                sqlmapCmdArguments += "--tor ";
                sqlmapCommandTextBox.AppendText("--tor ");
            }

            if (randomAgentCheckBox.Checked)
            {
                sqlmapCmdArguments += "--random-agent ";
                sqlmapCommandTextBox.AppendText("--random-agent ");
            }

            if (forceSSLCheckBox.Checked)
            {
                sqlmapCmdArguments += "--force-ssl ";
                sqlmapCommandTextBox.AppendText("--force-ssl ");
            }

            if (databaseTextBox.Text != String.Empty)
            {
                sqlmapCmdArguments += "-D " + databaseTextBox.Text + " ";
                sqlmapCommandTextBox.AppendText ("-D " + databaseTextBox.Text + " ");
            }

            if (tableTextBox.Text != String.Empty)
            {
                sqlmapCmdArguments += "-T " + tableTextBox.Text + " ";
                sqlmapCommandTextBox.AppendText("-T " + tableTextBox.Text + " ");
            }

            if (columnTextBox.Text != String.Empty)
            {
                sqlmapCmdArguments += "-C " + columnTextBox.Text + " ";
                sqlmapCommandTextBox.AppendText("-C " + columnTextBox.Text + " ");
            }

            if (cookieTextBox.Text != String.Empty)
            {
                sqlmapCmdArguments += "--cookie=" + "\"" + cookieTextBox.Text + "\"" + " ";
                sqlmapCommandTextBox.AppendText("--cookie=" + "\"" + cookieTextBox.Text + "\"" + " ");
            }
        }

        private void sqlmapUrl_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void setSqlmapURL_Click(object sender, EventArgs e)
        {
            if (sqlmapUrl.Text != String.Empty)
            {
                sqlmapUrl1 = sqlmapUrl.Text;
                sqlmapCmdArguments = "-u " + "\"" + sqlmapUrl1 + "\"" + " --batch ";
                sqlmapCommandTextBox.Text = "sqlmap " + sqlmapCmdArguments;
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }
    }
}
