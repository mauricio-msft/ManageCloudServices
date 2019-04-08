///-------------------------------------------------------------------
///   Namespace:      ManageCloudServices
///   Class:          Program
///   Description:    Automate, deploy, and test cloud infrastructure
///   Author:         Full Stack Engineer Mauricio Arroyo
///   Date:           04/04/2019
///   Notes:          Main concept here is the use of Management Certs
///                   using Microsoft Azure Management Libraries 2.0.0
///-------------------------------------------------------------------

// Imports dependencies
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Compute;
using Microsoft.WindowsAzure.Management.Compute.Models;
using System.Security.Cryptography.X509Certificates;

// Declares the namespace
namespace ManageCloudServices
{
    /// <summary>
    /// Main class of the ManageCloudServices project.
    /// </summary>
    class Program
    {
        // Declares static variables
        public static string SubscriptionId { get; set; }
        public static string CertificateThumbprint { get; set; }
        public static X509Certificate2 Certificate { get; set; }

        // Entrypoint and main method
        static void Main(string[] args)
        {

            // The azure subscription id
            SubscriptionId = "{subscriptionId}";

            // Management Certificate thumbprint
            CertificateThumbprint = "{thumbprint}";

            // Creates credentials / Management operations require the X509Certificate2 type 
            CertificateCloudCredentials credential = new CertificateCloudCredentials(SubscriptionId, GetStoreCertificate(CertificateThumbprint));

            using (var computeClient = new ComputeManagementClient(credential))
            {
                //Gets a list of Cloud Services inside the Subscription
                var myCloudServices = computeClient.HostedServices.List();

                // Loops the the HostedServices object
                foreach (HostedServiceListResponse.HostedService cs in myCloudServices)
                {
                    // Prints every Cloud Service found
                    Console.WriteLine(cs.ServiceName);
                }

                // Gets a specific Cloud Service
                var result = computeClient.HostedServices.GetDetailed("{cloud-service-name}");
                var productionDeployment = result.Deployments.Where(d => d.DeploymentSlot == DeploymentSlot.Production).FirstOrDefault();

                // Prints a specific Cloud Service Details
                Console.WriteLine("Service Name: " + result.ServiceName);
                Console.WriteLine("Status: " + productionDeployment.Status);
                Console.WriteLine("Public IP address: " + productionDeployment.VirtualIPAddresses[0].Address);
                Console.WriteLine("Deployment name: " + productionDeployment.Name);
                Console.WriteLine("Deployment label: " + productionDeployment.Label);
                Console.WriteLine("Deployment ID: " + productionDeployment.PrivateId);

            }
            Console.WriteLine("press any key to exit...");
            Console.ReadKey();
        }

        // Find and return the Management Certificate from User and LocalMachine stores
        private static X509Certificate2 GetStoreCertificate(string thumbprint)
        {
            // Declares the store's locations
            List<StoreLocation> locations = new List<StoreLocation>
            {
                StoreLocation.CurrentUser,
                StoreLocation.LocalMachine
            };

            // Loops every Store location looking to match the certificate thumbprint
            foreach (var location in locations)
            {
                // Creates the store object
                X509Store store = new X509Store("My", location);
                try
                {
                    // Opens a store
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                    // Proceeds to find any match
                    X509Certificate2Collection certificates = store.Certificates.Find(
                      X509FindType.FindByThumbprint, thumbprint, false);

                    // Found the certificate
                    if (certificates.Count == 1)
                    {
                        // Returns it
                        return certificates[0];
                    }
                }
                // Always executes this block
                finally
                {
                    // Close the Certs store
                    store.Close();
                }
            }
            // Manages an excetion in case none certificate matched
            throw new ArgumentException(string.Format(
              "A Certificate with Thumbprint '{0}' could not be located.",
              thumbprint));
        }
    }
}