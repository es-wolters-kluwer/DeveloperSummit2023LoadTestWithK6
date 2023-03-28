# First Test

1. First create folder **./src** and file **./src/index.js**
2. Write following code:
```javascript
import http from 'k6/http'
import {sleep} from 'k6'

export default function ()
{   
    http.get('http://localhost:3033')
    sleep(1);
}
```
3. Execute the test:
```k6 run .\src\index.js```

[Home](../README.md) [Next](2-Test%20Lifecycle.md)

