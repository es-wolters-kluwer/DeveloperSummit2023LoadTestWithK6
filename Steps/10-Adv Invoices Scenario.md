# Add Invoices Scenario

1. Change default function to add a new group
```javascript
export default function(data) {
    group('account edition', function() { AccountEditionScenario(data)});
    group('invoice edition', function() { InvoiceEditionScenario(data)});
}
```
2. Add the invoices edition scenario
```javascript
function InvoiceEditionScenario(data) {
    // Get home page
    let response = http.get(`${data.baseUrl}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags:{type:'query'} });
    const companyCorrelationId = GetCorrelationIdFromHtml(response.html(), 1, "companyCorrelationId");
    sleep(2);
    
    // Navigate to accounts list  
    response = http.get(`${data.baseUrl}/Invoices?companyCorrelationId=${companyCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags:{type:'query'}});
    
    const invoiceCorrelationId = GetCorrelationIdFromHtml(response.html(), 1, "invoiceCorrelationId");
    sleep(2)
    
    // Navigate to account edition
    response = http.get(`${data.baseUrl}/Invoices/Edit?companyCorrelationId=${companyCorrelationId}&invoiceCorrelationId=${invoiceCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags:{type:'query'}});
    sleep(2)
     
    let id = response.html().find('input[id="Id"]').attr('value');
    let accountCode = response.html().find('input[id="AccountCode"]').attr('value');
    let concept = CalculateNewDescription(response.html().find('input[id="Concept"]').attr('value'));
    let amount = response.html().find('input[id="Amount"]').attr('value');
    let taxAmount = response.html().find('input[id="TaxAmount"]').attr('value');
    const requestVerificationToken = response.html().find('input[name="__RequestVerificationToken"]').attr('value');
    
    // Edit invoice   
    const PAYLOAD = `CompanyCorrelationId=${companyCorrelationId}&CorrelationId=${invoiceCorrelationId}&Id=${id}&Concept=${concept}&AccountCode==${accountCode}&Amount=${amount}&TaxAmount=${taxAmount}}&__RequestVerificationToken=${requestVerificationToken}`;
    response = http.post(`${data.baseUrl}/Invoices/Edit`, PAYLOAD, 
        { headers: {'Accept': accept, 'Accept-Encoding': acceptEncoding, 'Content-Type': contentType },
          tags:{type:'command'}});
    check(response, { 'Entity edition with estatus code 200': (r) => r.status >= 200 && r.status <= 226 })
    
    // check value is edited    
    response = http.get(`${data.baseUrl}/Invoices/Details?companyCorrelationId=${companyCorrelationId}&invoiceCorrelationId=${invoiceCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
        tags:{type:'query'}});
    
        check(response, { 'Description well saved': (r) => r.html().find('dd[class="col-sm-10"]').toArray()[1].text().trim() == concept })
}
```


[Previous](9-Adv%20check%20saved%20description.md) [Home](../README.md)