# Add Environment Variable
Add URL as environment Variable
1. Configure Environment variable in setup function.
```javascript
export function setup() {
    console.log("Wow! Test started");
    let baseurl = '';
    if (__ENV.BASEURL) {
        baseurl = __ENV.BASEURL
    }
    return {'baseUrl': baseurl};
}
```

2. Consume this variable in the test (substitute http.get in default function)
```javascript
    http.get(`${data.baseUrl}`);
```

[Previous](3-Add%20Users.md) [Home](../README.md) [Next](5-First%20Scenario.md)