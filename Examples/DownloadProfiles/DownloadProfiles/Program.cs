using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Net.Pokeshot.JiveSdk.Models;

namespace DownloadProfiles
{
    class Program
    {
      
        static  void Main(string[] args)
        {
            getPeople("https://jiveinstance.jiveon.com/api/core/v3/people");

            Console.ReadLine();
        }

        static async void getPeople(string apiUrl)
        {
            try
            {


                //set up credentials for the API call
                //every call to the rest api needs to be authenticated separately. There are no sessions (this is a good thing)
                Console.WriteLine("Using " + apiUrl);
                string userName = "user";
                string password = "passsword";


                System.Net.Http.HttpClientHandler jiveHandler = new System.Net.Http.HttpClientHandler();


                NetworkCredential myCredentials = new NetworkCredential(userName, password);
                myCredentials.Domain = apiUrl;


                string cre = String.Format("{0}:{1}", userName, password);

                byte[] bytes = Encoding.UTF8.GetBytes(cre);

                string base64 = Convert.ToBase64String(bytes);

                jiveHandler.Credentials = myCredentials;
                jiveHandler.PreAuthenticate = true;
                jiveHandler.UseDefaultCredentials = true;

                //Setting up basic authentication and the Authorization header specifically

                System.Net.Http.HttpClient httpClient;

                httpClient = new System.Net.Http.HttpClient(jiveHandler);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64);


                //making the async call to the REST endpoint

                System.Net.Http.HttpResponseMessage activityResponse = await httpClient.GetAsync(String.Format(apiUrl));

                //Once we have received the payload back from the Jive server, we need to remove the string that Jive injects at the beginning
                //of each response to prevent cross site request forging
                String myActivityResponse = await activityResponse.Content.ReadAsStringAsync();
                string cleanResponseActivities = myActivityResponse.Replace("throw 'allowIllegalResourceCall is false.';", "");

                //uncomment this to see the response in the console
                //Console.WriteLine(cleanResponseActivities);


                //Using the Json.net library we are converting the payload into C# objects from the  .Net SDK
                //A PeopleList object holds an array of Person objects and pagination info in the links
                PeopleList myPeople = JsonConvert.DeserializeObject<PeopleList>(cleanResponseActivities);

                //Iterate over the list of Person objects from the PeopleList

                foreach (Person myPerson in myPeople.list)
                {
                    //Get the link to the persons images from the resources list of the person object
                    System.Net.Http.HttpResponseMessage imageReponse = await httpClient.GetAsync(myPerson.resources.images.@ref);

                    String myImagesReponse = await imageReponse.Content.ReadAsStringAsync();
                    string cleanImageResponse = myImagesReponse.Replace("throw 'allowIllegalResourceCall is false.';", "");
                    ImageList myImageList = JsonConvert.DeserializeObject<ImageList>(cleanImageResponse);

                    //Iterate over the list of images for the Person and save them to the local disk
                    foreach (Image myImage in myImageList.list)
                    {
                        Console.WriteLine(myImage.@ref);

                        byte[] data;
                        using (WebClient client = new WebClient())
                        {
                            client.Headers[HttpRequestHeader.Authorization] = "Basic " + base64;
                            data = client.DownloadData(myImage.@ref);
                        }
                        System.IO.File.WriteAllBytes(@"c:\images\" + myPerson.jive.username +"_" + myImage.index.ToString() + ".png", data);

                    }

                    
                }

                //if there are more person results retrieve them, otherwise end
                if (myPeople.links != null)
                {
                    if (myPeople.links.next != null)
                    {
                        getPeople(myPeople.links.next);
                    }
                    else
                    {
                        Console.ReadLine();
                    }

                }
                else
                {
                    Console.WriteLine("done");
                    Console.ReadLine();
                }
                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
