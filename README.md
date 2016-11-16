# EDUGraphAPI - Office 365 Education Code Sample#

## What is EDUGraphAPI?##

EDUGraphAPI  is a sample that demonstrates:

* Link local users and Office 365 users. The below 2 kinds of Graph APIs will be used:

  * [Microsoft Azure Active Directory Graph API](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/)
  * [Microsoft Graph API](https://www.nuget.org/packages/Microsoft.Graph/)

  After link, users could use either local or Office 365 account to log into the sample web site and use the functions as usually.

* Get and show schools/sections/teachers/students from Office 365 Education:

  * [Office 365 Schools REST API reference](https://msdn.microsoft.com/office/office365/api/school-rest-operations)

  [Differential query](https://msdn.microsoft.com/en-us/library/azure/ad/graph/howto/azure-ad-graph-api-differential-query) will be used to sync data that are cached in local database.

EDUGraphAPI is based on an ASP.NET MVC. [ASP.NET Identity](https://www.asp.net/identity) is used in this project.

## Documentation



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

4. Copy aside the **Key**. 

   ![](Images/bing-maps-key.png)

> **Note:** The key will be used in a subsequent step.

**Create an Application in you AAD**

1. Sign into [https://manage.windowsazure.com](https://manage.windowsazure.com).

2. Open the AAD in which you plan to create the application.

3. Click **ADD** on the bottom bar.

   ![](Images/aad-applications.png)

4. Click **Add an application my organization is developing**.

   ![](Images/aad-create-app-01.png)

5. Input a **Name**, and select **WEB APPLICATION AND/OR WEB API**. 

   ![](Images/aad-create-app-02.png)

   Click **->**.


5. Input the values:

   * SIGN-ON URL: https://localhost:44311/

   * APP ID URI: https://<<YOUR TENANT>>/EDUGraphAPI

     > **Note**: A domain from you tenant must be used here, since we are going to create a multi-tenant application.

   ![](Images/aad-create-app-03.png)

   Click the **✓**.

6. Click **CONFIGURE**.

   ![](Images/aad-configure-app-01.png)

7. Enable **APPLICATION IS MULTI-TENANT**.

   ![](Images/aad-configure-app-02.png)

8. Configure **permissions to other applications** as below.

   |                                | Application Permissions       | Delegated Permissions                    |
   | ------------------------------ | ----------------------------- | ---------------------------------------- |
   | Windows Azure Active Directory | Read and write directory data | Sign in and read user profile<br>Read and write directory data |
   | Microsoft Graph                | *None*                        | Read all groups<br>Read directory data<br>Access directory as the signed in user<br>Sign user in |

9. Create a key

   ![](Images/aad-configure-app-03.png)


   Click **Save**.

10. Copy the client id and section key aside.

   ![](Images/aad-configure-app-04.png)

**Deploy the Azure Components**

1. Check to ensure that the build is passing VSTS Build

2. Fork this repository to your GitHub account

3. Click the Deploy to Azure Button

   [![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3a%2f%2fraw.githubusercontent.com%2fTylerLu%2fEDUGraphAPI%2fmaster%2fazuredeploy.json)

4. Fill in the values in the deployment page.

   ![](Images/azure-auto-deploy.png)

5. Click **Purchase**.

### Authentication flows

In this sample, we allow users to login in with either a local account or an O365 account. Then connect to the other account.

**User Login Local**

To experience this sample, you should register a new user first. After register, the app will log you in automaticlly.

....

**Multi-tenant application**

admin consent

**Authentication Helper**



**Microsoft Azure Active Directory Graph API**

Get current user

Get current tenant

**Microsoft Graph API**

Get current user

Get current tenant



### Office 365 Education Data

**EducationServiceClient**



**Get data**

1. Get schools

   Get All schools

2. Get sections



**Differential query**



## How To: Configure your Development Environment









## Contributors
| Roles                                    | Author(s)                                |
| ---------------------------------------- | ---------------------------------------- |
| Project Lead / Architect / Documentation | Todd Baginski (Microsoft MVP, Canviz Consulting) @tbag |
| PM                                       | John Trivedi (Canviz Consulting)         |
| PM                                       | Arthur Zheng (Canviz Consulting)         |
| Developer                                | Tyler Lu (Canviz Consulting) @TylerLu    |
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
