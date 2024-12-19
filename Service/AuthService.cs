using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

        //public async Task<User> AddOrUpdateUserAsync(string email, string username, string provider)
        //{
        //    var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.Provider == provider);

        //    if (user == null)
        //    {
        //        user = new User
        //        {
        //            Username = username,
        //            Email = email,
        //            Provider = provider,
        //            UserType = false,  // Hoặc loại khác nếu cần
        //            Password = null  // OAuth không cần mật khẩu
        //        };

        //        _dbContext.Users.Add(user);
        //        await _dbContext.SaveChangesAsync();
        //    }

        //    return user;
        //}
    }
}
