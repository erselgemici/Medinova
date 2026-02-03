using System;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;

namespace Medinova.Services
{
    public class EmailService
    {
        public bool SendAppointmentMail(string toEmail, string patientName, DateTime date, string time, string doctorName)
        {
            try
            {
                string host = WebConfigurationManager.AppSettings["SmtpHost"];
                int port = int.Parse(WebConfigurationManager.AppSettings["SmtpPort"]);
                string senderEmail = WebConfigurationManager.AppSettings["SmtpMail"];
                string senderPass = WebConfigurationManager.AppSettings["SmtpPass"];
                string hospitalName = WebConfigurationManager.AppSettings["HospitalName"];

                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(senderEmail, senderPass);

                    var mail = new MailMessage();
                    mail.From = new MailAddress(senderEmail, hospitalName);
                    mail.To.Add(toEmail);
                    mail.Subject = $"Randevu Onayı - {hospitalName}";
                    mail.IsBodyHtml = true;

                    string dateStr = date.ToString("dd.MM.yyyy");

                    mail.Body = $@"
                        <div style='font-family: Arial, sans-serif; border: 1px solid #e0e0e0; padding: 20px; max-width: 600px;'>
                            <h2 style='color: #0d6efd;'>Sayın {patientName},</h2>
                            <p>Hastanemizden aldığınız randevu başarıyla oluşturulmuştur.</p>
                            <table style='width: 100%; text-align: left; background-color: #f9f9f9; padding: 10px; border-radius: 5px;'>
                                <tr>
                                    <th style='padding: 5px;'>Doktor:</th>
                                    <td>{doctorName}</td>
                                </tr>
                                <tr>
                                    <th style='padding: 5px;'>Tarih:</th>
                                    <td>{dateStr}</td>
                                </tr>
                                <tr>
                                    <th style='padding: 5px;'>Saat:</th>
                                    <td>{time}</td>
                                </tr>
                            </table>
                            <br/>
                            <p style='color: #888; font-size: 12px;'>Bu otomatik bir bilgilendirme mesajıdır. Lütfen cevaplamayınız.</p>
                            <p><b>{hospitalName}</b></p>
                        </div>";

                    client.Send(mail);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
