using System.Net.Mail;
using System.Net;
using Group_11.DAL;
using System;
using System.Linq;

namespace Group_11.Utilities
{
    public static class EmailMessaging
    {
        public static void SendEmail(String emailSubject, String emailBody, String recipientAddress)
        {
            // ***** Contants
            //Create a variable for YOUR TEAM'S Email address
            //This is the address that will be SENDING the emails (the FROM address)
            String strFromEmailAddress = "eq998@utexas.edu";

            //This is the app password for YOUR TEAM'S "fake" Gmail account
            //An app password is DIFFERENT than the password you set up when you created the account
            //You have to enable 2-factor authentication for the account, and then
            //set up the App Password (go into Account Settings and search for App Password)
            //NOTE: PLEASE DO NOT SHARE THIS; I will be deleting this after the project
            String strPassword = "dxbx txoq yhtw slvz";

            //This is the name of the business from which you are sending
            String strCompanyName = "UT Recruiting";
            // **************************************************************************************


            //Create an email client to send the emails
            //https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient?view=net-7.0
            //SmtpClient constructor takes two parameters: smtp server, and port #
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                //This is the SENDING email address and password
                //This will be your team's email address and password
                Credentials = new NetworkCredential(strFromEmailAddress, strPassword),
                EnableSsl = true
            };


            // update message with closing lines
            String finalMessage = emailBody + "\n\nHook 'Em!\n\n- UT Recruiting";

            //Create a new mail message
            //https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.mailmessage?view=net-7.0
            //has many properties, but you can just use subject, sender, from, to and body
            MailMessage mm = new MailMessage();

            //Set the subject line of the message (including your team number)
            mm.Subject = "Team 11 - " + emailSubject;

            //Create an email address object for the sender address
            //MaleAddress constructor takes 2 parameters: the FROM email, and the Display name for the email
            MailAddress senderEmail = new MailAddress(strFromEmailAddress, strCompanyName);

            //Set the sender address
            mm.Sender = senderEmail;

            //Set the from address
            mm.From = senderEmail;

            //Add the recipient (passed in as a parameter) to the list of people receiving the email
            //NOTE: EMAILS In DATABASE FOR USERS DO NOT EXIST 
            mm.To.Add(new MailAddress(recipientAddress));

            //Add the message (passed)
            mm.Body = finalMessage;

            //Send bundles up all properties from MailMessage and sends using Smtp parameters
            //send the message!
            client.Send(mm);
        }
    }
}
