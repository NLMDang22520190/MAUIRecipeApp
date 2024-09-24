using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.Service
{
    public class FirestoreService
    {
        private static FirestoreService _instance;
        private static readonly object _lock = new object();
        private FirestoreDb _db;

        // Private constructor để ngăn chặn khởi tạo từ bên ngoài
        private FirestoreService()
        {
            //var pathToServiceAccountKey = "C:\\Users\\TEKATOJI\\source\\repos\\NLMDang22520190\\MAUIRecipeApp\\recipeapp-3c612-firebase-adminsdk-blmv2-aabcd1703d.json";

            //var credential = GoogleCredential.FromFile(pathToServiceAccountKey);
            //FirestoreDb db = FirestoreDb.Create("recipeapp-3c612", new FirestoreClientBuilder { ChannelCredentials = credential.ToChannelCredentials() }.Build());

            // Lấy đường dẫn tuyệt đối đến file Firebase Admin SDK JSON

            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;

                // Kết hợp đường dẫn cơ bản với tên file (giả sử file nằm trong thư mục gốc của dự án)
                string credentialsPath = Path.Combine(basePath, "recipeapp-3c612-firebase-adminsdk-blmv2-aabcd1703d.json");

                // Kiểm tra file có tồn tại hay không
                if (!File.Exists(credentialsPath))
                {
                    throw new FileNotFoundException("Firebase Admin SDK JSON file not found.", credentialsPath);
                }

                // Sử dụng đường dẫn đến file JSON để thiết lập Firestore
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
                _db = FirestoreDb.Create("recipeapp-3c612");
            }
            catch (Exception ex) { 
                Debug.WriteLine("Error: " +  ex.Message);
            }
           
        }

        // Phương thức để lấy instance duy nhất của FirestoreService
        public static FirestoreService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new FirestoreService();
                        }
                    }
                }
                return _instance;
            }
        }

        // Trả về FirestoreDb
        public FirestoreDb Db => _db;
    }
}
