using DUT.Constants;
using DUT.Domain.Models;

namespace DUT.Application.Helpers
{
    public static class NotificationsHelper
    {
        private static Notification Plug => new Notification();

        public static Notification GetWelcomeNotification()
        {
            return new Notification
            {
                Title = "Вітаємо у ДУТ СДН!",
                Content = "Ласкаво просимо у систему ДУТ СДН. Сподіваємося вам сподобається досвід користування данною системою!",
                ImageUrl = "https://previews.123rf.com/images/foxysgraphic/foxysgraphic1907/foxysgraphic190700061/129432470-welcome-banner-speech-bubble-poster-concept-geometric-memphis-style-with-text-welcome-icon-balloon-w.jpg",
                CreatedAt = DateTime.Now,
                CreatedBy = Defaults.CreatedBy,
                CreatedFromIP = "::1",
                Type = NotificationType.Welcome,
                IsImportant = false,
                IsRead = false,
                ReadAt = null,
            };
        }

        public static Notification GetAcceptInGroupNotification(Group group)
        {
            return new Notification
            {
                Title = $"Вас доєднано до групи {group.Name}",
                Content = $"Ви були доєднані до групи {group.Name}. Бажаємо Вам успішного навчання та сдачі еказменів)",
                ImageUrl = "https://cdn1.iconfinder.com/data/icons/color-bold-style/21/34-512.png",
                CreatedAt = DateTime.Now,
                CreatedBy = Defaults.CreatedBy,
                CreatedFromIP = "::1",
                Type = NotificationType.Welcome,
                IsImportant = false,
                IsRead = false,
                ReadAt = null,
            };
        }

        public static Notification GetLoginNotification()
        {
            return Plug;
        }

        public static Notification GetChangePasswordNotification()
        {
            return new Notification
            {
                Title = "Пароль було змінено",
                Content = "Увага! Ваш пароль було змінено",
                ImageUrl = "https://thumbs.dreamstime.com/b/lock-login-password-safe-security-icon-vector-illustration-flat-design-lock-login-password-safe-security-icon-vector-illustration-131742100.jpg",
                CreatedAt = DateTime.Now,
                CreatedBy = Defaults.CreatedBy,
                CreatedFromIP = "::1",
                Type = NotificationType.ChangePassword,
                IsImportant = true,
                IsRead = false,
                ReadAt = null,
            };
        }

        public static Notification GetLogoutNotification()
        {
            return Plug;
        }

        public static Notification GetNewPostNotification()
        {
            return Plug;
        }
    }
}
