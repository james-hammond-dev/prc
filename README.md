# A round-robin TCP load balancer in .netcore (c#)

Hosted using BackGroundServiceWorker

Prc.LoadBalancerHost
  Hosts the worker services.
  Configuration file (appsettings.json) contains the details of the background services
  to use for load balancing.

Prc.LoadBalancerService
  Listens on TCP port 8080, routes incoming requests to the services configured in the main host.

Prc.HealthCheckService
  Runs at scheduled interval to check the status of the background services via simple
  connection attempts. Status changes are sent to Prc.ServiceSelector

Prc.ServiceSelector
  Shared (singleton) between LoadBalancer & HealthChecker, maintains background service state
  and provides available service details to the LoadBalancer.

# Build & Test

Uses Make to setup, build & run tests 
e.g.

make build

make test

UnitTest projects in *Test
Integration tests using running Host & background services in Prc.LoadBalancer.IntegrationTest

# Known Issues
  See issues file

