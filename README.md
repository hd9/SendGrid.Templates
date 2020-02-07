# SendGrid.Templates migration tool
A simple tool to migrate SendGrid templates between accounts (or subusers)
by using their api keys.

## Utilization
```
# ex 1. transfer between accounts
$ sendgrid -f <from-apikey> -t <to-apikey>>

# ex 2. save templates to disk
$ sendgrid -f <from-apikey> -s

# ex 3. save templates to disk specifying a regex format for filenmae
$ sendgrid -f <from-apikey> -s -r "TE\-\d+"

```

## Remarks
General assumptions:  
* this is a spike tool so expect bugs
* the tool could be optimized by processing certain operations in parallel
* the tool does not delete existing items
