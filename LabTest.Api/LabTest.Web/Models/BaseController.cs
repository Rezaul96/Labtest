using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LabTest.Web.Models
{
    public class BaseController : Controller
    {
        public BaseController()
        {

        }


        private RegistrationModelView _currentUser;
        public RegistrationModelView currentUser
        {
            get
            {
                RegistrationModelView usersModelView = new RegistrationModelView();
                if (_currentUser != null)
                    return _currentUser;
                JObject user = null;
                var cookie = Request.Cookies["user"];
                if (cookie != null)
                {
                    user = JsonConvert.DeserializeObject<JObject>(HttpUtility.UrlDecode(cookie));
                }
                usersModelView.Id = Convert.ToInt32(user?["Id"].ToString());
                usersModelView.Email = user?["Email"].ToString();
                usersModelView.MobileNumber = user?["MobileNumber"].ToString();
                _currentUser = usersModelView;
                return _currentUser;

            }

        }
    }
}