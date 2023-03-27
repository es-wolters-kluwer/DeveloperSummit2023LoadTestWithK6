# Introduction 
This project have all support files and projects for the Load Test with K6 Hands on presentation in WKE Developers Summit 2023.

This readme file is a guide to follow the hands on lab.

# Getting Started
Before start the hands on lab, it's necessary build and execute a sample app. Then we build a solution tests to load test this app.

1.	Building Sample Application.
Open an Powershell in the solution root folder and just execute:

    ```shell
    Docker build -f ./DockerTools/SampleWithBuild.DockerFile -t wke/sampleapp .
    ```

2. Execute Sample Application.
You can execute the following Docker-Compose file:

    ```shell
    docker-compose -f .\DockerTools\Sample-Docker-Compose.yml up -d
    ```

And access to: [localhost:3033](http://localhost:3033) in your browser.

# Hands on lab
## Let's Play
1. [First Test](./Steps/1-First%20Test.md)
2. [Test Lifecycle](./Steps/2-Test%20Lifecycle.md)
3. [Add users](./Steps/3-Add%20Users.md)
4. [Add environment variable](./Steps/4-Add%20environment%20variable.md)
5. [First Scenario](./Steps/5-First%20Scenario.md)
6. [Add Thresholds, Tags and Groups](./Steps/6-Add%20Thresholds%2C%20Tags%20and%20Groups.md)

## Let's See
1. [Monitoring](./Steps/7-Monitoring.md)

## Bonus Track
1. [Dynamic Test](./Steps/8-Adv%20Dynamic%20test.md)
2. [Check Saved Description](./Steps/9-Adv%20check%20saved%20description.md)
3. [Add Invoices Scenario](./Steps/10-Adv%20Invoices%20Scenario.md)
