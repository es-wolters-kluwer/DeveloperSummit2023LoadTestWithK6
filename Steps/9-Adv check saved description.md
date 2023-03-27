# Check Saved Description
1. First add a new function to create a random description
```javascript
// Random Description
function CalculateNewDescription(description) {
    const position = description.indexOf("_");
    if (position == -1)
        return description + `_${generateCodeFromDate()}`;
    else
    return description.substring(0, position) + `_${generateCodeFromDate()}`;
}
    
//Random value
function generateCodeFromDate() {
    let date = new Date();
     return date.valueOf()
}

```
2. Before saving the entity has to change the way the description is calculated:
```javascript
const description = CalculateNewDescription(response.html().find('input[id="Description"]').attr('value'));
```

3. After Saving the entity, we call to obtain the saved account and check that the description is the right one. So we add this code at the end of the Account edition escenario
```javascript
    response = http.get(`${data.baseUrl}/Accounts/Details?companyCorrelationId=${companyCorrelationId}&accountCorrelationId=${accountCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags:{type:'query'}});
    check(response, { 'Description well saved': (r) => r.html().find('dd[class="col-sm-10"]').last().text().trim() == description })
```

[Previous](8-Adv%20Dynamic%20test.md) [Home](../README.md) [Next](10-Adv%20Invoices%20Scenario.md)