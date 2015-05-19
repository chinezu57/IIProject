using Neo4jClient;
using ProjectII.Domain;
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
    public partial class DatabaseView : Form
    {

        private GraphClient client;

        public DatabaseView(GraphClient client)
        {
            InitializeComponent();
            this.client = client;
            listView1.View = View.List;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string url = textBox2.Text;
            List<Link> results = client.Cypher
                  .Match("(link:Link)")
                  .Return(link => link.As<Link>())
                  .Results.ToList();
            foreach(Link l in results){
                listView1.Items.Add(l.Name);  
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string url = textBox2.Text;
            List<Link> results = client.Cypher
                  .Match("(link:Link)")
                  .Return(link => link.As<Link>())
                  .Results.ToList();
            foreach (Link l in results)
            {
                listView1.Items.Add(l.Name);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            client.Cypher
                  .OptionalMatch("(link:Link)<-[r]-()")
                  .Where((Link link) => link.Name == textBox2.Text)
                  .Delete("r, link")
                  .ExecuteWithoutResults();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                textBox2.Text = listView1.SelectedItems[0].Text;
            }
            int a = 1;
        }

        private void DatabaseView_Load(object sender, EventArgs e)
        {

        }
    }
}
