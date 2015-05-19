using Neo4jClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neo4jClient.Cypher;
using ProjectII.Domain;
using ProjectII.Service;

namespace ProjectII
{
    public partial class WebCrawler : Form
    {
        private GraphClient client;
        public WebCrawler(GraphClient client)
        {
            InitializeComponent();
            this.client = client;
            comboBox1.Items.Add("10");
            comboBox1.Items.Add("100");
            comboBox1.Items.Add("1000");
            comboBox1.Items.Add("10000");
            comboBox1.Items.Add("100000");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SingleThreadedCrawler crawler = new SingleThreadedCrawler(client, Int32.Parse(textBox2.Text), Int32.Parse((String)comboBox1.SelectedItem), Int32.Parse(textBox3.Text));
            crawler.Crawl(textBox1.Text);

            label1.Text = "Something changed";
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
