# EDUGraphAPI - Office 365 Education Code Sample#

[TOC]

## What is EDUGraphAPI?##

EDUGraphAPI  is a sample that demonstrates:

* Linking local user accounts and Office 365 user accounts. 
* Calling Graph APIs, including:

  * [Microsoft Azure Active Directory Graph API](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/)
  * [Microsoft Graph API](https://www.nuget.org/packages/Microsoft.Graph/)

  After linking accounts, users can use either local or Office 365 accounts to log into the sample web site and use it.

* Get and show schools/sections/teachers/students from Office 365 Education:

  * [Office 365 Schools REST API reference](https://msdn.microsoft.com/office/office365/api/school-rest-operations)

  A [Differential query](https://msdn.microsoft.com/en-us/library/azure/ad/graph/howto/azure-ad-graph-api-differential-query) is used to sync data that is cached in a local database.

EDUGraphAPI is based on ASP.NET MVC. [ASP.NET Identity](https://www.asp.net/identity) is used in this project.

## How To: Configure your Development Environment

Download and install the following tools to run, build and/or develop this application locally.

- [Visual Studio 2015 Community](https://go.microsoft.com/fwlink/?LinkId=691978&clcid=0x409)

**GitHub Authorization**

1. Generate Token

   - Open https://github.com/settings/tokens in your web browser.
   - Sign into your GitHub account where you forked this repository.
   - Click **Generate Token**
   - Enter a value in the **Token description** text box
   - Select all the **check boxes**

   ![](Images/github-new-personal-access-token.png)

   - Click **Generate token**
   - Copy the token

2. Add the GitHub Token to Azure in the Azure Resource Explorer

   - Open https://resources.azure.com/providers/Microsoft.Web/sourcecontrols/GitHub in your web browser.
   - Log in with your Azure account.
   - Selected the correct Azure subscription.
   - Select **Read/Write** mode.
   - Click **Edit**.
   - Paste the token into the **token parameter**.

   ![](Images/update-github-token-in-azure-resource-explorer.png)

   - Click **PUT**

**Create a key to use the Bing Maps**

1. Open [https://www.bingmapsportal.com/](https://www.bingmapsportal.com/) in your web browser and sign in.

2. Click  **My account** -> **My keys**.

3. Create a **Basic** key, select **Public website** as the application type.

4. Copy the **Key** and save it. 

   ![](Images/bing-maps-key.png)

   >**Note:** The key is used in a subsequent step.

**Create an Application in you AAD**

1. Sign into the traditional azure portal: [https://manage.windowsazure.com](https://manage.windowsazure.com).

2. Open the AAD where you plan to create the application.

3. Click **ADD** on the bottom bar.

   ![](Images/aad-applications.png)

4. Click **Add an application my organization is developing**.

   ![](Images/aad-create-app-01.png)

5. Input a **Name**, and select **WEB APPLICATION AND/OR WEB API**. 

   ![](Images/aad-create-app-02.png)

6. Click **➔**.


7. Enter the following values:

   * **SIGN-ON URL:** https://localhost:44311/

   * **APP ID URI:** https://<<YOUR TENANT>>/EDUGraphAPI

   >**Note**: A domain from your tenant must be used here, since this is a multi-tenant application.

   ![](Images/aad-create-app-03.png)

8. Click the **✓**.

9. Click **CONFIGURE**.

   ![](Images/aad-configure-app-01.png)

10. Enable **APPLICATION IS MULTI-TENANT**.

   ![](Images/aad-configure-app-02.png)

11. Configure the following **permissions to other applications**.

|                                | Application Permissions       | Delegated Permissions                    |
| ------------------------------ | ----------------------------- | ---------------------------------------- |
| Windows Azure Active Directory | Read and write directory data | Sign in and read user profile<br>Read and write directory data |
| Microsoft Graph                | *None*                        | Read all groups<br>Read directory data<br>Access directory as the signed in user<br>Sign user in |

12. In the keys section, click the dropdown list and select a duration, then click **Save**.

   ![](Images/aad-configure-app-03.png)

13. Copy aside the Client ID and the key value.

   ![](Images/aad-configure-app-04.png)


**Deploy the Azure Components**

1. Check to ensure that the build is passing VSTS Build

2. Fork this repository to your GitHub account

3. Click the Deploy to Azure Button

   [![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3a%2f%2fraw.githubusercontent.com%2fTylerLu%2fEDUGraphAPI%2fmaster%2fazuredeploy.json)

4. Fill in the values in the deployment page and select the **I agree to the terms and conditions stated above** checkbox.

   ![](Images/azure-auto-deploy.png)

   * **Resource group**: we suggest you to Create a new group.

   * **Site Name**: please input a name. Like EDUGraphAPICanviz or EDUGraphAPI993.

     > Note: If the name you input is taken, you will get some validation errors:
     >
     > ![](Images/azure-auto-deploy-validation-errors-01.png)
     >
     > Click it you will get more details, like storage account is already in other resource group/subscription.
     >
     > In this case, please use another name.

   * **Sql Administrator Login Password**: please DO use a strong password.

   * **Source Code Repository URL**: replace <YOUR REPOSITORY> with the repository name of your fork.

   * **Source Code Manual Integration**: choose **false**, since you are deploying from your own fork.

   * **Client Id**: use the Client Id of the AAD Application your created earlier.

   * **Client Secret**: use the Key value of the AAD Application your created earlier.

   * **Bing Map Key**: use the key of Bing Map you got earlier.

   * Check **I agree to the terms and conditions stated above**.

5. Click **Purchase**.

**Add REPLY URL to the AAD Application**

1. After the deployment, open the resource group:

   ![](Images/azure-resource-group.png)

2. Click the web app.

   ![](Images/azure-web-app.png)

   Copy the URL aside and change the schema to **https**. This is the replay URL and will be used in next step.

3. Navigate to the AAD application in the traditional azure portal, then click the **Configure** tab.

   Add the reply URL:

   ![](Images/aad-add-reply-url.png)

4. Click **SAVE**.

## Documentation

### Introduction

**EDUGraphAPI.Web**

<<<<<<< HEAD
This web project is based on a ASP.NET MVC application with the **Individual User Accounts** selected. 
=======
This web project is based on an ASP.NET MVC application with the **Individual User Accounts** selected. 
>>>>>>> origin/master

![](Images/mvc-auth-individual-user-accounts.png)

The following files were created by the MVC template, and we only change them a little bit.

1. **/App_Start/Startup.Auth.Identity.cs** (The original name is Startup.Auth.cs)
2. **/Controllers/AccountController.cs**

This sample project uses [ASP.NET Identity](https://www.asp.net/identity) and [Owin](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/owin). The 2 technologies make different kinds of authentication coexist easily. Please get familiar them first.

Below are the important class files used in this web project:

| File                              | Description                              |
| --------------------------------- | ---------------------------------------- |
| /App_Start/Startup.Auth.AAD.cs    | Integrates with Azure Active Directory authentication |
| /Controllers/AdminController.cs   | Contains the administrative actions      |
| /Controllers/LinkController.cs    | Contains the actions to link user accounts |
| /Controllers/SchoolsController.cs | Contains the actions to show school data |

**EDUGraphAPI.SyncData**

This is the WebJob used to sync user data.

**EDUGraphAPI.Common**

The class library project is used both the **EDUGraphAPI.Web** and **EDUGraphAPI.Common**. 

The table below shows the folders in the project:

| Folder             | Description                              |
| ------------------ | ---------------------------------------- |
| /Data              | Contains ApplicationDbContext and entity classes |
| /DataSync          | Contains the UserSyncSerextensionsvice class which is used in the EDUGraphAPI.SyncData WebJob |
| /DifferentialQuery | Contains the DifferentialQueryService class which is used to send differential query and parse the result. |
| /Extensions        | Contains lots of extension methods which simplify coding the make code easy to read |
| /Utils             | Contains the wide used class AuthenticationHelper.cs |

**Microsoft.Education**

This project encapsulates the [Schools REST API](https://msdn.microsoft.com/en-us/office/office365/api/school-rest-operations). The most important class in this project is EducationServiceClient.

### Data Access and Data Models

ASP.NET Identity uses Entity Framework Code First to implement all of its persistence mechanism. Package [Microsoft.AspNet.Identity.EntityFramework](https://www.nuget.org/packages/Microsoft.AspNet.Identity.EntityFramework/) is created for this. 
<<<<<<< HEAD

In this sample **ApplicationDbContext** is created for access data from a SQL Database. It inherited from **IdentityDbContext** which is defined in the NuGet package mentioned above.

Below are the important Data Models (and their important properties) and used in this sample:

=======

In this sample **ApplicationDbContext** is created for access data from a SQL Database. It inherited from **IdentityDbContext** which is defined in the NuGet package mentioned above.

Below are the important Data Models (and their important properties) and used in this sample:

>>>>>>> origin/master
**ApplicationUsers**

Inherited from **IdentityUser**. 

| Property      | Description                              |
| ------------- | ---------------------------------------- |
| Organization  | The tenant of the user. For local unlinked user, its value is null |
| O365UserId    | Used to connection with an Office 365 account |
| O365Email     | The Email of the linked Office 365 account |
| JobTitle      | Used for demonstrating differential query |
| Department    | Used for demonstrating differential query |
| Mobile        | Used for demonstrating differential query |
| FavoriteColor | Used for demonstrating local data        |

**Organizations**

A row in this table represents a tenant in AAD.

| Property         | Description                          |
| ---------------- | ------------------------------------ |
| TenantId         | Guid of the tenant                   |
| Name             | Name of the tenant                   |
| IsAdminConsented | Is the tenant consented by any admin |

### Authentication flows

There are 4 authentication flows in this project.

The first 2 flows enable users to login in with either a local account or an Office 365 account, then link to the other account.

**Local Login Authentication Flow**

![](Images/auth-flow-local-login.png)

**O365 Login Authentication Flow**

![](Images/auth-flow-o365-login.png)

**Admin Login Authentication Flow**

![](Images/auth-flow-admin-login.png)

**Application Authentication Flow**

![](Images/auth-flow-app-login.png)

### Two Kinds of Graph API

There are two kinds of Graph API:

<<<<<<< HEAD
|              | [Azure AD Graph API](https://msdn.microsoft.com/en-us/library/azure/ad/graphInstall-Package) | [Microsoft Graph API]([https://graph.microsoft.io/](https://graph.microsoft.io/)) |
=======
|              | [Azure AD Graph API](https://msdn.microsoft.com/en-us/library/azure/ad/graphInstall-Package) | [Microsoft Graph API]([https://graph.microsoft.io/) |
>>>>>>> origin/master
| ------------ | ---------------------------------------- | ---------------------------------------- |
| Description  | The Azure Active Directory Graph API provides programmatic access to Azure Active Directory through REST API endpoints. Apps can use the Azure AD Graph API to perform create, read, update, and delete (CRUD) operations on directory data and directory objects, such as users, groups, and organizational contacts | A unified API that also includes APIs from other Microsoft services like Outlook, OneDrive, OneNote, Planner, and Office Graph, all accessed through a single endpoint with a single access token. |
| Client       | Install-Package [Microsoft.Azure.ActiveDirectory.GraphClient](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/) | Install-Package [Microsoft.Graph](https://www.nuget.org/packages/Microsoft.Graph/) |
| End Point    | https://graph.windows.net                | https://graph.microsoft.com              |
| API Explorer | https://graphexplorer.cloudapp.net/      | https://graph.microsoft.io/graph-explorer |

In this sample, we use the hierarchy below to demonstrate how to use them. 

![](Images/class-diagram-graphs.png)

The **IGraphClient** interface defines two method: GeCurrentUserAsync and GetTenantAsync.

**AADGraphClient** and **MSGraphClient** implement the **IGraphClient** interface with Azure AD Graph and Microsoft Graph client libraries separately.

The interface and the two classes resides in /Services/GraphClients folder of the web app. Some of code is list below to show how to get user and tenant with the 2 kinds of Graph APIs.

**Azure AD Graph** - AADGraphClient.cs

~~~c#
public async Task<UserInfo> GetCurrentUserAsync()
{
    var me = await activeDirectoryClient.Me.ExecuteAsync();
    return new UserInfo
    {
        Id = me.ObjectId,
        GivenName = me.GivenName,
        Surname = me.Surname,
        UserPrincipalName = me.UserPrincipalName,
        Roles = await GetRolesAsync(me)
    };
}
~~~

~~~c#
public async Task<TenantInfo> GetTenantAsync(string tenantId)
{
    var tenant = await activeDirectoryClient.TenantDetails
        .Where(i => i.ObjectId == tenantId)
        .ExecuteSingleAsync();
    return new TenantInfo
    {
        Id = tenant.ObjectId,
        Name = tenant.DisplayName
    };
}

~~~

**Microsoft Graph** - MSGraphClient.cs

~~~c#
public async Task<UserInfo> GetCurrentUserAsync()
{
    var me = await graphServiceClient.Me.Request()
        .Select("id,givenName,surname,userPrincipalName,assignedLicenses")
        .GetAsync();
    return new UserInfo
    {
        Id = me.Id,
        GivenName = me.GivenName,
        Surname = me.Surname,
        UserPrincipalName = me.UserPrincipalName,
        Roles = await GetRolesAsync(me)
    };
}

~~~

~~~c#
public async Task<TenantInfo> GetTenantAsync(string tenantId)
{
    var tenant = await graphServiceClient.Organization[tenantId].Request().GetAsync();
    return new TenantInfo
    {
        Id = tenant.Id,
        Name = tenant.DisplayName
    };
}

~~~

In AAD Application, you should configure permissions for them separately:

![](Images/aad-app-permissions.png) 

### Office 365 Education Data

**EducationServiceClient**

[Office 365 Education APIs](https://msdn.microsoft.com/office/office365/api/school-rest-operations) help extract data from your Office 365 tenant which has been synced to the cloud by Microsoft School Data Sync. These results provide information about schools, sections, teachers, students and rosters. The Schools REST API provides access to school entities in Office 365 for Education tenants.

**Get data**

1. Get schools

   You can get all schools, get a single school by its object_id, or get a collection of schools that match a set of query filters.

   Get All schools

   [Get all schools](https://msdn.microsoft.com/office/office365/api/school-rest-operations#get-all-schools) that exist in the Azure Active Directory tenant.

2. Get sections

   You can [get sections](https://msdn.microsoft.com/office/office365/api/school-rest-operations#get-sections-within-a-school) for a specific school by querying for groups based on their school id, using the extension_fe2174665583431c953114ff7268b7b3_Education_ObjectType attribute and the extension_fe2174665583431c953114ff7268b7b3_Education_SyncSource_SchoolId attribute together in the query.

3. Get students within a section

   Students are represented in Azure Active Directory as users. User Extension Attributes add student-specific information. For example, the extension_fe2174665583431c953114ff7268b7b3_Education_Grade attribute contains the student's grade level.

   You can [get students in a specific section](https://msdn.microsoft.com/office/office365/api/section-rest-operations#get-students-within-a-section), by getting the members of the section’s unified group and filtering out non-student users from the resulting collection within your application.


### Differential query

[A differential query](https://msdn.microsoft.com/en-us/Library/Azure/Ad/Graph/howto/azure-ad-graph-api-differential-query) request returns all changes made to specified entities during the time between two consecutive requests. For example, if you make a differential query request an hour after the previous differential query request, only the changes made during that hour will be returned. This functionality is especially useful when synchronizing tenant directory data with an application’s data store.

To make a differential query request to a tenant’s directory, your application must be authorized by the tenant. For more information, see [Integrating Applications with Azure Active Directory](https://azure.microsoft.com/en-us/documentation/articles/active-directory-integrating-applications/).

### Other

Multi-tenant application ...





## Contributors
| Roles                                    | Author(s)                                |
| ---------------------------------------- | ---------------------------------------- |
| Project Lead / Architect / Documentation | Todd Baginski (Microsoft MVP, Canviz Consulting) @tbag |
| PM                                       | John Trivedi (Canviz Consulting)         |
| Dev Leader                               | Tyler Lu (Canviz Consulting) @TylerLu    |
| Developer                                | Benny Zhang (Canviz Consulting)          |
| Testing                                  | Ring Li (Canviz Consulting)              |
| Testing                                  | Melody She (Canviz Consulting)           |
| UX Design                                | Justin So (Canviz Consulting)            |
| Sponsor / Support                        |                                          |
| Sponsor / Support                        |                                          |
| Sponsor / Support                        |                                          |
| Sponsor / Support                        |                                          |

## Version history

| Version | Date         | Comments        |
| ------- | ------------ | --------------- |
| 1.0     | Nov 26, 2016 | Initial release |

## Disclaimer
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**
