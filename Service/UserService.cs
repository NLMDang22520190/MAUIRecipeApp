using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUIRecipeApp.Models;

namespace MAUIRecipeApp.Service
{
    public class UserService
    {
        private static UserService _instance;  // Instance duy nhất của UserService
        private static readonly object _lock = new object();  // Lock object để đảm bảo thread safety

        public User CurrentUser { get; private set; }  // Thông tin người dùng hiện tại

        public User CurrentSignUpUser { get; private set; }  // Thông tin người dùng đang đăng ký

        public string CurrentRecoveryEmail { get; set; }

        // Private constructor để đảm bảo chỉ có thể tạo 1 instance
        private UserService()
        {
        }

        // Property để truy cập instance duy nhất của UserService
        public static UserService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new UserService();
                    }
                    return _instance;
                }
            }
        }

        // Phương thức để cập nhật thông tin người dùng hiện tại
        public void SetCurrentUser(User user)
        {
            CurrentUser = user;
        }

        // Phương thức để xóa thông tin người dùng (ví dụ: khi người dùng logout)
        public void ClearCurrentUser()
        {
            CurrentUser = null;
        }

        public void SetCurrentSignUpUser(User user)
        {
            CurrentSignUpUser = user;
        }

        public void ClearCurrentSignUpUser()
        {
            CurrentSignUpUser = null;
        }
    }

}
