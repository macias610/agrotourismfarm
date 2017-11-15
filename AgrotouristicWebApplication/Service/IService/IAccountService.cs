using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IAccountService 
    {
        void SendEmailConfirmingRegister(User user, string callbackUrl);
        void SendEmailResetingPassword(User user, string callbackUrl);
    }
}
