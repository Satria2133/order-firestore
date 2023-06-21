using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Cloud.Firestore;
using System.Web;

namespace crud_grpc_firebase
{
    public partial class crudForm : Form
    {
        FirestoreDb database;
        public crudForm()
        {
            InitializeComponent();
        }

        private void crudForm_Load(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "cloudfire.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            database = FirestoreDb.Create("crud-grpc-firebase");
        }

        void addDocumentWithAutoID(string namaBarang, string namaPembeli, int hargaBarang, int Kuantitas, string TokenUnik) 
        {
            CollectionReference collection = database.Collection("transaction");
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"NamaBarang", namaBarang },
                {"NamaPembeli", namaPembeli },
                {"Kuantitas", Kuantitas },
                {"HargaBarang", hargaBarang},
                {"TokenUnik", TokenUnik  }
            };

            collection.AddAsync(data);


        }

        public async void GetAllDocument(string nameofCollection) 
        {
            Query transactionQuery = database.Collection(nameofCollection);
            QuerySnapshot snap = await transactionQuery.GetSnapshotAsync();

            foreach(DocumentSnapshot documentSnapshot in snap.Documents) 
            {
                Transaction transaction = documentSnapshot.ConvertTo<Transaction>();

                if (documentSnapshot.Exists) 
                {
                    userDataGrid.Rows.Add( transaction.NamaBarang,transaction.NamaPembeli, "Rp." + transaction.HargaBarang.ToString() + ",00", transaction.Kuantitas.ToString() + " buah.", transaction.TokenUnik);
                }
            }
        }

        private void checkButton_Click(object sender, EventArgs e)
        {
            string namaBarang = namaBarangTextBox.Text;
            string namaPembeli = namaPembeliTextBox.Text;
            int kuantitas = int.Parse(kuantitasTextBox.Text);
            int hargaBarang = int.Parse(hargaBarangTextBox.Text);
            string TokenUnik = "";
            if (checkUniqueToken(tokenUnikTextBox.Text, confirmTokenUnikTextBox.Text)) 
            {
                TokenUnik = tokenUnikTextBox.Text;
            }
            if (TokenUnik != "")
            {
                addDocumentWithAutoID(namaBarang, namaPembeli, hargaBarang, kuantitas, TokenUnik);
                MessageBox.Show("Data added successfully");
            } else 
            {
                MessageBox.Show("Data cannot added");
            }
        }

        bool checkUniqueToken(string afterConfirm, string beforeConfirm) 
        {
            if (afterConfirm == beforeConfirm)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            userDataGrid.Rows.Clear();
            GetAllDocument("transaction");
        }
    }
}
