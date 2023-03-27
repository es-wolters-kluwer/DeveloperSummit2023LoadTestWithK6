# Test Lifecycle
1. Add setup function
```javascript
export function setup() {
    console.log("Wow! Test started");
}
```
2. Add log user in default function
```javascript
export default function(data) {
    console.log("Executing user: " + __VU);
    http.get('http://localhost:3033');
    sleep(2);
}
```
3. Add teardown function
```javascript
export function teardown(data) {
    console.log("Yeah! Test finished");
}
```

[Previous](1-First%20Test.md)  [Home](../README.md) [Next](./3-Add%20Users.md)