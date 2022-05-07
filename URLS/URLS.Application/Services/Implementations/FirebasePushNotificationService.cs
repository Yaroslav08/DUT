using CorePush.Google;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Firebase;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace URLS.Application.Services.Implementations
{
    public class FirebasePushNotificationService : IPushNotificationService
    {
        private readonly List<PendingRequest> _pendingRequests;
        private readonly List<SubscribeUser> _subscribedUsers;
        private readonly FirebaseMessaging _firebaseMessaging;

        public FirebasePushNotificationService()
        {
            _pendingRequests = new List<PendingRequest>();
            _subscribedUsers = new List<SubscribeUser>();
            _firebaseMessaging = GetFirebaseMessaging();
        }

        public async Task<PushResponse> SendPushAsync(int userId, PushMessage pushMessage)
        {
            var user = _subscribedUsers.FirstOrDefault(s => s.UserId == userId);
            if (user == null)
            {
                _pendingRequests.Add(new PendingRequest
                {
                    UserId = userId,
                    PushMessages = new List<PushMessage> { pushMessage }
                });
                return new PushResponse
                {
                    Message = "No devices for sent"
                };
            }

            var listOfMessages = new List<Message>();

            foreach (var deviceToken in user.DeviceTokens)
            {
                listOfMessages.Add(new Message
                {
                    Token = deviceToken,
                    Notification = new Notification
                    {
                        Title = pushMessage.Title,
                        Body = pushMessage.Body,
                        ImageUrl = ""
                    }
                });
            }
            var result = await _firebaseMessaging.SendAllAsync(listOfMessages);
            return new PushResponse(result);
        }

        public async Task<PushResponse> SendPushAsync(IEnumerable<int> userIds, PushMessage pushMessage)
        {
            var batchResponse = new List<BatchResponse>();

            foreach (var userId in userIds)
            {
                var user = _subscribedUsers.FirstOrDefault(s => s.UserId == userId);
                if (user == null)
                {
                    _pendingRequests.Add(new PendingRequest
                    {
                        UserId = userId,
                        PushMessages = new List<PushMessage> { pushMessage }
                    });
                }

                var listOfMessages = new List<Message>();

                foreach (var deviceToken in user.DeviceTokens)
                {
                    listOfMessages.Add(new Message
                    {
                        Token = deviceToken,
                        Notification = new Notification
                        {
                            Title = pushMessage.Title,
                            Body = pushMessage.Body,
                            ImageUrl = ""
                        }
                    });
                }
                var result = await _firebaseMessaging.SendAllAsync(listOfMessages);
                batchResponse.Add(result);
            }
            return new PushResponse(batchResponse);
        }

        private FirebaseMessaging GetFirebaseMessaging()
        {
            var defaultApp = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json"))
            });
            return FirebaseMessaging.GetMessaging(defaultApp);
        }
    }
}
