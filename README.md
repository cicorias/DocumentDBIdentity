# ASP.NET Identity Provider for Microsoft Azure DocumentDB #

----------
## Overview ##
The DocumentDBIdentity implementation is for the [ASP.NET Identity framework](http://www.asp.net/identity "ASP.NET Identity") as part of MVC5+..

This is a early release with just the Registration code path done.

## What is Microsoft Azure Document DB ##
You can find more about Azure DocumentDB [here](http://azure.microsoft.com/en-us/documentation/services/documentdb/ "Azure DocumentDB").  However, in short, Azure DocumentDB is a schema-less NoSQL implementation of a ["document" database](http://en.wikipedia.org/wiki/Document-oriented_database "Wikipedia document db")

## What is the ASP.NET Identity system ##
You can find out more [here](http://www.asp.net/identity "ASP.NET Identity").  However, the ASP.NET Identity system is a framework for user membership and profiles, among many other features that modern web applications desire. 
2 Factor authentication
Authentication against multiple logon providers (Facebook, Google, LinkedIn, OAuth2 Sign-on)
Recover self service

## Installing ##

### Basic Project Creation ###
1. First, create your ASP.NET project using the ASP.NET Web Project template in Visual Studio
1. Choose 'MVC' and ensure the Authentication is set to **Individual User Accounts**
### Update Project Files with "Identity" class files ###
Once you've created your Web Application project, we just need to mark 2 files in the existing project, then copy over 2 files from this solution.
NOTE: this WILL be done via Nuget packaging in the near future.
Mark as "None" for Compile Action
![](./docs/images/markNoCompile.png)



