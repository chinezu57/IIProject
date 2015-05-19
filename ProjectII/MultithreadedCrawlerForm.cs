using Neo4jClient;
using ProjectII.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectII
{
    public partial class MultithreadedCrawlerForm : Form
    {
        private GraphClient client;
        private WebCrawler singleThreadedCrawlerForm;
        private DatabaseView databaseViewForm;
        private Boolean isCrawling;

        public MultithreadedCrawlerForm(GraphClient client, WebCrawler singleThreadedCrawlerForm, DatabaseView databaseViewForm)
        {
            InitializeComponent();
            this.client = client;
            this.isCrawling = false;
            this.singleThreadedCrawlerForm = singleThreadedCrawlerForm;
            this.databaseViewForm = databaseViewForm;
            textBox1.Text = "http://www.redblobgames.com/";
            textBox2.Text = "10";
            textBox3.Text = "659";
            textBox4.Text = "100";
            textBox5.Text = "0";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.isCrawling = true;
            MultiThreadedCrawler multithreadedCrawler = new MultiThreadedCrawler(client);
            multithreadedCrawler.crawl(textBox1.Text, Int32.Parse(textBox2.Text), Int32.Parse(textBox3.Text), Int32.Parse(textBox4.Text), Int32.Parse(textBox5.Text));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            singleThreadedCrawlerForm.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            databaseViewForm.Show();
            this.Hide();
        }
    }
}
