﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUIRecipeApp.Service;

namespace MAUIRecipeApp.ViewModel.Auth
{
    public partial class PasswordVerifyCodePageViewModel: ObservableObject
    {
        [ObservableProperty] private string _code;

        [ObservableProperty] private string _timerString;

        [ObservableProperty] private string _errorMSG;

        private readonly IMemoryCache _cache;

        private CancellationTokenSource _cancellationTokenSource;

        public PasswordVerifyCodePageViewModel(IMemoryCache cache)
        {
            _cache = cache;
            SendCode();
        }

        [RelayCommand]
        private async Task VerifyCode()
        {
            var cacheCode = _cache.Get<string>("verify_code");
            if (Code == cacheCode)
            {
                await Shell.Current.GoToAsync("//newpassword");
            }
            else
            {
                ErrorMSG = "Invalid code";
                Debug.WriteLine("Invalid code");
            }
        }

        [RelayCommand]
        public void ResendCode()
        {
            SendCode();
            ErrorMSG = "Another code have been sent to your email";
        }

        private async void StartTimer(int minutes)
        {
            _cancellationTokenSource?.Cancel(); // Hủy timer trước đó nếu có
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            int totalSeconds = minutes * 60;

            try
            {
                while (totalSeconds > 0)
                {
                    if (token.IsCancellationRequested)
                        break;

                    int minutesLeft = totalSeconds / 60;
                    int secondsLeft = totalSeconds % 60;
                    TimerString = $"{minutesLeft:D2}:{secondsLeft:D2}";

                    await Task.Delay(1000, token); // Đợi 1 giây
                    totalSeconds--;

                }

                TimerString = "00:00"; // Kết thúc timer
            }
            catch (TaskCanceledException)
            {
                // Timer bị hủy, không cần xử lý thêm
            }
        }

        private async void SendCode()
        {
            StartTimer(3);
            var code = await MailService.Instance.SendVerificationCodeAsync(UserService.Instance.CurrentRecoveryEmail);
            _cache.Set("verify_code", code, TimeSpan.FromMinutes(3)); // Lưu mã xác thực vào cache
        }
    }
}
