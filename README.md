# ManageCloudServices

This sample shows how to manage Azure classic resources authenticating with Management Certificates and using Microsoft Azure Management Libraries.

## Getting Started with the sample solution

1. Create a [Management Certificate](https://docs.microsoft.com/en-us/azure/cloud-services/cloud-services-certs-create#what-are-management-certificates)
2. First, download the solution, either directly or by cloning the repo.
3. Run Visual Studio as **Administrator**
4. Open the solution ManageCloudServices
5. In Visual Studio, go to Tools -> Nuget Package Manager -> Package Manager Console.
6. Run the following in the Package Manager Console:
```
Install-Package Microsoft.WindowsAzure.Management.Libraries -Version 2.0.0
```
7. Specify the Azure subscription Id
8. Specify the Management Certificate thumbprint
9. Specify a Cloud Service name as per the sample