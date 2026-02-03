using Medinova.Models;
using System;
using System.Web;

namespace Medinova.Services
{
    public class LogService
    {

        public void CreateLog(int? userId, string role, string type, string action, string desc)
        {
            try
            {
                using (var _db = new MedinovaContext())
                {
                    string ip = HttpContext.Current?.Request?.UserHostAddress ?? "0.0.0.0";

                    if (ip.Length > 50) ip = ip.Substring(0, 50);

                    var log = new Log
                    {
                        UserId = userId,
                        UserRole = role ?? "Anonymous",
                        LogType = type,
                        Action = action,
                        Description = desc,
                        IPAddress = ip, 
                        CreatedDate = DateTime.Now
                    };

                    _db.Logs.Add(log);
                    _db.SaveChanges();
                }
            }
            catch
            {
                
            }
        }
    }
}
