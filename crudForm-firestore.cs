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
using System.Web;
using Google.Cloud.Firestore.V1;

namespace crud_grpc_firebase
{
    public partial class crudForm : Form
    {
        FirestoreDb database;
        private string selectedDocumentId;

        public crudForm()
        {
            InitializeComponent();
        }

        void dataLoad() 
        {
            userDataGrid.Rows.Clear();
            GetAllDocument("transaction");
        }

        private void crudForm_Load(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "cloudfire.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            database = FirestoreDb.Create("crud-grpc-firebase");
            dataLoad();
           
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
            dataLoad();


        }

        void getDocumentWithID(String documentId)
        {
            DocumentReference documentReference = database.Collection("transaction").Document(documentId);
            DocumentSnapshot documentSnapshot = documentReference.GetSnapshotAsync().GetAwaiter().GetResult();

            if (documentSnapshot.Exists)
            {
                Transaction transaction = documentSnapshot.ConvertTo<Transaction>();
                namaBarangTextBox.Text = transaction.NamaBarang;
                namaPembeliTextBox.Text = transaction.NamaPembeli;
                hargaBarangTextBox.Text = transaction.HargaBarang.ToString();
                kuantitasTextBox.Text = transaction.Kuantitas.ToString();
                tokenUnikTextBox.Text = transaction.TokenUnik;
                confirmTokenUnikTextBox.Text = transaction.TokenUnik;
            }
        }

        void updateDocument(String documentId)
        {
            DocumentReference documentReference = database.Collection("transaction").Document(documentId);
            Dictionary<string, object> updates = new Dictionary<string, object>()
                {
                    {"NamaBarang", namaBarangTextBox.Text },
                    {"NamaPembeli", namaPembeliTextBox.Text },
                    {"Kuantitas", int.Parse(kuantitasTextBox.Text) },
                    {"HargaBarang", int.Parse(hargaBarangTextBox.Text) },
                    {"TokenUnik", tokenUnikTextBox.Text }
                };

            documentReference.UpdateAsync(updates);
        }

     
        void deleteDocument(string documentId) 
        {
            DocumentReference documentReference = database.Collection("transaction").Document(documentId);
            documentReference.DeleteAsync();
            dataLoad();
        }

        public async void GetAllDocument(string nameofCollection)
        {
            Query transactionQuery = database.Collection(nameofCollection);
            QuerySnapshot snap = await transactionQuery.GetSnapshotAsync();

            userDataGrid.Rows.Clear();

            foreach (DocumentSnapshot documentSnapshot in snap.Documents)
            {
                Transaction transaction = documentSnapshot.ConvertTo<Transaction>();

                if (documentSnapshot.Exists)
                {
                    int rowIndex = userDataGrid.Rows.Add(documentSnapshot.Id, transaction.NamaBarang, transaction.NamaPembeli, "Rp." + transaction.HargaBarang.ToString() + ",00", transaction.Kuantitas.ToString() + " buah.", transaction.TokenUnik);

                    // Set the document ID as the value in the first cell of the row
                    userDataGrid.Rows[rowIndex].Cells[0].Value = documentSnapshot.Id;
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
            }
            else
            {
                MessageBox.Show("Data cannot added");
            }
        }

        bool checkUniqueToken(string afterConfirm, string beforeConfirm) => afterConfirm == beforeConfirm;

        private void userDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (userDataGrid.SelectedRows.Count > 0)
            {
                // Get the ID from the first cell of the selected row
                string documentId = userDataGrid.SelectedRows[0].Cells[0].Value.ToString();

                // Use the document ID to fetch the data from Firestore and populate the textboxes
                getDocumentWithID(documentId);
            }
        }

        private void userDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {

                if (e.ColumnIndex == userDataGrid.Columns["EditColumn"].Index)
                {
                    DataGridViewRow row = userDataGrid.Rows[e.RowIndex];
                    selectedDocumentId = row.Cells["idColumn"].Value.ToString();
                    getDocumentWithID(selectedDocumentId);
                }


                if (e.ColumnIndex == userDataGrid.Columns["DeleteColumn"].Index)
                {

                    DataGridViewRow row = userDataGrid.Rows[e.RowIndex];
                    selectedDocumentId = row.Cells["idColumn"].Value.ToString();
                    deleteDocument(selectedDocumentId);

                }
            }
        }
        private void updateButton_Click(object sender, EventArgs e)
        {
            updateDocument(selectedDocumentId);
            dataLoad();
        }
    }
}
