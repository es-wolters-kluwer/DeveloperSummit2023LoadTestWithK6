1. Step 1 - First test

- Create folder **src**
- Create file **index.js** inside src folder.
- Write code: 
```
import http from 'k6/http';
import { sleep } from 'k6';

export default function () {
    http.get('http://localhost:3033');
    sleep(1);
}
```
- Execute: ``` k6 run .\src\index.js ```
- Execute for more users:
> From command line: ``` k6 run --vus 10 --duration 30s .\src\index.js ```

2. Step 2 - Setting lifecycle and stages

- Setting lifecycling
> init 
>>``` 
>>export const options = {
>>  stages: [
>>      { target: 10, duration: '10s' },
>>      { target: 10, duration: '30s' },
>>      { target: 0, duration: '5s'  }
>>   ]
>>};
>>```
> Setup
>>```
>>export function setup() {
>>   console.log("Wow! Test started); 
>>}
>>```
> Default
>> ```console.log("Executing user: " + __VU);```
> TearDown
>>```
>>export function teardown() {
>>   console.log("Yeah! Test finished"); 
>>}
>>```

And execute: ``` k6 run .\src\index.js ```


3. Step 3 - First Scenario
Add this code in init:
```
const accept = 'text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9';
const acceptEncoding = 'gzip, deflate, br';
const contentType = 'application/x-www-form-urlencoded';
```

Modify default function, but using F12 results:
```
    // Get home page
    http.get('http://localhost:3033');
    sleep(1);

    // Navigate to accounts list
    http.get('http://localhost:3033/Accounts?companyCorrelationId=c662356e-f23f-4cdc-88a1-087c6dee37fa',
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding }});
    sleep(1)

    // Navigate to account edition
    http.get('http://localhost:3033/Accounts/Edit?companyCorrelationId=c662356e-f23f-4cdc-88a1-087c6dee37fa&accountCorrelationId=1b5adebd-0595-4927-afdd-9c4d2f9b9da2',
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding }});
    sleep(1)

    // Edit account   
    const PAYLOAD = "CompanyCorrelationId=c662356e-f23f-4cdc-88a1-087c6dee37fa&CorrelationId=1b5adebd-0595-4927-afdd-9c4d2f9b9da2&Code=1000000000&Id=9b70dee4-023a-433d-aa0d-e8409a898fb0&Description=Capital+social+user_"+__VU+"&__RequestVerificationToken=CfDJ8AODveWItFxMkSsNuqTpDsRXdhnJaLw1kayoD_EkHMSvkiGutYe4bdtLsqD4ulnicWXGrMcvOfzVPzrP5sugwzX8wzqo6s7bv7p0rK8JgR1S1aWf3j6vU_PMzTCTjmTwexzqw6emymzrLXc8SnBOF5g";
    let response = http.post('http://localhost:3033/Accounts/Edit', PAYLOAD,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding, 'Content-Type': contentType }});
    if (response.status != 200)
        console.log("Response: " + response.status_text);

```
And execute in command line: ``` k6 run .\src\index.js ```

4. Step 4: Set first check

Import check from k6 ```` import { sleep, check } from 'k6';```
Change code Console.log by: ```check(response, { 'Account edition with estatus code 200': (r) => r.status >= 200 && r.status <= 226 })```
And execute in command line: ``` k6 run .\src\index.js ```

5. Step 5: Fix error

Change this code:
```
    ...

    const requestVerificationToken = response.html().find('input[name="__RequestVerificationToken"]').attr('value');
    
    // Edit account   
    const PAYLOAD = "CompanyCorrelationId=f2e593b0-4c93-463d-8593-ab9a08fa5007&CorrelationId=49a7aad6-1ebc-4a25-bc8a-69bf2ce64851&Code=1000000000&Id=9b70dee4-023a-433d-aa0d-e8409a898fb0&Description=Capital+social+user_"+__VU+"&__RequestVerificationToken="+requestVerificationToken;

    ...
```

6. Step 6: Set environment variable

Init: ``` let baseUrl = 'http://localhost:3033'; ```

Setup: 
```
if (__ENV.BASEURL) {
        baseUrl = __ENV.BASEURL;
    }
