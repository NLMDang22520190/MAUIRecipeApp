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
            try
            {
                string credentialsPath;

                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    credentialsPath = Path.Combine(FileSystem.AppDataDirectory, "recipeapp-3c612-firebase-adminsdk-blmv2-aabcd1703d.json");

                    // Debugging output to check the file existence
                    Debug.WriteLine("Attempting to open file from app package...");

                    Stream stream = null;
                    try
                    {
                        stream = FileSystem.OpenAppPackageFileAsync("recipeapp-3c612-firebase-adminsdk-blmv2-aabcd1703d.json").Result;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error accessing file: {ex.Message}");
                        return; // Exit if there was an error accessing the file
                    }

                    // Check if the stream is null
                    if (stream == null)
                    {
                        Debug.WriteLine("Error: File not found in app package.");
                        return; // Exit if the file doesn't exist
                    }

                    using (var fileStream = File.Create(credentialsPath))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
                else if (DeviceInfo.Platform == DevicePlatform.WinUI || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                {
                    credentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "recipeapp-3c612-firebase-adminsdk-blmv2-aabcd1703d.json");
                }
                else
                {
                    throw new PlatformNotSupportedException("This platform is not supported.");
                }

                // Kiểm tra file có tồn tại hay không
                if (!File.Exists(credentialsPath))
                {
                    throw new FileNotFoundException("Firebase Admin SDK JSON file not found.", credentialsPath);
                }

                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
                _db = FirestoreDb.Create("recipeapp-3c612");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }

        // Phương thức để lấy instance duy nhất của FirestoreService

        public async Task<bool> AddDocumentAsync<T>(string collectionName, T document)
        {
            try
            {
                FirestoreDb db = FirestoreService.Instance.Db;

                if (db == null)
                {
                    Debug.WriteLine("FirestoreDb instance is null.");
                    return false;
                }

                CollectionReference collectionRef = db.Collection(collectionName);
                DocumentReference docRef = await collectionRef.AddAsync(document);

                Debug.WriteLine($"Document added successfully! Document ID: {docRef.Id}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding document: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> UpdateDocumentAsync(string collectionName, string documentId, Dictionary<string, object> updatedData)
        {
            FirestoreDb db = FirestoreService.Instance.Db;

            try
            {
                DocumentReference docRef = db.Collection(collectionName).Document(documentId);
                await docRef.UpdateAsync(updatedData);
                Debug.WriteLine("Document updated successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating document: {ex.Message}");
                return false;
            }
        }

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
