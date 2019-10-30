# Simple Html/Text Emailer
The Simple HTML/Text Emailer is a simple template email generator.  It simplifies the work necessary to send out emails using templates in .Net.

# Features
- Supports both HTML and text email templates (multiple view email)
- Variable substitutions by email type (HTML, text or both)
- Embedded images into HTML email templates

# Use Cases
The original code here was written for a custom eCommerce site.  The primary use case was to send out order confirmation emails.  These included images of the products ordered, and other dynamic text content such as customer name, etc.  We also substituted text depending on the user language preference.

# How To Use
There are just a few steps to use the emailer with a template
1) Create an HTML and/or text email template
1) Write code



## Create HTML Template

Templates can be simple or elaborate depending on what you need to do.  In the TestFiles folder, there's an example HTML email template.  For the example, we'll keep it very simple.
``` html
<html>
  <head></head>
  <body>
    <h1>Hello __CUSTOMERNAME__</h1>
    <p>Here is a cool image for you!</p>
    <p><img src="cid:someimagecidname" /></p>
    <p>click <a href="__CLICKMEURL__">here</a> for more.</p>
  </body>
</html>
```

Text-based emails are much easier.  They are optional since most folks don't want text emails anymore.
```
Hello __CUSTOMERNAME__
You won't see an image, but we have a url for you!
__CLICKMEURL__
```


## Write Code
When integrating this into your code, there are a few things you need to know. 
1) Substituting Variables
1) Adding Images
1) Adding "to" email addresses
1) Adding a sender and from address
1) Adding a subject line
1) Applying the email template(s)
1) Sending an email

### Working Example
There are good examples in the test cases here:
[Test Case](https://github.com/gord888/HtmlEmailer/blob/master/ThirteenSides/Common/ThirteenSides.Common.Tests/EmailTests.cs)




### Substituting Variables
Substituting variables is a straight find/replace.  You should take care to use unique variable names for substitution.  What generally worked for me is something like __SOMEVAR__. 

For the above example:
```csharp
 HtmlEmailer emailer = new HtmlEmailer();
 emailer.AddVariable("__CUSTOMERNAME__", "John Doe", SubstitutionVariable.VariableType.Both);
```
The **SubstitutionVariable.VariableType** allows you to specify if this variable substitution is for HTML, text, or both templates.

for example:
```csharp
emailer.AddVariable("__CLICKMEURL__", "http://google.ca", SubstitutionVariable.VariableType.Text);
emailer.AddVariable("__CLICKMEURL__", "http://bing.com", SubstitutionVariable.VariableType.Html);
```
The text-based email template will get a different URL than the HTML based one.  Generally speaking you'll probably always use "both".


### Adding Images
Make sure your HTML image src is set for cid.  If your template looks like this:
```html
<img src="cid:someimagecidname" />
```
then your code will be this:
```csharp
// adding an image file.  the registerbutton matches the cid:someimagecidname of the image source.
emailer.AddImageAttachment("./TestFiles/registerbutton.png", "image/png", "someimagecidname");
```

### Adding to Email addresses
```csharp
emailer.ToEmails.Add("yourself@gmail.com");
emailer.ToEmails.Add("yourself2@gmail.com");
```

### Adding sender/from Email Addresses
```csharp
emailer.Sender = "user@host.net";
emailer.FromAddress = "user@host.net";
```

### Adding a Subject
```csharp
emailer.Subject = "hello world";
```

### Applying Email Template(s)
```csharp
emailer.HtmlBodyTemplate = "<html>  <head></head>  <body>    <h1>Hello __CUSTOMERNAME__</h1>    <p>Here is a cool image for you!</p>    <p><img src=\"cid:someimagecidname\" /></p>    <p>click <a href=\"__CLICKMEURL__\">here</a> for more.</p>  </body></html>";
emailer.TextBodyTemplate = "Hello __CUSTOMERNAME__ You won't see an image, but we have a url for you! __CLICKMEURL__";
```

### Sending an Email
```csharp
SmtpClient smtpClient = new SmtpClient();
smtpClient.Host = "smtp.gmail.com";
smtpClient.Port = 587;
	
//smtpClient.UseDefaultCredentials = true;

smtpClient.UseDefaultCredentials = false;
smtpClient.Credentials = new System.Net.NetworkCredential("*****@gmail.com", "******");

smtpClient.EnableSsl = true;

// if you're using gmail and you get an authentication error message.  you may need to "allow less secure apps" on your google account.
smtpClient.Send(mail);
```