```


Default: Change url: **let response = http.get(`${baseUrl}`);**

Execute: ``` k6 run .\src\index.js --env BASEURL="http://localhost:3033" ```

7. Step 7: Get dynamic data.

Add new function to extract id's from html
```
//extract Id
function GetCorrelationIdFromHtml(body, option, name ) {
    // Get the rows
    const rows = body.find('tbody').children("tr").toArray();
    // calculate row index
    let index = __VU - 1;
    if (__VU > rows.length)
        index = (__VU  % rows.length); 

    // Get the column with options
    const optionsColumns = rows[index].last('td').find('a').toArray();
    // Get the href from accounts (first option)
    const accountRecourse = optionsColumns[option].attr('href');
        
    const initialPosition = accountRecourse.indexOf(`${name}=`) + `${name}=`.length;
    //extract id with name 
    return accountRecourse.substring(initialPosition , initialPosition + 36); 
}
```
And change Default function
// Test execution
export default function () {
    
    // Get home page
    let response = http.get(`${baseUrl}`);
    const companyCorrelationId = GetCorrelationIdFromHtml(response.html(), 0, "companyCorrelationId");    
    sleep(1);
  
    // Navigate to accounts list  
    response = http.get(`${baseUrl}/Accounts?companyCorrelationId=${companyCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding }});

    const accountCorrelationId = GetCorrelationIdFromHtml(response.html(), 0, "accountCorrelationId");        
    sleep(1)
  
    // Navigate to account edition
    response = http.get(`${baseUrl}/Accounts/Edit?companyCorrelationId=${companyCorrelationId}&accountCorrelationId=${accountCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding }});
    sleep(1)
    
    let id = response.html().find('input[id="Id"]').attr('value');
    let accountCode = response.html().find('input[id="Code"]').attr('value');
    let description = response.html().find('input[id="Description"]').attr('value') + ´_user:${__VU}`;
    const requestVerificationToken = response.html().find('input[name="__RequestVerificationToken"]').attr('value');
    
    // Edit account   
    const PAYLOAD = `CompanyCorrelationId=${companyCorrelationId}&CorrelationId=${accountCorrelationId}&Code=${accountCode}&Id=${id}&Description=${description}&__RequestVerificationToken=${requestVerificationToken}`;
    response = http.post(`${baseUrl}/Accounts/Edit`, PAYLOAD,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding, 'Content-Type': contentType }});
    check(response, { 'Account edition with estatus code 200': (r) => r.status >= 200 && r.status <= 226 })
}

8. Step 8: Check value is saved

Add function to calculate new "random" description
```
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

Change code in default:
```
let description = CalculateNewDescription(response.html().find('input[id="Description"]').attr('value'));
const requestVerificationToken = response.html().find('input[name="__RequestVerificationToken"]').attr('value');

.....

 // check value is edited    
response = http.get(`${baseUrl}/Accounts/Details?companyCorrelationId=${companyCorrelationId}&accountCorrelationId=${accountCorrelationId}`,
    { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding }});
check(response, { 'Description well saved': (r) => r.html().find('dd[class="col-sm-10"]').last().text().trim() == description })   

```

9. Step 9: New scenario.

Change default:
```
export default function () {    
    accountEditionScenario();
    invoiceEditionScenario();
}

