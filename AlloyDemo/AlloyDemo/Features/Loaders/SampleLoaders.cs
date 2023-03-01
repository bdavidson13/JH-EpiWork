using AlloyDemo.Managers;
using AlloyDemo.Profiles;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DeaneBarker.Optimizely.ProfileVisitorGroups.TestingCode
{
    public interface IpopulateProfile
    {
        void PopulateProfile();
    }
    public class AIQPopulateProfile : IpopulateProfile
    {

        public void PopulateProfile()
        {
            throw new NotImplementedException();
        }
    }
    public static class SampleLoaders
    {
        // Some external system
        public static void PopulateProfileFromExternalSource_One(IProfile profile)
        {
            profile["first_name"] = "Deane";
            profile["last_name"] = "Barker";
            profile["dob"] = "1971-09-03";
            profile["annual_salary"] = "10000";
            profile["state"] = "SD";
            profile["country"] = "USA";
            profile["last_visited"] = "2022-12-01";
        }

        // Some really slow external system
        public static void PopulateProfileFromExternalSource_Two(IProfile profile)
        {
            // Simulates a long-running operation
            // This will come back and update the profile later
            // Visitor Group assignment will adjust in real-time
            Task.Run(() =>
            {
                Thread.Sleep(10000);

                var data = new Dictionary<string, string>
                {
                    ["dogs_name"] = "Lavallette"
                };

                var profileManager = ServiceLocator.Current.GetInstance<IProfileManager>();
                profileManager.Update(profile.Id, data);                
            });
        }

        public static void PopulateProfileFromFakeWebServiceCall(IProfile profile)
        {
            // What profile type are we used?
            if (ServiceLocator.Current.GetInstance<IProfile>() is JsonProfile)
            {
                ((JsonProfile)profile).LoadJson(readFile("json"));
            }
            else
            {
                ((XmlProfile)profile).LoadXml(readFile("xml"));
            }

            string readFile(string extension)
            {
                return File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"App_Data/Profiles/{profile.Id}.{extension}"));
            }
        }
    }
}
