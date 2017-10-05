using Repository.IRepo;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repository.Repo
{
    public class AccountRepository : Email, IAccountRepository
    {
        private readonly IAgrotourismContext db;

        public AccountRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public void SendEmailResetingPassword(User user, string callbackUrl)
        {
            string body = string.Format("Drogi {0}<BR/>, kliknij link poniżej w celu zresetowania hasła: <a href=\"{1}\" title=\"User Reset Password\">{1}</a>", user.UserName, callbackUrl);
            base.SendEmail(user.Email, "Zresetowanie hasła", body);
        }

        public void SendEmailConfirmingRegister(User user, string callbackUrl)
        {
            string body = string.Format("Drogi {0}<BR/>Dziękujemy za rejestrację, kliknij link poniżej w celu ukończenia rejestracji: <a href=\"{1}\" title=\"User Email Confirm\">{1}</a>", user.UserName, callbackUrl);
            base.SendEmail(user.Email, "Potwierdzenie adresu e-mail", body);
        }
    }
}