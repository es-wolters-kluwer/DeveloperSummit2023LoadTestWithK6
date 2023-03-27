# Monitoring
1. Execute the Monitoring system
```powershell
docker-compose -f ./DockerTools/Monitor-Docker-Compose.yml up -d
```

2. Configure datasource

|Property           | Value                  |
|-------------------|------------------------|
|URL                | http://prometheus:9090 |
|Scrape interval    |                    15s |
|Query timeout      |                    60s |
|Prometheus type    | Prometheus             |
|Prometheus version | 2.19.x                 |

3. Create a new dashboard. Use Import option and copy the content from [dashboard.json](./resources/dashboard.json)

4. Increase the duration of the test
```javascript
//Configure users
export const options = {
    stages: [
        { target: 10, duration: '1s' },
        { target: 10, duration: '5m'},
        { target: 0, duration: '30s'}
    ],
    thresholds: {
        http_req_failed:['rate<0.01'],
        'http_req_duration{type:command}':['p(99)<200'],
        'http_req_duration{type:query}':['p(99)<200']
    }
}
```
5. Finally execute de tests with --output property and adding a tag for testId.
```powershell
k6 run .\src\index.js --env BASEURL="http://localhost:3033" -o experimental-prometheus-rw --tag testid="4"
```

[Previous](6-Add%20Thresholds%2C%20Tags%20and%20Groups.md) [Home](../README.md) [Bonus Track](8-Adv%20Dynamic%20test.md)