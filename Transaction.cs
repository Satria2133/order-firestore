using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace crud_grpc_firebase
{
    [FirestoreData]
    public class Transaction
    {
        [FirestoreProperty]
        public string NamaBarang { get; set; }
        [FirestoreProperty]
        public string NamaPembeli { get; set; }
        [FirestoreProperty]
        public int HargaBarang { get; set; }
        [FirestoreProperty]
        public int Kuantitas { get; set; }
        [FirestoreProperty]
        public string TokenUnik { get; set; }
    }
}
