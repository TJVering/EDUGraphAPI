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
