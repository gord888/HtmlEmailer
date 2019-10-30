using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using ThirteenSides.Common.Email;
using Xunit;

namespace ThirteenSides.Common.Tests
{
    public class EmailTests
    {
        [Fact]
        public void SendEmail()
        {

            // Use this for selfsigned certificates if sending email fails because of an invalid cert
            // You shouldn't use this in production.
            ServicePointManager.ServerCertificateValidationCallback =   (sender, certificate, chain, sslPolicyErrors) => true;

            HtmlEmailer emailer = new HtmlEmailer();

            emailer.AddVariable("__INTRODUCTION_HEADER__", "HELLO WORLD", SubstitutionVariable.VariableType.Both);
            emailer.AddVariable("__INTRODUCTION_SUB_HEADER__", "Some kind of sub header", SubstitutionVariable.VariableType.Both);
            emailer.AddVariable("__INTRODUCTION__", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum", SubstitutionVariable.VariableType.Both);

            emailer.AddVariable("__BODY_LEFT_HEADER__", "Hello", SubstitutionVariable.VariableType.Both);
            emailer.AddVariable("__BODY_LEFT_BODY__", "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat", SubstitutionVariable.VariableType.Both);

            emailer.AddVariable("__BODY_RIGHT_HEADER__", "World", SubstitutionVariable.VariableType.Both);
            emailer.AddVariable("__BODY_RIGHT_BODY__", "Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur", SubstitutionVariable.VariableType.Both);


            // it could be the case that if a user can only get text-based emails, we may provide
            // a different URL
            emailer.AddVariable("__REGISTERURL__", "http://google.ca", SubstitutionVariable.VariableType.Text);
            emailer.AddVariable("__REGISTERURL__", "http://bing.com", SubstitutionVariable.VariableType.Html);

            emailer.AddImageAttachment("./TestFiles/registerbutton.png", "image/png", "registerbutton");

            emailer.HtmlBodyTemplate = File.ReadAllText(@".\TestFiles\emailtemplate.html");
            emailer.TextBodyTemplate = File.ReadAllText(@".\TestFiles\emailtemplate.txt");

            emailer.ToEmails.Add("somewhere@someplace.ca");
            emailer.Sender = "user111@host.net";
            emailer.FromAddress = "user@host.net";
            emailer.Subject = "hello world";
            
            
            


            MailMessage mail = emailer.GenerateMail();
            


            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.server.com";
            smtpClient.Port = 25;
            smtpClient.UseDefaultCredentials = true;
            //smtpClient.Credentials = new System.Net.NetworkCredential("user", "pass");
            //smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;

            smtpClient.Send(mail);

        }
    
    
        [Fact]
        public void SendEmailSimpleExample()
        {
            // Use this for selfsigned certificates if sending email fails because of an invalid cert.  You shouldn't use this in production.
            //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            // create a new emailer
            HtmlEmailer emailer = new HtmlEmailer();

            // change the customer name
            emailer.AddVariable("__CUSTOMERNAME__", "John Doe");

            // it could be the case that if a user can only get text-based emails, we may provide a different URL
            emailer.AddVariable("__CLICKMEURL__", "http://google.ca", SubstitutionVariable.VariableType.Text);
            emailer.AddVariable("__CLICKMEURL__", "http://bing.com", SubstitutionVariable.VariableType.Html);



            // adding an image file.  the registerbutton matches the cid:someimagecidname of the image source.
            emailer.AddImageAttachment("./TestFiles/registerbutton.png", "image/png", "someimagecidname");

            // You'll probably use File.ReadAllText rather than this.
            emailer.HtmlBodyTemplate = "<html>  <head></head>  <body>    <h1>Hello __CUSTOMERNAME__</h1>    <p>Here is a cool image for you!</p>    <p><img src=\"cid:someimagecidname\" /></p>    <p>click <a href=\"__CLICKMEURL__\">here</a> for more.</p>  </body></html>";
            emailer.TextBodyTemplate = "Hello __CUSTOMERNAME__ You won't see an image, but we have a url for you! __CLICKMEURL__";
            

            emailer.ToEmails.Add("yourself@gmail.com");
            
            emailer.Sender = "user@host.net";
            emailer.FromAddress = "user@host.net";
                       
            emailer.Subject = "hello world";


            MailMessage mail = emailer.GenerateMail();


            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            
            //smtpClient.UseDefaultCredentials = true;

            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("*****@gmail.com", "******");
            
            smtpClient.EnableSsl = true;

            // if you're using gmail and you get an authentication error message.  you may need to "allow less secure apps" on your google account.
            smtpClient.Send(mail);

        }

    }
}
