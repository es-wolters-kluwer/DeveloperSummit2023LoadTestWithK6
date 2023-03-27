# Add Thresholds, Tags and Groups
1. Add thresholds in Options variable. We add two thresholds: Requests failed less than 1% and the response time are less than 500 ms for percentile 99.
```javascript
export const options = {
    stages: [
        { target: 10, duration: '5s' },
        { target: 10, duration: '10s'},
        { target: 0, duration: '2s'}
    ],
    thresholds: {
        http_req_failed:['rate<0.01'],
        http_req_duration:['p(99)<500']
    }
}
```
2. We add a tag to filter between queries(get) and commands(post) and define a different threshold for each one:
```javascript
export const options = {
    stages: [
        { target: 10, duration: '5s' },
        { target: 10, duration: '10s'},
        { target: 0, duration: '2s'}
    ],
    thresholds: {
        http_req_failed:['rate<0.01'],
        'http_req_duration{type:command}':['p(99)<200'],
        'http_req_duration{type:query}':['p(99)<200']
    }
}


// setup function
// --------------

export default function(data)
{
    http.get(`${data.baseUrl}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags: {type:'query'} });
    sleep(2);

    http.get(`${data.baseUrl}/Accounts?companyCorrelationId=3bff1ad3-5ce6-4790-8812-df02e4c300c0`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags: {type:'query'}});
    sleep(2);

    let response = http.get(`${data.baseUrl}/Accounts/Edit?companyCorrelationId=3bff1ad3-5ce6-4790-8812-df02e4c300c0&accountCorrelationId=6bb23bc1-63e9-4447-91a9-e1f529d4b51e`, 
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags: {type:'query'}});
    sleep(2);
    const requestVerificationToken = response.html().find('input[name="__RequestVerificationToken"]').attr('value');

    const payload = `CompanyCorrelationId=3bff1ad3-5ce6-4790-8812-df02e4c300c0&CorrelationId=6bb23bc1-63e9-4447-91a9-e1f529d4b51e&Code=1000000000&Id=925c3dc8-fd41-49a7-8716-8941178b56ba&Description=Capital+social+_${__VU}&__RequestVerificationToken=${requestVerificationToken}`;
    response = http.post(`${data.baseUrl}/Accounts/Edit`, payload, 
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding,'Content-Type': contentType },
          tags: {type:'command'}});
    
    check(response, {'Entity saved succesfully': (r) => r.status >= 200 && r.status <= 226});
}

```

3. Finally define the scenario as a group
```javascript
import { sleep, check, group } from 'k6'

// init and setup function

export default function(data)
{
    group('account edition', function() { AccountEditionScenario(data)});
}

function AccountEditionScenario(data){
    // All the staff
}
```

[Previous](5-First%20Scenario.md) [Home](../README.md) [Next](./7-Monitoring.md)