function accountEditionScenario() {
    // Get home page
    let response = http.get(`${baseUrl}`);
    const companyCorrelationId = GetCorrelationIdFromHtml(response.html(), 0, "companyCorrelationId");    
    sleep(1);
  
    // Navigate to accounts list  
    response = http.get(`${baseUrl}/Accounts?companyCorrelationId=${companyCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding }});

    const accountCorrelationId = GetCorrelationIdFromHtml(response.html(), 1, "accountCorrelationId");        
    sleep(1)
  
    // Navigate to account edition
    response = http.get(`${baseUrl}/Accounts/Edit?companyCorrelationId=${companyCorrelationId}&accountCorrelationId=${accountCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding }});
    sleep(1)

    let id = response.html().find('input[id="Id"]').attr('value');
    let accountCode = response.html().find('input[id="Code"]').attr('value');
    let description = CalculateNewDescription(response.html().find('input[id="Description"]').attr('value'));
    const requestVerificationToken = response.html().find('input[name="__RequestVerificationToken"]').attr('value');
    
    // Edit account   
    const PAYLOAD = `CompanyCorrelationId=${companyCorrelationId}&CorrelationId=${accountCorrelationId}&Code=${accountCode}&Id=${id}&Description=${description}&__RequestVerificationToken=${requestVerificationToken}`;
    response = http.post(`${baseUrl}/Accounts/Edit`, PAYLOAD,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding, 'Content-Type': contentType }});
    check(response, { 'Entity edition with estatus code 200': (r) => r.status >= 200 && r.status <= 226 })

    // check value is edited    
    response = http.get(`${baseUrl}/Accounts/Details?companyCorrelationId=${companyCorrelationId}&accountCorrelationId=${accountCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding }});
    check(response, { 'Description well saved': (r) => r.html().find('dd[class="col-sm-10"]').last().text().trim() == description })     
}

function invoiceEditionScenario() {
    // Get home page
    let response = http.get(`${baseUrl}`);
    const companyCorrelationId = GetCorrelationIdFromHtml(response.html(), 1, "companyCorrelationId");    
    sleep(1);
  
    // Navigate to accounts list  
    response = http.get(`${baseUrl}/Invoices?companyCorrelationId=${companyCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding }});

    const invoiceCorrelationId = GetCorrelationIdFromHtml(response.html(), 1, "invoiceCorrelationId");        
    sleep(1)
  
    // Navigate to account edition
    response = http.get(`${baseUrl}/Invoices/Edit?companyCorrelationId=${companyCorrelationId}&invoiceCorrelationId=${invoiceCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding }});
    sleep(1)
    
    let id = response.html().find('input[id="Id"]').attr('value');
    let accountCode = response.html().find('input[id="AccountCode"]').attr('value');
    let concept = CalculateNewDescription(response.html().find('input[id="Concept"]').attr('value'));
    let amount = response.html().find('input[id="Amount"]').attr('value');
    let taxAmount = response.html().find('input[id="TaxAmount"]').attr('value');    
    const requestVerificationToken = response.html().find('input[name="__RequestVerificationToken"]').attr('value');
    
    // Edit invoice   
    const PAYLOAD = `CompanyCorrelationId=${companyCorrelationId}&CorrelationId=${invoiceCorrelationId}&Id=${id}&Concept=${concept}&AccountCode==${accountCode}&Amount=${amount}&TaxAmount=${taxAmount}}&__RequestVerificationToken=${requestVerificationToken}`;

    response = http.post(`${baseUrl}/Invoices/Edit`, PAYLOAD,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding, 'Content-Type': contentType }});
    check(response, { 'Entity edition with estatus code 200': (r) => r.status >= 200 && r.status <= 226 })

    // check value is edited    
    response = http.get(`${baseUrl}/Invoices/Details?companyCorrelationId=${companyCorrelationId}&invoiceCorrelationId=${invoiceCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding }});   

    check(response, { 'Description well saved': (r) => r.html().find('dd[class="col-sm-10"]').toArray()[1].text().trim() == concept })     
}
```

10. Step 10: Setting groups
Change init:
```import { sleep, check, group } from 'k6';```
Change default:
```
// Test execution
export default function () {    
    group('account edition', accountEditionScenario);
    group('invoice edition', invoiceEditionScenario);
}
```

11. Step 11: Setting Thresholds

Add this thresholds in options class:
```
export const options = {
  
  ...

    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(99)<500']
    }
};
```

12. Step 12: Setting Tags

Add some tag to http calls and configure an thershold on it

Add tag to calls:

``` http.get(url,{headers:{},tags:{type:'query'}})```
``` http.post(url,{headers:{},tags:{type:'command'}})```

Configure threshold:
```
export const options = {
  
  ...

    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration{type:command}: ['p(99)<200'],
        http_req_duration{type:query}: ['p(99)<500']
    }
};
```

13. Step 13: Monitoring

First execute docker compose for monitoring: ```docker-compose -f .\DockerTools\Monitor-Docker-Compose.yml up -d ```
Second set environment variables:
```$env:K6_PROMETHEUS_RW_SERVER_URL="http://localhost:9090/api/v1/write"```
```$env:K6_PROMETHEUS_RW_TREND_AS_NATIVE_HISTOGRAM='true'```
Third execute K6 ```k6 run .\src\index.js --env BASEURL="http://localhost:3033" -o experimental-prometheus-rw --tag testid="1"```
Forth let explore grafana
    - Create datasource 
    - Create two dashboards
        - Users and throughput
        - Duration
        k6-users
        k6-http
        k6-http-req-duration-seconds


CPU:
"irate(container_cpu_system_seconds_total{name=\"dockertools-sampleloadapp-1\"}[120s])* 100"