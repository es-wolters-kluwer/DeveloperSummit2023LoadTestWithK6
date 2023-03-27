# Add Users

Configure stages in init:

```javascript
// Configure users
export const options = {
    stages: [
        { target: 10, duration: '5s' },
        { target: 10, duration: '10s'},
        { target: 0, duration: '2s'}
    ]
}
```

[Previous](./2-Test%20Lifecycle.md) [Home](../README.md) [Next](4-Add%20environment%20variable.md)