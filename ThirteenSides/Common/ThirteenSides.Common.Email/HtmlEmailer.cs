using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.IO;

namespace ThirteenSides.Common.Email 
{
    public class ImageAttachment
    {
        public String MimeType { get; set; }
        public String ContentId { get; set; }
    }

    
    /// <summary>
    /// Gords Famous emailer!
    /// </summary>
    public class HtmlEmailer
    {
        #region Properties
        public List<String> ToEmails { get; set; }
        public String Subject { get; set; }

        public String Sender { get; set; }
        public String FromAddress { get; set; }
        public String HtmlBodyTemplate { get; set; }
        public String TextBodyTemplate { get; set; }
        public List<SubstitutionVariable> Variables { get; set; }
        public Dictionary<String, ImageAttachment> ImageAttachments { get; set; } // image file location

        #endregion

        #region Constructors
        public HtmlEmailer()
        {
            this.ImageAttachments = new Dictionary<String, ImageAttachment>();
            this.Variables = new List<SubstitutionVariable>();
            this.ToEmails = new List<String>();
        }

        public HtmlEmailer(List<SubstitutionVariable> variables)
        {
            this.ToEmails = new List<String>();
            Variables = variables;
        }
        #endregion

        #region Add/Remove Variables
        public void AddVariable(String variable, String value, SubstitutionVariable.VariableType type = SubstitutionVariable.VariableType.Both)
        {
            if (!this.Variables.Contains(new SubstitutionVariable() { Variable = variable, Value = value, SubstitutionType = type }))
                this.Variables.Add(new SubstitutionVariable() { Variable = variable, Value = value, SubstitutionType = type });
        }
        public Boolean RemoveVariable(String variable, SubstitutionVariable.VariableType type = SubstitutionVariable.VariableType.Both)
        {
            SubstitutionVariable deleteMe = this.Variables.FirstOrDefault(v => v.Variable == variable && v.SubstitutionType == type);
            if (deleteMe != null)
                return this.Variables.Remove(deleteMe);

            return false;
        }
        #endregion

        #region Add/Remove ToEmails
        public void AddToEmail(String toEmail)
        {
            this.ToEmails.Add(toEmail);
        }
        public Boolean RemoveToEmail(String toEmail)
        {
            return this.ToEmails.Remove(toEmail);
        }
        #endregion

        #region Add/Remove Image Attachments
        public void AddImageAttachment(String fileLocation, String mimeType, String contentId)
        {
            this.ImageAttachments.Add(fileLocation, new ImageAttachment() { MimeType = mimeType, ContentId = contentId });
        }
        public Boolean RemoveImageAttachment(String fileLocation)
        {
            return this.ImageAttachments.Remove(fileLocation);
        }
        public void RemoveAllImageAttachments()
        {
            this.ImageAttachments.Clear();
        }
        #endregion

        /// <summary>
        /// Apply the substitutions of variables to characters.
        /// </summary>
        /// <param name="bodyTemplate">The template with variable substitutions</param>
        /// <param name="variables">the dictionary list, the key being the variable</param>
        /// <returns>A string of the original message substitued in.</returns>
        private static String ApplyVariables(String bodyTemplate, List<SubstitutionVariable> variables, Boolean isHtml)
        {
            String mailBody = new String(bodyTemplate.ToCharArray());

            List<SubstitutionVariable> substitutionsVariables = new List<SubstitutionVariable>();

            if (isHtml)
            {
                substitutionsVariables = variables.Where(v => v.SubstitutionType == SubstitutionVariable.VariableType.Both || v.SubstitutionType == SubstitutionVariable.VariableType.Html).ToList();
            }
            else
            {
                substitutionsVariables = variables.Where(v => v.SubstitutionType == SubstitutionVariable.VariableType.Both || v.SubstitutionType == SubstitutionVariable.VariableType.Text).ToList();
            }

            foreach (SubstitutionVariable variable in substitutionsVariables)
            {
                mailBody = mailBody.Replace(variable.Variable, variable.Value);
            }
            return mailBody;
        }

        /// <summary>
        /// Builds a html alternateview object applying the image attachements.
        /// </summary>
        /// <param name="htmlBody">the body with variables applied</param>
        /// <param name="imageAttachments">the image attachments</param>
        /// <returns>the alternateview of the html email passed in with added images.</returns>
        private static AlternateView BuildHtmlView(String htmlBody, Dictionary<String, ImageAttachment> imageAttachments)
        {
            AlternateView htmlView;
            htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");

            foreach (String key in imageAttachments.Keys)
            {
                LinkedResource imageLink = new LinkedResource(key, imageAttachments[key].MimeType);
                imageLink.ContentId = imageAttachments[key].ContentId;
                imageLink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                htmlView.LinkedResources.Add(imageLink);
            }

            return htmlView;
        }

        /// <summary>
        /// Builds a text alternateview object applying.
        /// </summary>
        /// <param name="textBody">The text body</param>
        /// <returns>the alternateview of the text email.</returns>
        private static AlternateView BuildTextView(String textBody)
        {
            AlternateView textView;
            textView = AlternateView.CreateAlternateViewFromString(textBody, null, "text/plain");

            return textView;
        }

        /// <summary>
        /// Send the mail message using settings defined in the application configuration.
        /// </summary>
        public void SendMail()
        {
            MailMessage mail = GenerateMail();
            
            // build the smtp client to send the mail
            SmtpClient smtp = new SmtpClient();
            smtp.Send(mail);
        }



        public MailMessage GenerateMail()
        {
            // create a new mail message
            MailMessage mail = new MailMessage();

            // apply all the "to" email addresses
            foreach (String toEmail in this.ToEmails)
            {
                mail.To.Add(toEmail);
            }

            // apply the from address if available
            if (!String.IsNullOrEmpty(FromAddress))
                mail.From = new MailAddress(FromAddress);

            if (!String.IsNullOrEmpty(Sender))
                mail.Sender = new MailAddress(Sender);

            // apply the subject
            mail.Subject = this.Subject;

            // apply the text body template if applicable
            if (!String.IsNullOrEmpty(this.TextBodyTemplate))
                mail.AlternateViews.Add(BuildTextView(ApplyVariables(this.TextBodyTemplate, this.Variables, false)));

            // apply the html body template if applicable
            if (!String.IsNullOrEmpty(this.HtmlBodyTemplate))
                mail.AlternateViews.Add(BuildHtmlView(ApplyVariables(this.HtmlBodyTemplate, this.Variables, true), this.ImageAttachments));

            // ensure that the mail body is set to true to render the htmlalternateview first
            mail.IsBodyHtml = true;

            return mail;
        }
    }
}
