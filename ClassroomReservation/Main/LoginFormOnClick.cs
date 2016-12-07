using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomReservation.Main
{
    public interface LoginFormOnClick
    {
        void OnClick(LoginForm form, string Id, string password);
    }
}
