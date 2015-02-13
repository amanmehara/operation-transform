using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Popups;

namespace Operation_Transform.Common
{
    public class RatingHelper
    {
        public static async void RateAndReview()
        {
            if (!Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("number"))
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["number"] = 0;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["isReviewed"] = false;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["feedbackGiven"] = false;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["feedbackRejected"] = false;
            }
            else
            {
                int number = (int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["number"];
                bool isReviewed = (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["isReviewed"];
                bool feedbackGiven = (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["feedbackGiven"];
                bool feedbackRejected = (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["feedbackRejected"];

                if (!isReviewed)
                {
                    ++number;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["number"] = number;

                    if (number % 10 == 0)
                    {
                        MessageDialog reviewDialog = new MessageDialog("We'd love you to rate our app 5 stars. \n\nShowing us some love on the store helps us to continue to work on the app and make things even better!", "Enjoying Programming!");

                        reviewDialog.Commands.Add(new UICommand("Rate 5 Stars :)", async (reviewCommand) =>
                        {
                            Windows.Storage.ApplicationData.Current.LocalSettings.Values["isReviewed"] = true;

                            string appid = "aeb15bd5-cb59-44db-af83-53e98f992b63";
                            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + appid));
                        }));

                        reviewDialog.Commands.Add(new UICommand("Later", async (reviewCommand) =>
                        {
                            if (!feedbackGiven && !feedbackRejected)
                            {
                                var easClientDeviceInformation = new EasClientDeviceInformation();

                                MessageDialog feedbackDialog = new MessageDialog("Sorry to hear you didn't want to rate Programming! :'( \n\nTell us about your experience or suggest how can we make it even better. :)", "Can we make it better?");

                                feedbackDialog.Commands.Add(new UICommand("Give Feedback", async (feedbackCommand) =>
                                {
                                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["feedbackGiven"] = true;

                                    EmailRecipient amanm = new EmailRecipient()
                                    {
                                        Name = "Aman Mehara",
                                        Address = "amanmehara@hotmail.com"
                                    };

                                    EmailMessage feedbackEmail = new EmailMessage();

                                    string version = "2.3.1.2";


                                    feedbackEmail.To.Add(amanm);
                                    feedbackEmail.Subject = "Programming! Customer Feedback";
                                    feedbackEmail.Body = String.Format("\n[Your Feedback Here] \n\n--------------------------------\nDevice Details.. \nName: {0} \nManufacturer: {1} \nFirmware Version: {2} \nHardware Version: {3} \nApplication Version: {4} \n--------------------------------\n\nDisclaimer.. \nThis email exchange is governed by Programming!'s Privacy Policy and T&C. \n",
                                        easClientDeviceInformation.SystemProductName,
                                        easClientDeviceInformation.SystemManufacturer,
                                        easClientDeviceInformation.SystemFirmwareVersion,
                                        easClientDeviceInformation.SystemHardwareVersion,
                                        version);

                                    await EmailManager.ShowComposeNewEmailAsync(feedbackEmail);
                                }));

                                feedbackDialog.Commands.Add(new UICommand("No Thanks", (feedbackCommand) =>
                                {
                                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["feedbackRejected"] = true;
                                }));


                                feedbackDialog.DefaultCommandIndex = 0;

                                await feedbackDialog.ShowAsync();

                            }

                        }));

                        reviewDialog.DefaultCommandIndex = 0;

                        await reviewDialog.ShowAsync();
                    }
                }
            }
        }
    }
}
