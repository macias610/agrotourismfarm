using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Repository.IRepo
{
    public interface IAccountRepository: IDisposable
    {
        void SendEmailConfirmingRegister(User user,string callbackUrl);
        void SendEmailResetingPassword(User user, string callbackUrl);
    }
}
