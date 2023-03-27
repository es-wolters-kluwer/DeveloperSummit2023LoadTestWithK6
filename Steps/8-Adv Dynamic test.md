# Dynamic Test
We want to use the data returned by the test.
1. First create a function to obtain the Id from the html depending on the user
```javascript
//extract Id
function GetCorrelationIdFromHtml(body, option, name ) {
    // Get the rows
    const rows = body.find('tbody').children("tr").toArray();
    // calculate row index
    let index = __VU - 1;
    if (__VU > rows.length)
        index = (__VU % rows.length); 
    // Get the column with options
    const optionsColumns = rows[index].last('td').find('a').toArray();
    // Get the href from accounts (first option)
    const accountResource = optionsColumns[option].attr('href');
    
    const initialPosition = accountResource.indexOf(`${name}=`) + `${name}=`.length;
    //extract id with name 
    return accountResource.substring(initialPosition , initialPosition + 36); 
}
```
2. Modify the function AccountEditionScenario to obtain de Id's
```javascript
function AccountEditionScenario(data) {
    let response = http.get(`${data.baseUrl}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags:{type:'query'} });
    sleep(2);
    // Get companyCorrelationId
    const companyCorrelationId = GetCorrelationIdFromHtml(response.html(), 0, 'companyCorrelationId');

    response = http.get(`${data.baseUrl}/Accounts?companyCorrelationId=${companyCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags:{type:'query'}});
    sleep(2);

    //Get accountCorrelationId
    const accountCorrelationId = GetCorrelationIdFromHtml(response.html(), 0, 'accountCorrelationId');

    response = http.get(`${data.baseUrl}/Accounts/Edit?companyCorrelationId=${companyCorrelationId}&accountCorrelationId=${accountCorrelationId}`, 
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags:{type:'query'}});
    sleep(2);
    //Get the form values
    const id = response.html().find('input[id="Id"]').attr('value');
    const accountCode = response.html().find('input[id="Code"]').attr('value');
    const description = response.html().find('input[id="Description"]').attr('value') + `_user:${_VU}`;
    const requestVerificationToken = response.html().find('input[name="__RequestVerificationToken"]').attr('value');


    const payload = `CompanyCorrelationId=${companyCorrelationId}&CorrelationId=${accountCorrelationId}&Code=${accountCode}&Id=${id}&Description=${description}&__RequestVerificationToken=${requestVerificationToken}`;
    response = http.post(`${data.baseUrl}/Accounts/Edit`, payload, 
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding,'Content-Type': contentType },
          tags:{type:'command'}});
    
    check(response, {'Entity saved succesfully': (r) => r.status >= 200 && r.status <= 226});
}

```
[Previous](7-Monitoring.md) [Home](../README.md) [Next](9-Adv%20check%20saved%20description.md)

