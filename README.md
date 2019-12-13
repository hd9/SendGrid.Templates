# SendGrid.Templates migration tool
A simple tool to migrate SendGrid templates between accounts (or subusers)
by using their api keys.

## Utilization
```
$ sendgrid --srctoken <<source-api-token>> --dsttoken <<destination-api-token>>
```

## Remarks
General assumptions:  
	* expect bugs: this is a very simple tool built in a couple of hours
	* the tool could be optimizing by processing certain operations in parallel
	* the tool does not delete existing items
	
# See Also
Don't forget to visit my blog at: [blog.hildenco.com](https://blog.hildenco.com).
