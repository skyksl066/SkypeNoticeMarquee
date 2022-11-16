using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Lync.Model;

namespace SkypeMarquee
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lyncClient = LyncClient.GetClient();
            Contact self = lyncClient.Self.Contact;
            string note = (string)self.GetContactInformation(ContactInformationType.PersonalNote);
            marquee(lyncClient, note ?? $"Current time: {DateTime.Now:yyyy/MM/dd HH:mm:ss}                  ");
            //StatusChange(lyncClient, self);
            Console.ReadKey();
        }

        static void marquee(LyncClient lyncClient, string text)
        {
            if (lyncClient.State == ClientState.SignedIn)
            {
                while (true)
                {
                    string newst = text.Substring(0, 1); //第一個字
                    text = text.Substring(1, text.Length - 1) + newst;
                    Dictionary<PublishableContactInformationType, object> status = new Dictionary<PublishableContactInformationType, object>();
                    status.Add(PublishableContactInformationType.PersonalNote, text);
                    lyncClient.Self.BeginPublishContactInformation(status, null, null);
                    Thread.Sleep(300);
                }
            }
        }

        static void StatusChange(LyncClient lyncClient, Contact self)
        {
            if (lyncClient.State == ClientState.SignedIn)
            {
                while (true)
                {
                    Dictionary<PublishableContactInformationType, object> status = new Dictionary<PublishableContactInformationType, object>();
                    status.Add(PublishableContactInformationType.PersonalNote, "9998");
                    if (self.GetContactInformation(ContactInformationType.Availability).ToString() == ((int)ContactAvailability.Busy).ToString())
                    {
                        Console.WriteLine("Free");
                        status.Add(PublishableContactInformationType.Availability, ContactAvailability.Free);
                    }
                    else if (self.GetContactInformation(ContactInformationType.Availability).ToString() == ((int)ContactAvailability.Free).ToString())
                    {

                        Console.WriteLine("Away");
                        status.Add(PublishableContactInformationType.Availability, ContactAvailability.Away);
                    }
                    else if (self.GetContactInformation(ContactInformationType.Availability).ToString() == ((int)ContactAvailability.Away).ToString())
                    {
                        Console.WriteLine("Busy");
                        status.Add(PublishableContactInformationType.Availability, ContactAvailability.Busy);
                    }
                    lyncClient.Self.BeginPublishContactInformation(status, null, null);
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
