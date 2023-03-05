/*Project - Phone Book in C# - InfoBrother*/

using System;
using System.Data;
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace PhoneBook
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Code For "New" Button:
        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                panel1.Enabled = true;
                //Add a New Row:
                App.PhoneBook.AddPhoneBookRow(App.PhoneBook.NewPhoneBookRow());
                phoneBookBindingSource.MoveLast();
                txtName.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.PhoneBook.RejectChanges();
            }
        }

        //Code for "Edit" button:
        private void btnEdit_Click(object sender, EventArgs e)
        {
            panel1.Enabled = true;
            txtPhone.Focus();
        }


        //Code for "Cancel"  Button:
        private void btnCancel_Click(object sender, EventArgs e)
        {
            phoneBookBindingSource.ResetBindings(false);
            panel1.Enabled = false;
        }

        //Code for "Save"  button:
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //End Edit, Save Data To file:
                phoneBookBindingSource.EndEdit();
                App.PhoneBook.AcceptChanges();
                App.PhoneBook.WriteXml(string.Format("{0}//data.dat", Application.StartupPath));
                panel1.Enabled = false;

                MessageBox.Show("Number Store Successfully:");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.PhoneBook.RejectChanges();
            }
        }


        //Use Singleton Pattern to Create an Instance.
        static PhoneData db;
        protected static PhoneData App
        {
            get
            {
                if (db == null)
                {
                    db = new PhoneData();
                }
                return db;
            }
        }


        //Code for "Form".
        private void Form1_Load(object sender, EventArgs e)
        {
            string filename = string.Format("{0}//data.dat", Application.StartupPath);
            if (File.Exists(filename))
            {
                App.PhoneBook.ReadXml(filename);
            }
            phoneBookBindingSource.DataSource = App.PhoneBook;
            panel1.Enabled = false;
        }


        //Code for "DataGridView". 
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("Are You sure that you want to Delete this Record?", "Message",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    phoneBookBindingSource.RemoveCurrent();
                }
            }
        }

        //code for "Search box";
        private void txtSearch_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) //enter Key:
            {
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {

                    //we can use linq to Query data:
                    var query = from o in App.PhoneBook
                                where o.PhoneNo == txtSearch.Text 
                                ||    o.FullName.ToLowerInvariant().Contains(txtSearch.Text.ToLowerInvariant())
                                ||    o.Email.ToLowerInvariant() == txtSearch.Text.ToLowerInvariant()
                                select o;
                    dataGridView1.DataSource = query.ToList();
                }
                else
                {
                    dataGridView1.DataSource = phoneBookBindingSource;
                }
            }
        }
    }
}
