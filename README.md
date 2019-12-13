# SendGrid.Templates migration tool
A simple tool to migrate [SendGrid](//sendgrid.com) templates between accounts (or subusers)
by using their api keys.

## Utilization
```
$ sendgrid --srctoken <<source-api-token>> --dsttoken <<destination-api-token>>
```

## Remarks
General assumptions:  
* expect bugs: this is a very simple tool built in a couple of hours
* the tool could be optimized by processing certain operations in parallel
* I didn't test but the tool should not delete existing items
* the tool could be easily extended to process more of [SendGrid's Api functionality](https://sendgrid.com/docs/API_Reference/api_v3.html) on the command-line
* it shouldn't be complicated to port it to [.NET Core](//docs.microsoft.com/en-us/dotnet/core/) so it can be run from Linux environments.
	
# See Also
Don't forget to visit my blog at: [blog.hildenco.com](https://blog.hildenco.com).
