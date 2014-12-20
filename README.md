Palantir
========

Social media parser of VK.com. The project consists of 4 VS solutions:

###  Palantir-Core 

Contains main logic of data fetching and presenting. It contains all the utilities and framework projects as well as service layer.

### Palantir-Engine

Contains the classes required to connect and fetch data from VK on scheduled based principle. ActiveMQ is required to make it works.

### Palantir-WebApp 

The presentation part of the projects. Displays bunch of charts and tables using ASP.NET MVC and http://www.flotcharts.org/

### Palantir-Backend 

A backend of the project where you can add/remove accounts and see basic statistics.
