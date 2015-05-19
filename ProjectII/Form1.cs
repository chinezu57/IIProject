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
        MultithreadedCrawlerForm multithreadedCrawlerForm;
        DatabaseView databaseViewForm;
        private Boolean isCrawling;

        public WebCrawler(GraphClient client)
        {
            InitializeComponent();
            this.client = client;
            this.isCrawling = false;
            this.databaseViewForm = new DatabaseView(client);
            multithreadedCrawlerForm = new MultithreadedCrawlerForm(client, this, databaseViewForm);
            comboBox1.Items.Add("10");
            comboBox1.Items.Add("100");
            comboBox1.Items.Add("1000");
            comboBox1.Items.Add("10000");
            comboBox1.Items.Add("100000");
            textBox1.Text = "http://www.redblobgames.com/";
            textBox2.Text = "458";
            textBox3.Text = "1200";
            comboBox1.SelectedIndex = 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.isCrawling = true;
            SingleThreadedCrawler crawler = new SingleThreadedCrawler(client, Int32.Parse(textBox2.Text), Int32.Parse((String)comboBox1.SelectedItem), Int32.Parse(textBox3.Text));
            crawler.Crawl(textBox1.Text);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            multithreadedCrawlerForm.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            databaseViewForm.Show();
            this.Hide();
        }
    }
}
