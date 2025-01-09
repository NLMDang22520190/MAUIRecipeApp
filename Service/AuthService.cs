using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUIRecipeApp.DTO;


namespace MAUIRecipeApp.Service
{
    public class AuthService
    {
        private static AuthService _instance;  // Dùng để lưu trữ instance duy nhất của AuthService
        private static readonly object _lock = new object(); // Lock object để đảm bảo thread safety

        private readonly FirestoreDb _db;

        // Private constructor để tránh việc tạo instance bên ngoài
        private AuthService()
        {
            _db = FirestoreService.Instance.Db;
            if (_db == null)
            {
                Debug.WriteLine("Firestore DB is null");
            }
        }

        public User Login(string email, string password)
        {
            // Lấy dữ liệu người dùng từ Firestore theo email
            var userCollection = _db.Collection("User");  // Thay "users" bằng tên collection chứa người dùng của bạn
            var query = userCollection.WhereEqualTo("Email", email);  // Tìm người dùng theo email

            var snapshot = query.GetSnapshotAsync().Result;  // Lấy dữ liệu người dùng từ Firestore

            if (snapshot.Documents.Count == 0)
            {
                Debug.WriteLine("User not found");
                return null;  // Trả về null nếu không tìm thấy người dùng
            }

            var userDoc = snapshot.Documents.First();
            var user = userDoc.ConvertTo<User>();  // Chuyển đổi từ Firestore document sang đối tượng User
            user.Uid = userDoc.Id;  // Lưu ID của người dùng

            // Kiểm tra mật khẩu
            if (PasswordHasherService.VerifyPassword(password, user.Password))
            {
                return user;  // Trả về người dùng nếu mật khẩu đúng
            }
            else
            {
                Debug.WriteLine("Invalid password");
                return null;  // Trả về null nếu mật khẩu không đúng
            }
        }

        public User SignUp(string email, string password, string username)
        {
            // Lấy dữ liệu người dùng từ Firestore theo email
            var userCollection = _db.Collection("User");
            var query = userCollection.WhereEqualTo("Email", email);

            var snapshot = query.GetSnapshotAsync().Result;

            // Kiểm tra nếu email đã tồn tại
            if (CheckEmailExist(email))
            {
                return null;
            }

            // Tạo người dùng mới
            var newUser = new User
            {
                Email = email,
                Username = username,
                Password = PasswordHasherService.HashPassword(password) // Hash mật khẩu
            };

            //userCollection.Document().SetAsync(newUser).Wait(); // Lưu người dùng mới vào Firestore
            //Debug.WriteLine("User registered successfully");
            return newUser;
        }

        public async Task<bool> AddNewUser(User user, bool isAdmin)
        {
            try
            {
                var userCollection = _db.Collection("User");

                // 1. Lấy số lượng user hiện có
                var snapshot = await userCollection.GetSnapshotAsync();
                int userCount = snapshot.Count;

                // 2. Tạo ID dạng "UUID + số thứ tự 3 chữ số"
                string customId = $"UUID{(userCount + 1):D3}";

                // 3. Chuẩn bị dữ liệu người dùng
                var newUser = new FirestoreUserDto
                {
                    Email = user.Email,
                    Username = user.Username,
                    Password = user.Password,
                    isAdmin = isAdmin
                };

                // 4. Lưu người dùng với Document ID tuỳ chỉnh
                await userCollection.Document(customId).SetAsync(newUser);

                // 5. Trả về true nếu lưu thành công
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding user: {ex.Message}");
                // 6. Trả về false nếu có lỗi xảy ra
                return false;
            }
        }

        public async Task<bool> UpdateUserPassword(string email, string newPassword)
        {
            try
            {
                var userCollection = _db.Collection("User");
                var query = userCollection.WhereEqualTo("Email", email);
                var snapshot = await query.GetSnapshotAsync();

                // Lấy tài liệu đầu tiên
                var userDoc = snapshot.Documents.First();

                // Hash mật khẩu mới
                var hashedPassword = PasswordHasherService.HashPassword(newPassword);

                // Chỉ cập nhật trường Password
                await userDoc.Reference.UpdateAsync("Password", hashedPassword);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating user password: {ex.Message}");
                return false;
            }
        }



        public bool CheckEmailExist(string email)
        {
            if (CountUserWithEmail(email) > 0)
                return true;
            return false;
        }

        private int CountUserWithEmail(string email)
        {
            // Lấy dữ liệu người dùng từ Firestore theo email
            var userCollection = _db.Collection("User");
            var query = userCollection.WhereEqualTo("Email", email);

            var snapshot = query.GetSnapshotAsync().Result;

            return snapshot.Documents.Count;
            
        }

        // Property để truy cập instance duy nhất của AuthService
        public static AuthService Instance
        {
            get
            {
                // Kiểm tra xem instance đã được tạo chưa, nếu chưa thì tạo mới
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new AuthService();
                    }
                    return _instance;
                }
            }
        }

    }
}
